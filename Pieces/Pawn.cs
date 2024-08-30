using Chess.Pieces;
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class Pawn : Node2D, IPiece_Directional
{
    public Sprite2D background_node { get; set; }
    public Sprite2D piece_sprite { get; set; }
    public Player owner_player { get; private set; }

    public Vector2I board_position { get; set; }
    public bool selected { get; set; }
    public bool attacked { get; set; }
    public bool has_moved { get; set; }

    public int orientation { get; set; }


    public static Pawn Load(Player owner, Vector2I position)
    {

        Pawn piece = (Pawn)(GD.Load<PackedScene>("res://Pieces/Pawn.tscn")).Instantiate();
        piece.owner_player = owner;
        piece.selected = false;
        piece.attacked = false;
        piece.board_position = position;

        piece.orientation = -1;

        return piece;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Link_Child_Nodes();
        piece_sprite.Texture = GD.Load<Texture2D>($"res://assets/{this.GetType()}_{owner_player.id.ToString()}.png");
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);

    }

    public void Link_Child_Nodes()
    {
        background_node = (Sprite2D)GetNode("Background");
        piece_sprite = (Sprite2D)GetNode("PieceSprite");
    }

    public List<Vector2I> All_Destinations(IPiece[,] board)
    {

        List<Vector2I> destinations = new List<Vector2I>();

        if (board.GetLength(1) > board_position.Y + orientation && board_position.Y + orientation >= 0) //can't check for specific direction because board flipping
        {
            if (board[board_position.X, board_position.Y + orientation] is null)
            {
                destinations.Add(new Vector2I(board_position.X, board_position.Y + orientation));

                if (!has_moved && board[board_position.X, board_position.Y + orientation * 2] is null) //Can't be out of range on a proper board setup
                {
                    destinations.Add(new Vector2I(board_position.X, board_position.Y + orientation * 2));
                }
            }
            //TODO add EnPassant check. "last move() on Game?"
            if (board_position.X > 0 && board[board_position.X - 1, board_position.Y + orientation] is not null && board[board_position.X - 1, board_position.Y + orientation].owner_player != this.owner_player)
            {
                destinations.Add(new Vector2I(board_position.X - 1, board_position.Y + orientation));
            }
            if (board_position.X + 1 < board.GetLength(1) && board[board_position.X + 1, board_position.Y + orientation] is not null && board[board_position.X + 1, board_position.Y + orientation].owner_player != this.owner_player)
            {
                destinations.Add(new Vector2I(board_position.X + 1, board_position.Y + orientation));
            }
        }

        return destinations;
    }

    public void Flip()
    {
        this.orientation = IPiece_Directional.Flip(this.orientation);
    }

}
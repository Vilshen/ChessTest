using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class King : Node2D, IPiece
{
    public Sprite2D background_node { get; set; }
    public Sprite2D piece_sprite { get; set; }
    public Player owner_player { get; private set; }

    public Vector2I board_position { get; set; }
    public bool selected { get; set; }
    public bool attacked{ get; set; }
    public bool has_moved { get; set; }


    public static King Load(Player owner, Vector2I position)
    {

        King piece = (King)(GD.Load<PackedScene>("res://Pieces/King.tscn")).Instantiate();
        piece.owner_player = owner;
        piece.selected = false;
        piece.attacked = false;
        piece.board_position = position;
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

    

    

    public (List<Vector2I>, List<Vector2I>) All_Destinations(IPiece[,] board)
    {   

        List<Vector2I> destinations = new List<Vector2I>();

        bool up_safe = board_position.Y - 1 >= 0;
        bool down_safe = board_position.Y + 1 < board.GetLength(0);
        bool left_safe = board_position.X - 1 >= 0;
        bool right_safe = board_position.X + 1 < board.GetLength(0);

        if (up_safe)
        {
            Check_Tile(board_position.X, board_position.Y-1);
            if (left_safe)
            {
                Check_Tile(board_position.X - 1, board_position.Y-1);
            }
            if (right_safe)
            {
                Check_Tile(board_position.X + 1, board_position.Y-1);
            }
        }
        if (down_safe)
        {
            Check_Tile(board_position.X, board_position.Y+1);
            if (left_safe)
            {
                Check_Tile(board_position.X - 1, board_position.Y+1);
            }
            if (right_safe)
            {
                Check_Tile(board_position.X + 1, board_position.Y+1);
            }
        }
        if (left_safe)
        {
            Check_Tile(board_position.X - 1, board_position.Y);
        }
        if (right_safe)
        {
            Check_Tile(board_position.X + 1, board_position.Y);
        }
        void Check_Tile(int i, int j)
        {
            if (board[i, j] is null || board[i, j].owner_player != this.owner_player)
            {
                destinations.Add(new Vector2I(i, j));
            }
        }
        List<Vector2I> castling_destinations = null;
        if (!has_moved && !attacked)
        {
            castling_destinations = new List<Vector2I>();
        }
        return (destinations,castling_destinations);
    }

    public string Notation()
    {
        return "K";
    }
    public bool Check_Special_Move(IPiece[,] board, Vector2I target) //TODO
    {
        return false;
    }
    public (IPiece secondary_target, Vector2I sec_target_origin, Vector2I? sec_target_dest) Perform_Special_Move(IPiece[,] board, Vector2I target)
    {
        throw new NotImplementedException();
    }
}
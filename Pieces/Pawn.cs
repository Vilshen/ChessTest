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

    private Game game;

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

        game = (Game)GetParent();
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
        List<Vector2I> en_passant_destinations = new List<Vector2I>();

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

            if (board_position.X > 0)
            {
                if (board[board_position.X - 1, board_position.Y + orientation] is null)
                {
                    en_passant_destinations.Add(new Vector2I(board_position.X - 1, board_position.Y + orientation));
                }
                else if (board[board_position.X - 1, board_position.Y + orientation].owner_player != this.owner_player)
                {
                    destinations.Add(new Vector2I(board_position.X - 1, board_position.Y + orientation));
                }
            }
            if (board_position.X + 1 < board.GetLength(1))
            {
                if (board[board_position.X + 1, board_position.Y + orientation] is null)
                {
                    en_passant_destinations.Add(new Vector2I(board_position.X + 1, board_position.Y + orientation));
                }
                else if (board[board_position.X + 1, board_position.Y + orientation].owner_player != this.owner_player)
                {
                    destinations.Add(new Vector2I(board_position.X + 1, board_position.Y + orientation));
                }
            }
        }

        return (destinations,en_passant_destinations);
    }

    public void Flip()
    {
        this.orientation = IPiece_Directional.Flip(this.orientation);
    }

    public string Notation()
    {
        return "";
    }

    public bool Check_Special_Move(IPiece[,] board, Vector2I target) // :(
    {
        if (board[target.X,target.Y-orientation] is Pawn)
        {
            string last_move = game.Last_Move();
            string expected_string;
            if (owner_player.id == Team_Enum.White)
            {
                expected_string = $"{(char)('a' + target.X)}{board.GetLength(1) - (target.Y + 2 * orientation)-1}{(char)('a' + target.X)}{board.GetLength(1) - target.Y-1}";
            }
            else
            {
                expected_string = $"{(char)('a' + (board.GetLength(0) - target.X-1))}{target.Y}{(char)('a' + (board.GetLength(0) - target.X-1))}{target.Y - 2 * orientation}";
            }
            if (last_move == expected_string)
            {
                return true;
            }
        }
        return false;
    }

    public (IPiece secondary_target, Vector2I sec_target_origin, Vector2I? sec_target_dest) Perform_Special_Move(IPiece[,] board, Vector2I target)
    {
        IPiece secondary_target = board[target.X, target.Y - orientation];
        return (secondary_target, secondary_target.board_position, null);
    }

    public string State_Dump()
    {
        return "P";
    }

    public void Promote()
    {

    }

}
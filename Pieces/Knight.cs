using Godot;
using System;
using System.Collections.Generic;

public partial class Knight : Node2D, IPiece
{
    public Sprite2D background_node { get; set; }
    public Sprite2D piece_sprite { get; set; }
    public Player owner_player { get; private set; }

    public Vector2I board_position { get; set; }
    public bool selected { get; set; }
    public bool attacked { get; set; }
    public bool has_moved { get; set; }

    public void Link_Child_Nodes()
    {
        background_node = (Sprite2D)GetNode("Background");
        piece_sprite = (Sprite2D)GetNode("PieceSprite");
    }

    public static Knight Load(Player owner, Vector2I position)
    {

        Knight piece = (Knight)(GD.Load<PackedScene>("res://Pieces/Knight.tscn")).Instantiate();
        piece.owner_player = owner;
        piece.selected = false;
        piece.attacked = false;
        piece.board_position = position;
        return piece;
    }

    public (List<Vector2I>, List<Vector2I>) All_Destinations(IPiece[,] board)
    {
        List<Vector2I> destinations = new List<Vector2I>();

        if (board_position.Y - 2 >= 0)
        {
            if (board_position.X - 1 >= 0)
            {
                Check_Tile(board_position.X - 1, board_position.Y - 2);
            }
            if (board_position.X + 1 < board.GetLength(0))
            {
                Check_Tile(board_position.X + 1, board_position.Y - 2);
            }
        }
        if (board_position.Y + 2 < board.GetLength(1))
        {
            if (board_position.X - 1 >= 0)
            {
                Check_Tile(board_position.X - 1, board_position.Y + 2);
            }
            if (board_position.X + 1 < board.GetLength(0))
            {
                Check_Tile(board_position.X + 1, board_position.Y + 2);
            }
        }

        if (board_position.X - 2 >= 0)
        {
            if (board_position.Y - 1 >= 0)
            {
                Check_Tile(board_position.X - 2, board_position.Y - 1);
            }
            if (board_position.Y + 1 < board.GetLength(0))
            {
                Check_Tile(board_position.X - 2, board_position.Y + 1);
            }
        }
        if (board_position.X + 2 < board.GetLength(0))
        {
            if (board_position.Y - 1 >= 0)
            {
                Check_Tile(board_position.X + 2, board_position.Y - 1);
            }
            if (board_position.Y + 1 < board.GetLength(0))
            {
                Check_Tile(board_position.X + 2, board_position.Y + 1);
            }
        }

        void Check_Tile(int i, int j)
        {
            if (board[i, j] is null || board[i, j].owner_player != this.owner_player)
            {
                destinations.Add(new Vector2I(i, j));
            }
        }

        return (destinations,null);
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
    public string Notation()
    {
        return "N";
    }

    public bool Check_Special_Move(IPiece[,] board, Vector2I target)
    {
        return false;
    }

    public (IPiece secondary_target, Vector2I sec_target_origin, Vector2I? sec_target_dest) Perform_Special_Move(IPiece[,] board, Vector2I target)
    {
        throw new InvalidOperationException();
    }

    public string State_Dump()
    {
        return "N";
    }
}

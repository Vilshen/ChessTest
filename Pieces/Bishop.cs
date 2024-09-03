using Godot;
using System;
using System.Collections.Generic;

public partial class Bishop : Node2D, IPiece
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

    public static Bishop Load(Player owner, Vector2I position)
    {

        Bishop piece = (Bishop)(GD.Load<PackedScene>("res://Pieces/Bishop.tscn")).Instantiate();
        piece.owner_player = owner;
        piece.selected = false;
        piece.attacked = false;
        piece.board_position = position;
        return piece;
    }

    public (List<Vector2I>, List<Vector2I>) All_Destinations(IPiece[,] board)
    {
        List<Vector2I> destinations = new List<Vector2I>();

        bool Check_Tile(int i, int j)
        {
            if (board[i, j] is null)
            {
                destinations.Add(new Vector2I(i, j));
                return false;
            }
            else if (board[i, j].owner_player == this.owner_player)
            {
                return true;
            }
            else
            {
                destinations.Add(new Vector2I(i, j));
                return true;
            }
        }

        //Right-Up
        for (int i = board_position.X+1, j = board_position.Y-1; i < board.GetLength(0) && j>=0; i++, j--)
        {
            if (Check_Tile(i, j))
            {
                break;
            }
        }
        //Right-Down
        for (int i=board_position.X+1,j=board_position.Y+1; i < board.GetLength(0) && j < board.GetLength(1); i++, j++)
        {
            if (Check_Tile(i, j))
            {
                break;
            }
        }
        //Left-Up
        for (int i = board_position.X-1, j = board_position.Y-1; i >= 0 && j >= 0; i--, j--)
        {
            if (Check_Tile(i, j))
            {
                break;
            }
        }
        //Left-Down
        for (int i = board_position.X-1, j = board_position.Y+1; i>=0 && j < board.GetLength(1); i--, j++)
        {
            if (Check_Tile(i, j))
            {
                break;
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
        return "B";
    }
    public bool Check_Special_Move(IPiece[,] board, Vector2I target)
    {
        return false;
    }
    public (IPiece secondary_target, Vector2I sec_target_origin, Vector2I? sec_target_dest) Perform_Special_Move(IPiece[,] board, Vector2I target)
    {
        throw new InvalidOperationException();
    }
}

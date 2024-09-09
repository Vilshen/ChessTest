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

    private Game game;

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
            if (board_position.X + 2 < board.GetLength(0) && board[board_position.X + 1, board_position.Y] is null && board[board_position.X + 2, board_position.Y] is null)
            {
                castling_destinations.Add(new Vector2I(board_position.X + 2, board_position.Y));
            }
            if (board_position.X - 2 >= 0 && board[board_position.X - 1,board_position.Y] is null && board[board_position.X - 2, board_position.Y] is null)
            {
                castling_destinations.Add(new Vector2I(board_position.X - 2, board_position.Y));
            }
            
        }
        return (destinations,castling_destinations);
    }

    public string Notation()
    {
        return "K";
    }
    public bool Check_Special_Move(IPiece[,] board, Vector2I target) //TODO
    {
        if (target.X < board_position.X)
        {
            if (board[0, board_position.Y] is Rook
                && !board[0, board_position.Y].has_moved
                && board[0, board_position.Y].owner_player == owner_player)
            {
                if (game.Move_Safety(owner_player, board_position, new Vector2I(board_position.X - 1, board_position.Y)))
                {
                    return true;
                }
            }
        }
        else
        {
            if (board[board.GetLength(0) - 1, board_position.Y] is Rook
                && !board[board.GetLength(0) - 1, board_position.Y].has_moved
                && board[board.GetLength(0) - 1, board_position.Y].owner_player == owner_player)
            {
                if (game.Move_Safety(owner_player, board_position, new Vector2I(board_position.X + 1, board_position.Y)))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public (IPiece secondary_target, Vector2I sec_target_origin, Vector2I? sec_target_dest) Perform_Special_Move(IPiece[,] board, Vector2I target)
    {
        if (target.X < board_position.X)
        {
            return (board[0, board_position.Y], board[0, board_position.Y].board_position, new Vector2I(board_position.X - 1, board_position.Y));
        }
        else
        {
            return (board[board.GetLength(0)-1, board_position.Y], board[board.GetLength(0)-1, board_position.Y].board_position, new Vector2I(board_position.X + 1, board_position.Y));
        }
    }
}
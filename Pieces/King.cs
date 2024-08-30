using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class King : Node2D, IPiece
{
    Game game;
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
        game = (Game)GetParent();
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

        bool up_safe = board_position.Y - 1 >= 0;
        bool down_safe = board_position.Y + 1 < board.GetLength(0);
        bool left_safe = board_position.X - 1 >= 0;
        bool right_safe = board_position.X + 1 < board.GetLength(0);

        if (up_safe)
        {
            if (board[board_position.X,board_position.Y-1] is null || board[board_position.X, board_position.Y - 1].owner_player != this.owner_player)
            {
                destinations.Add(new Vector2I(board_position.X, board_position.Y - 1));
            }
            if (left_safe)
            {
                if (board[board_position.X-1, board_position.Y - 1] is null || board[board_position.X-1, board_position.Y - 1].owner_player != this.owner_player)
                {
                    destinations.Add(new Vector2I(board_position.X-1, board_position.Y - 1));
                }
            }
            if (right_safe)
            {
                if (board[board_position.X + 1, board_position.Y - 1] is null || board[board_position.X + 1, board_position.Y - 1].owner_player != this.owner_player)
                {
                    destinations.Add(new Vector2I(board_position.X + 1, board_position.Y - 1));
                }
            }
        }
        if (down_safe)
        {
            if (board[board_position.X, board_position.Y + 1] is null || board[board_position.X, board_position.Y + 1].owner_player != this.owner_player)
            {
                destinations.Add(new Vector2I(board_position.X, board_position.Y + 1));
            }
            if (left_safe)
            {
                if (board[board_position.X - 1, board_position.Y + 1] is null || board[board_position.X - 1, board_position.Y + 1].owner_player != this.owner_player)
                {
                    destinations.Add(new Vector2I(board_position.X - 1, board_position.Y + 1));
                }
            }
            if (right_safe)
            {
                if (board[board_position.X + 1, board_position.Y + 1] is null || board[board_position.X + 1, board_position.Y + 1].owner_player != this.owner_player)
                {
                    destinations.Add(new Vector2I(board_position.X + 1, board_position.Y + 1));
                }
            }
        }
        if (left_safe)
        {
            if (board[board_position.X - 1, board_position.Y] is null || board[board_position.X - 1, board_position.Y].owner_player != this.owner_player)
            {
                destinations.Add(new Vector2I(board_position.X - 1, board_position.Y));
            }
        }
        if (right_safe)
        {
            if (board[board_position.X + 1, board_position.Y] is null || board[board_position.X + 1, board_position.Y].owner_player != this.owner_player)
            {
                destinations.Add(new Vector2I(board_position.X + 1, board_position.Y));
            }
        }

        //TODO Castling
        return destinations;
    }
}
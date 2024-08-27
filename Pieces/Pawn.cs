using Godot;
using System;

public partial class Pawn : Node2D, IPiece
{
    Sprite2D background_node;
    Sprite2D Piece_sprite;
    public Player owner_player;
    public bool selected;
    public bool attacked;

    public static Pawn Load(Player owner)
    {
        Pawn piece = (Pawn)(GD.Load<PackedScene>("res://Pieces/Pawn.tscn")).Instantiate();
        piece.owner_player = owner;
        piece.selected = false;
        piece.attacked = false;
        return piece;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Link_Child_Nodes();
        switch (owner_player.id)
        {
            case Team_Enum.White:
                Piece_sprite.Texture = GD.Load<Texture2D>("res://assets/Pawn_White.png");
                break;
            case Team_Enum.Black:
                Piece_sprite.Texture = GD.Load<Texture2D>("res://assets/Pawn_Black.png");
                break;
            default:
                throw new IndexOutOfRangeException($"Piece with team ID higher than {(int)Team_Enum.Black} created");

        }
    }
        

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);

    }

    public void Link_Child_Nodes()
    {
        background_node = (Sprite2D)GetNode("Background");
        Piece_sprite = (Sprite2D)GetNode("PieceSprite");
    }

    public void Click(Player curr_player)
    {   
        if (curr_player == owner_player)
        {
            if (owner_player.selected_piece == this)
            {
                this.Deselect();
            }
            else
            {
                if (owner_player.selected_piece is not null)
                {
                    owner_player.selected_piece.Deselect();
                }
                this.Select();
            }
        }


        Set_Background();
    }

    public void Set_Background()
    {
        if (selected)
        {
            background_node.Show();
            background_node.Texture = GD.Load<Texture2D>("res://assets/Background_Blue.png");
        }
        else if (attacked)
        {
            background_node.Show();
            background_node.Texture = GD.Load<Texture2D>("res://assets/Background_Red.png");
        }
        else
        {
            background_node.Hide();
        }
    }

    public void Select()
    {
        selected = true;
        owner_player.selected_piece = this;
        Set_Background();
    }
    public void Deselect()
    {
        selected = false;
        owner_player.selected_piece = null;
        Set_Background();
    }
}
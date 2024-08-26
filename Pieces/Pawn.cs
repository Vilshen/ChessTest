using Godot;
using System;

public partial class Pawn : Node2D, IPiece
{
    public Sprite2D background_node;
    public Sprite2D Piece_sprite;
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
        if (owner_player.id==0)
        {
            Piece_sprite.Texture = GD.Load<Texture2D>("res://assets/Pawn_White.png");
        }
        else
        {
            Piece_sprite.Texture = GD.Load<Texture2D>("res://assets/Pawn_Black.png");
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

    public void Click()
    {   //must be called with player context. distinguish between own and enemy pieces.
        selected = !selected;
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
}
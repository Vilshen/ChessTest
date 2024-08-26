using Godot;
using System;

public partial class Piece : Node2D
{
    public Sprite2D BackgroundNode;
    public Sprite2D PieceSprite;
    public bool Owner_Player;


    public static PackedScene Load()
    {
        return GD.Load<PackedScene>("res://Piece.tscn");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Link_Child_Nodes();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void Link_Child_Nodes()
    {
        BackgroundNode = (Sprite2D)GetNode("Background");
        PieceSprite = (Sprite2D)GetNode("PieceSprite");
    }
}


/*if (Owner_Player)
            {
                PieceSprite.Texture = GD.Load<Texture2D>("res://assets/Pawn_White.png");
            }
            else
            {
                PieceSprite.Texture = GD.Load<Texture2D>("res://assets/Pawn_Black.png");
            }*/

using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Game : Node
{
    Sprite2D board_texture;
    Vector2 board_start;
    Vector2 board_end;
    Piece testPawn;

    Piece[,] board;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        base._Ready();

        board_texture = (Sprite2D)GetNode("Board");
        board_start = board_texture.Position- board_texture.Texture.GetSize()/2;
        board_end = board_texture.Texture.GetSize() + board_start;

        Setup_Game();

        testPawn = (Piece)Piece.Load().Instantiate();
        AddChild(testPawn);
        testPawn.PieceSprite.Texture = GD.Load<Texture2D>("res://assets/Pawn_White.png");


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        base._Process(delta);
    }

    

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsReleased() && mouseEvent.ButtonIndex==MouseButton.Left)
		{
			if (mouseEvent.Position.X >= board_start.X && mouseEvent.Position.Y >= board_start.Y && mouseEvent.Position.X <= board_end.X && mouseEvent.Position.Y <= board_end.Y)
            {
                GD.Print(mouseEvent.Position);

                testPawn.Position = mouseEvent.Position;
                // board click event
            }

            
		}
    }

    public void Setup_Game()
    {
        void Setup_Board(Piece[,] board)
        {

        }
    }
}

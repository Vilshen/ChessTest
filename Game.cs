
using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
using static Godot.Projection;

public partial class Game : Node
{
    Sprite2D board_texture;
    Vector2 board_start;
    Vector2 board_end;
    Vector2 cell_offset;

    IPiece[,] board;
    IPiece selected;

    Player[] players;

    int clock_length;
    int per_move_clock_bonus;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        //
        clock_length = 600;
        //

        board_texture = (Sprite2D)GetNode("Board");
        board_start = board_texture.Position - board_texture.Texture.GetSize() / 2;
        board_end = board_texture.Texture.GetSize() + board_start;
        cell_offset = (board_end - board_start) / 16;

        players = new Player[] { (Player)GetNode("Player_White"), (Player)GetNode("Player_Black") };
        players[0].Setup(0, clock_length);
        players[1].Setup(1, clock_length);

        Setup_Game();


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
                Vector2I cell = (Vector2I)Pixel_to_Grid(mouseEvent.Position);
                if (board[cell.X,cell.Y] is Node2D)
                {
                    board[cell.X, cell.Y].Click();
                } 
               
            }

            
		}
    }

    public void Setup_Game()
    {
        void Setup_Board()
        {
            board = new IPiece[8,8];
            for (int i = 0; i < 8; i++)
            {
                board[i, 1] = Pawn.Load(players[0]);
                AddChild((Pawn)board[i, 1]);
                (board[i, 1] as Pawn).Position = Grid_to_Pixel(new Vector2(i, 1));
                board[i, 6] = Pawn.Load(players[1]);
                AddChild((Pawn)board[i, 6]);
                (board[i, 6] as Pawn).Position = Grid_to_Pixel(new Vector2(i, 6));
            }
        }
        
        Setup_Board();
        GD.Print(board.Length);
    }

    public Vector2 Pixel_to_Grid(Vector2 position)
    {
        return ((position-board_start)/((board_end - board_start) / 8)).Floor();
    }
    public Vector2 Grid_to_Pixel(Vector2 cell)
    {
        return (board_end - board_start) * cell/8+board_start+cell_offset;
    }
}

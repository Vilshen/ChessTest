
using Chess.Pieces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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
    int curr_player;

    int clock_length;
    int per_move_clock_bonus;

    Sprite2D after_move_bg_start;
    Sprite2D after_move_bg_end;

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

        players[(int)Team_Enum.White].Setup(Team_Enum.White, clock_length);
        players[(int)Team_Enum.Black].Setup(Team_Enum.Black, clock_length);

        curr_player = (int)Team_Enum.White;

        after_move_bg_start = (Sprite2D)GetNode("After_Move_Background_Start");
        after_move_bg_end = (Sprite2D)GetNode("After_Move_Background_End");

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
            //Within board sprite
			if (mouseEvent.Position.X >= board_start.X && mouseEvent.Position.Y >= board_start.Y && mouseEvent.Position.X <= board_end.X && mouseEvent.Position.Y <= board_end.Y)
            {
                Board_Click(mouseEvent.Position);               
            }  
		}
    }

    void Board_Click(Vector2 click_position)
    {
        Vector2I cell = (Vector2I)Pixel_to_Grid(click_position);


        if (board[cell.X, cell.Y] is not null)
        {
            if (board[cell.X, cell.Y].owner_player == players[curr_player])
            {
                board[cell.X, cell.Y].Click(players[curr_player]);
            }
            else if(players[curr_player].selected_piece is not null)
            {
                Capture(players[curr_player].selected_piece, board[cell.X, cell.Y]);
            }
        }
        else if (players[curr_player].selected_piece is not null)
        {
            Move(players[curr_player].selected_piece, cell);
        }
    }

    public void Setup_Game()
    {
        void Setup_Board() //should be able to plug pretty much any rectangular board
        {
            board = new IPiece[8,8];
            for (int i = 0; i < 8; i++)
            {
                board[i, 6] = Pawn.Load(players[0], new Vector2I(i, 6));
                AddChild((Pawn)board[i, 6]);
                (board[i, 6] as Pawn).Position = Grid_to_Pixel(new Vector2(i, 6));
                board[i, 1] = Pawn.Load(players[1], new Vector2I(i, 1));
                AddChild((Pawn)board[i, 1]);
                (board[i, 1] as Pawn).Position = Grid_to_Pixel(new Vector2(i, 1));
            }
            board[4, 7] = King.Load(players[0], new Vector2I(4, 7));
            players[0].checkmate_target = board[4, 7];
            AddChild((King)board[4, 7]);
            (board[4, 7] as King).Position = Grid_to_Pixel(new Vector2(4, 7));
            board[4, 0] = King.Load(players[1], new Vector2I(4, 0));
            players[1].checkmate_target = board[4, 0];
            AddChild((King)board[4, 0]);
            (board[4, 0] as King).Position = Grid_to_Pixel(new Vector2(4, 0));

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i,j] is IPiece_Directional dir_piece)
                    {
                        if (board[i, j].owner_player.id == Team_Enum.Black)
                        {
                            dir_piece.Flip();
                        }
                    }
                }
            }

        }
        
        Setup_Board();
        
    }
    //Screen coordinates to board cell
    public Vector2 Pixel_to_Grid(Vector2 position)
    {
        return ((position-board_start)/((board_end - board_start) / 8)).Floor();
    }
    //Board cell to screen coordinates
    public Vector2 Grid_to_Pixel(Vector2 cell)
    {
        return (board_end - board_start) * cell/8+board_start+cell_offset;
    }

    public void Move(IPiece piece, Vector2I target)
    {
        if (!piece.All_Destinations(board).Contains(target))
        {
            return;
        }
        Node2D subject = piece as Node2D;

        Vector2I original_cell = piece.board_position;
        if (!Move_Safety(players[curr_player], original_cell, target))
        {
            return;
        }
        board[original_cell.X, original_cell.Y] = null;
        board[target.X, target.Y] = piece;

        

        
        subject.Position = Grid_to_Pixel(target);        
        piece.board_position = target;


        piece.has_moved = true;
        piece.Deselect();
        after_move_bg_start.Position = Grid_to_Pixel(original_cell);
        after_move_bg_end.Position = Grid_to_Pixel(target);

        curr_player = (curr_player + 1) % 2;
        Rotate_Board();

    }

    public void Capture(IPiece caller, IPiece target)
    {

    }

    //Simulates a move and checks if the King would be attacked
    public bool Move_Safety(Player current_player,Vector2I origin,Vector2I destination)
    {

        if (current_player.checkmate_target is null)
        {
            return true;
        }

        IPiece mover = board[origin.X,origin.Y];
        IPiece target = board[destination.X, destination.Y];

        board[origin.X, origin.Y] = null;
        board[destination.X, destination.Y] = mover;
        mover.board_position = destination;

        if (target is not null) 
        {
            target.board_position = new Vector2I(-1000, -1000);
        }
        

        HashSet<Vector2I> attacked_cells = new HashSet<Vector2I>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i,j] is not null && board[i, j].owner_player != current_player)
                {
                    attacked_cells.UnionWith(board[i, j].All_Destinations(board));
                }
            }
        }
        bool safety;

        if (attacked_cells.Contains(current_player.checkmate_target.board_position))
        {
            safety = false;
        }
        else
        {
            safety = true;
        }

        board[origin.X, origin.Y] = mover;
        mover.board_position = origin;
        board[destination.X, destination.Y] = target;
        if (target is not null)
        {
            target.board_position = destination;
        }

        return safety;

    }

    void Rotate_Board()
    {

        Vector2 player_buffer = players[0].Position;
        players[0].Position = players[1].Position;
        players[1].Position = player_buffer;

        IPiece buffer = null;
        int x_len = board.GetLength(0)-1;
        int y_len = board.GetLength(1)-1;

        for (int i = 0; i < (x_len+1); i++) 
        {
            for (int j = 0; j < (y_len+1)/2; j++)
            {
                Swap(i, j);
            }
        }
        if (y_len % 2 == 0)
        {
            int j = y_len / 2;
            for (int i = 0; i < (x_len + 1) / 2; i++)
            {
                Swap(i, j);
            }
        }
        for (int i = 0; i < (x_len + 1); i++)
        {
            for (int j = 0; j < (y_len + 1); j++)
            {
                if (board[i, j] is not null)
                {
                    var cell = new Vector2I(i, j);
                    board[i, j].Update_Position(cell, Grid_to_Pixel(cell));

                    if (board[i, j] is IPiece_Directional dir_piece)
                    {
                        dir_piece.Flip();
                    }

                }
            }
        }

        void Swap(int i,int j)
        {
            buffer = board[i, j];
            board[i, j] = board[x_len - i, y_len - j];
            board[x_len - i, y_len - j] = buffer;
        }


    }

}

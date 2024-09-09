
using Chess.Pieces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class Game : Node
{
    Sprite2D board_texture;
    Vector2 board_start;
    Vector2 board_end;
    Vector2 cell_offset;

    Sprite2D board_coords_white;
    Sprite2D board_coords_black;

    IPiece[,] board;

    Player[] players;
    int curr_player;

    int clock_length;
    int per_move_clock_bonus;

    Sprite2D after_move_bg_start;
    Sprite2D after_move_bg_end;

    List<string> moves;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        //
        clock_length = 600;
        //

        board_texture = (Sprite2D)GetNode("Board");
        board_coords_white = (Sprite2D)GetNode("Board/Coordinates_White");
        board_coords_black = (Sprite2D)GetNode("Board/Coordinates_Black");

        board_start = board_texture.Position - board_texture.Texture.GetSize() / 2;
        board_end = board_texture.Texture.GetSize() + board_start;
        cell_offset = (board_end - board_start) / 16;

        players = new Player[] { (Player)GetNode("Player_White"), (Player)GetNode("Player_Black") };

        players[(int)Team_Enum.White].Setup(Team_Enum.White, clock_length);
        players[(int)Team_Enum.Black].Setup(Team_Enum.Black, clock_length);

        curr_player = (int)Team_Enum.White;

        after_move_bg_start = (Sprite2D)GetNode("After_Move_Background_Start");
        after_move_bg_end = (Sprite2D)GetNode("After_Move_Background_End");

        moves = new List<string>();

        Setup_Board();
        players[0].Toggle_Clock();


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
            else if (players[curr_player].selected_piece is not null)
            {
                Move(players[curr_player].selected_piece, cell);
            }
        }
        else if (players[curr_player].selected_piece is not null)
        {
            Move(players[curr_player].selected_piece, cell);
        }

    }

    public void Setup_Board()  //should be able to plug pretty much any rectangular board
    {
        board = new IPiece[8, 8];

        for (int i = 0; i < 8; i++)
        {
            Set_Piece<Pawn>(players[0], i, 6);
            Set_Piece<Pawn>(players[1], i, 1);
        }

        Set_Piece<King>(players[0], 4, 7);
        players[0].checkmate_target = board[4, 7];

        Set_Piece<King>(players[1], 4, 0);
        players[1].checkmate_target = board[4, 0];

        Set_Piece<Queen>(players[0], 3, 7);
        Set_Piece<Queen>(players[1], 3, 0);

        Set_Piece<Bishop>(players[0], 2, 7);
        Set_Piece<Bishop>(players[0], 5, 7);
        Set_Piece<Bishop>(players[1], 2, 0);
        Set_Piece<Bishop>(players[1], 5, 0);

        Set_Piece<Knight>(players[0], 1, 7);
        Set_Piece<Knight>(players[0], 6, 7);
        Set_Piece<Knight>(players[1], 1, 0);
        Set_Piece<Knight>(players[1], 6, 0);

        Set_Piece<Rook>(players[0], 0, 7);
        Set_Piece<Rook>(players[0], 7, 7);
        Set_Piece<Rook>(players[1], 0, 0);
        Set_Piece<Rook>(players[1], 7, 0);

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] is IPiece_Directional dir_piece)
                {
                    if (board[i, j].owner_player.id == Team_Enum.Black)
                    {
                        dir_piece.Flip();
                    }
                }
            }
        }

        void Set_Piece<T>(Player player, int i, int j)
        {
            object[] args = { player, new Vector2I(i, j) };
            board[i, j] = (IPiece)typeof(T).GetMethod("Load").Invoke(null, args);
            AddChild((Node)board[i, j]);
            (board[i, j] as Node2D).Position = Grid_to_Pixel(new Vector2(i, j));
        }
        
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
        (List<Vector2I> normal_moves, List<Vector2I> special_moves) = piece.All_Destinations(board);

        if (normal_moves.Contains(target)) 
        {
            if (!Move_Safety(players[curr_player], piece.board_position, target))
            {
                return;
            }
            Normal_Move(piece, target);
            
        }
        else
        {
            if (special_moves is null || !special_moves.Contains(target)) { return; }
            if (!piece.Check_Special_Move(board, target))
            {
                return;
            }
            else
            {
                (IPiece secondary_target, Vector2I sec_target_origin, Vector2I? sec_target_dest) = piece.Perform_Special_Move(board, target);
                board[sec_target_origin.X, sec_target_origin.Y] = null;
                if (!Move_Safety(players[curr_player], piece.board_position, target))
                {
                    board[sec_target_origin.X, sec_target_origin.Y] = secondary_target;//UNDO UNDO
                    return;
                }
                if (sec_target_dest is null)
                {
                    Process_Capture(secondary_target);
                }
                else
                {
                    board[((Vector2I)sec_target_dest).X, ((Vector2I)sec_target_dest).Y] = secondary_target;
                    secondary_target.board_position = (Vector2I)sec_target_dest;
                }
                Normal_Move(piece, target);

            }
        }

        

    }

    void Normal_Move(IPiece piece, Vector2I target)
    {
        Node2D subject = piece as Node2D;
        Vector2I original_cell = piece.board_position;

        Record_Move(piece, target);

        board[original_cell.X, original_cell.Y] = null;

        if (board[target.X, target.Y] is not null)
        {
            Process_Capture(board[target.X, target.Y]);
        }
        board[target.X, target.Y] = piece;

        subject.Position = Grid_to_Pixel(target);
        piece.board_position = target;


        piece.has_moved = true;
        piece.Deselect();


        after_move_bg_start.Position = Grid_to_Pixel(original_cell);
        after_move_bg_end.Position = Grid_to_Pixel(target);

        Next_Turn();
    }

    //Simulates a move and checks if the King would be attacked
    public bool Move_Safety(Player current_player,Vector2I origin,Vector2I destination)
    {

        if (current_player.checkmate_target is null)
        {
            return true;
        }
        if (current_player != players[curr_player])
        {
            return true; //Response moves ignore checks
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
                    attacked_cells.UnionWith(Combine_Destinations(i,j));
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

    void Process_Capture(IPiece piece)
    {
        (piece as Node).QueueFree();
        players[curr_player].Record_Capture(piece);
    }
    void Rotate_Board()
    {
        board_coords_white.Visible = !board_coords_white.Visible;
        board_coords_black.Visible = !board_coords_black.Visible;

        Vector2 player_buffer = players[0].Position;
        players[0].Position = players[1].Position;
        players[1].Position = player_buffer;

        IPiece buffer = null;
        int x_len = board.GetLength(0) - 1;
        int y_len = board.GetLength(1) - 1;

        for (int i = 0; i < (x_len + 1); i++)
        {
            for (int j = 0; j < (y_len + 1) / 2; j++)
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

        after_move_bg_start.Position = board_end + board_start - after_move_bg_start.Position;
        after_move_bg_end.Position = board_end + board_start - after_move_bg_end.Position;


        void Swap(int i, int j)
        {
            buffer = board[i, j];
            board[i, j] = board[x_len - i, y_len - j];
            board[x_len - i, y_len - j] = buffer;
        }


    }

    void Next_Turn()
    {
        players[curr_player].checkmate_target.attacked = false;

        players[1 - curr_player].checkmate_target.attacked = All_Safe_Moves(players[curr_player])
            .Contains(players[1 - curr_player].checkmate_target.board_position);


        curr_player = 1 - curr_player;
        HashSet<Vector2I> safe_moves = All_Safe_Moves(players[curr_player]);
        if (safe_moves.Count==0)
        {
            if (players[curr_player].checkmate_target.attacked)
            {
                Checkmate(players[curr_player]);
            }
            else
            {
                Declare_Draw();
            }
        }
        players[0].Toggle_Clock();
        players[1].Toggle_Clock();
        Rotate_Board();
        players[1 - curr_player].checkmate_target.Set_Background();
        players[curr_player].checkmate_target.Set_Background();
    }

    HashSet<Vector2I> All_Safe_Moves(Player player)
    {
        HashSet<Vector2I> moves = new HashSet<Vector2I>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] is IPiece && board[i, j].owner_player == player)
                {
                    moves.UnionWith(
                        Combine_Destinations(i,j)
                        .Where(move => Move_Safety(player, board[i, j].board_position, move)));
                }
            }
        }
        return moves;
    }


    void Checkmate(Player player)
    {
        GD.Print($"TODO, player {player.id} got mated");
    }

    void Declare_Draw()
    {
        GD.Print($"TODO, game ended in a draw");
    }

    void Record_Move(IPiece piece, Vector2I destination)
    {
        bool capture = board[destination.X, destination.Y] is IPiece;
        string output;
        if (moves.Count % 2 == 0) //I regret flipping the board on moves now
        {
            char origin_file = (char)('a' + piece.board_position.X);
            char target_file = (char)('a' + destination.X);
            output = $"{piece.Notation()}{origin_file}{board.GetLength(1) - piece.board_position.Y}{(capture ? "x" : "")}{target_file}{board.GetLength(1) - destination.Y}";
        }
        else
        {
            char origin_file = (char)('a' + (board.GetLength(0) - piece.board_position.X-1));
            char target_file = (char)('a' + (board.GetLength(0) - destination.X-1));
            output = $"{piece.Notation()}{origin_file}{piece.board_position.Y+1}{(capture ? "x" : "")}{target_file}{destination.Y+1}";
        }
        moves.Add(output);
    }

    public string Last_Move() //I hate En Passant
    {
        if (moves.Count == 0)
        {
            return "";
        }
        return moves.Last();
    }

    List<Vector2I> Combine_Destinations(int i,int j)
    {
        (List<Vector2I>, List<Vector2I>) tupl = board[i, j].All_Destinations(board);
        if (tupl.Item2 is null)
        {
            return tupl.Item1;
        }
        IEnumerable<Vector2I> all_destinations = tupl.Item1
            .Union(tupl.Item2.Where(sp_move => board[i,j].Check_Special_Move(board,sp_move)));

        return all_destinations.ToList();
    }
}

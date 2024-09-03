using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public partial class Player : Node2D
{
    public Dictionary<Type,int> captures { get; set; }

    public Team_Enum id { get; private set; }
    public IPiece selected_piece;

    Timer clock;
    Label clock_label;

    public IPiece checkmate_target;
    public void Setup(Team_Enum player_id,int clock_length)
    {
        id = player_id;
        clock = (Timer)GetNode("Clock");
        clock.WaitTime = clock_length;

        clock_label = (Label)GetNode("Clock_Label");
        clock.Start(); 
        clock.Paused = true;

        captures = new Dictionary<Type, int>();

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        clock_label.Text =String.Format("{0:D2}:{1:D2}", (int)(clock.TimeLeft / 60), (int)clock.TimeLeft % 60);
    }

    public void Record_Capture(IPiece piece)
    {
        Type p_type = piece.GetType();
        if (captures.ContainsKey(p_type)){
            captures[p_type] += 1;
        }
        else
        {
            captures.Add(p_type, 1);
        }
    }

    public void Toggle_Clock()
    {
        clock.Paused = !clock.Paused;
    }

}

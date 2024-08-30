using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public partial class Player : Node2D
{
    public Dictionary<IPiece,int> captures { get; private set; }
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

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        clock_label.Text =String.Format("{0:D2}:{1:D2}", (int)(clock.TimeLeft / 60), (int)clock.TimeLeft % 60);
    }

}

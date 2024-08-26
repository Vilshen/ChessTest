using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public partial class Player : Node
{
    Dictionary<IPiece,int> captures;
    public int id;
    Timer clock;

    public void Setup(int player_id,int clock_length)
    {
        id = player_id;
        clock = (Timer)GetNode("Clock");
        clock.WaitTime = clock_length;

    }

}

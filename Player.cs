using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public partial class Player : Node2D
{
    public Team_Enum id { get; private set; }
    public IPiece selected_piece;

    Timer clock;
    Label clock_label;
    Node2D capture_list_node;

    public IPiece checkmate_target;
    public void Setup(Team_Enum player_id, int clock_length)
    {
        id = player_id;
        clock = (Timer)GetNode("Clock");
        clock.WaitTime = clock_length;

        clock_label = (Label)GetNode("Clock_Label");

        capture_list_node = (Node2D)GetNode("Capture_List");

        clock.Start();
        clock.Paused = true;
        Game game = (Game)GetParent();
        clock.Timeout += () => game.Checkmate(this, "timeout");


    }


    public override void _Process(double delta)
    {
        base._Process(delta);
        clock_label.Text = String.Format("{0:D2}:{1:D2}", (int)(clock.TimeLeft / 60), (int)clock.TimeLeft % 60);
    }

    public void Record_Capture(IPiece new_capture)
    {
        Sprite2D new_sprite = new Sprite2D();
        new_sprite.Texture = GD.Load<Texture2D>($"res://assets/{new_capture.GetType()}_{(1 - this.id).ToString()}.png");
        new_sprite.ApplyScale(new Vector2((float)0.25, (float)0.25));
        new_sprite.Position += new Vector2(capture_list_node.GetChildren().Count * 32,0);
        capture_list_node.AddChild(new_sprite);
    }

    public void Toggle_Clock()
    {
        clock.Paused = !clock.Paused;
    }

    public void Add_Time(int bonus_time)
    {
        clock.Start(clock.TimeLeft + bonus_time);
    }

}

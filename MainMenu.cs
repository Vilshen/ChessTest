using Godot;
using System;

public partial class MainMenu : Node
{
	Game game;
	Button launch_button;
	OptionButton timer_selector;
    OptionButton bonus_selector;
    Button exit_button;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		launch_button = (Button)GetNode("Launch_Button");
        launch_button.ButtonUp += Start_Game;

		timer_selector = (OptionButton)GetNode("Timer_Selector");
        bonus_selector = (OptionButton)GetNode("Bonus_Selector");

        exit_button = (Button)GetNode("Exit_Button");
        exit_button.ButtonUp += () => GetTree().Quit();


    }

	void Start_Game()
	{
		int timer = 0;
		switch (timer_selector.Selected) {
			case 0: timer = 0; break;
            case 1: timer = 60 * 1; break;
            case 2: timer = 60 * 3; break;
            case 3: timer = 60 * 5; break;
            case 4: timer = 60 * 10; break;
            case 5: timer = 60 * 15; break;
            case 6: timer = 60 * 20; break;
            case 7: timer = 60 * 30; break;
            case 8: timer = 60 * 60; break;
        }
		int bonus_time = 0;
        switch (bonus_selector.Selected)
        {
            case 0: bonus_time = 0; break;
            case 1: bonus_time = 1; break;
            case 2: bonus_time = 2; break;
            case 3: bonus_time = 3; break;
            case 4: bonus_time = 4; break;
            case 5: bonus_time = 5; break;
            case 6: bonus_time = 10; break;
        }
        Set_All_Invisible();
        game = (Game)(GD.Load<PackedScene>("res://game.tscn")).Instantiate();
        game.clock_length = timer;
        game.per_move_clock_bonus = bonus_time;
        AddChild(game);
	}

	public void End_Game()
	{
		game.QueueFree();
        Set_All_Visible();

    }
    void Set_All_Invisible()
    {
        launch_button.Visible = false;
        timer_selector.Visible = false;
        bonus_selector.Visible = false;
    }
    void Set_All_Visible()
    {
        launch_button.Visible = true;
        timer_selector.Visible = true;
        bonus_selector.Visible = true;
    }
}

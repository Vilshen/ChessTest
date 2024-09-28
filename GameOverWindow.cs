using Godot;
using System;

public partial class GameOverWindow : Sprite2D
{
	Label game_over_description;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		game_over_description = (Label)GetNode("Game_Over_Description_Label");
		Button return_to_menu_button = (Button)GetNode("Return_To_Menu_Button");
		return_to_menu_button.Pressed += End_Game;

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Activate(bool checkmate, Player player, string details)
	{
		if (this.Visible)
		{
			return;
		}
		this.Show();
		if (checkmate)
		{
			game_over_description.Text = $"{player.id.ToString()} has won via {details}.";

        }
		else
		{
			game_over_description.Text = $"Game has ended in a draw due to {details}.";

        }

    }

	void End_Game()
	{
		
	}

}

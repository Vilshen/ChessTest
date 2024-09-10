using Godot;
using System;
using System.Collections.Generic;

public partial class PromotionHandler : Node2D
{
	Button[] buttons;
	Type[] pieces;

    Game game;

	IPiece promotee;
	public void Load(Type[] pieces)
	{
		this.pieces = pieces;
		buttons = new Button[pieces.Length];
        for (int i = 0; i < pieces.Length; i++)
		{
			buttons[i] = new Button();
            buttons[i].ActionMode = BaseButton.ActionModeEnum.Release;
			Type p_type = pieces[i];
			buttons[i].Pressed += () => game.Promote(promotee,p_type);
			buttons[i].OffsetLeft = (128+6) * i;
			AddChild(buttons[i]);
			
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		game = (Game)GetParent();
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Set_Icons(Team_Enum player)
	{
        for (int i = 0; i < pieces.Length; i++)
        {
            buttons[i].Icon = GD.Load<Texture2D>($"res://assets/{pieces[i]}_{player.ToString()}.png");
        }
    }

	public void Deploy(IPiece piece,Vector2 position)
	{
		Set_Icons(piece.owner_player.id);
		Position = position+new Vector2(-64,128);
		promotee = piece;
	}

}

using Godot;
using System;

public partial class ScrollTextbox : ScrollContainer
{
	ScrollBar scroll_bar;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scroll_bar = this.GetVScrollBar();
		this.ChildOrderChanged += () => this.ScrollVertical = (int)scroll_bar.MaxValue;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

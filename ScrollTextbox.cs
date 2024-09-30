using Godot;
using System;

public partial class ScrollTextbox : ScrollContainer
{
	ScrollBar scroll_bar;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scroll_bar = this.GetVScrollBar();
		VBoxContainer container = (VBoxContainer)GetChild(0);
		container.ChildOrderChanged += async () => { 
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); //without this it takes 1-2 extra turns before it actually scrolls
			this.ScrollVertical = (int)scroll_bar.MaxValue; 
		};
	}

}

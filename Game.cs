using Godot;
using System;

public partial class Game : Node2D
{

  [Export]
  public Player player;
  [Export]
  public HBoxContainer healthBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	int i = 0;
	foreach (Node heart in healthBar.GetChildren()) {
	  ((TextureRect)heart).Modulate = Colors.White * Mathf.Clamp(player.Health - i, 0, 1);
	  i++;
	}
	}
}

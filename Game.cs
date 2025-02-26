using Godot;
using System;

public partial class Game : Node2D
{

	[Export]
	public Player player;
	[Export]
	public HBoxContainer healthBar;
	[Export]
	public NormalizedCamera camera;

	public static Game instance;

	private void _OnTreeChildEnter(Node node) {
		NodeAttribute.AssignNodes(node);
		GD.Print(node.GetType().Name, " entered tree");
		node.ChildEnteredTree += _OnTreeChildEnter;
	}

	public override void _EnterTree() {
		GetTree().Root.ChildEnteredTree += _OnTreeChildEnter;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		int i = 0;
		foreach (Node heart in healthBar.GetChildren()) {
			((TextureRect)heart).Modulate = Colors.White * Mathf.Clamp(player.Health - i, 0, 1);
			i++;
		}
		foreach (var child in GetChildren()) {
			if (child is Room room && room != camera.currentRoom) {
				if (room.Rect.HasPoint(player.Position)) {
					camera.TransitionToRoom(room);
				}
			}
		}
	}
}

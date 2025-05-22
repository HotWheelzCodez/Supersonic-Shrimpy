using Godot;
using System;

public partial class EnemySpawner : Node2D {

	public static string[] enemyPaths = [
		"res://enemies/crab/Crab.tscn",
		"res://enemies/spinefish/Spinefish.tscn",
	];

	public static Enemy[] enemies;
	public static Random rand = new();

	public override void _Ready() {
		if (enemies == null) {
			enemies = new Enemy[enemyPaths.Length];
			for (int i = 0; i < enemyPaths.Length; i++) {
				var scene = GD.Load<PackedScene>(enemyPaths[i]);
				GD.Print("enemy scene " + scene.ToString());
				enemies[i] = scene.Instantiate<Enemy>();
			}
		}
		Room room = GetParent<Room>();
		GD.Print(room);
		enemies.Shuffle(rand);
		foreach (var enemy in enemies) {
			if (room.enemyPoints >= enemy.cost) {
				room.enemyPoints -= enemy.cost;
				var node = (Enemy)enemy.Duplicate((int)DuplicateFlags.UseInstantiation);
				//node.Position = Position;
				node.player = Game.instance.player;
				AddChild(node);
				break;
			}
		}
	}
}

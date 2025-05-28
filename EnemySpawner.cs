using Godot;
using System;

public partial class EnemySpawner : Node2D {

	public static string[] enemyPaths = [
		"res://enemies/crab/Crab.tscn",
		"res://enemies/spinefish/Spinefish.tscn",
		"res://enemies/spinefish/Snipefish.tscn",
	];

	public static Enemy[] enemies;
	public static Random rand = new();

	public bool spawned = false;
	public GpuParticles2D particles;

	public override void _Ready() {
		particles = GD.Load<PackedScene>("res://enemies/SpawnParticles.tscn").Instantiate<GpuParticles2D>();
		AddChild(particles);
	}

	public override void _Process(double delta) {
		if (!spawned) {
			spawned = true;
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
						particles.Emitting = true;
					room.enemyPoints -= enemy.cost;
					var node = (Enemy)enemy.Duplicate((int)DuplicateFlags.UseInstantiation);
					node.Position = Position;
					node.player = Game.instance.player;
					room.AddChild(node);
					break;
				}
			}
		}
	}
}

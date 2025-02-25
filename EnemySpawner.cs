using Godot;

public partial class EnemySpawner : Node2D {

	[Export]
	public PackedScene enemy;
	[Export]
	public Player target;
	[Export]
	public float rate;
	[Export]
	public int limit;
	[Export]
	public bool enabled;

	private Enemy[] enemies;
	private float wait;


	public override void _Ready() {
		enemies = new Enemy[limit];
	}

	public override void _Process(double delta) {
		wait -= (float)delta;
		if (enabled && wait <= 0) {
			for (int i = 0; i < limit; i++) {
				var e = enemies[i];
				if (e == null || !IsInstanceValid(e)) {
					wait += 1 / rate;
					var node = (Enemy)enemy.Instantiate();
					node.Transform = Transform;
					node.player = target;
					GetParent().AddChild(node);
					enemies[i] = node;
					break;
				}
			}
		}
	}

}

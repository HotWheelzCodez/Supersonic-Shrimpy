using Godot;

public partial class EnemySpawner : Node2D {

	[Export]
	public PackedScene enemy;
	[Export]
	public Player target;
	[Export]
	public float rate;

	public override void _Process(double delta) {
		if (GD.Randf() < delta * rate) {
			var node = (Enemy)enemy.Instantiate();
			node.Transform = Transform;
			node.player = target;
			GetParent().AddChild(node);
		}
	}

}

using Godot;
using System;

public partial class SpiderCrab : Boss
{
	public PackedScene leg;
	[Export]
	public Vector2 legOrigin;
	[Node("AnimationPlayer")]
	public AnimationPlayer anims;
	[Node("BossAnims")]
	public AnimationPlayer bossAnims;

	public override void _Ready() {
		base._Ready();
		leg = GD.Load<PackedScene>("res://bosses/spidercrab/spider_crab_leg.tscn");
	}

	/*
	public void _OnStompTimer() {
		var player = Game.instance.player;
		var vel = player.Velocity.Rotated(Mathf.Pow(GD.Randf() * 2 - 1, 2) * Mathf.Pi) * GD.Randf();
		var node = (SpiderCrabLeg)leg.Instantiate();
		node.Position = player.GlobalPosition + vel * 0.5f - GlobalPosition;
		AddChild(node);
	}
	*/

	public override void _Process(double delta) {
		if (health > 0 && !anims.IsPlaying()) {
			PickLegAttack();
		}
		//anims.SpeedScale = 2 - (health / maxHealth);
	}

	public void PickLegAttack() {
		var list = anims.GetAnimationList();
		var i = GD.RandRange(0, list.Length-1);
		legOrigin = Game.instance.player.GlobalPosition;
		anims.Play(list[i]);
		GD.Print("leg", i);
	}

	public void SpawnLeg(Vector2 offset) {
		var node = (SpiderCrabLeg)leg.Instantiate();
		node.Position = legOrigin + offset - GlobalPosition + Vector2.Right.Rotated(GD.Randf() * Mathf.Pi * 2) * (1 - health / maxHealth) * 16;
		AddChild(node);
	}

	public override void Die() {
		bossAnims.Play("die");
		((ShaderMaterial)GetNode<Sprite2D>("Body").Material).SetShaderParameter("origin", GetNode<Node2D>("Particles").GlobalPosition);
	}
}

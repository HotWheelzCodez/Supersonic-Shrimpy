using Godot;
using System;
using System.Collections.Generic;


public class Claw {

	public Player player;
  public List<dynamic> behaviors = new();
  public Stats stats = new();

  public float punch = 0;

  public Claw(Player player) {
    this.player = player;
  }

  public void Update(float delta) {
    GD.Print(punch, stats.cooldown);
    punch = Mathf.MoveToward(punch, 0, delta / stats.cooldown);
    foreach (var behavior in behaviors) {
      behavior.Update(delta);
    }
  }

  public void Punch() {
    if (punch > 0) return;
    punch = 1;
    foreach (var behavior in behaviors) {
      behavior.Punch();
    }
  }

  public struct Stats {
    public float damage = 1;
    public float cooldown = 0.2f;

    public Stats() {}
  }
}

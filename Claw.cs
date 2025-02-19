using Godot;
using System;
using System.Collections.Generic;

public class Claw
{
	public Player player;
  public List<object> behaviors;
  public Dictionary<string, float> stats = new() {
    {"damage", 1},
    {"cooldown", 0.2f}
  };

  public void Update(float delta) {

  }
}

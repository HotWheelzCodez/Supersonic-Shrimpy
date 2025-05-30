using Godot;
using System;
using System.Collections.Generic;

public partial class GameOver : Control
{
	public char[] name = {'A','A','A'};
	public int idx = 0;
	public double time = 0;

	[Node("%Initials")]
	public GridContainer initials;
	[Node("%GreatScore")]
	public Label greatScore;

	[Node("%Leaderboard")]
	public GridContainer leaderboard;

	public List<(string, int)> scores = [
		("N/A", 0),
		("N/A", 0),
		("N/A", 0),
		("N/A", 0),
		("N/A", 0),
		("N/A", 0),
		("N/A", 0),
		("N/A", 0),
		("N/A", 0),
		("N/A", 0),
	];

	public override void _Ready() {
		var file = FileAccess.Open("user://scores.csv", FileAccess.ModeFlags.Read);
		if (file != null) {
			int i = 0;
			for (var line = file.GetCsvLine(); line.Length > 0; line = file.GetCsvLine()) {
				scores[i] = (line[0], line[1].ToInt());
				i++;
			}
			file.Close();
		}
	}

	public void InsertScore(string name, int score) {
		for (int i = 0; i < 10; i++) {
			if (scores[i].Item2 < score) {
				scores.Insert(i, (name, score));
				scores.RemoveAt(10);
				break;
			}
		}
	}

	public void WriteScores() {
		var file = FileAccess.Open("user://scores.csv", FileAccess.ModeFlags.Write);
		foreach (var score in scores) {
			file.StoreString($"{score.Item1}, {score.Item2.ToString()}\n");
		}
		file.Close();
	}

	public override void _Process(double delta) {
		if (!Visible) return;
		if (time == 0 && Game.instance.Score > scores[9].Item2) {
			greatScore.Visible = true;
			initials.Visible = true;
			leaderboard.Visible = false;
		}
		time += delta;
		name[idx] = (char)(Mathf.PosMod(((int)(name[idx] - 'A') + (Input.IsActionJustPressed("move_down") ? 1 : 0) - (Input.IsActionJustPressed("move_up") ? 1 : 0)), 26) + 'A');
		idx = (idx + (Input.IsActionJustPressed("move_right") ? 1 : 0) - (Input.IsActionJustPressed("move_left") ? 1 : 0)) % 3;
		if (time > 3 && (Input.IsActionJustPressed("punch_left") || Input.IsActionJustPressed("punch_right"))) {
			if (greatScore.Visible) {
				greatScore.Visible = false;
				initials.Visible = false;
				leaderboard.Visible = true;
				InsertScore($"{name[0]}{name[1]}{name[2]}", Game.instance.Score);
				WriteScores();
			} else {
				Game.instance.loading.Visible = true;
			}
		}
		for (int i = leaderboard.GetChildCount(); i --> 0;) {
			var c = leaderboard.GetChild<Label>(i);
			var row = i / 2;
			switch (i % 2) {
				case 0:
					c.Text = scores[row].Item1;
					break;
				case 1:
					c.Text = scores[row].Item2.ToString();
					break;
			}
		}
		for (int i = initials.GetChildCount(); i --> 0;) {
			var c = initials.GetChild<Label>(i);
			var col = i % 3;
			switch (i / 3) {
				case 0:
					if (col != idx) {
						c.Text = "";
					} else {
						c.Text = ((char)(Mathf.PosMod((name[idx] - 'A' - 1), 26) + 'A')).ToString();
					}
					break;
				case 1:
					c.Text = name[col].ToString();
					break;
				case 2:
					if (col != idx) {
						c.Text = "";
					} else {
						c.Text = ((char)(Mathf.PosMod((name[idx] - 'A' + 1), 26) + 'A')).ToString();
					}
					break;
			}
		}
	}
}

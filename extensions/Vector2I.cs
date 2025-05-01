using Godot;

public static class Vector2IExt {
	public static Vector2 Float(this Vector2I v) {
		return new (v.X, v.Y);
	}
}

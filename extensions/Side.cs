using Godot;

public static class SideExt {
	public static Side Reverse(this Side side) => (side) switch {
		Side.Top => Side.Bottom,
		Side.Bottom => Side.Top,
		Side.Left => Side.Right,
		Side.Right => Side.Left,
	};
}

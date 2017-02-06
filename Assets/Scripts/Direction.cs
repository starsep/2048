public enum Direction {
	Left,
	Right,
	Up,
	Down
};

static class DirectionMethods {
	public static bool Vertical(this Direction direction) {
		switch (direction) {
		case Direction.Down:
		case Direction.Up:
			return true;
		default:
			return false;
		}
	}

	public static bool Reversed(this Direction direction) {
		switch (direction) {
		case Direction.Right:
		case Direction.Up:
			return true;
		default:
			return false;
		}
	}
}

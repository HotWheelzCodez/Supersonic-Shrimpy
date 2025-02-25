public struct AutoFloat {
	double value;

	public static implicit operator float(AutoFloat x) => (float)x.value;
	public static implicit operator double(AutoFloat x) => x.value;
	public static implicit operator AutoFloat(float x) => new AutoFloat(x);
	public static implicit operator AutoFloat(double x) => new AutoFloat(x);

	public AutoFloat(float x) {
		value = x;
	}
	public AutoFloat(double x) {
		value = x;
	}
}

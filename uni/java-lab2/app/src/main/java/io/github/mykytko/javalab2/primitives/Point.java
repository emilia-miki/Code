package io.github.mykytko.javalab2.primitives;

public class Point {
	private final Float x;
	private final Float y;

	public Point(float x, float y) {
		this.x = x;
		this.y = y;
	}

	public Float getX() { return x; }
	public Float getY() { return y; }

	@Override
	public String toString() {
		return "(" + x + ", " + y + ")";
	}
}

package io.github.mykytko.javalab2.primitives.triangle;

import io.github.mykytko.javalab2.primitives.Color;
import io.github.mykytko.javalab2.primitives.Point;

import java.util.ArrayList;

public class WhiteTriangle extends Triangle {
	private final Color color = Color.WHITE;

	public WhiteTriangle(ArrayList<Point> vertices) throws InvalidPointsAmountException {
		super(vertices);
	}

	public Color getColor() {
		return color;
	}

	@Override
	public String toString() {
		return color + " " + super.toString();
	}
}

package io.github.mykytko.javalab2.factories;

import java.util.ArrayList;
import io.github.mykytko.javalab2.primitives.circle.Circle;
import io.github.mykytko.javalab2.primitives.triangle.InvalidPointsAmountException;
import io.github.mykytko.javalab2.primitives.triangle.Triangle;
import io.github.mykytko.javalab2.primitives.circle.WhiteCircle;
import io.github.mykytko.javalab2.primitives.triangle.WhiteTriangle;
import io.github.mykytko.javalab2.primitives.Point;

public class WhiteFactory implements BaseFactory {
    @Override
    public Circle createCircle(Point center, float radius) {
      return new WhiteCircle(center, radius);
    }

    @Override
    public Triangle createTriangle(ArrayList <Point> vertices) throws InvalidPointsAmountException {
      return new WhiteTriangle(vertices);
    }
}

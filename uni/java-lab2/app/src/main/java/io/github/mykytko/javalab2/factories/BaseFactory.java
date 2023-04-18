package io.github.mykytko.javalab2.factories;

import java.util.ArrayList;

import io.github.mykytko.javalab2.primitives.circle.Circle;
import io.github.mykytko.javalab2.primitives.triangle.InvalidPointsAmountException;
import io.github.mykytko.javalab2.primitives.triangle.Triangle;
import io.github.mykytko.javalab2.primitives.Point;

public interface BaseFactory {
    Circle createCircle(Point center, float radius);
    Triangle createTriangle(ArrayList <Point> vertices) throws InvalidPointsAmountException;
}

package io.github.mykytko.javalab2.primitives.circle;

import io.github.mykytko.javalab2.primitives.Point;
import io.github.mykytko.javalab2.primitives.Color;
import io.github.mykytko.javalab2.primitives.Figure;

public abstract class Circle implements Figure {
    private final Point center;
    private final Float radius;
    private final Color color;

    public Circle(Point a, float b, Color c) {
        center = a;
        radius = b;
        color = c;
    }

    public Float calculateArea() {
        return (float) (Math.PI * Math.pow(radius, 2));
    }

    public Point getCenter() {
        return this.center;
    }

    public Float getRadius() {
        return this.radius;
    }
    
    public Color getColor() {
        return this.color;   
    }

    @Override
    public String toString() {
        return this.color + " circle:" +
               "\nCenter: " + center.toString() +
               "\nRadius: " + this.radius +
               "\nArea: " + calculateArea();
    }
}

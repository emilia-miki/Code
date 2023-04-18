package io.github.mykytko.javalab2;

import io.github.mykytko.javalab2.factories.AbstractFactory;
import io.github.mykytko.javalab2.factories.BaseFactory;
import io.github.mykytko.javalab2.factories.UnsupportedColorException;
import io.github.mykytko.javalab2.primitives.Color;
import io.github.mykytko.javalab2.primitives.Point;
import io.github.mykytko.javalab2.primitives.circle.Circle;
import io.github.mykytko.javalab2.primitives.triangle.InvalidPointsAmountException;
import io.github.mykytko.javalab2.primitives.triangle.Triangle;

import java.util.ArrayList;
import java.util.Random;

public class Main {
    public static void main(String[] args) {
        BaseFactory blackFactory;
        BaseFactory whiteFactory;

        try {
            blackFactory = AbstractFactory.getFactory(Color.BLACK);
            whiteFactory = AbstractFactory.getFactory(Color.WHITE);
        } catch (UnsupportedColorException e) {
            e.printStackTrace();
            return;
        }

        Random rand = new Random();

        var test1 = TestFactory(blackFactory, rand);
        var test2 = TestFactory(whiteFactory, rand);

        System.out.println(test1);
        System.out.println();
        System.out.println(test2);
    }

    private static Pair<Circle, Triangle> TestFactory(BaseFactory factory, Random rand) {
        var points = new ArrayList<Point>();

        for(var i = 0; i < 4; i++) {
            var x = (float) ((float) (rand.nextInt(2000) - 1000) / 10.0);
            var y = (float) ((float) (rand.nextInt(2000) - 1000) / 10.0);
            var point = new Point(x, y);

            points.add(point);
        }

        var center = points.get(0);
        var radius = (float) ((float) (rand.nextInt(500)) / 10.0);
        var circle = factory.createCircle(center, radius);

        points.remove(0);
        Triangle triangle = null;
        try {
            triangle = factory.createTriangle(points);
        } catch (InvalidPointsAmountException e) {
            e.printStackTrace();
        }

        return new Pair<>(circle, triangle);
    }
}

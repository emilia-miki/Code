package io.github.mykytko.javalab2.factories;

import io.github.mykytko.javalab2.primitives.Color;

public class AbstractFactory {
    public static BaseFactory getFactory(Color color) throws UnsupportedColorException {
        return switch (color) {
            case WHITE -> new WhiteFactory();
            case BLACK -> new BlackFactory();
            default -> throw new UnsupportedColorException();
        };
    }
}

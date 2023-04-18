package io.github.mykytko.javalab2.primitives;

public enum Color {
    BLACK {
        @Override
        public String toString() {
            return "black";
        }
    },
    WHITE {
        @Override
        public String toString() {
            return "white";
        }
    }
}

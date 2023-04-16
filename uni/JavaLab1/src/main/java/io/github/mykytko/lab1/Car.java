package io.github.mykytko.lab1;

public final class Car {

    private static int counter = 0;

    private final Integer id;
    private final String brand;
    private final String model;
    private final Integer year;
    private final Integer color;
    private final Integer price;
    private final String registrationNumber;

    public Car(String brand, String model, Integer year,
    		Integer color, Integer price, String registrationNumber) {

    	counter++;
        id = counter;

        this.brand = brand;
        this.model = model;
        this.year = year;
        this.color = color;
        this.price = price;
        this.registrationNumber = registrationNumber;
    }

    @Override
    public String toString() {
        var builder = new StringBuilder();
        var lineSeparator = System.getProperty("line.separator");

        builder.append("Brand: ").append(brand);
        builder.append(lineSeparator);
        builder.append("Model: ").append(model);
        builder.append(lineSeparator);
        builder.append("Year: ").append(Integer.toString(year));
        builder.append(lineSeparator);
        builder.append("Color: #").append(Integer.toHexString(color));
        builder.append(lineSeparator);
        builder.append("Price: ").append(Integer.toString(price));
        builder.append(lineSeparator);
        builder.append("Registration number: ").append(registrationNumber);

        return builder.toString();
    }

    public Integer getId() {
        return id;
    }

    public String getBrand() {
        return brand;
    }

    public String getModel() {
        return model;
    }

    public Integer getYear() {
        return year;
    }

    public Integer getColor() {
        return color;
    }

    public Integer getPrice() {
        return price;
    }

    public String getRegistrationNumber() {
        return registrationNumber;
    }
}

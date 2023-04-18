package io.github.mykytko.lab1;

import java.io.FileOutputStream;
import java.io.IOException;
import java.net.URL;
import java.nio.channels.Channels;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.time.YearMonth;


public class Main {

    private static ArrayList<Car> cars = new ArrayList<Car>();
	
    public static void main(String[] args) {
        var url = "https://raw.githubusercontent.com/rashida048/Datasets/master/cars.csv";
        var fileName = "cars.csv";

        List<String> lines;
        try {
            downloadData(url, fileName);
            lines = Files.readAllLines(Paths.get(fileName), StandardCharsets.UTF_8);
        } catch (IOException ex) {
            System.out.println(ex.getMessage());
            return;
        }

        var rand = new Random();

        for (int i = 1; i < lines.size(); i++) {
            var splits = lines.get(i).split(",");
            var brand = splits[1];
            var model = splits[2];
            var year = Integer.parseInt(splits[0]);
            var color = rand.nextInt(0xffffff + 1);
            var price = 10000 + rand.nextInt(400) * 100 - rand.nextInt(2);
            var registrationNumber = generateRegistrationNumber(rand);

            var car = new Car(brand, model, year, color, price, registrationNumber);
            cars.add(car);
        }
        
        //taskA
        String brandA = "tesla";
        var carsA = taskA(brandA);
        
        System.out.println("Cars with a specific brand(" + brandA + "):\n");

        for (var car : carsA) {
          System.out.println(car.toString() + '\n');
        }
        
        //taskB
        String modelB = "MODEL S (60 kWh battery)";
        int yearsB = 8;
        var carsB = taskB(modelB, yearsB);
        
        System.out.println("Cars with a specific model(" + modelB + ") that were used for " + yearsB + " or more years:\n");

        for (var car : carsB) {
          System.out.println(car.toString() + '\n');
        }
        
        //taskC
        int yearC = 2012;
        int priceC = 10000;
        var carsC = taskC(yearC, priceC);
        
        System.out.println("Cars with a specific year of manufacture(" + yearC + ") that cost " + priceC+ "$ or more:\n");
        
        for (var car : carsC) {
        	System.out.println(car.toString() + '\n');
        }
    }

    private static String generateRegistrationNumber(Random rand) {
    	var number = new StringBuilder();
    	int length = 8;
    	
    	for (int i = 0; i < length; i++) {
    		char symbol = (char)((rand.nextInt(10) + 48) * rand.nextInt(2));
    		
    		if (symbol == 0) {
    			symbol = (char)(rand.nextInt(26) + 65);
    		}
    		
    		number.append(symbol);
    	}
    	
        return number.toString();
    }

    private static ArrayList<Car> taskA(String brand) {
        var carsList = new ArrayList<Car>();

        for (var car : cars) {
          if (car.getBrand().equalsIgnoreCase(brand)) {
            carsList.add(car);
          }
        }

        return carsList;
    }

    private static ArrayList<Car> taskB(String model, int usedFor) {
      var carsList = new ArrayList<Car>();
      int currentYear = YearMonth.now().getYear();

      for (var car : cars) {
        if (
          car.getModel().equalsIgnoreCase(model)
          && (currentYear - car.getYear()) >= usedFor
        ) {
          carsList.add(car);
        }
      }

      return carsList;
    }

    private static ArrayList<Car> taskC(int year, int price) {
    	var carslist = new ArrayList<Car>();
    	
    	for (var car : cars) {
    	  if (
    	    car.getYear().equals(year)
    		&& car.getPrice() >= price
    	  ) {
    		carslist.add(car);
    	  }
    	}
    	
        return carslist;
    }

    private static void downloadData(String url, String fileName) throws IOException {
        var readableByteChannel = Channels.newChannel(new URL(url).openStream());
        var fileOutputStream = new FileOutputStream(fileName);
        var fileChannel = fileOutputStream.getChannel();

        fileChannel.transferFrom(readableByteChannel, 0, Long.MAX_VALUE);
        fileChannel.close();
        fileOutputStream.close();
        readableByteChannel.close();
    }
    
}

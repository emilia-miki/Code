using System.Globalization;
using CsvHelper.Configuration;

var file = File.Open("~/Downloads/minute_weather.csv", FileMode.Open);
var newFile = File.Open("~/Downloads/new_minute_weather.csv", FileMode.Create);

var reader = new CsvHelper.CsvReader(new StreamReader(file), 
    new CsvConfiguration(CultureInfo.CurrentCulture));
var writer = new CsvHelper.CsvWriter(new StreamWriter(newFile),
    new CsvConfiguration(CultureInfo.CurrentCulture));
while (true)
{
    var line = reader.GetRecord();
    if (string.IsNullOrEmpty(line))
    {
        continue;
    }
    
    if (line.)
}
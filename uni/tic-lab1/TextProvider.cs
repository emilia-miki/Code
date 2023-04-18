using System.Text;

namespace Entropy;

public static class TextProvider
{
    public static string GetText()
    {
        Console.WriteLine("Do you want to enter text manually " +
                          "or read from a file?");
        Console.WriteLine("1. Manually");
        Console.WriteLine("2. Read from file");

        while (true)
        {
            Console.Write("Response: ");
            var response = Console.ReadLine();
            if (int.TryParse(response, out var digit))
            {
                return digit switch
                {
                    1 => GetFromConsole(),
                    2 => GetFromFile()[..20000],
                    _ => string.Empty
                };
                
            }
                
            Console.WriteLine("Invalid response!");
        }
    }

    private static string GetFromConsole()
    {
        Console.WriteLine("Enter text:");
        var builder = new StringBuilder();
        while (true)
        {
            var line = Console.ReadLine();
            if (line == string.Empty)
            {
                break;
            }

            builder.Append(line);
        }

        return builder.ToString().ToLowerInvariant();
    }
    
    private static string GetFromFile()
    {
        while (true)
        {
            Console.Write("Enter file name: ");
            var fileName = Console.ReadLine()!;
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }
            
            Console.WriteLine("The specified file does not exist!");
        }
    }
}
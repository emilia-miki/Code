using System.Text;

namespace ShannonCoding;

public record Result(byte[] Bytes, string Text, bool IsText);

public static class Prompt
{
    public static Result GetData()
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
                if (digit == 1)
                {
                    return new Result(Array.Empty<byte>(), GetFromConsole(), true);
                }

                Console.WriteLine("Is it a text file? (Y/n)");
                response = Console.ReadLine()!;
                if (response.TrimStart().StartsWith('n'))
                {
                    return new Result(GetBytesFromFile(), string.Empty, false);
                }
                else
                {
                    return new Result(Array.Empty<byte>(), GetTextFromFile(), true);
                }
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

        return builder.ToString();
    }

    private static string GetTextFromFile()
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

    private static byte[] GetBytesFromFile()
    {
        while (true)
        {
            Console.Write("Enter file name: ");
            var fileName = Console.ReadLine()!;
            if (File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName);
            }

            Console.WriteLine("The specified file does not exist!");
        }
    }
}
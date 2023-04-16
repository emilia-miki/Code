using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace strokesCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbReader = new StreamReader(@"Unihan/Unihan_IRGSources.txt");
            var db = new Dictionary<char, int>();
            while (!dbReader.EndOfStream)
            {
                var line = dbReader.ReadLine();
                if (line.Length == 0 || line[0] == '#') continue;
                var values = line.Split();
                if (values[1] != "kTotalStrokes") continue;
                char key = (char)int.Parse(values[0].Split('+')[1], System.Globalization.NumberStyles.HexNumber);
                if (key > 0x9FFF) break;
                var value = int.Parse(values[2]);
                if (!db.ContainsKey(key)) db.Add(key, value);
            }

            var chineseReader = new StreamReader(@"chinese.txt");
            int chinese_strokes = 0;
            int chinese_characters = 0;
            while (!chineseReader.EndOfStream)
            {
                var line = chineseReader.ReadLine();
                foreach (char ch in line)
                {
                    if (db.ContainsKey(ch))
                    {
                        chinese_characters++;
                        chinese_strokes += db[ch];
                    }
                }
            }

            var englishReader = new StreamReader(@"english.txt");
            int english_letters = 0;
            int english_spaces = 0;
            while (!englishReader.EndOfStream)
            {
                var line = englishReader.ReadLine();
                line = line.Trim();
                line = Regex.Replace(line, @"\s+", " ");

                foreach (char ch in line)
                {
                    if (Char.IsLetter(ch)) english_letters++;
                    else if (ch == ' ') english_spaces++;
                }
            }

            int english_strokes = 2 * english_letters;

            System.Console.WriteLine("The english text has {0} characters and {1} strokes,",
                english_letters + english_spaces, english_strokes);
            System.Console.WriteLine("while the chinese text has {0} characters and {1} strokes.",
                chinese_characters, chinese_strokes);
        }
    }
}

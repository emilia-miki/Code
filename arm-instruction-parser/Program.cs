using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace ARM_Instruction_Parser
{
    static class Program
    {
        const string DefaultOutput = "instructions.txt";
        const string DefaultInput = "/Users/mykytko/Downloads/ISA_A64_xml_v88A-2021-09.pdf";
        private static readonly char[] Delimiters = { ' ', ',' };
        private static readonly string[] Conditions = {"EQ", "GE", "GT", "HI", "HS", "LE", "LO", "LS", "LT", "NE"};

        private static readonly string[] Names = new string[]
        {
            "A64 -- Base Instructions (alphabetic order)",
            "A64 -- SIMD and Floating-point Instructions (alphabetic order)",
            "A64 -- SVE Instructions (alphabetic order)"
        };

        private static void Main(string[] args)
        {
            string input = null;
            string output = null;
            ParseArguments(args, ref input, ref output);
            
            if (!File.Exists(input))
            {
                Console.WriteLine("Specified input file {0} does not exist!", input);
                Environment.Exit(-1);
            }

            HashSet<string>[] data = new HashSet<string>[Names.Length];
            for (int i = 0; i < Names.Length; i++)
            {
                data[i] = new HashSet<string>();
            }
            var doc = new PdfDocument(new PdfReader(input));
            int nameIndex = 0;
            var first = true;
            for (int i = 1; i < doc.GetNumberOfPages(); i++)
            {
                var page = PdfTextExtractor.GetTextFromPage(doc.GetPage(i));
                var index = page.IndexOf(Names[nameIndex], StringComparison.Ordinal);
                if (index == -1)
                {
                    continue;
                }

                index += Names[nameIndex].Length;

                page = page[index..];
                if (first)
                {
                    index = page.IndexOf(Names[nameIndex], StringComparison.Ordinal) + Names[nameIndex].Length;
                    page = page[index..];
                    first = false;
                }
                page = page[(page.IndexOf('\n') + 1)..];
                var lines = page.Split('\n');
                foreach (var line in lines)
                {
                    var tokens = line.Split(Delimiters);
                    tokens[0] = tokens[0].Trim();
                    var colonIsFound = false;
                    foreach (var token in tokens)
                    {
                        if (token.Trim().EndsWith(':'))
                        {
                            colonIsFound = true;
                            break;
                        }
                    }
                    
                    if (!colonIsFound)
                    {
                        continue;
                    }
                    
                    if (tokens[0].ToUpper() != tokens[0] && !tokens[0].Contains(".cond") && !tokens[0].Contains("<cc>"))
                    {
                        nameIndex++;
                        first = true;
                        break;
                    }
                    
                    var searchingForParentheses = false;
                    for (int j = 0;; j++)
                    {
                        tokens[j] = tokens[j].Trim();
                        if (tokens[j] == "")
                        {
                            continue;
                        }
                        
                        if (tokens[j].StartsWith('('))
                        {
                            searchingForParentheses = true;
                        }

                        if (searchingForParentheses)
                        {
                            if (tokens[j].EndsWith(':'))
                            {
                                break;
                            }
                            
                            if (tokens[j].EndsWith(')'))
                            {
                                searchingForParentheses = false;
                            }

                            continue;
                        }

                        string suffix;
                        if (tokens[j].Contains(".cond"))
                        {
                            suffix = ".cond";
                        }
                        else if (tokens[j].Contains("<cc>"))
                        {
                            suffix = "<cc>";
                        }
                        else
                        {
                            suffix = null;
                        }

                        if (suffix != null)
                        {
                            foreach (var cond in Conditions)
                            {
                                var str = tokens[j].Replace(suffix, cond);
                                if (str.EndsWith(':'))
                                {
                                    str = str[..^2];
                                }
                                data[nameIndex].Add(str);
                                Console.WriteLine(str);
                            }
                            
                            if (tokens[j].EndsWith(':'))
                            {
                                break;
                            }

                            continue;
                        }
                        else if (tokens[j].EndsWith(':'))
                        {
                            data[nameIndex].Add(tokens[j][..(tokens[j].Length - 1)]);
                            Console.WriteLine(tokens[j][..(tokens[j].Length - 1)]);
                        }

                        if (tokens[j].EndsWith(':'))
                        {
                            break;
                        }
                        
                        data[nameIndex].Add(tokens[j]);
                        Console.WriteLine(tokens[j]);
                    }
                }

                if (nameIndex == Names.Length)
                {
                    break;
                }
            }
            
            var fs = new FileStream(output, FileMode.Create);
            for (int i = 0; i < Names.Length; i++)
            {
                fs.Write(Encoding.UTF8.GetBytes(Names[i] + "\n"));
                foreach (var instruction in data[i])
                {
                    fs.Write(Encoding.UTF8.GetBytes(instruction.ToLower() + "|"));
                }
                fs.SetLength(fs.Length - Encoding.UTF8.GetBytes("|").Length);
                fs.Write(Encoding.UTF8.GetBytes("\n\n"));
            }
            fs.Close();
        }

        static void ParseArguments(string[] args, ref string input, ref string output)
        {
            if (args.Length > 2)
            {
                Console.WriteLine("Too many arguments!");
                Environment.Exit(-1);
            }

            foreach (var arg in args)
            {
                ParseArgument(arg, ref input, ref output);
            }

            input ??= DefaultInput;
            output ??= DefaultOutput;
        }

        static void ParseArgument(string arg, ref string input, ref string output)
        {
            var tokens = arg.Split('=');
            if (tokens.Length != 2)
            {
                Console.WriteLine("Invalid argument form! The correct form is " +
                                  "input=/path/to/instruction_set.pdf output=/path/to/output_file.txt");
                Environment.Exit(-1);
            }

            switch (tokens[0])
            {
                case "input":
                {
                    if (input != null)
                    {
                        Console.WriteLine("Conflicting arguments: input is specified twice!");
                        Environment.Exit(-1);
                    }

                    input = tokens[1];
                    break;
                }
                case "output":
                {
                    if (output != null)
                    {
                        Console.WriteLine("Conflicting arguments: output is specified twice!");
                        Environment.Exit(-1);
                    }

                    if (!File.Exists(tokens[1]))
                    {
                        Console.WriteLine("Specified output file does not exist!");
                        Environment.Exit(-1);
                    }

                    output = tokens[1];
                    break;
                }
            }
        }
    }
}
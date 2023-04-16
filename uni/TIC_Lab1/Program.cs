using Entropy;

Console.WriteLine("Entropy");
var text = TextProvider.GetText();
if (string.IsNullOrWhiteSpace(text))
{
    return;
}

var symbolsCount = text.Length;
var counts = new Dictionary<char, int>();
foreach (var symbol in text)
{ 
    if (counts.ContainsKey(symbol))
    {
        counts[symbol] += 1;
    }
    else
    {
        counts.Add(symbol, 1);
    }
}

var probabilities = new Dictionary<char, double>();
foreach (var (key, value) in counts)
{
    probabilities.Add(key, (double) value / symbolsCount);
}

var entropies = new Dictionary<char, double>();
foreach (var (key, value) in probabilities)
{
    entropies.Add(key, -value * Math.Log2(value));
}

var uniqueSymbolsCount = counts.Count;
var baselineEntropy = Math.Log2(uniqueSymbolsCount);
var entropy = entropies.Select(pair => pair.Value)
    .Aggregate((seed, next) => seed + next);
var amountOfInformation = entropy * text.Length;
var absoluteRedundancy = baselineEntropy - entropy;
var relativeRedundancy = absoluteRedundancy / entropy;

Console.WriteLine($"File size - {symbolsCount} B");
Console.WriteLine($"Entropy - {entropy:F2} bit");
Console.WriteLine($"Amount of information - {amountOfInformation:F2} bit");
Console.WriteLine($"Baseline entropy - {baselineEntropy:F2} bit");
Console.WriteLine($"Absolute redundancy - {absoluteRedundancy:F2} bit");
Console.WriteLine($"Relative redundancy - {relativeRedundancy * 100:F2}%");

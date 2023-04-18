namespace ShannonCoding;

public static class EntropyCalculator<T> where T : notnull {
    public static double Calculate(T[] data)
    {
        var counts = new Dictionary<T, int>();
        foreach (var item in data)
        {
            if (counts.ContainsKey(item))
            {
                counts[item] += 1;
            }
            else
            {
                counts.Add(item, 1);
            }
        }

        var probabilities = counts.Select(pair =>
                new ItemAndProbability<T>(
                    pair.Key, (double) pair.Value / data.Length));

        var entropy = 0.0;
        foreach (var itemAndProbability in probabilities)
        {
            var p = itemAndProbability.Probability;
            entropy += -p * Math.Log2(p);
        }

        return entropy;
    }
}
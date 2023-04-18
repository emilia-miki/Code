namespace ShannonCoding;

public class Encoder<T> where T : notnull
{
    private readonly Dictionary<T, Coding> _codings;
    private readonly IEnumerable<T> _items;
    private List<bool> _encoded;

    public Dictionary<T, Coding> Codings => _codings;
    public IEnumerable<bool> Encoded => _encoded;

    public Encoder(T[] items)
    {
        _items = items;
        _encoded = new List<bool>();

        var counts = new Dictionary<T, int>();
        foreach (var item in items)
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

        var probabilities = counts
            .OrderBy(pair => pair.Key)
            .Select(pair => new ItemAndProbability<T>(pair.Key, (double) pair.Value / items.Length))
            .ToArray();

        _codings = GetCodings(probabilities, 0, probabilities.Length);
        var average = (double) _codings.Select(pair =>
                counts[pair.Key] * pair.Value.ToString().Length).Sum()
            / items.Length;
    }

    public List<bool> Encode()
    {
        _encoded = new List<bool>();
        foreach(var b in _items)
        {
            foreach (var bit in _codings[b].Bits)
            {
                _encoded.Add(bit);
            }
        }

        return _encoded;
    }

    private Dictionary<T, Coding> GetCodings(
        ItemAndProbability<T>[] probabilities, int beginning, int end)
    {
        var count = end - beginning;
        switch (count)
        {
            case 0:
                return new Dictionary<T, Coding>();
            case 1:
                return new Dictionary<T, Coding>
                {
                    {
                        probabilities[beginning].Item,
                        new Coding(0)
                    }
                };
            case 2:
                return new Dictionary<T, Coding>
                {
                    {
                        probabilities[beginning].Item,
                        new Coding(0)
                    },
                    {
                        probabilities[beginning + 1].Item,
                        new Coding(1)
                    }
                };
            default:
                var pivot = FindPivot(probabilities, beginning, end);
                var left = GetCodings(
                    probabilities, beginning, pivot);
                var right = GetCodings(
                    probabilities, pivot, end);
                return Merge(left, right);
        }
    }

    private static int FindPivot(
        IReadOnlyList<ItemAndProbability<T>> probabilities, int beginning, int end)
    {
        var left = beginning;
        var right = end - 1;
        var leftSum = 0.0;
        var rightSum = 0.0;

        while (left < right)
        {
            leftSum += probabilities[left].Probability;
            left += 1;
            while (rightSum < leftSum && left < right)
            {
                rightSum += probabilities[right].Probability;
                right -= 1;
            }
        }

        return right;
    }

    private static Dictionary<T, Coding> Merge(
        Dictionary<T, Coding> dict1, Dictionary<T, Coding> dict2)
    {
        foreach (var key in dict1.Keys)
        {
            dict1[key].Prepend(0);
        }

        foreach (var key in dict2.Keys)
        {
            dict2[key].Prepend(1);
        }

        foreach (var pair in dict2)
        {
            dict1.Add(pair.Key, pair.Value);
        }

        return dict1;
    }
}
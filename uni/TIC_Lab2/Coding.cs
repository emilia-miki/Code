using System.Text;

namespace ShannonCoding;

public class Coding
{
    private readonly LinkedList<bool> _bits = new();

    public IEnumerable<bool> Bits => _bits;

    public Coding(int bit)
    {
        if (bit != 0 && bit != 1)
        {
            throw new ArgumentException("Must be 0 or 1", nameof(bit));
        }
        
        _bits.AddFirst(bit == 1);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        
        foreach (var bit in _bits)
        {
            builder.Append(bit ? "1" : "0");
        }
        
        return builder.ToString();
    }

    public void Prepend(int bit)
    {
        if (bit != 0 && bit != 1)
        {
            throw new ArgumentException("Must be 0 or 1", nameof(bit));
        }
        
        _bits.AddFirst(bit == 1);
    }
}
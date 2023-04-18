namespace ShannonCoding;

public static class Converter {
    public static byte[] BoolArrayToByteArray(bool[] array)
    {
        var currentByte = new List<bool>();
        var result = new List<byte>();
        var padding = 8 - array.Length / 8;
        for (var i = 0; i < padding; i++)
        {
            currentByte.Add(false);
        }

        foreach (var bit in array)
        {
            if (currentByte.Count == 8)
            {
                result.Add(BoolListToByte(currentByte));
                currentByte.Clear();
            }

            currentByte.Add(bit);
        }

        return result.ToArray();
    }

    private static byte BoolListToByte(List<bool> list)
    {
        if (list.Count != 8)
        {
            throw new ArgumentException("Length must be 8", nameof(list));
        }

        byte b = 0;
        for (var i = 0; i < 8; i++)
        {
            b = (byte) (b | ((list[i] ? 1 : 0) << (7 - i)));
        }

        return b;
    }
}
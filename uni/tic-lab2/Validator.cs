namespace ShannonCoding;

public static class Validator<T> where T : IComparable {
    public static void Validate(T[] original, T[] decoded)
    {
        if (original.Length != decoded.Length)
        {
            throw new EncodingDecodingMismatchException();
        }

        for (var i = 0; i < original.Length; i++)
        {
            if (original[i].CompareTo(decoded[i]) != 0)
            {
                throw new EncodingDecodingMismatchException();
            }
        }
    }
}
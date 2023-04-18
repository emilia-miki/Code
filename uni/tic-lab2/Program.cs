using ShannonCoding;

Console.WriteLine("Shannon-Fano coding");

double originalEntropy;
int originalLength;
double encodedEntropy;
int encodedLength;

var result = Prompt.GetData();
if (result.IsText)
{
    if (result.Text == string.Empty)
    {
        return;
    }

    var original = result.Text;
    originalEntropy = EntropyCalculator<char>.Calculate(original.ToCharArray());

    var encoder = new Encoder<char>(result.Text.ToCharArray());
    originalLength = result.Text.Length;

    var encoded = encoder.Encode();
    encodedLength = (int) Math.Ceiling((double) encoded.Count / 8);
    encodedEntropy = EntropyCalculator<byte>.Calculate(
            Converter.BoolArrayToByteArray(encoded.ToArray()));

    var decoder = new Decoder<char>(encoder.Codings);
    var decoded = decoder.Decode(encoded);

    Validator<char>.Validate(original.ToCharArray(), decoded.ToArray());
}
else
{
    if (result.Bytes.Length == 0)
    {
        return;
    }

    var original = result.Bytes;
    originalEntropy = EntropyCalculator<byte>.Calculate(original);

    var encoder = new Encoder<byte>(result.Bytes);
    originalLength = original.Length;

    var encoded = encoder.Encode();
    encodedLength = (int) Math.Ceiling((double) encoded.Count / 8);
    encodedEntropy = EntropyCalculator<byte>.Calculate(
            Converter.BoolArrayToByteArray(encoded.ToArray()));

    var decoder = new Decoder<byte>(encoder.Codings);
    var decoded = decoder.Decode(encoded);

    Validator<byte>.Validate(original, decoded.ToArray());
}

var coefficient = (double) originalLength / encodedLength;

Console.WriteLine($"Original file size -- {originalLength} bytes");
Console.WriteLine($"Original file entropy -- {originalEntropy} bits");
Console.WriteLine($"Encoded file size -- {encodedLength} bytes");
Console.WriteLine($"Encoded file entropy -- {encodedEntropy} bits");
Console.WriteLine($"Compression coefficient -- {coefficient:F3}");
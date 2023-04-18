double Function(double x) => x * x - 8 * x; // Math.Abs(5 * x * x * x - 3 * x * x + 7 * x - 7) + 5;
double a = 3.1;
double b = 6.3;
const double epsilon = 0.01;

var goldenRatio = (3 - Math.Sqrt(5)) / 2;

// Find minimum
var iterationResults = new List<(double, double, double, double, double)>(); // x1, x2, f1, f2, L
double x1, x2, f1, f2, l;
l = b - a;
var indent = goldenRatio * l;
x1 = a + indent;
x2 = b - indent;
f1 = Function(x1);
f2 = Function(x2);

while (Math.Abs(x1 - x2) > epsilon)
{
    if (f1 > f2)
    {
        a = x1;
        l = b - a;
        x1 = x2;
        f1 = f2;
        x2 = b - goldenRatio * l;
        f2 = Function(x2);
    }
    else
    {
        b = x2;
        l = b - a;
        x2 = x1;
        f2 = f1;
        x1 = a + goldenRatio * l;
        f1 = Function(x1);
    }
    
    iterationResults.Add((x1, x2, f1, f2, l));
}

// Show results
Console.WriteLine("i x1 x2 f1 f2 L");
for (var i = 0; i < iterationResults.Count; i++)
{
    var results = iterationResults[i];
    Console.WriteLine($"{i + 1}, {results.Item1:F3}, {results.Item2:F3}, {results.Item3:F3}, {results.Item4:F3}, " +
                      $"{results.Item5:F6}");
}
var function = (double r3) =>
{
    const double phi2desired = 2.4;
    const double ue1 = 5.0;
    const double r1 = 5.0;
    const double r2 = 5.0;
    const double r4 = 5.0;
    var phi2 = ue1 * r3 / (r1 * (1 + r3 / r1 + r3 / (r2 + r4)));
    return Math.Pow(phi2 - phi2desired, 2);
};
const double r30 = 5.0;
int functionCalls;

bool Svenn(Func<double, double> func, double x0, double step, out double left, out double right)
{
    // 1. 
    var f0 = func(x0);
    var x1 = x0 - step;
    var x2 = x0 + step;
    var f1 = func(x1);
    var f2 = func(x2);
    functionCalls += 3;
    var iteration = 0;

    if (f1 > f0 && f0 < f2) // a)
    {
        left = x1;
        right = x2;
        Console.WriteLine($"i = {iteration}, a = {left}, b = {right}, h = {step};");
        return true;
    }

    if (f1 < f0 && f0 > f2) // b)
    {
        Console.WriteLine("Змініть початкову точку x0.");
        left = 0;
        right = 0;
        return false;
    }

    if (f1 > f0 && f0 > f2) // c)
    {
        x1 = x2;
        f1 = func(x1);
        functionCalls++;
    } 
    else if (f1 < f0 && f0 < f2)
    {
        step = -step;
    }
    
    // 2.
    step = 2 * step;
    x2 = x1 + step;
    f2 = func(x2);
    functionCalls++;
    iteration++;
    Console.WriteLine($"i = {iteration}, a = {x0}, b = {x2}, h = {step};");

    while (f2 < f1)
    {
        x0 = x1;
        x1 = x2;
        f1 = f2;

        step = 2 * step;
        x2 = x1 + step;
        f2 = func(x2);
        functionCalls++;
        iteration++;
        Console.WriteLine($"i = {iteration}, a = {x0}, b = {x2}, h = {step};");
    }

    // 3.
    var x3 = x2 - step / 2;
    var f3 = func(x3);
    functionCalls++;
    if (f1 < f3)
    {
        left = x0;
        right = x3;
    }
    else
    {
        left = x1;
        right = x2;
    }

    iteration++;
    Console.WriteLine($"i = {iteration}, a = {left}, b = {right}, h = {- step / 2};");

    return true;
}

var goldenRatio = (3 - Math.Sqrt(5)) / 2;
const double epsilon = 0.001;
const int h = 1;

functionCalls = 0;
if (!Svenn(function, r30, h, out var a, out var b))
{
    return;
}
Console.WriteLine($"FCalls = {functionCalls}, a = {a}, b = {b}.");

// Find minimum
var iterationResults = new List<(double, double, double, double, double)>(); // x1, x2, f1, f2, L
double x1, x2, f1, f2, l;
l = b - a;
var indent = goldenRatio * l;
x1 = a + indent;
x2 = b - indent;
f1 = function(x1);
f2 = function(x2);

while (Math.Abs(x1 - x2) > epsilon)
{
    if (f1 > f2)
    {
        a = x1;
        l = b - a;
        x1 = x2;
        f1 = f2;
        x2 = b - goldenRatio * l;
        f2 = function(x2);
    }
    else
    {
        b = x2;
        l = b - a;
        x2 = x1;
        f2 = f1;
        x1 = a + goldenRatio * l;
        f1 = function(x1);
    }
    
    iterationResults.Add((x1, x2, f1, f2, l));
}

// Show results
Console.WriteLine("i x1 x2 f1 f2 L");
for (var i = 0; i < iterationResults.Count; i++)
{
    var results = iterationResults[i];
    Console.WriteLine($"{i}, {results.Item1:F3}, {results.Item2:F3}, {results.Item3:F3}, {results.Item4:F3}, " +
                      $"{results.Item5:F6}");
}

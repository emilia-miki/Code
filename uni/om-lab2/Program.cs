var function = (double x) => x * x - 8 * x; // Math.Abs(5 * x * x * x - 3 * x * x + 7 * x - 7) + 5;
var x0 = 0;
var fcalls = 0;

bool Calculate(Func<double, double> function, double x0, double h, out double a, out double b)
{
    // 1.
    var f0 = function(x0);
    var x1 = x0 - h;
    var x2 = x0 + h;
    var f1 = function(x1);
    var f2 = function(x2);
    fcalls += 3;

    if (f1 > f0 && f0 < f2) // a)
    {
        a = x1;
        b = x2;
        return true;
    }

    if (f1 < f0 && f0 > f2) // b)
    {
        Console.WriteLine("Змініть початкову точку x0.");
        a = 0;
        b = 0;
        return false;
    }

    if (f1 > f0 && f0 > f2) // c)
    {
        x1 = x2;
        f1 = function(x1);
        fcalls++;
    } 
    else if (f1 < f0 && f0 < f2)
    {
        h = -h;
    }
    
    // 2.
    h = 2 * h;
    x2 = x1 + h;
    f2 = function(x2);
    fcalls++;

    while (f2 < f1)
    {
        Console.WriteLine($"FCalls = {fcalls}, " +
                          $"f({x0:F2}) = {f0:F2}, f({x1:F2}) = {f1:F2}, f({x2:F2}) = {f2:F2}, h = {h}.");
        x0 = x1;
        f0 = f1;
        x1 = x2;
        f1 = f2;

        h = 2 * h;
        x2 = x1 + h;
        f2 = function(x2);
        fcalls++;
    }

    // 3.
    var x3 = x2 - h / 2;
    var f3 = function(x3);
    fcalls++;
    if (f1 < f3)
    {
        a = x0;
        b = x3;
    }
    else
    {
        a = x1;
        b = x2;
    }

    return true;
}

foreach (var h in new double[] { 0.01, 0.1, 1, 2 })
{
    fcalls = 0;
    x0 = 0;
    if (!Calculate(function, x0, h, out var a, out var b))
    {
        return;
    }
    Console.WriteLine($"FCalls = {fcalls}, a = {a}, b = {b}.");
}

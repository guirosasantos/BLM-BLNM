namespace Lib;

public static class RandomHelper
{
    public static int GetRandomFromValues(int[] values)
    {
        var rnd = new Random();
        return values[rnd.Next(values.Length)];
    }

    public static double GetRandomFromValuesDecimal(double[] values)
    {
        var rnd = new Random();
        return values[rnd.Next(values.Length)];
    }

    public static int GetRandomFromValuesRange(int min, int max)
    {
        var rnd = new Random();
        return rnd.Next(min, max);
    }
}
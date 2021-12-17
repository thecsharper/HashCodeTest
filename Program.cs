using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNet.Samples
{
    public class Program
    {
        public static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(HashCodeTest).Assembly).RunAll();
    }
}

public class HashCodeTest
{
    [Benchmark(Description = "OverrideHash(10_000)")]
    public void Test1() => OverrideHash.Test(10_000);

    [Benchmark(Description = "NoOverrideHash(10_000)")]
    public void Test2() => NoOverrideHash.Test(10_000);

    [Benchmark(Description = "OverrideHash(100_000)")]
    public void Test3() => OverrideHash.Test(100_000);

    [Benchmark(Description = "NoOverrideHash(100_000)")]
    public void Test4() => NoOverrideHash.Test(100_000);

    [Benchmark(Description = "OverrideHash(1_000_000)")]
    public void Test5() => OverrideHash.Test(1_000_000);

    [Benchmark(Description = "NoOverrideHash(1_000_000)")]
    public void Test6() => NoOverrideHash.Test(1_000_000);

    [Benchmark(Description = "OverrideHash(10_000_000)")]
    public void Test7() => OverrideHash.Test(10_000_000);

    [Benchmark(Description = "NoOverrideHash(10_000_000)")]
    public void Test8() => NoOverrideHash.Test(10_000_000);
}

public unsafe class OverrideHash
{
    public string? Name;

    public object? Value;

    public static void Test(int count)
    {
        var dic = new Dictionary<OverrideHash, int>(count);

        for (var i = 0; i < count; i++)
        {
            dic.Add(new OverrideHash
            {
                Name = "test",
                Value = i
            }, i);
        }
    }

    public override int GetHashCode()
    {
        Span<byte> span;
        if (Value is long l)
        {
            span = new Span<byte>(&l, sizeof(long));
        }
        else if (Value is int i)
        {
            span = new Span<byte>(&i, sizeof(int));
        }
        else
        {
            throw new NotSupportedException("Unknown value");
        }

        var hc = (uint)Name.GetHashCode();

        var code = (int)Standart.Hash.xxHash.xxHash32.ComputeHash(span, span.Length, hc);

        return code;
    }
}

public unsafe class NoOverrideHash
{
    public string? Name;

    public object? Value;

    public static void Test(int count)
    {
        var dic = new Dictionary<NoOverrideHash, int>(count);

        for (var i = 0; i < count; i++)
        {
            dic.Add(new NoOverrideHash
            {
                Name = "test",
                Value = i
            }, i);
        }
    }
}
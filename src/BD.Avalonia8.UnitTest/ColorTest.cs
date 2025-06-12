using BD.Avalonia8.Media;
using System.Collections.Immutable;
using System.Drawing;
using System.Reflection;
using SDColor = System.Drawing.Color;
using AvaColor = Avalonia.Media.Color;
using AvaColors = Avalonia.Media.Colors;

namespace BD.Avalonia8.UnitTest;

sealed class ColorTest
{
    static FormattableString ToString<T>(T t) where T : struct
    {
        var s = t.ToString()!;
        return $"{s}{(s.Contains('.') ? null : ".0")}";
    }

    ImmutableDictionary<string, SDColor> systemColors = null!, knownColors = null!;
    ImmutableDictionary<string, AvaColor> knownAvaColors = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        {
            Dictionary<string, SDColor> systemColors = new();
            var query = from m in typeof(SystemColors).GetProperties(
                BindingFlags.Public
                | BindingFlags.Static)
                        where m.PropertyType == typeof(SDColor)
                        && m.CanRead
                        let v = (SDColor?)m.GetMethod?.Invoke(null, null)
                        where v != null
                        select KeyValuePair.Create(m.Name, v.Value!);
            foreach (var it in query)
            {
                systemColors.Add(it.Key, it.Value);
            }
            this.systemColors = systemColors.ToImmutableDictionary();
        }
        {
            Dictionary<string, SDColor> knownColors = new();
            foreach (var it in Enum.GetValues<KnownColor>())
            {
                var name = it.ToString();
                var value = (SDColor?)typeof(SDColor).GetProperty(name, BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                if (!value.HasValue)
                {
                    continue;
                }
                knownColors.Add(name, value.Value);
            }
            this.knownColors = knownColors.ToImmutableDictionary();
        }
        {
            Dictionary<string, AvaColor> knownColors = new();
            var query = from m in typeof(AvaColors).GetProperties(
                BindingFlags.Public
                | BindingFlags.Static)
                        where m.PropertyType == typeof(AvaColor)
                        && m.CanRead
                        let v = (AvaColor?)m.GetMethod?.Invoke(null, null)
                        where v != null
                        select KeyValuePair.Create(m.Name, v.Value!);
            foreach (var it in query)
            {
                knownColors.Add(it.Key, it.Value);
            }
            knownAvaColors = knownColors.ToImmutableDictionary();
        }
    }

    [Test]
    public void ByteTest()
    {
        for (ushort i = 0; i <= byte.MaxValue; i++)
        {
            byte b = unchecked((byte)i);
            var f = ColorF.sRgbToScRgb(b);
            var d = ColorF.sRgbToScRgbD(b);
            var fb = ColorF.ScRgbTosRgb(f);
            var db = ColorF.ScRgbTosRgb(d);
            if (b != fb)
            {
                throw new Exception($"Byte {b} to float {f} and back to byte {fb} mismatch.");
            }
            else if (fb != db)
            {
                throw new Exception($"Byte {b} to float {f} to double {d} and back to byte {db} mismatch.");
            }
            var str =
$"""
Byte: {b}({fb}|{db}), Float: {ToString(f)}, Double: {ToString(d)}, X2: {b:X2}
""";
            TestContext.Out.WriteLine(str);
        }
    }

    static void Print<T>(IReadOnlyDictionary<string, T> dict, Func<T, (byte a, byte r, byte g, byte b)> func)
    {
        foreach (var it in dict)
        {
            (byte A, byte R, byte G, byte B) = func(it.Value);
            TestContext.Out.WriteLine($"{it.Key}:");
            {
                var a = ColorF.sRgbToScRgb(A);
                var r = ColorF.sRgbToScRgb(R);
                var g = ColorF.sRgbToScRgb(G);
                var b = ColorF.sRgbToScRgb(B);
                TestContext.Out.WriteLine($"rgba:({ToString(r)},{ToString(g)},{ToString(b)},{ToString(a)})");
            }
            {
                var a = ColorF.sRgbToScRgbD(A);
                var r = ColorF.sRgbToScRgbD(R);
                var g = ColorF.sRgbToScRgbD(G);
                var b = ColorF.sRgbToScRgbD(B);
                TestContext.Out.WriteLine($"rgba:({ToString(r)},{ToString(g)},{ToString(b)},{ToString(a)})");
            }
        }
    }

    static void Print(IReadOnlyDictionary<string, SDColor> dict)
        => Print(dict, static c => (c.A, c.R, c.G, c.B));

    static void Print(IReadOnlyDictionary<string, AvaColor> dict)
        => Print(dict, static c => (c.A, c.R, c.G, c.B));

    [Test]
    public void SDColorTest()
    {
        TestContext.Out.WriteLine("----- SystemColors -----");
        Print(systemColors);
        TestContext.Out.WriteLine("----- KnownColors -----");
        Print(knownColors);
    }

    [Test]
    public void AvaColorTest()
    {
        TestContext.Out.WriteLine("----- Avalonia KnownColors -----");
        Print(knownAvaColors);
    }
}

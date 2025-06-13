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

    static void EqualsTest(KeyValuePair<string, ColorF> it)
    {
        {
            AvaColor avaColor = it.Value;
            Assert.That(avaColor == it.Value, Is.True, $"{it.Key} == {it.Value} failed.");
            Assert.That(it.Value == avaColor, Is.True, $"{it.Key} == {avaColor} failed.");
            Assert.That(avaColor != it.Value, Is.False, $"{it.Key} != {it.Value} failed.");
            Assert.That(it.Value != avaColor, Is.False, $"{it.Key} != {avaColor} failed.");
            Assert.That(it.Value.Equals(avaColor), Is.True, $"{it.Key}.Equals({avaColor}) failed.");
        }
        {
            SDColor sDColor = it.Value;
            Assert.That(sDColor == it.Value, Is.True, $"{it.Key} == {it.Value} failed.");
            Assert.That(it.Value == sDColor, Is.True, $"{it.Key} == {sDColor} failed.");
            Assert.That(sDColor != it.Value, Is.False, $"{it.Key} != {it.Value} failed.");
            Assert.That(it.Value != sDColor, Is.False, $"{it.Key} != {sDColor} failed.");
            Assert.That(it.Value.Equals(sDColor), Is.True, $"{it.Key}.Equals({sDColor}) failed.");
        }
        {
            AvaColor? avaColorN = null;
            Assert.That(avaColorN == it.Value, Is.False, $"{avaColorN} == {it.Value} failed.");
            Assert.That(it.Value == avaColorN, Is.False, $"{it.Value} == {avaColorN} failed.");
            Assert.That(avaColorN != it.Value, Is.True, $"{avaColorN} != {it.Value} failed.");
            Assert.That(it.Value != avaColorN, Is.True, $"{it.Value} != {avaColorN} failed.");
            Assert.That(!it.Value.Equals(avaColorN), Is.True, $"!{it.Value}.Equals({avaColorN}) failed.");
        }
        {
            ColorF? colorFN = null;
            Assert.That(colorFN == it.Value, Is.False, $"{colorFN} == {it.Value} failed.");
            Assert.That(it.Value == colorFN, Is.False, $"{it.Value} == {colorFN} failed.");
            Assert.That(colorFN != it.Value, Is.True, $"{colorFN} != {it.Value} failed.");
            Assert.That(it.Value != colorFN, Is.True, $"{it.Value} != {colorFN} failed.");
            Assert.That(!it.Value.Equals(colorFN), Is.True, $"!{it.Value}.Equals({colorFN}) failed.");
        }
        {
            string argbHex = it.Value;
            ColorF? colorFN = argbHex;
            Assert.That(colorFN == it.Value, Is.True, $"{colorFN} == {it.Value} failed.");
            Assert.That(it.Value == colorFN, Is.True, $"{it.Value} == {colorFN} failed.");
            Assert.That(colorFN != it.Value, Is.False, $"{colorFN} != {it.Value} failed.");
            Assert.That(it.Value != colorFN, Is.False, $"{it.Value} != {colorFN} failed.");
            Assert.That(!it.Value.Equals(colorFN), Is.False, $"!{it.Value}.Equals({colorFN}) failed.");
            ColorF colorF = colorFN.GetValueOrDefault();
            Assert.That(colorF == it.Value, Is.True, $"{colorF} == {it.Value} failed.");
            Assert.That(it.Value == colorF, Is.True, $"{it.Value} == {colorF} failed.");
            Assert.That(colorF != it.Value, Is.False, $"{colorF} != {it.Value} failed.");
            Assert.That(it.Value != colorF, Is.False, $"{it.Value} != {colorF} failed.");
            Assert.That(!it.Value.Equals(colorF), Is.False, $"!{it.Value}.Equals({colorF}) failed.");
        }
    }

    [Test]
    public void EqualsTest()
    {
        foreach (var it in knownAvaColors)
        {
            ColorF val = it.Value;
            var it2 = KeyValuePair.Create(it.Key, val);
            EqualsTest(it2);
        }
        foreach (var it in knownColors)
        {
            ColorF val = it.Value;
            var it2 = KeyValuePair.Create(it.Key, val);
            EqualsTest(it2);
        }
        foreach (var it in systemColors)
        {
            ColorF val = it.Value;
            var it2 = KeyValuePair.Create(it.Key, val);
            EqualsTest(it2);
        }
    }
}

using System.Buffers;

namespace BD.Avalonia8.Media;

partial struct ColorF // Hex
{
    /// <summary>
    /// 返回 <see cref="ColorF"/> 的十六进制 <see langword="string"/> 表示形式（仅 #RRGGBB）
    /// </summary>
    /// <returns></returns>
    public string ToHex()
    {
        const int strLen = 7;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        return string.Create(strLen, this, static (s, c) =>
        {
            s[0] = '#';
            s = s[1..];
            ColorF.ToHex(c.Red, s);
            s = s[2..];
            ColorF.ToHex(c.Green, s);
            s = s[2..];
            ColorF.ToHex(c.Blue, s);
        });
#else
        var chars = ArrayPool<char>.Shared.Rent(strLen);
        try
        {
            var s = chars.AsSpan();
            s[0] = '#';
            s = s[1..];
            ColorF.ToHex(Red, s);
            s = s[2..];
            ColorF.ToHex(Green, s);
            s = s[2..];
            ColorF.ToHex(Blue, s);
            return new string(chars, 0, strLen);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
        }
#endif
        //return "#" + ToHex(Red) + ToHex(Green) + ToHex(Blue);
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/> 的 ARGB 十六进制 <see langword="string"/> 表示形式（#AARRGGBB）
    /// </summary>
    /// <param name="includeAlpha"></param>
    /// <returns></returns>
    public string ToArgbHex(bool includeAlpha = false)
    {
#if IOS || MACCATALYST || MACOS
        if (includeAlpha || Alpha < 1)
#else
        if (includeAlpha || Alpha < byte.MaxValue)
#endif
        {
            const int strLen = 9;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            return string.Create(strLen, this, static (s, c) =>
            {
                s[0] = '#';
                s = s[1..];
                ColorF.ToHex(c.Alpha, s);
                s = s[2..];
                ColorF.ToHex(c.Red, s);
                s = s[2..];
                ColorF.ToHex(c.Green, s);
                s = s[2..];
                ColorF.ToHex(c.Blue, s);
            });
#else
            var chars = ArrayPool<char>.Shared.Rent(strLen);
            try
            {
                var s = chars.AsSpan();
                s[0] = '#';
                s = s[1..];
                ColorF.ToHex(Alpha, s);
                s = s[2..];
                ColorF.ToHex(Red, s);
                s = s[2..];
                ColorF.ToHex(Green, s);
                s = s[2..];
                ColorF.ToHex(Blue, s);
                return new string(chars, 0, strLen);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(chars);
            }
#endif
            //return "#" + ToHex(Alpha) + ToHex(Red) + ToHex(Green) + ToHex(Blue);
        }

        return ToHex();
        //return "#" + ToHex(Red) + ToHex(Green) + ToHex(Blue);
    }

    /// <summary>
    /// 返回 <see cref="ColorF"/> 的 RGBA 十六进制 <see langword="string"/> 表示形式
    /// </summary>
    /// <param name="includeAlpha"></param>
    /// <returns></returns>
    public string ToRgbaHex(bool includeAlpha = false)
    {
#if IOS || MACCATALYST || MACOS
        if (includeAlpha || Alpha < 1)
#else
        if (includeAlpha || Alpha < byte.MinValue)
#endif
        {
            const int strLen = 9;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            return string.Create(strLen, this, static (s, c) =>
            {
                s[0] = '#';
                s = s[1..];
                ColorF.ToHex(c.Red, s);
                s = s[2..];
                ColorF.ToHex(c.Green, s);
                s = s[2..];
                ColorF.ToHex(c.Blue, s);
                s = s[2..];
                ColorF.ToHex(c.Alpha, s);
            });
#else
            var chars = ArrayPool<char>.Shared.Rent(strLen);
            try
            {
                var s = chars.AsSpan();
                s[0] = '#';
                s = s[1..];
                ColorF.ToHex(Red, s);
                s = s[2..];
                ColorF.ToHex(Green, s);
                s = s[2..];
                ColorF.ToHex(Blue, s);
                s = s[2..];
                ColorF.ToHex(Alpha, s);
                return new string(chars, 0, strLen);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(chars);
            }
#endif
            //return "#" + ToHex(Red) + ToHex(Green) + ToHex(Blue) + ToHex(Alpha);
        }

        return ToHex();
        //return "#" + ToHex(Red) + ToHex(Green) + ToHex(Blue);
    }

#if IOS || MACCATALYST || MACOS
    public static void ToHex(nfloat v, Span<char> s)
        => ToHex(ScRgbTosRgb(v.Value), s);
#endif

    public static void ToHex(byte v, Span<char> s)
    {
        switch (v)
        {
            case 0:
                s[0] = '0';
                s[1] = '0';
                break;
            case 1:
                s[0] = '0';
                s[1] = '1';
                break;
            case 2:
                s[0] = '0';
                s[1] = '2';
                break;
            case 3:
                s[0] = '0';
                s[1] = '3';
                break;
            case 4:
                s[0] = '0';
                s[1] = '4';
                break;
            case 5:
                s[0] = '0';
                s[1] = '5';
                break;
            case 6:
                s[0] = '0';
                s[1] = '6';
                break;
            case 7:
                s[0] = '0';
                s[1] = '7';
                break;
            case 8:
                s[0] = '0';
                s[1] = '8';
                break;
            case 9:
                s[0] = '0';
                s[1] = '9';
                break;
            case 10:
                s[0] = '0';
                s[1] = 'A';
                break;
            case 11:
                s[0] = '0';
                s[1] = 'B';
                break;
            case 12:
                s[0] = '0';
                s[1] = 'C';
                break;
            case 13:
                s[0] = '0';
                s[1] = 'D';
                break;
            case 14:
                s[0] = '0';
                s[1] = 'E';
                break;
            case 15:
                s[0] = '0';
                s[1] = 'F';
                break;
            case 16:
                s[0] = '1';
                s[1] = '0';
                break;
            case 17:
                s[0] = '1';
                s[1] = '1';
                break;
            case 18:
                s[0] = '1';
                s[1] = '2';
                break;
            case 19:
                s[0] = '1';
                s[1] = '3';
                break;
            case 20:
                s[0] = '1';
                s[1] = '4';
                break;
            case 21:
                s[0] = '1';
                s[1] = '5';
                break;
            case 22:
                s[0] = '1';
                s[1] = '6';
                break;
            case 23:
                s[0] = '1';
                s[1] = '7';
                break;
            case 24:
                s[0] = '1';
                s[1] = '8';
                break;
            case 25:
                s[0] = '1';
                s[1] = '9';
                break;
            case 26:
                s[0] = '1';
                s[1] = 'A';
                break;
            case 27:
                s[0] = '1';
                s[1] = 'B';
                break;
            case 28:
                s[0] = '1';
                s[1] = 'C';
                break;
            case 29:
                s[0] = '1';
                s[1] = 'D';
                break;
            case 30:
                s[0] = '1';
                s[1] = 'E';
                break;
            case 31:
                s[0] = '1';
                s[1] = 'F';
                break;
            case 32:
                s[0] = '2';
                s[1] = '0';
                break;
            case 33:
                s[0] = '2';
                s[1] = '1';
                break;
            case 34:
                s[0] = '2';
                s[1] = '2';
                break;
            case 35:
                s[0] = '2';
                s[1] = '3';
                break;
            case 36:
                s[0] = '2';
                s[1] = '4';
                break;
            case 37:
                s[0] = '2';
                s[1] = '5';
                break;
            case 38:
                s[0] = '2';
                s[1] = '6';
                break;
            case 39:
                s[0] = '2';
                s[1] = '7';
                break;
            case 40:
                s[0] = '2';
                s[1] = '8';
                break;
            case 41:
                s[0] = '2';
                s[1] = '9';
                break;
            case 42:
                s[0] = '2';
                s[1] = 'A';
                break;
            case 43:
                s[0] = '2';
                s[1] = 'B';
                break;
            case 44:
                s[0] = '2';
                s[1] = 'C';
                break;
            case 45:
                s[0] = '2';
                s[1] = 'D';
                break;
            case 46:
                s[0] = '2';
                s[1] = 'E';
                break;
            case 47:
                s[0] = '2';
                s[1] = 'F';
                break;
            case 48:
                s[0] = '3';
                s[1] = '0';
                break;
            case 49:
                s[0] = '3';
                s[1] = '1';
                break;
            case 50:
                s[0] = '3';
                s[1] = '2';
                break;
            case 51:
                s[0] = '3';
                s[1] = '3';
                break;
            case 52:
                s[0] = '3';
                s[1] = '4';
                break;
            case 53:
                s[0] = '3';
                s[1] = '5';
                break;
            case 54:
                s[0] = '3';
                s[1] = '6';
                break;
            case 55:
                s[0] = '3';
                s[1] = '7';
                break;
            case 56:
                s[0] = '3';
                s[1] = '8';
                break;
            case 57:
                s[0] = '3';
                s[1] = '9';
                break;
            case 58:
                s[0] = '3';
                s[1] = 'A';
                break;
            case 59:
                s[0] = '3';
                s[1] = 'B';
                break;
            case 60:
                s[0] = '3';
                s[1] = 'C';
                break;
            case 61:
                s[0] = '3';
                s[1] = 'D';
                break;
            case 62:
                s[0] = '3';
                s[1] = 'E';
                break;
            case 63:
                s[0] = '3';
                s[1] = 'F';
                break;
            case 64:
                s[0] = '4';
                s[1] = '0';
                break;
            case 65:
                s[0] = '4';
                s[1] = '1';
                break;
            case 66:
                s[0] = '4';
                s[1] = '2';
                break;
            case 67:
                s[0] = '4';
                s[1] = '3';
                break;
            case 68:
                s[0] = '4';
                s[1] = '4';
                break;
            case 69:
                s[0] = '4';
                s[1] = '5';
                break;
            case 70:
                s[0] = '4';
                s[1] = '6';
                break;
            case 71:
                s[0] = '4';
                s[1] = '7';
                break;
            case 72:
                s[0] = '4';
                s[1] = '8';
                break;
            case 73:
                s[0] = '4';
                s[1] = '9';
                break;
            case 74:
                s[0] = '4';
                s[1] = 'A';
                break;
            case 75:
                s[0] = '4';
                s[1] = 'B';
                break;
            case 76:
                s[0] = '4';
                s[1] = 'C';
                break;
            case 77:
                s[0] = '4';
                s[1] = 'D';
                break;
            case 78:
                s[0] = '4';
                s[1] = 'E';
                break;
            case 79:
                s[0] = '4';
                s[1] = 'F';
                break;
            case 80:
                s[0] = '5';
                s[1] = '0';
                break;
            case 81:
                s[0] = '5';
                s[1] = '1';
                break;
            case 82:
                s[0] = '5';
                s[1] = '2';
                break;
            case 83:
                s[0] = '5';
                s[1] = '3';
                break;
            case 84:
                s[0] = '5';
                s[1] = '4';
                break;
            case 85:
                s[0] = '5';
                s[1] = '5';
                break;
            case 86:
                s[0] = '5';
                s[1] = '6';
                break;
            case 87:
                s[0] = '5';
                s[1] = '7';
                break;
            case 88:
                s[0] = '5';
                s[1] = '8';
                break;
            case 89:
                s[0] = '5';
                s[1] = '9';
                break;
            case 90:
                s[0] = '5';
                s[1] = 'A';
                break;
            case 91:
                s[0] = '5';
                s[1] = 'B';
                break;
            case 92:
                s[0] = '5';
                s[1] = 'C';
                break;
            case 93:
                s[0] = '5';
                s[1] = 'D';
                break;
            case 94:
                s[0] = '5';
                s[1] = 'E';
                break;
            case 95:
                s[0] = '5';
                s[1] = 'F';
                break;
            case 96:
                s[0] = '6';
                s[1] = '0';
                break;
            case 97:
                s[0] = '6';
                s[1] = '1';
                break;
            case 98:
                s[0] = '6';
                s[1] = '2';
                break;
            case 99:
                s[0] = '6';
                s[1] = '3';
                break;
            case 100:
                s[0] = '6';
                s[1] = '4';
                break;
            case 101:
                s[0] = '6';
                s[1] = '5';
                break;
            case 102:
                s[0] = '6';
                s[1] = '6';
                break;
            case 103:
                s[0] = '6';
                s[1] = '7';
                break;
            case 104:
                s[0] = '6';
                s[1] = '8';
                break;
            case 105:
                s[0] = '6';
                s[1] = '9';
                break;
            case 106:
                s[0] = '6';
                s[1] = 'A';
                break;
            case 107:
                s[0] = '6';
                s[1] = 'B';
                break;
            case 108:
                s[0] = '6';
                s[1] = 'C';
                break;
            case 109:
                s[0] = '6';
                s[1] = 'D';
                break;
            case 110:
                s[0] = '6';
                s[1] = 'E';
                break;
            case 111:
                s[0] = '6';
                s[1] = 'F';
                break;
            case 112:
                s[0] = '7';
                s[1] = '0';
                break;
            case 113:
                s[0] = '7';
                s[1] = '1';
                break;
            case 114:
                s[0] = '7';
                s[1] = '2';
                break;
            case 115:
                s[0] = '7';
                s[1] = '3';
                break;
            case 116:
                s[0] = '7';
                s[1] = '4';
                break;
            case 117:
                s[0] = '7';
                s[1] = '5';
                break;
            case 118:
                s[0] = '7';
                s[1] = '6';
                break;
            case 119:
                s[0] = '7';
                s[1] = '7';
                break;
            case 120:
                s[0] = '7';
                s[1] = '8';
                break;
            case 121:
                s[0] = '7';
                s[1] = '9';
                break;
            case 122:
                s[0] = '7';
                s[1] = 'A';
                break;
            case 123:
                s[0] = '7';
                s[1] = 'B';
                break;
            case 124:
                s[0] = '7';
                s[1] = 'C';
                break;
            case 125:
                s[0] = '7';
                s[1] = 'D';
                break;
            case 126:
                s[0] = '7';
                s[1] = 'E';
                break;
            case 127:
                s[0] = '7';
                s[1] = 'F';
                break;
            case 128:
                s[0] = '8';
                s[1] = '0';
                break;
            case 129:
                s[0] = '8';
                s[1] = '1';
                break;
            case 130:
                s[0] = '8';
                s[1] = '2';
                break;
            case 131:
                s[0] = '8';
                s[1] = '3';
                break;
            case 132:
                s[0] = '8';
                s[1] = '4';
                break;
            case 133:
                s[0] = '8';
                s[1] = '5';
                break;
            case 134:
                s[0] = '8';
                s[1] = '6';
                break;
            case 135:
                s[0] = '8';
                s[1] = '7';
                break;
            case 136:
                s[0] = '8';
                s[1] = '8';
                break;
            case 137:
                s[0] = '8';
                s[1] = '9';
                break;
            case 138:
                s[0] = '8';
                s[1] = 'A';
                break;
            case 139:
                s[0] = '8';
                s[1] = 'B';
                break;
            case 140:
                s[0] = '8';
                s[1] = 'C';
                break;
            case 141:
                s[0] = '8';
                s[1] = 'D';
                break;
            case 142:
                s[0] = '8';
                s[1] = 'E';
                break;
            case 143:
                s[0] = '8';
                s[1] = 'F';
                break;
            case 144:
                s[0] = '9';
                s[1] = '0';
                break;
            case 145:
                s[0] = '9';
                s[1] = '1';
                break;
            case 146:
                s[0] = '9';
                s[1] = '2';
                break;
            case 147:
                s[0] = '9';
                s[1] = '3';
                break;
            case 148:
                s[0] = '9';
                s[1] = '4';
                break;
            case 149:
                s[0] = '9';
                s[1] = '5';
                break;
            case 150:
                s[0] = '9';
                s[1] = '6';
                break;
            case 151:
                s[0] = '9';
                s[1] = '7';
                break;
            case 152:
                s[0] = '9';
                s[1] = '8';
                break;
            case 153:
                s[0] = '9';
                s[1] = '9';
                break;
            case 154:
                s[0] = '9';
                s[1] = 'A';
                break;
            case 155:
                s[0] = '9';
                s[1] = 'B';
                break;
            case 156:
                s[0] = '9';
                s[1] = 'C';
                break;
            case 157:
                s[0] = '9';
                s[1] = 'D';
                break;
            case 158:
                s[0] = '9';
                s[1] = 'E';
                break;
            case 159:
                s[0] = '9';
                s[1] = 'F';
                break;
            case 160:
                s[0] = 'A';
                s[1] = '0';
                break;
            case 161:
                s[0] = 'A';
                s[1] = '1';
                break;
            case 162:
                s[0] = 'A';
                s[1] = '2';
                break;
            case 163:
                s[0] = 'A';
                s[1] = '3';
                break;
            case 164:
                s[0] = 'A';
                s[1] = '4';
                break;
            case 165:
                s[0] = 'A';
                s[1] = '5';
                break;
            case 166:
                s[0] = 'A';
                s[1] = '6';
                break;
            case 167:
                s[0] = 'A';
                s[1] = '7';
                break;
            case 168:
                s[0] = 'A';
                s[1] = '8';
                break;
            case 169:
                s[0] = 'A';
                s[1] = '9';
                break;
            case 170:
                s[0] = 'A';
                s[1] = 'A';
                break;
            case 171:
                s[0] = 'A';
                s[1] = 'B';
                break;
            case 172:
                s[0] = 'A';
                s[1] = 'C';
                break;
            case 173:
                s[0] = 'A';
                s[1] = 'D';
                break;
            case 174:
                s[0] = 'A';
                s[1] = 'E';
                break;
            case 175:
                s[0] = 'A';
                s[1] = 'F';
                break;
            case 176:
                s[0] = 'B';
                s[1] = '0';
                break;
            case 177:
                s[0] = 'B';
                s[1] = '1';
                break;
            case 178:
                s[0] = 'B';
                s[1] = '2';
                break;
            case 179:
                s[0] = 'B';
                s[1] = '3';
                break;
            case 180:
                s[0] = 'B';
                s[1] = '4';
                break;
            case 181:
                s[0] = 'B';
                s[1] = '5';
                break;
            case 182:
                s[0] = 'B';
                s[1] = '6';
                break;
            case 183:
                s[0] = 'B';
                s[1] = '7';
                break;
            case 184:
                s[0] = 'B';
                s[1] = '8';
                break;
            case 185:
                s[0] = 'B';
                s[1] = '9';
                break;
            case 186:
                s[0] = 'B';
                s[1] = 'A';
                break;
            case 187:
                s[0] = 'B';
                s[1] = 'B';
                break;
            case 188:
                s[0] = 'B';
                s[1] = 'C';
                break;
            case 189:
                s[0] = 'B';
                s[1] = 'D';
                break;
            case 190:
                s[0] = 'B';
                s[1] = 'E';
                break;
            case 191:
                s[0] = 'B';
                s[1] = 'F';
                break;
            case 192:
                s[0] = 'C';
                s[1] = '0';
                break;
            case 193:
                s[0] = 'C';
                s[1] = '1';
                break;
            case 194:
                s[0] = 'C';
                s[1] = '2';
                break;
            case 195:
                s[0] = 'C';
                s[1] = '3';
                break;
            case 196:
                s[0] = 'C';
                s[1] = '4';
                break;
            case 197:
                s[0] = 'C';
                s[1] = '5';
                break;
            case 198:
                s[0] = 'C';
                s[1] = '6';
                break;
            case 199:
                s[0] = 'C';
                s[1] = '7';
                break;
            case 200:
                s[0] = 'C';
                s[1] = '8';
                break;
            case 201:
                s[0] = 'C';
                s[1] = '9';
                break;
            case 202:
                s[0] = 'C';
                s[1] = 'A';
                break;
            case 203:
                s[0] = 'C';
                s[1] = 'B';
                break;
            case 204:
                s[0] = 'C';
                s[1] = 'C';
                break;
            case 205:
                s[0] = 'C';
                s[1] = 'D';
                break;
            case 206:
                s[0] = 'C';
                s[1] = 'E';
                break;
            case 207:
                s[0] = 'C';
                s[1] = 'F';
                break;
            case 208:
                s[0] = 'D';
                s[1] = '0';
                break;
            case 209:
                s[0] = 'D';
                s[1] = '1';
                break;
            case 210:
                s[0] = 'D';
                s[1] = '2';
                break;
            case 211:
                s[0] = 'D';
                s[1] = '3';
                break;
            case 212:
                s[0] = 'D';
                s[1] = '4';
                break;
            case 213:
                s[0] = 'D';
                s[1] = '5';
                break;
            case 214:
                s[0] = 'D';
                s[1] = '6';
                break;
            case 215:
                s[0] = 'D';
                s[1] = '7';
                break;
            case 216:
                s[0] = 'D';
                s[1] = '8';
                break;
            case 217:
                s[0] = 'D';
                s[1] = '9';
                break;
            case 218:
                s[0] = 'D';
                s[1] = 'A';
                break;
            case 219:
                s[0] = 'D';
                s[1] = 'B';
                break;
            case 220:
                s[0] = 'D';
                s[1] = 'C';
                break;
            case 221:
                s[0] = 'D';
                s[1] = 'D';
                break;
            case 222:
                s[0] = 'D';
                s[1] = 'E';
                break;
            case 223:
                s[0] = 'D';
                s[1] = 'F';
                break;
            case 224:
                s[0] = 'E';
                s[1] = '0';
                break;
            case 225:
                s[0] = 'E';
                s[1] = '1';
                break;
            case 226:
                s[0] = 'E';
                s[1] = '2';
                break;
            case 227:
                s[0] = 'E';
                s[1] = '3';
                break;
            case 228:
                s[0] = 'E';
                s[1] = '4';
                break;
            case 229:
                s[0] = 'E';
                s[1] = '5';
                break;
            case 230:
                s[0] = 'E';
                s[1] = '6';
                break;
            case 231:
                s[0] = 'E';
                s[1] = '7';
                break;
            case 232:
                s[0] = 'E';
                s[1] = '8';
                break;
            case 233:
                s[0] = 'E';
                s[1] = '9';
                break;
            case 234:
                s[0] = 'E';
                s[1] = 'A';
                break;
            case 235:
                s[0] = 'E';
                s[1] = 'B';
                break;
            case 236:
                s[0] = 'E';
                s[1] = 'C';
                break;
            case 237:
                s[0] = 'E';
                s[1] = 'D';
                break;
            case 238:
                s[0] = 'E';
                s[1] = 'E';
                break;
            case 239:
                s[0] = 'E';
                s[1] = 'F';
                break;
            case 240:
                s[0] = 'F';
                s[1] = '0';
                break;
            case 241:
                s[0] = 'F';
                s[1] = '1';
                break;
            case 242:
                s[0] = 'F';
                s[1] = '2';
                break;
            case 243:
                s[0] = 'F';
                s[1] = '3';
                break;
            case 244:
                s[0] = 'F';
                s[1] = '4';
                break;
            case 245:
                s[0] = 'F';
                s[1] = '5';
                break;
            case 246:
                s[0] = 'F';
                s[1] = '6';
                break;
            case 247:
                s[0] = 'F';
                s[1] = '7';
                break;
            case 248:
                s[0] = 'F';
                s[1] = '8';
                break;
            case 249:
                s[0] = 'F';
                s[1] = '9';
                break;
            case 250:
                s[0] = 'F';
                s[1] = 'A';
                break;
            case 251:
                s[0] = 'F';
                s[1] = 'B';
                break;
            case 252:
                s[0] = 'F';
                s[1] = 'C';
                break;
            case 253:
                s[0] = 'F';
                s[1] = 'D';
                break;
            case 254:
                s[0] = 'F';
                s[1] = 'E';
                break;
            case 255:
                s[0] = 'F';
                s[1] = 'F';
                break;
        }
    }
}
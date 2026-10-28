using System.Text;
using UnityEngine;

public static class NGUIText
{
	private static Color mInvisible = new Color(0f, 0f, 0f, 0f);

	private static BetterList<Color> mColors = new BetterList<Color>();

	private static CharacterInfo mTempChar;

	public static Color ParseColor(string text, int offset)
	{
		int num = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		float num4 = 1f / 255f;
		return new Color(num4 * (float)num, num4 * (float)num2, num4 * (float)num3);
	}

	public static string EncodeColor(Color c)
	{
		int num = 0xFFFFFF & (NGUIMath.ColorToInt(c) >> 8);
		return NGUIMath.DecimalToHex(num);
	}

	public static int ParseSymbol(string text, int index)
	{
		int length = text.Length;
		if (index + 2 < length)
		{
			if (text[index + 1] == '-')
			{
				if (text[index + 2] == ']')
				{
					return 3;
				}
			}
			else if (index + 7 < length && text[index + 7] == ']')
			{
				Color c = ParseColor(text, index + 1);
				if (EncodeColor(c) == text.Substring(index + 1, 6).ToUpper())
				{
					return 8;
				}
			}
		}
		return 0;
	}

	public static bool ParseSymbol(string text, ref int index)
	{
		int num = ParseSymbol(text, index);
		if (num != 0)
		{
			index += num;
			return true;
		}
		return false;
	}

	public static bool ParseSymbol(string text, ref int index, BetterList<Color> colors, bool premultiply)
	{
		if (colors == null)
		{
			return ParseSymbol(text, ref index);
		}
		int length = text.Length;
		if (index + 2 < length)
		{
			if (text[index + 1] == '-')
			{
				if (text[index + 2] == ']')
				{
					if (colors != null && colors.size > 1)
					{
						colors.RemoveAt(colors.size - 1);
					}
					index += 3;
					return true;
				}
			}
			else if (index + 7 < length && text[index + 7] == ']')
			{
				if (colors != null)
				{
					Color color = ParseColor(text, index + 1);
					if (EncodeColor(color) != text.Substring(index + 1, 6).ToUpper())
					{
						return false;
					}
					color.a = colors[colors.size - 1].a;
					if (premultiply && color.a != 1f)
					{
						color = Color.Lerp(mInvisible, color, color.a);
					}
					colors.Add(color);
				}
				index += 8;
				return true;
			}
		}
		return false;
	}

	public static string StripSymbols(string text)
	{
		if (text != null)
		{
			int num = 0;
			int length = text.Length;
			while (num < length)
			{
				char c = text[num];
				if (c == '[')
				{
					int num2 = ParseSymbol(text, num);
					if (num2 != 0)
					{
						text = text.Remove(num, num2);
						length = text.Length;
						continue;
					}
				}
				num++;
			}
		}
		return text;
	}

	public static void Align(BetterList<Vector3> verts, int indexOffset, TextAlignment alignment, int pos, int lineWidth)
	{
		if (alignment == TextAlignment.Left)
		{
			return;
		}
		float num = 0f;
		if (alignment == TextAlignment.Right)
		{
			num = lineWidth - pos;
			if (num < 0f)
			{
				num = 0f;
			}
		}
		else
		{
			num = Mathf.RoundToInt((float)(lineWidth - pos) * 0.5f);
			if (num < 0f)
			{
				num = 0f;
			}
			if ((lineWidth & 1) == 1)
			{
				num += 0.5f;
			}
		}
		for (int i = indexOffset; i < verts.size; i++)
		{
			Vector3 vector = verts.buffer[i];
			vector.x += num;
			verts.buffer[i] = vector;
		}
	}

	public static void EndLine(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && s[num] == ' ')
		{
			s[num] = '\n';
		}
		else
		{
			s.Append('\n');
		}
	}

	public static Vector2 CalculatePrintedSize(string text, Font font, int size, FontStyle style, bool encoding)
	{
		Vector2 zero = Vector2.zero;
		if (font != null && !string.IsNullOrEmpty(text))
		{
			if (encoding)
			{
				text = StripSymbols(text);
			}
			font.RequestCharactersInTexture(text, size, style);
			float num = 0f;
			float num2 = 0f;
			float num3 = size;
			float num4 = 0f;
			int i = 0;
			for (int length = text.Length; i < length; i++)
			{
				char c = text[i];
				if (c == '\n')
				{
					if (num > num4)
					{
						num4 = num;
					}
					num = 0f;
					num2 += num3;
				}
				else if (c >= ' ')
				{
					if (font.GetCharacterInfo(c, out mTempChar, size, style))
					{
						num += mTempChar.width;
					}
					zero.x = ((!(num > num4)) ? num4 : num);
					zero.y = num2 + num3;
				}
			}
		}
		return zero;
	}

	public static int CalculateOffsetToFit(string text, Font font, int size, FontStyle style, int lineWidth)
	{
		if (font == null || string.IsNullOrEmpty(text) || lineWidth < 1)
		{
			return 0;
		}
		font.RequestCharactersInTexture(text, size, style);
		int length = text.Length;
		int num = lineWidth;
		int num2 = length;
		while (num2 > 0 && num > 0)
		{
			char ch = text[--num2];
			if (font.GetCharacterInfo(ch, out mTempChar, size, style))
			{
				num -= (int)mTempChar.width;
			}
		}
		if (num < 0)
		{
			num2++;
		}
		return num2;
	}

	public static bool WrapText(string text, Font font, int size, FontStyle style, int width, int height, int maxLines, bool encoding, out string finalText)
	{
		if (width < 1 || height < 1)
		{
			finalText = string.Empty;
			return false;
		}
		int a = ((maxLines <= 0) ? 999999 : maxLines);
		a = Mathf.Min(a, height / size);
		if (a == 0)
		{
			finalText = string.Empty;
			return false;
		}
		if (font != null)
		{
			font.RequestCharactersInTexture(text, size, style);
		}
		StringBuilder s = new StringBuilder();
		int length = text.Length;
		int num = width;
		int num2 = 0;
		int i = 0;
		int j = 0;
		bool flag = true;
		bool flag2 = maxLines != 1;
		int num3 = 1;
		for (; j < length; j++)
		{
			char c = text[j];
			if (c == '\n')
			{
				if (!flag2 || num3 == a)
				{
					break;
				}
				num = width;
				if (i < j)
				{
					s.Append(text.Substring(i, j - i + 1));
				}
				else
				{
					s.Append(c);
				}
				flag = true;
				num3++;
				i = j + 1;
				num2 = 0;
				continue;
			}
			if (c == ' ' && num2 != 32 && i < j)
			{
				s.Append(text.Substring(i, j - i + 1));
				flag = false;
				i = j + 1;
				num2 = c;
			}
			if (ParseSymbol(text, ref j))
			{
				j--;
				continue;
			}
			int num4 = 0;
			if (font.GetCharacterInfo(c, out mTempChar, size, style))
			{
				num4 = Mathf.RoundToInt(mTempChar.width);
			}
			num -= num4;
			if (num < 0)
			{
				if (flag || !flag2 || num3 == a)
				{
					s.Append(text.Substring(i, Mathf.Max(0, j - i)));
					if (!flag2 || num3 == a)
					{
						i = j;
						break;
					}
					EndLine(ref s);
					flag = true;
					num3++;
					if (c == ' ')
					{
						i = j + 1;
						num = width;
					}
					else
					{
						i = j;
						num = width - num4;
					}
					num2 = 0;
				}
				else
				{
					for (; i < length && text[i] == ' '; i++)
					{
					}
					flag = true;
					num = width;
					j = i - 1;
					num2 = 0;
					if (!flag2 || num3 == a)
					{
						break;
					}
					num3++;
					EndLine(ref s);
				}
			}
			else
			{
				num2 = c;
			}
		}
		if (i < j)
		{
			s.Append(text.Substring(i, j - i));
		}
		finalText = s.ToString();
		return !flag2 || j == length || (maxLines > 0 && num3 <= maxLines);
	}

	public static void Print(string text, Font font, int size, FontStyle style, Color32 color, bool encoding, TextAlignment alignment, int lineWidth, bool premultiply, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (font == null || string.IsNullOrEmpty(text))
		{
			return;
		}
		float num = 0f;
		font.RequestCharactersInTexture("j", size, style);
		font.GetCharacterInfo('j', out mTempChar, size, style);
		num = (float)size + mTempChar.vert.yMax;
		font.RequestCharactersInTexture(text, size, style);
		mColors.Add(color);
		int size2 = verts.size;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector2 zero3 = Vector2.zero;
		Vector2 zero4 = Vector2.zero;
		int length = text.Length;
		for (int i = 0; i < length; i++)
		{
			char c = text[i];
			if (c == '\n')
			{
				if (num3 > num2)
				{
					num2 = num3;
				}
				if (alignment != TextAlignment.Left)
				{
					Align(verts, size2, alignment, num3, lineWidth);
					size2 = verts.size;
				}
				num3 = 0;
				num4 += size;
			}
			else
			{
				if (c < ' ')
				{
					continue;
				}
				if (encoding && ParseSymbol(text, ref i, mColors, premultiply))
				{
					color = mColors[mColors.size - 1];
					i--;
				}
				else if (font.GetCharacterInfo(c, out mTempChar, size, style))
				{
					zero.x = (float)num3 + mTempChar.vert.xMin;
					zero.y = 0f - ((float)num4 - mTempChar.vert.yMax + num);
					zero2.x = zero.x + mTempChar.vert.width;
					zero2.y = zero.y - mTempChar.vert.height;
					zero3.x = mTempChar.uv.xMin;
					zero3.y = mTempChar.uv.yMin;
					zero4.x = mTempChar.uv.xMax;
					zero4.y = mTempChar.uv.yMax;
					num3 += (int)mTempChar.width;
					for (int j = 0; j < 4; j++)
					{
						cols.Add(color);
					}
					if (mTempChar.flipped)
					{
						uvs.Add(new Vector2(zero3.x, zero4.y));
						uvs.Add(new Vector2(zero3.x, zero3.y));
						uvs.Add(new Vector2(zero4.x, zero3.y));
						uvs.Add(new Vector2(zero4.x, zero4.y));
					}
					else
					{
						uvs.Add(new Vector2(zero4.x, zero3.y));
						uvs.Add(new Vector2(zero3.x, zero3.y));
						uvs.Add(new Vector2(zero3.x, zero4.y));
						uvs.Add(new Vector2(zero4.x, zero4.y));
					}
					verts.Add(new Vector3(zero2.x, zero.y));
					verts.Add(new Vector3(zero.x, zero.y));
					verts.Add(new Vector3(zero.x, zero2.y));
					verts.Add(new Vector3(zero2.x, zero2.y));
				}
			}
		}
		if (alignment != TextAlignment.Left && size2 < verts.size)
		{
			Align(verts, size2, alignment, num3, lineWidth);
			size2 = verts.size;
		}
		mColors.Clear();
	}
}

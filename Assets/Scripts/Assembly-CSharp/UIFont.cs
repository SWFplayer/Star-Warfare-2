using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Font")]
public class UIFont : MonoBehaviour
{
	public enum SymbolStyle
	{
		None = 0,
		Uncolored = 1,
		Colored = 2
	}

	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[SerializeField]
	[HideInInspector]
	private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private BMFont mFont = new BMFont();

	[SerializeField]
	[HideInInspector]
	private int mSpacingX;

	[SerializeField]
	[HideInInspector]
	private int mSpacingY;

	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[HideInInspector]
	[SerializeField]
	private UIFont mReplacement;

	[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	[SerializeField]
	[HideInInspector]
	private List<BMSymbol> mSymbols = new List<BMSymbol>();

	[HideInInspector]
	[SerializeField]
	private Font mDynamicFont;

	[SerializeField]
	[HideInInspector]
	private int mDynamicFontSize = 16;

	[HideInInspector]
	[SerializeField]
	private FontStyle mDynamicFontStyle;

	private UISpriteData mSprite;

	private int mPMA = -1;

	private bool mSpriteSet;

	private static BetterList<Color> mColors = new BetterList<Color>();

	public BMFont bmFont
	{
		get
		{
			return (!(mReplacement != null)) ? mFont : mReplacement.bmFont;
		}
	}

	public int texWidth
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texWidth : ((mFont == null) ? 1 : mFont.texWidth);
		}
	}

	public int texHeight
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texHeight : ((mFont == null) ? 1 : mFont.texHeight);
		}
	}

	public bool hasSymbols
	{
		get
		{
			return (!(mReplacement != null)) ? (mSymbols.Count != 0) : mReplacement.hasSymbols;
		}
	}

	public List<BMSymbol> symbols
	{
		get
		{
			return (!(mReplacement != null)) ? mSymbols : mReplacement.symbols;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return (!(mReplacement != null)) ? mAtlas : mReplacement.atlas;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.atlas = value;
			}
			else
			{
				if (!(mAtlas != value))
				{
					return;
				}
				if (value == null)
				{
					if (mAtlas != null)
					{
						mMat = mAtlas.spriteMaterial;
					}
					if (sprite != null)
					{
						mUVRect = uvRect;
					}
				}
				mPMA = -1;
				mAtlas = value;
				MarkAsDirty();
			}
		}
	}

	public Material material
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.material;
			}
			if (mAtlas != null)
			{
				return mAtlas.spriteMaterial;
			}
			if (mMat != null)
			{
				if (mDynamicFont != null && mMat != mDynamicFont.material)
				{
					mMat.mainTexture = mDynamicFont.material.mainTexture;
				}
				return mMat;
			}
			if (mDynamicFont != null)
			{
				return mDynamicFont.material;
			}
			return null;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.material = value;
			}
			else if (mMat != value)
			{
				mPMA = -1;
				mMat = value;
				MarkAsDirty();
			}
		}
	}

	public float pixelSize
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.pixelSize;
			}
			if (mAtlas != null)
			{
				return mAtlas.pixelSize;
			}
			return mPixelSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.pixelSize = value;
				return;
			}
			if (mAtlas != null)
			{
				mAtlas.pixelSize = value;
				return;
			}
			float num = Mathf.Clamp(value, 0.25f, 4f);
			if (mPixelSize != num)
			{
				mPixelSize = num;
				MarkAsDirty();
			}
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.premultipliedAlpha;
			}
			if (mAtlas != null)
			{
				return mAtlas.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public Texture2D texture
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.texture;
			}
			Material material = this.material;
			return (!(material != null)) ? null : (material.mainTexture as Texture2D);
		}
	}

	public Rect uvRect
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.uvRect;
			}
			if (mAtlas != null && mSprite == null && sprite != null)
			{
				Texture texture = mAtlas.texture;
				if (texture != null)
				{
					mUVRect = new Rect(mSprite.x - mSprite.paddingLeft, mSprite.y - mSprite.paddingTop, mSprite.width + mSprite.paddingLeft + mSprite.paddingRight, mSprite.height + mSprite.paddingTop + mSprite.paddingBottom);
					mUVRect = NGUIMath.ConvertToTexCoords(mUVRect, texture.width, texture.height);
					if (mSprite.hasPadding)
					{
						Trim();
					}
				}
			}
			return mUVRect;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.uvRect = value;
			}
			else if (sprite == null && mUVRect != value)
			{
				mUVRect = value;
				MarkAsDirty();
			}
		}
	}

	public string spriteName
	{
		get
		{
			return (!(mReplacement != null)) ? mFont.spriteName : mReplacement.spriteName;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteName = value;
			}
			else if (mFont.spriteName != value)
			{
				mFont.spriteName = value;
				MarkAsDirty();
			}
		}
	}

	public int horizontalSpacing
	{
		get
		{
			return (!(mReplacement != null)) ? mSpacingX : mReplacement.horizontalSpacing;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.horizontalSpacing = value;
			}
			else if (mSpacingX != value)
			{
				mSpacingX = value;
				MarkAsDirty();
			}
		}
	}

	public int verticalSpacing
	{
		get
		{
			return (!(mReplacement != null)) ? mSpacingY : mReplacement.verticalSpacing;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.verticalSpacing = value;
			}
			else if (mSpacingY != value)
			{
				mSpacingY = value;
				MarkAsDirty();
			}
		}
	}

	public bool isValid
	{
		get
		{
			return mDynamicFont != null || mFont.isValid;
		}
	}

	[Obsolete("Use UIFont.defaultSize instead")]
	public int size
	{
		get
		{
			return defaultSize;
		}
		set
		{
			defaultSize = value;
		}
	}

	public int defaultSize
	{
		get
		{
			return (mReplacement != null) ? mReplacement.defaultSize : ((!isDynamic) ? mFont.charSize : mDynamicFontSize);
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.defaultSize = value;
			}
			else
			{
				mDynamicFontSize = value;
			}
		}
	}

	public UISpriteData sprite
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.sprite;
			}
			if (!mSpriteSet)
			{
				mSprite = null;
			}
			if (mSprite == null)
			{
				if (mAtlas != null && !string.IsNullOrEmpty(mFont.spriteName))
				{
					mSprite = mAtlas.GetSprite(mFont.spriteName);
					if (mSprite == null)
					{
						mSprite = mAtlas.GetSprite(base.name);
					}
					mSpriteSet = true;
					if (mSprite == null)
					{
						mFont.spriteName = null;
					}
				}
				int i = 0;
				for (int count = mSymbols.Count; i < count; i++)
				{
					symbols[i].MarkAsDirty();
				}
			}
			return mSprite;
		}
	}

	public UIFont replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIFont uIFont = value;
			if (uIFont == this)
			{
				uIFont = null;
			}
			if (mReplacement != uIFont)
			{
				if (uIFont != null && uIFont.replacement == this)
				{
					uIFont.replacement = null;
				}
				if (mReplacement != null)
				{
					MarkAsDirty();
				}
				mReplacement = uIFont;
				MarkAsDirty();
			}
		}
	}

	public bool isDynamic
	{
		get
		{
			return mDynamicFont != null;
		}
	}

	public Font dynamicFont
	{
		get
		{
			return (!(mReplacement != null)) ? mDynamicFont : mReplacement.dynamicFont;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFont = value;
			}
			else if (mDynamicFont != value)
			{
				if (mDynamicFont != null)
				{
					material = null;
				}
				mDynamicFont = value;
				MarkAsDirty();
			}
		}
	}

	public FontStyle dynamicFontStyle
	{
		get
		{
			return (!(mReplacement != null)) ? mDynamicFontStyle : mReplacement.dynamicFontStyle;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFontStyle = value;
			}
			else if (mDynamicFontStyle != value)
			{
				mDynamicFontStyle = value;
				MarkAsDirty();
			}
		}
	}

	private Texture dynamicTexture
	{
		get
		{
			if ((bool)mReplacement)
			{
				return mReplacement.dynamicTexture;
			}
			if (isDynamic)
			{
				return mDynamicFont.material.mainTexture;
			}
			return null;
		}
	}

	private void Trim()
	{
		Texture texture = mAtlas.texture;
		if (texture != null && mSprite != null)
		{
			Rect rect = NGUIMath.ConvertToPixels(mUVRect, this.texture.width, this.texture.height, true);
			Rect rect2 = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
			int xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
			int yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
			int xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
			int yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
			mFont.Trim(xMin, yMin, xMax, yMax);
		}
	}

	private bool References(UIFont font)
	{
		if (font == null)
		{
			return false;
		}
		if (font == this)
		{
			return true;
		}
		return mReplacement != null && mReplacement.References(font);
	}

	public static bool CheckIfRelated(UIFont a, UIFont b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		if (a.isDynamic && b.isDynamic && a.dynamicFont.fontNames[0] == b.dynamicFont.fontNames[0])
		{
			return true;
		}
		return a == b || a.References(b) || b.References(a);
	}

	public void MarkAsDirty()
	{
		if (mReplacement != null)
		{
			mReplacement.MarkAsDirty();
		}
		mSprite = null;
		UILabel[] array = NGUITools.FindActive<UILabel>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			UILabel uILabel = array[i];
			if (uILabel.enabled && NGUITools.GetActive(uILabel.gameObject) && CheckIfRelated(this, uILabel.bitmapFont))
			{
				UIFont bitmapFont = uILabel.bitmapFont;
				uILabel.bitmapFont = null;
				uILabel.bitmapFont = bitmapFont;
			}
		}
		int j = 0;
		for (int count = mSymbols.Count; j < count; j++)
		{
			symbols[j].MarkAsDirty();
		}
	}

	public Vector2 CalculatePrintedSize(string text, int size, bool encoding, SymbolStyle symbolStyle)
	{
		if (mReplacement != null)
		{
			return mReplacement.CalculatePrintedSize(text, size, encoding, symbolStyle);
		}
		if (isDynamic)
		{
			return NGUIText.CalculatePrintedSize(text, mDynamicFont, size, mDynamicFontStyle, encoding);
		}
		Vector2 zero = Vector2.zero;
		if (mFont != null && mFont.isValid && !string.IsNullOrEmpty(text))
		{
			if (encoding)
			{
				text = NGUIText.StripSymbols(text);
			}
			int length = text.Length;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = size + mSpacingY;
			bool flag = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (c == '\n')
				{
					if (num2 > num)
					{
						num = num2;
					}
					num2 = 0;
					num3 += num5;
					num4 = 0;
					continue;
				}
				if (c < ' ')
				{
					num4 = 0;
					continue;
				}
				BMSymbol bMSymbol = ((!flag) ? null : MatchSymbol(text, i, length));
				if (bMSymbol == null)
				{
					BMGlyph glyph = mFont.GetGlyph(c);
					if (glyph != null)
					{
						num2 += mSpacingX + ((num4 == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(num4)));
						num4 = c;
					}
				}
				else
				{
					num2 += mSpacingX + bMSymbol.width;
					i += bMSymbol.length - 1;
					num4 = 0;
				}
			}
			zero.x = ((num2 <= num) ? num : num2);
			zero.y = num3 + num5;
		}
		return zero;
	}

	public string GetEndOfLineThatFits(string text, int size, int lineWidth, bool encoding, SymbolStyle symbolStyle)
	{
		int length = text.Length;
		int num = CalculateOffsetToFit(text, size, lineWidth, encoding, symbolStyle);
		return text.Substring(num, length - num);
	}

	public int CalculateOffsetToFit(string text, int size, int lineWidth, bool encoding, SymbolStyle symbolStyle)
	{
		if (lineWidth < 1)
		{
			return 0;
		}
		if (mReplacement != null)
		{
			return mReplacement.CalculateOffsetToFit(text, size, lineWidth, encoding, symbolStyle);
		}
		if (isDynamic)
		{
			return NGUIText.CalculateOffsetToFit(text, mDynamicFont, size, mDynamicFontStyle, lineWidth);
		}
		int length = text.Length;
		int num = lineWidth;
		BMGlyph bMGlyph = null;
		int num2 = length;
		bool flag = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
		while (num2 > 0 && num > 0)
		{
			char c = text[--num2];
			BMSymbol bMSymbol = ((!flag) ? null : MatchSymbol(text, num2, length));
			int num3 = mSpacingX;
			if (bMSymbol != null)
			{
				num3 += bMSymbol.advance;
			}
			else
			{
				BMGlyph glyph = mFont.GetGlyph(c);
				if (glyph == null)
				{
					bMGlyph = null;
					continue;
				}
				num3 += glyph.advance + ((bMGlyph != null) ? bMGlyph.GetKerning(c) : 0);
				bMGlyph = glyph;
			}
			num -= num3;
		}
		if (num < 0)
		{
			num2++;
		}
		return num2;
	}

	public bool WrapText(string text, int size, out string finalText, int width, int height, int maxLines, bool encoding)
	{
		return WrapText(text, size, out finalText, width, height, maxLines, encoding, SymbolStyle.None);
	}

	public bool WrapText(string text, int size, out string finalText, int width, int height, int maxLineCount)
	{
		return WrapText(text, size, out finalText, width, height, maxLineCount, false, SymbolStyle.None);
	}

	public bool WrapText(string text, int size, out string finalText, int width, int height, int maxLines, bool encoding, SymbolStyle symbolStyle)
	{
		if (mReplacement != null)
		{
			return mReplacement.WrapText(text, size, out finalText, width, height, maxLines, encoding, symbolStyle);
		}
		if (isDynamic)
		{
			return NGUIText.WrapText(text, mDynamicFont, size, mDynamicFontStyle, width, height, maxLines, encoding, out finalText);
		}
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
		StringBuilder s = new StringBuilder();
		int length = text.Length;
		int num = width;
		int num2 = 0;
		int i = 0;
		int j = 0;
		bool flag = true;
		bool flag2 = maxLines != 1;
		int num3 = 1;
		bool flag3 = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
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
			if (NGUIText.ParseSymbol(text, ref j))
			{
				j--;
				continue;
			}
			BMSymbol bMSymbol = ((!flag3) ? null : MatchSymbol(text, j, length));
			int num4 = mSpacingX;
			if (bMSymbol != null)
			{
				num4 += bMSymbol.advance;
			}
			else
			{
				BMGlyph bMGlyph = ((bMSymbol != null) ? null : mFont.GetGlyph(c));
				if (bMGlyph == null)
				{
					continue;
				}
				num4 += ((num2 == 0) ? bMGlyph.advance : (bMGlyph.advance + bMGlyph.GetKerning(num2)));
			}
			num -= num4;
			if (num < 0)
			{
				if (!flag && flag2 && num3 != a)
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
					NGUIText.EndLine(ref s);
					continue;
				}
				s.Append(text.Substring(i, Mathf.Max(0, j - i)));
				if (!flag2 || num3 == a)
				{
					i = j;
					break;
				}
				NGUIText.EndLine(ref s);
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
				num2 = c;
			}
			if (bMSymbol != null)
			{
				j += bMSymbol.length - 1;
				num2 = 0;
			}
		}
		if (i < j)
		{
			s.Append(text.Substring(i, j - i));
		}
		finalText = s.ToString();
		return !flag2 || j == length || (maxLines > 0 && num3 <= maxLines);
	}

	public void Print(string text, int size, Color32 color, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, bool encoding, SymbolStyle symbolStyle, TextAlignment alignment, int lineWidth, bool premultiply)
	{
		if (mReplacement != null)
		{
			mReplacement.Print(text, size, color, verts, uvs, cols, encoding, symbolStyle, alignment, lineWidth, premultiply);
		}
		else
		{
			if (text == null)
			{
				return;
			}
			if (!isValid)
			{
				Debug.LogError("Attempting to print using an invalid font!");
				return;
			}
			if (isDynamic)
			{
				NGUIText.Print(text, dynamicFont, size, mDynamicFontStyle, color, encoding, alignment, lineWidth, premultiply, verts, uvs, cols);
				return;
			}
			mColors.Clear();
			mColors.Add(color);
			int num = verts.size;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = size + mSpacingY;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector2 zero3 = Vector2.zero;
			Vector2 zero4 = Vector2.zero;
			float num7 = uvRect.width / (float)mFont.texWidth;
			float num8 = mUVRect.height / (float)mFont.texHeight;
			int length = text.Length;
			bool flag = encoding && symbolStyle != SymbolStyle.None && hasSymbols && sprite != null;
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
						NGUIText.Align(verts, num, alignment, num3, lineWidth);
						num = verts.size;
					}
					num3 = 0;
					num4 += num6;
					num5 = 0;
					continue;
				}
				if (c < ' ')
				{
					num5 = 0;
					continue;
				}
				if (encoding && NGUIText.ParseSymbol(text, ref i, mColors, premultiply))
				{
					color = mColors[mColors.size - 1];
					i--;
					continue;
				}
				BMSymbol bMSymbol = ((!flag) ? null : MatchSymbol(text, i, length));
				if (bMSymbol == null)
				{
					BMGlyph glyph = mFont.GetGlyph(c);
					if (glyph == null)
					{
						continue;
					}
					if (num5 != 0)
					{
						num3 += glyph.GetKerning(num5);
					}
					if (c == ' ')
					{
						num3 += mSpacingX + glyph.advance;
						num5 = c;
						continue;
					}
					zero.x = num3 + glyph.offsetX;
					zero.y = -(num4 + glyph.offsetY);
					zero2.x = zero.x + (float)glyph.width;
					zero2.y = zero.y - (float)glyph.height;
					zero3.x = mUVRect.xMin + num7 * (float)glyph.x;
					zero3.y = mUVRect.yMax - num8 * (float)glyph.y;
					zero4.x = zero3.x + num7 * (float)glyph.width;
					zero4.y = zero3.y - num8 * (float)glyph.height;
					num3 += mSpacingX + glyph.advance;
					num5 = c;
					if (glyph.channel == 0 || glyph.channel == 15)
					{
						for (int j = 0; j < 4; j++)
						{
							cols.Add(color);
						}
					}
					else
					{
						Color color2 = color;
						color2 *= 0.49f;
						switch (glyph.channel)
						{
						case 1:
							color2.b += 0.51f;
							break;
						case 2:
							color2.g += 0.51f;
							break;
						case 4:
							color2.r += 0.51f;
							break;
						case 8:
							color2.a += 0.51f;
							break;
						}
						for (int k = 0; k < 4; k++)
						{
							cols.Add(color2);
						}
					}
				}
				else
				{
					zero.x = num3 + bMSymbol.offsetX;
					zero.y = -(num4 + bMSymbol.offsetY);
					zero2.x = zero.x + (float)bMSymbol.width;
					zero2.y = zero.y - (float)bMSymbol.height;
					Rect rect = bMSymbol.uvRect;
					zero3.x = rect.xMin;
					zero3.y = rect.yMax;
					zero4.x = rect.xMax;
					zero4.y = rect.yMin;
					num3 += mSpacingX + bMSymbol.advance;
					i += bMSymbol.length - 1;
					num5 = 0;
					if (symbolStyle == SymbolStyle.Colored)
					{
						for (int l = 0; l < 4; l++)
						{
							cols.Add(color);
						}
					}
					else
					{
						Color32 item = Color.white;
						item.a = color.a;
						for (int m = 0; m < 4; m++)
						{
							cols.Add(item);
						}
					}
				}
				verts.Add(new Vector3(zero2.x, zero.y));
				verts.Add(new Vector3(zero2.x, zero2.y));
				verts.Add(new Vector3(zero.x, zero2.y));
				verts.Add(new Vector3(zero.x, zero.y));
				uvs.Add(new Vector2(zero4.x, zero3.y));
				uvs.Add(new Vector2(zero4.x, zero4.y));
				uvs.Add(new Vector2(zero3.x, zero4.y));
				uvs.Add(new Vector2(zero3.x, zero3.y));
			}
			if (alignment != TextAlignment.Left && num < verts.size)
			{
				NGUIText.Align(verts, num, alignment, num3, lineWidth);
				num = verts.size;
			}
		}
	}

	private BMSymbol GetSymbol(string sequence, bool createIfMissing)
	{
		int i = 0;
		for (int count = mSymbols.Count; i < count; i++)
		{
			BMSymbol bMSymbol = mSymbols[i];
			if (bMSymbol.sequence == sequence)
			{
				return bMSymbol;
			}
		}
		if (createIfMissing)
		{
			BMSymbol bMSymbol2 = new BMSymbol();
			bMSymbol2.sequence = sequence;
			mSymbols.Add(bMSymbol2);
			return bMSymbol2;
		}
		return null;
	}

	private BMSymbol MatchSymbol(string text, int offset, int textLength)
	{
		int count = mSymbols.Count;
		if (count == 0)
		{
			return null;
		}
		textLength -= offset;
		for (int i = 0; i < count; i++)
		{
			BMSymbol bMSymbol = mSymbols[i];
			int length = bMSymbol.length;
			if (length == 0 || textLength < length)
			{
				continue;
			}
			bool flag = true;
			for (int j = 0; j < length; j++)
			{
				if (text[offset + j] != bMSymbol.sequence[j])
				{
					flag = false;
					break;
				}
			}
			if (flag && bMSymbol.Validate(atlas))
			{
				return bMSymbol;
			}
		}
		return null;
	}

	public void AddSymbol(string sequence, string spriteName)
	{
		BMSymbol symbol = GetSymbol(sequence, true);
		symbol.spriteName = spriteName;
		MarkAsDirty();
	}

	public void RemoveSymbol(string sequence)
	{
		BMSymbol symbol = GetSymbol(sequence, false);
		if (symbol != null)
		{
			symbols.Remove(symbol);
		}
		MarkAsDirty();
	}

	public void RenameSymbol(string before, string after)
	{
		BMSymbol symbol = GetSymbol(before, false);
		if (symbol != null)
		{
			symbol.sequence = after;
		}
		MarkAsDirty();
	}

	public bool UsesSprite(string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			if (s.Equals(spriteName))
			{
				return true;
			}
			int i = 0;
			for (int count = symbols.Count; i < count; i++)
			{
				BMSymbol bMSymbol = symbols[i];
				if (s.Equals(bMSymbol.spriteName))
				{
					return true;
				}
			}
		}
		return false;
	}
}

using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Label")]
[ExecuteInEditMode]
public class UILabel : UIWidget
{
	public enum Effect
	{
		None = 0,
		Shadow = 1,
		Outline = 2
	}

	public enum Overflow
	{
		ShrinkContent = 0,
		ClampContent = 1,
		ResizeFreely = 2,
		ResizeHeight = 3
	}

	public enum Crispness
	{
		Never = 0,
		OnDesktop = 1,
		Always = 2
	}

	public Crispness keepCrispWhenShrunk = Crispness.OnDesktop;
    
	[SerializeField]
	private Font mTrueTypeFont;

	[SerializeField]
	
	private UIFont mFont;
    
	[SerializeField]
	[Multiline(6)]
	private string mText = string.Empty;

	[HideInInspector]
	[SerializeField]
	private int mFontSize = 16;

	[SerializeField]
	
	private FontStyle mFontStyle;

	[SerializeField]
	
	private bool mEncoding = true;

	[SerializeField]
	
	private int mMaxLineCount;

	[SerializeField]
	
	private Effect mEffectStyle;
    
	[SerializeField]
	private Color mEffectColor = Color.black;

	[SerializeField]
	
	private UIFont.SymbolStyle mSymbols = UIFont.SymbolStyle.Uncolored;

	[SerializeField]
	
	private Vector2 mEffectDistance = Vector2.one;
    
	[SerializeField]
	private Overflow mOverflow;

	[SerializeField]
	
	private bool mShrinkToFit;
    
	[SerializeField]
	private int mMaxLineWidth;
    
	[SerializeField]
	private int mMaxLineHeight;

	[SerializeField]
	
	private float mLineWidth;

	[SerializeField]
	
	private bool mMultiline = true;

	private Font mActiveTTF;

	private bool mShouldBeProcessed = true;

	private string mProcessedText;

	private bool mPremultiply;

	private Vector2 mCalculatedSize = Vector2.zero;

	private float mScale = 1f;

	private int mLastWidth;

	private int mLastHeight;

	private int mPrintedSize;

	private bool mUseDynamicFont;

	private bool hasChanged
	{
		get
		{
			return mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				mChanged = true;
				mShouldBeProcessed = true;
			}
			else
			{
				mShouldBeProcessed = false;
			}
		}
	}

	public override Material material
	{
		get
		{
			if (mFont != null)
			{
				return mFont.material;
			}
			if (mTrueTypeFont != null)
			{
				return mTrueTypeFont.material;
			}
			return null;
		}
	}

	[Obsolete("Use UILabel.bitmapFont instead")]
	public UIFont font
	{
		get
		{
			return bitmapFont;
		}
		set
		{
			bitmapFont = value;
		}
	}

	public UIFont bitmapFont
	{
		get
		{
			return mFont;
		}
		set
		{
			if (!(mFont != value))
			{
				return;
			}
			if (value != null && value.dynamicFont != null)
			{
				trueTypeFont = value.dynamicFont;
				return;
			}
			if (trueTypeFont != null)
			{
				trueTypeFont = null;
			}
			else
			{
				RemoveFromPanel();
			}
			mFont = value;
			MarkAsChanged();
		}
	}

	public Font trueTypeFont
	{
		get
		{
			return mTrueTypeFont;
		}
		set
		{
			if (mTrueTypeFont != value)
			{
				SetActiveFont(null);
				RemoveFromPanel();
				mTrueTypeFont = value;
				hasChanged = true;
				mFont = null;
				SetActiveFont(value);
				ProcessAndRequest();
				if (mActiveTTF != null)
				{
					base.MarkAsChanged();
				}
			}
		}
	}

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			return (!(mFont != null)) ? ((UnityEngine.Object)mTrueTypeFont) : ((UnityEngine.Object)mFont);
		}
		set
		{
			UIFont uIFont = value as UIFont;
			if (uIFont != null)
			{
				bitmapFont = uIFont;
			}
			else
			{
				trueTypeFont = value as Font;
			}
		}
	}

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mText))
				{
					mText = string.Empty;
					hasChanged = true;
					ProcessAndRequest();
				}
			}
			else if (mText != value)
			{
				mText = value;
				hasChanged = true;
				ProcessAndRequest();
			}
		}
	}

	public int fontSize
	{
		get
		{
			if (mFont != null)
			{
				return mFont.defaultSize;
			}
			return mFontSize;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 144);
			if (mFontSize != value)
			{
				mFontSize = value;
				hasChanged = true;
				ProcessAndRequest();
			}
		}
	}

	public FontStyle fontStyle
	{
		get
		{
			return mFontStyle;
		}
		set
		{
			if (mFontStyle != value)
			{
				mFontStyle = value;
				hasChanged = true;
				ProcessAndRequest();
			}
		}
	}

	private bool usePrintedSize
	{
		get
		{
			if (trueTypeFont != null && overflowMethod == Overflow.ShrinkContent && keepCrispWhenShrunk != Crispness.Never)
			{
				return keepCrispWhenShrunk == Crispness.Always;
			}
			return false;
		}
	}

	public bool supportEncoding
	{
		get
		{
			return mEncoding;
		}
		set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				hasChanged = true;
			}
		}
	}

	public UIFont.SymbolStyle symbolStyle
	{
		get
		{
			return mSymbols;
		}
		set
		{
			if (mSymbols != value)
			{
				mSymbols = value;
				hasChanged = true;
			}
		}
	}

	public Overflow overflowMethod
	{
		get
		{
			return mOverflow;
		}
		set
		{
			if (mOverflow != value)
			{
				mOverflow = value;
				hasChanged = true;
			}
		}
	}

	[Obsolete("Use 'width' instead")]
	public int lineWidth
	{
		get
		{
			return base.width;
		}
		set
		{
			base.width = value;
		}
	}

	[Obsolete("Use 'height' instead")]
	public int lineHeight
	{
		get
		{
			return base.height;
		}
		set
		{
			base.height = value;
		}
	}

	public bool multiLine
	{
		get
		{
			return mMaxLineCount != 1;
		}
		set
		{
			if (mMaxLineCount != 1 != value)
			{
				mMaxLineCount = ((!value) ? 1 : 0);
				hasChanged = true;
			}
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			if (hasChanged)
			{
				ProcessText();
			}
			return base.localCorners;
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			if (hasChanged)
			{
				ProcessText();
			}
			return base.worldCorners;
		}
	}

	public int maxLineCount
	{
		get
		{
			return mMaxLineCount;
		}
		set
		{
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				hasChanged = true;
				if (overflowMethod == Overflow.ShrinkContent)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public Effect effectStyle
	{
		get
		{
			return mEffectStyle;
		}
		set
		{
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				hasChanged = true;
			}
		}
	}

	public Color effectColor
	{
		get
		{
			return mEffectColor;
		}
		set
		{
			if (!mEffectColor.Equals(value))
			{
				mEffectColor = value;
				if (mEffectStyle != Effect.None)
				{
					hasChanged = true;
				}
			}
		}
	}

	public Vector2 effectDistance
	{
		get
		{
			return mEffectDistance;
		}
		set
		{
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				hasChanged = true;
			}
		}
	}

	[Obsolete("Use 'overflowMethod == UILabel.Overflow.ShrinkContent' instead")]
	public bool shrinkToFit
	{
		get
		{
			return mOverflow == Overflow.ShrinkContent;
		}
		set
		{
			if (value)
			{
				overflowMethod = Overflow.ShrinkContent;
			}
		}
	}

	public string processedText
	{
		get
		{
			if (mLastWidth != mWidth || mLastHeight != mHeight)
			{
				mLastWidth = mWidth;
				mLastHeight = mHeight;
				mShouldBeProcessed = true;
			}
			if (hasChanged)
			{
				ProcessText();
			}
			return mProcessedText;
		}
	}

	public Vector2 printedSize
	{
		get
		{
			if (hasChanged)
			{
				ProcessText();
			}
			return mCalculatedSize;
		}
	}

	public override Vector2 localSize
	{
		get
		{
			if (hasChanged)
			{
				ProcessText();
			}
			return base.localSize;
		}
	}

	private bool isValid
	{
		get
		{
			return mFont != null || mTrueTypeFont != null;
		}
	}

	private float pixelSize
	{
		get
		{
			return (!(mFont != null)) ? 1f : mFont.pixelSize;
		}
	}

	private void ProcessAndRequest()
	{
		if (ambigiousFont != null)
		{
			ProcessText();
			if (mActiveTTF != null)
			{
				mActiveTTF.RequestCharactersInTexture(mText, (!usePrintedSize) ? fontSize : mPrintedSize, mFontStyle);
			}
		}
	}

	protected override void OnValidate()
	{
		mFontSize = Mathf.Clamp(mFontSize, 0, 144);
		UIFont uIFont = mFont;
		Font font = mTrueTypeFont;
		mFont = null;
		mTrueTypeFont = null;
		SetActiveFont(null);
		if (font != null && (uIFont == null || !mUseDynamicFont))
		{
			bitmapFont = null;
			trueTypeFont = font;
			mUseDynamicFont = true;
		}
		else if (uIFont != null)
		{
			if (uIFont.isDynamic)
			{
				trueTypeFont = uIFont.dynamicFont;
				mFontStyle = uIFont.dynamicFontStyle;
				mUseDynamicFont = true;
			}
			else
			{
				bitmapFont = uIFont;
				mUseDynamicFont = false;
			}
			mFontSize = uIFont.defaultSize;
		}
		else
		{
			trueTypeFont = font;
			mUseDynamicFont = true;
		}
		hasChanged = true;
		ProcessAndRequest();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (mTrueTypeFont == null && mFont != null && mFont.isDynamic)
		{
			mTrueTypeFont = mFont.dynamicFont;
			mFontSize = mFont.defaultSize;
			mFontStyle = mFont.dynamicFontStyle;
			mFont = null;
		}
		SetActiveFont(mTrueTypeFont);
	}

	protected override void OnDisable()
	{
		SetActiveFont(null);
		base.OnDisable();
	}

	protected void SetActiveFont(Font fnt)
	{
		if (mActiveTTF != fnt)
		{
			if (mActiveTTF != null)
			{
				Font obj = mActiveTTF;
				obj.textureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Remove(obj.textureRebuildCallback, new Font.FontTextureRebuildCallback(MarkAsChanged));
			}
			mActiveTTF = fnt;
			if (mActiveTTF != null)
			{
				Font obj2 = mActiveTTF;
				obj2.textureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Combine(obj2.textureRebuildCallback, new Font.FontTextureRebuildCallback(MarkAsChanged));
			}
		}
	}

	protected override void UpgradeFrom265()
	{
		ProcessText(true);
		if (mShrinkToFit)
		{
			overflowMethod = Overflow.ShrinkContent;
			mMaxLineCount = 0;
		}
		if (mMaxLineWidth != 0)
		{
			base.width = mMaxLineWidth;
			overflowMethod = ((mMaxLineCount > 0) ? Overflow.ResizeHeight : Overflow.ShrinkContent);
		}
		else
		{
			overflowMethod = Overflow.ResizeFreely;
		}
		if (mMaxLineHeight != 0)
		{
			base.height = mMaxLineHeight;
		}
		if (mFont != null)
		{
			int num = Mathf.RoundToInt((float)mFont.defaultSize * mFont.pixelSize);
			if (base.height < num)
			{
				base.height = num;
			}
		}
		mMaxLineWidth = 0;
		mMaxLineHeight = 0;
		mShrinkToFit = false;
		if (GetComponent<BoxCollider>() != null)
		{
			NGUITools.AddWidgetCollider(base.gameObject, true);
		}
	}

	protected override void OnStart()
	{
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}
		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}
		mPremultiply = material != null && material.shader != null && material.shader.name.Contains("Premultiplied");
		if (trueTypeFont != null)
		{
			trueTypeFont.RequestCharactersInTexture(mText, fontSize, fontStyle);
		}
	}

	public override void MarkAsChanged()
	{
		hasChanged = true;
		base.MarkAsChanged();
	}

	private void ProcessText()
	{
		ProcessText(false);
	}

	private void ProcessText(bool legacyMode)
	{
		if (!isValid)
		{
			return;
		}
		mChanged = true;
		hasChanged = false;
		int num = fontSize;
		float num2 = pixelSize;
		float num3 = 1f / num2;
		mPrintedSize = Mathf.Abs((!legacyMode) ? num : Mathf.RoundToInt(base.cachedTransform.localScale.x));
		float num4 = ((!legacyMode) ? ((float)base.width * num3) : ((mMaxLineWidth == 0) ? 1000000f : ((float)mMaxLineWidth * num3)));
		float num5 = ((!legacyMode) ? ((float)base.height * num3) : ((mMaxLineHeight == 0) ? 1000000f : ((float)mMaxLineHeight * num3)));
		if (mPrintedSize > 0)
		{
			bool flag;
			do
			{
				mScale = (float)mPrintedSize / (float)num;
				flag = true;
				int num6 = ((mOverflow != Overflow.ResizeFreely) ? Mathf.RoundToInt(num4 / mScale) : 100000);
				int num7 = ((mOverflow != Overflow.ResizeFreely && mOverflow != Overflow.ResizeHeight) ? Mathf.RoundToInt(num5 / mScale) : 100000);
				if (num4 > 0f || num5 > 0f)
				{
					flag = ((!(mFont != null)) ? NGUIText.WrapText(mText, mTrueTypeFont, num, mFontStyle, num6, num7, mMaxLineCount, mEncoding, out mProcessedText) : mFont.WrapText(mText, num, out mProcessedText, num6, num7, mMaxLineCount, mEncoding, mSymbols));
				}
				else
				{
					mProcessedText = mText;
				}
				if (!string.IsNullOrEmpty(mProcessedText))
				{
					if (mFont != null)
					{
						mCalculatedSize = mFont.CalculatePrintedSize(mProcessedText, num, mEncoding, mSymbols);
					}
					else
					{
						mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText, mTrueTypeFont, num, mFontStyle, mEncoding);
					}
				}
				else
				{
					mCalculatedSize = Vector2.zero;
				}
				if (mOverflow == Overflow.ResizeFreely)
				{
					mWidth = Mathf.RoundToInt(mCalculatedSize.x * num2);
					mHeight = Mathf.RoundToInt(mCalculatedSize.y * num2);
					break;
				}
				if (mOverflow == Overflow.ResizeHeight)
				{
					mHeight = Mathf.RoundToInt(mCalculatedSize.y * num2);
					break;
				}
			}
			while (mOverflow == Overflow.ShrinkContent && !flag && --mPrintedSize > 1);
			if (legacyMode)
			{
				base.width = Mathf.RoundToInt(mCalculatedSize.x * num2);
				base.height = Mathf.RoundToInt(mCalculatedSize.y * num2);
				base.cachedTransform.localScale = Vector3.one;
			}
		}
		else
		{
			base.cachedTransform.localScale = Vector3.one;
			mProcessedText = string.Empty;
			mScale = 1f;
		}
	}

	public override void MakePixelPerfect()
	{
		if (ambigiousFont != null)
		{
			float num = ((!(bitmapFont != null)) ? 1f : bitmapFont.pixelSize);
			Vector3 localPosition = base.cachedTransform.localPosition;
			localPosition.x = Mathf.RoundToInt(localPosition.x);
			localPosition.y = Mathf.RoundToInt(localPosition.y);
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			base.cachedTransform.localPosition = localPosition;
			base.cachedTransform.localScale = Vector3.one;
			if (mOverflow == Overflow.ResizeFreely)
			{
				AssumeNaturalSize();
				return;
			}
			Overflow overflow = mOverflow;
			mOverflow = Overflow.ShrinkContent;
			ProcessText(false);
			mOverflow = overflow;
			int num2 = Mathf.RoundToInt(mCalculatedSize.x * num);
			int num3 = Mathf.RoundToInt(mCalculatedSize.y * num);
			if (base.width < num2)
			{
				base.width = num2;
			}
			if (base.height < num3)
			{
				base.height = num3;
			}
		}
		else
		{
			base.MakePixelPerfect();
		}
	}

	public void AssumeNaturalSize()
	{
		if (ambigiousFont != null)
		{
			ProcessText(false);
			float num = ((!(bitmapFont != null)) ? 1f : bitmapFont.pixelSize);
			base.width = Mathf.RoundToInt(mCalculatedSize.x * num);
			base.height = Mathf.RoundToInt(mCalculatedSize.y * num);
		}
	}

	private void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end, float x, float y)
	{
		Color color = mEffectColor;
		color.a *= base.alpha * mPanel.alpha;
		Color32 color2 = ((!(bitmapFont != null) || !bitmapFont.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (int i = start; i < end; i++)
		{
			verts.Add(verts.buffer[i]);
			uvs.Add(uvs.buffer[i]);
			cols.Add(cols.buffer[i]);
			Vector3 vector = verts.buffer[i];
			vector.x += x;
			vector.y += y;
			verts.buffer[i] = vector;
			cols.buffer[i] = color2;
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (!isValid)
		{
			return;
		}
		Pivot pivot = base.pivot;
		int size = verts.size;
		Color color = base.color;
		color.a *= mPanel.alpha;
		if (mFont != null && mFont.premultipliedAlpha)
		{
			color = NGUITools.ApplyPMA(color);
		}
		string text = processedText;
		float num = ((!(mFont != null)) ? 1f : mFont.pixelSize);
		float num2 = mScale * num;
		bool flag = usePrintedSize;
		int size2 = ((!flag) ? fontSize : mPrintedSize);
		int num3 = ((!flag) ? Mathf.RoundToInt((float)base.width / num2) : base.width);
		int size3 = verts.size;
		TextAlignment alignment = TextAlignment.Center;
		switch (pivot)
		{
		case Pivot.TopLeft:
		case Pivot.Left:
		case Pivot.BottomLeft:
			alignment = TextAlignment.Left;
			break;
		case Pivot.TopRight:
		case Pivot.Right:
		case Pivot.BottomRight:
			alignment = TextAlignment.Right;
			break;
		}
		if (mFont != null)
		{
			mFont.Print(text, size2, color, verts, uvs, cols, mEncoding, mSymbols, alignment, num3, mPremultiply);
		}
		else
		{
			NGUIText.Print(text, mTrueTypeFont, size2, fontStyle, color, mEncoding, alignment, num3, mPremultiply, verts, uvs, cols);
		}
		Vector2 vector = base.pivotOffset;
		float num4 = Mathf.Lerp(0f, -mWidth, vector.x);
		float num5 = Mathf.Lerp(mHeight, 0f, vector.y);
		num5 = Mathf.RoundToInt(num5 + Mathf.Lerp(mCalculatedSize.y * num2 - (float)mHeight, 0f, vector.y));
		if (flag || num2 == 1f)
		{
			for (int i = size3; i < verts.size; i++)
			{
				verts.buffer[i].x += num4;
				verts.buffer[i].y += num5;
			}
		}
		else
		{
			for (int j = size3; j < verts.size; j++)
			{
				verts.buffer[j].x = num4 + verts.buffer[j].x * num2;
				verts.buffer[j].y = num5 + verts.buffer[j].y * num2;
			}
		}
		if (effectStyle != Effect.None)
		{
			int size4 = verts.size;
			float num6 = num;
			num4 = num6 * mEffectDistance.x;
			num5 = num6 * mEffectDistance.y;
			ApplyShadow(verts, uvs, cols, size, size4, num4, 0f - num5);
			if (effectStyle == Effect.Outline)
			{
				size = size4;
				size4 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size4, 0f - num4, num5);
				size = size4;
				size4 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size4, num4, num5);
				size = size4;
				size4 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size4, 0f - num4, 0f - num5);
			}
		}
	}

	public int CalculateOffsetToFit(string text)
	{
		if (bitmapFont != null)
		{
			return bitmapFont.CalculateOffsetToFit(text, fontSize, base.width, false, UIFont.SymbolStyle.None);
		}
		return NGUIText.CalculateOffsetToFit(text, trueTypeFont, fontSize, fontStyle, base.width);
	}
}

using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite")]
[ExecuteInEditMode]
public class UISprite : UIWidget
{
	public enum Type
	{
		Simple = 0,
		Sliced = 1,
		Tiled = 2,
		Filled = 3
	}

	public enum FillDirection
	{
		Horizontal = 0,
		Vertical = 1,
		Radial90 = 2,
		Radial180 = 3,
		Radial360 = 4
	}
    
	[SerializeField]
	private UIAtlas mAtlas;

	[SerializeField]
	
	private string mSpriteName;
    
	[SerializeField]
	private bool mFillCenter = true;
    
	[SerializeField]
	private Type mType;
    
	[SerializeField]
	private FillDirection mFillDirection = FillDirection.Radial360;
    
	[Range(0f, 1f)]
	[SerializeField]
	private float mFillAmount = 1f;
    
	[SerializeField]
	private bool mInvert;

	protected UISpriteData mSprite;

	protected Rect mInnerUV = default(Rect);

	protected Rect mOuterUV = default(Rect);

	private bool mSpriteSet;

	public virtual Type type
	{
		get
		{
			return mType;
		}
		set
		{
			if (mType != value)
			{
				mType = value;
				MarkAsChanged();
			}
		}
	}

	public override Material material
	{
		get
		{
			return (!(mAtlas != null)) ? null : mAtlas.spriteMaterial;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (mAtlas != value)
			{
				RemoveFromPanel();
				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;
				if (string.IsNullOrEmpty(mSpriteName) && mAtlas != null && mAtlas.spriteList.Count > 0)
				{
					SetAtlasSprite(mAtlas.spriteList[0]);
					mSpriteName = mSprite.name;
				}
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					string text = mSpriteName;
					mSpriteName = string.Empty;
					spriteName = text;
					MarkAsChanged();
				}
			}
		}
	}

	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					mSpriteName = string.Empty;
					mSprite = null;
					mChanged = true;
					mSpriteSet = false;
				}
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
			}
		}
	}

	public bool isValid
	{
		get
		{
			return GetAtlasSprite() != null;
		}
	}

	public bool fillCenter
	{
		get
		{
			return mFillCenter;
		}
		set
		{
			if (mFillCenter != value)
			{
				mFillCenter = value;
				MarkAsChanged();
			}
		}
	}

	public FillDirection fillDirection
	{
		get
		{
			return mFillDirection;
		}
		set
		{
			if (mFillDirection != value)
			{
				mFillDirection = value;
				mChanged = true;
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return mFillAmount;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mFillAmount != num)
			{
				mFillAmount = num;
				mChanged = true;
			}
		}
	}

	public bool invert
	{
		get
		{
			return mInvert;
		}
		set
		{
			if (mInvert != value)
			{
				mInvert = value;
				mChanged = true;
			}
		}
	}

	public override Vector4 border
	{
		get
		{
			if (type == Type.Sliced)
			{
				UISpriteData atlasSprite = GetAtlasSprite();
				if (atlasSprite == null)
				{
					return Vector2.zero;
				}
				return new Vector4(atlasSprite.borderLeft, atlasSprite.borderBottom, atlasSprite.borderRight, atlasSprite.borderTop);
			}
			return base.border;
		}
	}

	public override int minWidth
	{
		get
		{
			if (type == Type.Sliced)
			{
				Vector4 vector = border;
				if (atlas != null)
				{
					vector *= atlas.pixelSize;
				}
				int num = Mathf.RoundToInt(vector.x + vector.z);
				return Mathf.Max(base.minWidth, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minWidth;
		}
	}

	public override int minHeight
	{
		get
		{
			if (type == Type.Sliced)
			{
				Vector4 vector = border;
				if (atlas != null)
				{
					vector *= atlas.pixelSize;
				}
				int num = Mathf.RoundToInt(vector.y + vector.w);
				return Mathf.Max(base.minHeight, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minHeight;
		}
	}

	private Vector4 drawingDimensions
	{
		get
		{
			if (mSprite == null)
			{
				return new Vector4(0f, 0f, mWidth, mHeight);
			}
			int paddingLeft = mSprite.paddingLeft;
			int paddingBottom = mSprite.paddingBottom;
			int num = mSprite.paddingRight;
			int num2 = mSprite.paddingTop;
			Vector2 vector = base.pivotOffset;
			int num3 = mSprite.width + mSprite.paddingLeft + mSprite.paddingRight;
			int num4 = mSprite.height + mSprite.paddingBottom + mSprite.paddingTop;
			if ((num3 & 1) == 1)
			{
				num++;
			}
			if ((num4 & 1) == 1)
			{
				num2++;
			}
			float num5 = 1f / (float)num3;
			float num6 = 1f / (float)num4;
			Vector4 result = new Vector4((float)paddingLeft * num5, (float)paddingBottom * num6, (float)(num3 - num) * num5, (float)(num4 - num2) * num6);
			result.x -= vector.x;
			result.y -= vector.y;
			result.z -= vector.x;
			result.w -= vector.y;
			result.x *= mWidth;
			result.y *= mHeight;
			result.z *= mWidth;
			result.w *= mHeight;
			return result;
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		mFillAmount = Mathf.Clamp01(mFillAmount);
	}

	public UISpriteData GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UISpriteData sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UISpriteData uISpriteData = mAtlas.spriteList[0];
				if (uISpriteData == null)
				{
					return null;
				}
				SetAtlasSprite(uISpriteData);
				if (mSprite == null)
				{
					Debug.LogError(mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				mSpriteName = mSprite.name;
			}
		}
		return mSprite;
	}

	protected void SetAtlasSprite(UISpriteData sp)
	{
		mChanged = true;
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? string.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public override void MakePixelPerfect()
	{
		if (!isValid)
		{
			return;
		}
		base.MakePixelPerfect();
		Type type = this.type;
		if (type != Type.Simple && type != Type.Filled)
		{
			return;
		}
		Texture texture = mainTexture;
		UISpriteData atlasSprite = GetAtlasSprite();
		if (texture != null && atlasSprite != null)
		{
			int num = Mathf.RoundToInt(atlas.pixelSize * (float)(atlasSprite.width + atlasSprite.paddingLeft + atlasSprite.paddingRight));
			int num2 = Mathf.RoundToInt(atlas.pixelSize * (float)(atlasSprite.height + atlasSprite.paddingTop + atlasSprite.paddingBottom));
			if ((num & 1) == 1)
			{
				num++;
			}
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.width = num;
			base.height = num2;
		}
	}

	public override void Update()
	{
		base.Update();
		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mSprite = null;
			mChanged = true;
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = mainTexture;
		if (texture != null)
		{
			if (mSprite == null)
			{
				mSprite = atlas.GetSprite(spriteName);
			}
			if (mSprite == null)
			{
				return;
			}
			mOuterUV.Set(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
			mInnerUV.Set(mSprite.x + mSprite.borderLeft, mSprite.y + mSprite.borderTop, mSprite.width - mSprite.borderLeft - mSprite.borderRight, mSprite.height - mSprite.borderBottom - mSprite.borderTop);
			mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, texture.width, texture.height);
			mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, texture.width, texture.height);
		}
		switch (type)
		{
		case Type.Simple:
			SimpleFill(verts, uvs, cols);
			break;
		case Type.Sliced:
			SlicedFill(verts, uvs, cols);
			break;
		case Type.Filled:
			FilledFill(verts, uvs, cols);
			break;
		case Type.Tiled:
			TiledFill(verts, uvs, cols);
			break;
		}
	}

	protected void SimpleFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector2 item = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		Vector2 item2 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		Vector4 vector = drawingDimensions;
		verts.Add(new Vector3(vector.x, vector.y));
		verts.Add(new Vector3(vector.x, vector.w));
		verts.Add(new Vector3(vector.z, vector.w));
		verts.Add(new Vector3(vector.z, vector.y));
		uvs.Add(item);
		uvs.Add(new Vector2(item.x, item2.y));
		uvs.Add(item2);
		uvs.Add(new Vector2(item2.x, item.y));
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item3 = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
	}

	protected void SlicedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mSprite == null)
		{
			return;
		}
		if (!mSprite.hasBorder)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}
		Vector4 vector = border * atlas.pixelSize;
		Vector2 vector2 = base.pivotOffset;
		float num = 1f / (float)mWidth;
		float num2 = 1f / (float)mHeight;
		Vector2[] array = new Vector2[4]
		{
			new Vector2((float)mSprite.paddingLeft * num, (float)mSprite.paddingBottom * num2),
			default(Vector2),
			default(Vector2),
			new Vector2(1f - (float)mSprite.paddingRight * num, 1f - (float)mSprite.paddingTop * num2)
		};
		array[1].x = array[0].x + num * vector.x;
		array[1].y = array[0].y + num2 * vector.y;
		array[2].x = array[3].x - num * vector.z;
		array[2].y = array[3].y - num2 * vector.w;
		for (int i = 0; i < 4; i++)
		{
			array[i].x -= vector2.x;
			array[i].y -= vector2.y;
			array[i].x *= mWidth;
			array[i].y *= mHeight;
		}
		Vector2[] array2 = new Vector2[4]
		{
			new Vector2(mOuterUV.xMin, mOuterUV.yMin),
			new Vector2(mInnerUV.xMin, mInnerUV.yMin),
			new Vector2(mInnerUV.xMax, mInnerUV.yMax),
			new Vector2(mOuterUV.xMax, mOuterUV.yMax)
		};
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (int j = 0; j < 3; j++)
		{
			int num3 = j + 1;
			for (int k = 0; k < 3; k++)
			{
				if (mFillCenter || j != 1 || k != 1)
				{
					int num4 = k + 1;
					verts.Add(new Vector3(array[j].x, array[k].y));
					verts.Add(new Vector3(array[j].x, array[num4].y));
					verts.Add(new Vector3(array[num3].x, array[num4].y));
					verts.Add(new Vector3(array[num3].x, array[k].y));
					uvs.Add(new Vector2(array2[j].x, array2[k].y));
					uvs.Add(new Vector2(array2[j].x, array2[num4].y));
					uvs.Add(new Vector2(array2[num3].x, array2[num4].y));
					uvs.Add(new Vector2(array2[num3].x, array2[k].y));
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
				}
			}
		}
	}

	protected void TiledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = material.mainTexture;
		if (texture == null)
		{
			return;
		}
		Vector2 vector = new Vector2(mInnerUV.width * (float)texture.width, mInnerUV.height * (float)texture.height);
		vector *= atlas.pixelSize;
		float num = Mathf.Abs(vector.x / (float)mWidth);
		float num2 = Mathf.Abs(vector.y / (float)mHeight);
		if (num * num2 < 0.0001f)
		{
			num = 0.01f;
			num2 = 0.01f;
		}
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		Vector2 vector2 = base.pivotOffset;
		Vector2 vector3 = new Vector2(mInnerUV.xMin, mInnerUV.yMin);
		Vector2 vector4 = new Vector2(mInnerUV.xMax, mInnerUV.yMax);
		Vector2 vector5 = vector4;
		for (float num3 = 0f; num3 < 1f; num3 += num2)
		{
			float num4 = 0f;
			vector5.x = vector4.x;
			float num5 = num3 + num2;
			if (num5 > 1f)
			{
				vector5.y = vector3.y + (vector4.y - vector3.y) * (1f - num3) / (num5 - num3);
				num5 = 1f;
			}
			for (; num4 < 1f; num4 += num)
			{
				float num6 = num4 + num;
				if (num6 > 1f)
				{
					vector5.x = vector3.x + (vector4.x - vector3.x) * (1f - num4) / (num6 - num4);
					num6 = 1f;
				}
				float x = (num4 - vector2.x) * (float)mWidth;
				float x2 = (num6 - vector2.x) * (float)mWidth;
				float y = (num3 - vector2.y) * (float)mHeight;
				float y2 = (num5 - vector2.y) * (float)mHeight;
				verts.Add(new Vector3(x, y));
				verts.Add(new Vector3(x, y2));
				verts.Add(new Vector3(x2, y2));
				verts.Add(new Vector3(x2, y));
				uvs.Add(new Vector2(vector3.x, vector3.y));
				uvs.Add(new Vector2(vector3.x, vector5.y));
				uvs.Add(new Vector2(vector5.x, vector5.y));
				uvs.Add(new Vector2(vector5.x, vector3.y));
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
			}
		}
	}

	protected void FilledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mFillAmount < 0.001f)
		{
			return;
		}
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		Vector2[] array = new Vector2[4];
		Vector2[] array2 = new Vector2[4];
		Vector4 vector = drawingDimensions;
		float num = mOuterUV.xMin;
		float num2 = mOuterUV.yMin;
		float num3 = mOuterUV.xMax;
		float num4 = mOuterUV.yMax;
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical)
		{
			if (mFillDirection == FillDirection.Horizontal)
			{
				float num5 = (num3 - num) * mFillAmount;
				if (mInvert)
				{
					vector.x = vector.z - (vector.z - vector.x) * mFillAmount;
					num = num3 - num5;
				}
				else
				{
					vector.z = vector.x + (vector.z - vector.x) * mFillAmount;
					num3 = num + num5;
				}
			}
			else if (mFillDirection == FillDirection.Vertical)
			{
				float num6 = (num4 - num2) * mFillAmount;
				if (mInvert)
				{
					vector.y = vector.w - (vector.w - vector.y) * mFillAmount;
					num2 = num4 - num6;
				}
				else
				{
					vector.w = vector.y + (vector.w - vector.y) * mFillAmount;
					num4 = num2 + num6;
				}
			}
		}
		array[0] = new Vector2(vector.x, vector.y);
		array[1] = new Vector2(vector.x, vector.w);
		array[2] = new Vector2(vector.z, vector.w);
		array[3] = new Vector2(vector.z, vector.y);
		array2[0] = new Vector2(num, num2);
		array2[1] = new Vector2(num, num4);
		array2[2] = new Vector2(num3, num4);
		array2[3] = new Vector2(num3, num2);
		if (mFillAmount < 1f)
		{
			if (mFillDirection == FillDirection.Radial90)
			{
				if (RadialCut(array, array2, mFillAmount, mInvert, 0))
				{
					for (int i = 0; i < 4; i++)
					{
						verts.Add(array[i]);
						uvs.Add(array2[i]);
						cols.Add(item);
					}
				}
				return;
			}
			if (mFillDirection == FillDirection.Radial180)
			{
				for (int j = 0; j < 2; j++)
				{
					float t = 0f;
					float t2 = 1f;
					float t3;
					float t4;
					if (j == 0)
					{
						t3 = 0f;
						t4 = 0.5f;
					}
					else
					{
						t3 = 0.5f;
						t4 = 1f;
					}
					array[0].x = Mathf.Lerp(vector.x, vector.z, t3);
					array[1].x = array[0].x;
					array[2].x = Mathf.Lerp(vector.x, vector.z, t4);
					array[3].x = array[2].x;
					array[0].y = Mathf.Lerp(vector.y, vector.w, t);
					array[1].y = Mathf.Lerp(vector.y, vector.w, t2);
					array[2].y = array[1].y;
					array[3].y = array[0].y;
					array2[0].x = Mathf.Lerp(num, num3, t3);
					array2[1].x = array2[0].x;
					array2[2].x = Mathf.Lerp(num, num3, t4);
					array2[3].x = array2[2].x;
					array2[0].y = Mathf.Lerp(num2, num4, t);
					array2[1].y = Mathf.Lerp(num2, num4, t2);
					array2[2].y = array2[1].y;
					array2[3].y = array2[0].y;
					float value = (mInvert ? (mFillAmount * 2f - (float)(1 - j)) : (fillAmount * 2f - (float)j));
					if (RadialCut(array, array2, Mathf.Clamp01(value), !mInvert, NGUIMath.RepeatIndex(j + 3, 4)))
					{
						for (int k = 0; k < 4; k++)
						{
							verts.Add(array[k]);
							uvs.Add(array2[k]);
							cols.Add(item);
						}
					}
				}
				return;
			}
			if (mFillDirection == FillDirection.Radial360)
			{
				for (int l = 0; l < 4; l++)
				{
					float t5;
					float t6;
					if (l < 2)
					{
						t5 = 0f;
						t6 = 0.5f;
					}
					else
					{
						t5 = 0.5f;
						t6 = 1f;
					}
					float t7;
					float t8;
					if (l == 0 || l == 3)
					{
						t7 = 0f;
						t8 = 0.5f;
					}
					else
					{
						t7 = 0.5f;
						t8 = 1f;
					}
					array[0].x = Mathf.Lerp(vector.x, vector.z, t5);
					array[1].x = array[0].x;
					array[2].x = Mathf.Lerp(vector.x, vector.z, t6);
					array[3].x = array[2].x;
					array[0].y = Mathf.Lerp(vector.y, vector.w, t7);
					array[1].y = Mathf.Lerp(vector.y, vector.w, t8);
					array[2].y = array[1].y;
					array[3].y = array[0].y;
					array2[0].x = Mathf.Lerp(num, num3, t5);
					array2[1].x = array2[0].x;
					array2[2].x = Mathf.Lerp(num, num3, t6);
					array2[3].x = array2[2].x;
					array2[0].y = Mathf.Lerp(num2, num4, t7);
					array2[1].y = Mathf.Lerp(num2, num4, t8);
					array2[2].y = array2[1].y;
					array2[3].y = array2[0].y;
					float value2 = ((!mInvert) ? (mFillAmount * 4f - (float)(3 - NGUIMath.RepeatIndex(l + 2, 4))) : (mFillAmount * 4f - (float)NGUIMath.RepeatIndex(l + 2, 4)));
					if (RadialCut(array, array2, Mathf.Clamp01(value2), mInvert, NGUIMath.RepeatIndex(l + 2, 4)))
					{
						for (int m = 0; m < 4; m++)
						{
							verts.Add(array[m]);
							uvs.Add(array2[m]);
							cols.Add(item);
						}
					}
				}
				return;
			}
		}
		for (int n = 0; n < 4; n++)
		{
			verts.Add(array[n]);
			uvs.Add(array2[n]);
			cols.Add(item);
		}
	}

	private static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if ((corner & 1) == 1)
		{
			invert = !invert;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (invert)
		{
			num = 1f - num;
		}
		num *= (float)Math.PI / 2f;
		float cos = Mathf.Cos(num);
		float sin = Mathf.Sin(num);
		RadialCut(xy, cos, sin, invert, corner);
		RadialCut(uv, cos, sin, invert, corner);
		return true;
	}

	private static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
	{
		int num = NGUIMath.RepeatIndex(corner + 1, 4);
		int num2 = NGUIMath.RepeatIndex(corner + 2, 4);
		int num3 = NGUIMath.RepeatIndex(corner + 3, 4);
		if ((corner & 1) == 1)
		{
			if (sin > cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num2].x = xy[num].x;
				}
			}
			else if (cos > sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num3].y = xy[num2].y;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (!invert)
			{
				xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
			else
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
			return;
		}
		if (cos > sin)
		{
			sin /= cos;
			cos = 1f;
			if (!invert)
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
				xy[num2].y = xy[num].y;
			}
		}
		else if (sin > cos)
		{
			cos /= sin;
			sin = 1f;
			if (invert)
			{
				xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
				xy[num3].x = xy[num2].x;
			}
		}
		else
		{
			cos = 1f;
			sin = 1f;
		}
		if (invert)
		{
			xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
		}
		else
		{
			xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
		}
	}
}

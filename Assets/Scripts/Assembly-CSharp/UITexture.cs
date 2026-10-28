using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Texture")]
public class UITexture : UIWidget
{
	[SerializeField]
	
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);
    
	[SerializeField]
	private Shader mShader;

	[SerializeField]
	
	private Texture mTexture;
    
	[SerializeField]
	private Material mMat;

	private bool mCreatingMat;

	private Material mDynamicMat;

	private int mPMA = -1;

	public Rect uvRect
	{
		get
		{
			return mRect;
		}
		set
		{
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
		}
	}

	public Shader shader
	{
		get
		{
			if (mShader == null)
			{
				Material material = this.material;
				if (material != null)
				{
					mShader = material.shader;
				}
				if (mShader == null)
				{
					mShader = Shader.Find("Unlit/Transparent Colored");
				}
			}
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				mShader = value;
				Material material = this.material;
				if (material != null)
				{
					material.shader = value;
				}
				mPMA = -1;
			}
		}
	}

	public bool hasDynamicMaterial
	{
		get
		{
			return mDynamicMat != null;
		}
	}

	public override Material material
	{
		get
		{
			if (mMat != null)
			{
				return mMat;
			}
			if (mDynamicMat != null)
			{
				return mDynamicMat;
			}
			if (!mCreatingMat && mDynamicMat == null)
			{
				mCreatingMat = true;
				if (mShader == null)
				{
					mShader = Shader.Find("Unlit/Transparent Colored");
				}
				Cleanup();
				mDynamicMat = new Material(mShader);
				mDynamicMat.hideFlags = HideFlags.DontSave;
				mDynamicMat.mainTexture = mTexture;
				mPMA = 0;
				mCreatingMat = false;
			}
			return mDynamicMat;
		}
		set
		{
			if (mMat != value)
			{
				Cleanup();
				mMat = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (mMat != null)
			{
				return mMat.mainTexture;
			}
			if (mTexture != null)
			{
				return mTexture;
			}
			return null;
		}
		set
		{
			RemoveFromPanel();
			Material material = this.material;
			if (material != null)
			{
				mPanel = null;
				mTexture = value;
				material.mainTexture = value;
				MarkAsChangedLite();
				if (base.enabled)
				{
					CreatePanel();
				}
				if (mPanel != null)
				{
					mPanel.Refresh();
				}
			}
		}
	}

	private Vector4 drawingDimensions
	{
		get
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			Texture texture = mainTexture;
			Rect rect = ((!(texture != null)) ? new Rect(0f, 0f, mWidth, mHeight) : new Rect(0f, 0f, texture.width, texture.height));
			Vector2 vector = base.pivotOffset;
			int num5 = Mathf.RoundToInt(rect.width);
			int num6 = Mathf.RoundToInt(rect.height);
			float num7 = (((num5 & 1) != 0) ? (num5 + 1) : num5);
			float num8 = (((num6 & 1) != 0) ? (num6 + 1) : num6);
			Vector4 result = new Vector4(num / num7, num2 / num8, ((float)num5 - num3) / num7, ((float)num6 - num4) / num8);
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

	private void OnDestroy()
	{
		Cleanup();
	}

	private void Cleanup()
	{
		if (mDynamicMat != null)
		{
			NGUITools.Destroy(mDynamicMat);
			mDynamicMat = null;
		}
	}

	public override void MakePixelPerfect()
	{
		Texture texture = mainTexture;
		if (texture != null)
		{
			int num = texture.width;
			if ((num & 1) == 1)
			{
				num++;
			}
			int num2 = texture.height;
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.width = num;
			base.height = num2;
		}
		base.MakePixelPerfect();
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		Vector4 vector = drawingDimensions;
		verts.Add(new Vector3(vector.x, vector.y));
		verts.Add(new Vector3(vector.x, vector.w));
		verts.Add(new Vector3(vector.z, vector.w));
		verts.Add(new Vector3(vector.z, vector.y));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
	}
}

using UnityEngine;

[AddComponentMenu("NGUI/Internal/Draw Call")]
[ExecuteInEditMode]
public class UIDrawCall : MonoBehaviour
{
	public enum Clipping
	{
		None = 0,
		AlphaClip = 2,
		SoftClip = 3
	}

	public static BetterList<UIDrawCall> list = new BetterList<UIDrawCall>();

	private Transform mTrans;

	private Material mSharedMat;

	private Mesh mMesh0;

	private Mesh mMesh1;

	private MeshFilter mFilter;

	private MeshRenderer mRen;

	private Clipping mClipping;

	private Vector4 mClipRange;

	private Vector2 mClipSoft;

	private Material mMat;

	private int[] mIndices;

	private bool mDirty;

	private bool mReset = true;

	private bool mEven = true;

	private int mRenderQueue;

	public UIPanel panel { get; set; }

	public bool isDirty
	{
		get
		{
			return mDirty;
		}
		set
		{
			mDirty = value;
		}
	}

	public int renderQueue
	{
		get
		{
			return mRenderQueue;
		}
		set
		{
			if (mRenderQueue != value)
			{
				mRenderQueue = value;
				if (mMat != null && mSharedMat != null)
				{
					mMat.renderQueue = mSharedMat.renderQueue + value;
				}
			}
		}
	}

	public int finalRenderQueue
	{
		get
		{
			if (mMat != null)
			{
				return mMat.renderQueue;
			}
			return ((!(mSharedMat != null)) ? 3000 : mSharedMat.renderQueue) + mRenderQueue;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	public Material baseMaterial
	{
		get
		{
			return mSharedMat;
		}
		set
		{
			mSharedMat = value;
		}
	}

	public Material dynamicMaterial
	{
		get
		{
			return mMat;
		}
	}

	public Texture mainTexture
	{
		get
		{
			return (!(mMat != null)) ? null : mMat.mainTexture;
		}
		set
		{
			if (mMat != null)
			{
				mMat.mainTexture = value;
			}
		}
	}

	public int triangles
	{
		get
		{
			Mesh mesh = ((!mEven) ? mMesh1 : mMesh0);
			return (mesh != null) ? (mesh.vertexCount >> 1) : 0;
		}
	}

	public bool isClipped
	{
		get
		{
			return mClipping != Clipping.None;
		}
	}

	public Clipping clipping
	{
		get
		{
			return mClipping;
		}
		set
		{
			if (mClipping != value)
			{
				mClipping = value;
				mReset = true;
			}
		}
	}

	public Vector4 clipRange
	{
		get
		{
			return mClipRange;
		}
		set
		{
			mClipRange = value;
		}
	}

	public Vector2 clipSoftness
	{
		get
		{
			return mClipSoft;
		}
		set
		{
			mClipSoft = value;
		}
	}

	private Mesh GetMesh(ref bool rebuildIndices, int vertexCount)
	{
		mEven = !mEven;
		if (mEven)
		{
			if (mMesh0 == null)
			{
				mMesh0 = new Mesh();
				mMesh0.hideFlags = HideFlags.DontSave;
				mMesh0.name = "Mesh0 for " + mSharedMat.name;
				mMesh0.MarkDynamic();
				rebuildIndices = true;
			}
			else if (rebuildIndices || mMesh0.vertexCount != vertexCount)
			{
				rebuildIndices = true;
				mMesh0.Clear();
			}
			return mMesh0;
		}
		if (mMesh1 == null)
		{
			mMesh1 = new Mesh();
			mMesh1.hideFlags = HideFlags.DontSave;
			mMesh1.name = "Mesh1 for " + mSharedMat.name;
			mMesh1.MarkDynamic();
			rebuildIndices = true;
		}
		else if (rebuildIndices || mMesh1.vertexCount != vertexCount)
		{
			rebuildIndices = true;
			mMesh1.Clear();
		}
		return mMesh1;
	}

	public void RebuildMaterial()
	{
		NGUITools.DestroyImmediate(mMat);
		mMat = new Material(mSharedMat);
		mMat.hideFlags = HideFlags.DontSave;
		mMat.CopyPropertiesFromMaterial(mSharedMat);
		if (mMat.shader != null && mMat.shader.name == "GUI/Text Shader")
		{
			Shader shader = Shader.Find("Unlit/Text");
			if (shader != null)
			{
				mMat.shader = shader;
			}
		}
		mMat.renderQueue = mSharedMat.renderQueue + mRenderQueue;
	}

	private void UpdateMaterials()
	{
		bool flag = mClipping != Clipping.None;
		if (mMat == null)
		{
			RebuildMaterial();
		}
		if (flag && mClipping != Clipping.None)
		{
			Shader shader = null;
			string text = mSharedMat.shader.name;
			text = text.Replace("GUI/Text Shader", "Unlit/Text");
			text = text.Replace(" (AlphaClip)", string.Empty);
			text = text.Replace(" (SoftClip)", string.Empty);
			shader = ((mClipping != Clipping.SoftClip) ? Shader.Find(text + " (AlphaClip)") : Shader.Find(text + " (SoftClip)"));
			if (shader != null)
			{
				mMat.shader = shader;
			}
			else
			{
				mClipping = Clipping.None;
				Debug.LogError(text + " doesn't have a clipped shader version for " + mClipping);
			}
		}
		if (mRen.sharedMaterial != mMat)
		{
			mRen.sharedMaterials = new Material[1] { mMat };
		}
	}

	public void Set(BetterList<Vector3> verts, BetterList<Vector3> norms, BetterList<Vector4> tans, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		int size = verts.size;
		if (size > 0 && size == uvs.size && size == cols.size && size % 4 == 0)
		{
			if (mFilter == null)
			{
				mFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			if (mFilter == null)
			{
				mFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (mRen == null)
			{
				mRen = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (mRen == null)
			{
				mRen = base.gameObject.AddComponent<MeshRenderer>();
				UpdateMaterials();
			}
			else if (mMat != null && mMat.mainTexture != mSharedMat.mainTexture)
			{
				UpdateMaterials();
			}
			if (verts.size < 65000)
			{
				int num = (size >> 1) * 3;
				bool rebuildIndices = mIndices == null || mIndices.Length != num;
				if (rebuildIndices)
				{
					mIndices = new int[num];
					int num2 = 0;
					for (int i = 0; i < size; i += 4)
					{
						mIndices[num2++] = i;
						mIndices[num2++] = i + 1;
						mIndices[num2++] = i + 2;
						mIndices[num2++] = i + 2;
						mIndices[num2++] = i + 3;
						mIndices[num2++] = i;
					}
				}
				Mesh mesh = GetMesh(ref rebuildIndices, verts.size);
				mesh.vertices = verts.ToArray();
				if (norms != null)
				{
					mesh.normals = norms.ToArray();
				}
				if (tans != null)
				{
					mesh.tangents = tans.ToArray();
				}
				mesh.uv = uvs.ToArray();
				mesh.colors32 = cols.ToArray();
				if (rebuildIndices)
				{
					mesh.triangles = mIndices;
				}
				mesh.RecalculateBounds();
				mFilter.mesh = mesh;
			}
			else
			{
				if (mFilter.mesh != null)
				{
					mFilter.mesh.Clear();
				}
				Debug.LogError("Too many vertices on one panel: " + verts.size);
			}
		}
		else
		{
			if (mFilter.mesh != null)
			{
				mFilter.mesh.Clear();
			}
			Debug.LogError("UIWidgets must fill the buffer with 4 vertices per quad. Found " + size);
		}
	}

	private void OnWillRenderObject()
	{
		if (mReset)
		{
			mReset = false;
			UpdateMaterials();
		}
		if (mMat != null && isClipped)
		{
			mMat.mainTextureOffset = new Vector2((0f - mClipRange.x) / mClipRange.z, (0f - mClipRange.y) / mClipRange.w);
			mMat.mainTextureScale = new Vector2(1f / mClipRange.z, 1f / mClipRange.w);
			Vector2 vector = new Vector2(1000f, 1000f);
			if (mClipSoft.x > 0f)
			{
				vector.x = mClipRange.z / mClipSoft.x;
			}
			if (mClipSoft.y > 0f)
			{
				vector.y = mClipRange.w / mClipSoft.y;
			}
			mMat.SetVector("_ClipSharpness", vector);
		}
	}

	private void OnDestroy()
	{
		NGUITools.DestroyImmediate(mMesh0);
		NGUITools.DestroyImmediate(mMesh1);
		NGUITools.DestroyImmediate(mMat);
	}
}

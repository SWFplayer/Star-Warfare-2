using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Widget")]
public class UIWidget : MonoBehaviour
{
	public enum Pivot
	{
		TopLeft = 0,
		Top = 1,
		TopRight = 2,
		Left = 3,
		Center = 4,
		Right = 5,
		BottomLeft = 6,
		Bottom = 7,
		BottomRight = 8
	}

	public static BetterList<UIWidget> list = new BetterList<UIWidget>();

	[HideInInspector]
	[SerializeField]
	protected Color mColor = Color.white;

	[SerializeField]
	[HideInInspector]
	protected Pivot mPivot = Pivot.Center;

	[SerializeField]
	[HideInInspector]
	protected int mWidth = 100;

	[SerializeField]
	[HideInInspector]
	protected int mHeight = 100;

	[HideInInspector]
	[SerializeField]
	protected int mDepth;

	public bool autoResizeBoxCollider;

	protected GameObject mGo;

	protected Transform mTrans;

	protected UIPanel mPanel;

	protected bool mChanged = true;

	protected bool mPlayMode = true;

	private bool mStarted;

	private Vector3 mDiffPos;

	private Quaternion mDiffRot;

	private Vector3 mDiffScale;

	private Matrix4x4 mLocalToPanel;

	private bool mVisibleByPanel = true;

	private float mLastAlpha;

	private UIGeometry mGeom = new UIGeometry();

	private Vector3[] mCorners = new Vector3[4];

	private bool mForceVisible;

	private Vector3 mOldV0;

	private Vector3 mOldV1;

	public UIDrawCall drawCall { get; set; }

	public bool isVisible
	{
		get
		{
			return mVisibleByPanel && finalAlpha > 0.001f;
		}
	}

	public int width
	{
		get
		{
			return mWidth;
		}
		set
		{
			int num = minWidth;
			if (value < num)
			{
				value = num;
			}
			if (mWidth != value)
			{
				mWidth = value;
				if (autoResizeBoxCollider)
				{
					ResizeCollider();
				}
				MarkAsChanged();
			}
		}
	}

	public int height
	{
		get
		{
			return mHeight;
		}
		set
		{
			int num = minHeight;
			if (value < num)
			{
				value = num;
			}
			if (mHeight != value)
			{
				mHeight = value;
				if (autoResizeBoxCollider)
				{
					ResizeCollider();
				}
				MarkAsChanged();
			}
		}
	}

	public Color color
	{
		get
		{
			return mColor;
		}
		set
		{
			if (!mColor.Equals(value))
			{
				mColor = value;
				mChanged = true;
			}
		}
	}

	public float alpha
	{
		get
		{
			return mColor.a;
		}
		set
		{
			Color color = mColor;
			color.a = value;
			this.color = color;
		}
	}

	public float finalAlpha
	{
		get
		{
			if (mPanel == null)
			{
				CreatePanel();
			}
			return (!(mPanel != null)) ? mColor.a : (mColor.a * mPanel.alpha);
		}
	}

	public Pivot pivot
	{
		get
		{
			return mPivot;
		}
		set
		{
			if (mPivot != value)
			{
				Vector3 vector = worldCorners[0];
				mPivot = value;
				mChanged = true;
				Vector3 vector2 = worldCorners[0];
				Transform transform = cachedTransform;
				Vector3 position = transform.position;
				float z = transform.localPosition.z;
				position.x += vector.x - vector2.x;
				position.y += vector.y - vector2.y;
				cachedTransform.position = position;
				position = cachedTransform.localPosition;
				position.x = Mathf.Round(position.x);
				position.y = Mathf.Round(position.y);
				position.z = z;
				cachedTransform.localPosition = position;
			}
		}
	}

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				mDepth = value;
				UIPanel.SetDirty();
			}
		}
	}

	public int raycastDepth
	{
		get
		{
			return (!(mPanel != null)) ? mDepth : (mDepth + mPanel.depth * 1000);
		}
	}

	public virtual Vector3[] localCorners
	{
		get
		{
			Vector2 vector = pivotOffset;
			float num = (0f - vector.x) * (float)mWidth;
			float num2 = (0f - vector.y) * (float)mHeight;
			float x = num + (float)mWidth;
			float y = num2 + (float)mHeight;
			mCorners[0] = new Vector3(num, num2, 0f);
			mCorners[1] = new Vector3(num, y, 0f);
			mCorners[2] = new Vector3(x, y, 0f);
			mCorners[3] = new Vector3(x, num2, 0f);
			return mCorners;
		}
	}

	public virtual Vector2 localSize
	{
		get
		{
			Vector3[] array = localCorners;
			return array[2] - array[0];
		}
	}

	public virtual Vector3[] worldCorners
	{
		get
		{
			Vector2 vector = pivotOffset;
			float num = (0f - vector.x) * (float)mWidth;
			float num2 = (0f - vector.y) * (float)mHeight;
			float x = num + (float)mWidth;
			float y = num2 + (float)mHeight;
			Transform transform = cachedTransform;
			mCorners[0] = transform.TransformPoint(num, num2, 0f);
			mCorners[1] = transform.TransformPoint(num, y, 0f);
			mCorners[2] = transform.TransformPoint(x, y, 0f);
			mCorners[3] = transform.TransformPoint(x, num2, 0f);
			return mCorners;
		}
	}

	public Vector3[] innerWorldCorners
	{
		get
		{
			Vector2 vector = pivotOffset;
			float num = (0f - vector.x) * (float)mWidth;
			float num2 = (0f - vector.y) * (float)mHeight;
			float num3 = num + (float)mWidth;
			float num4 = num2 + (float)mHeight;
			Vector4 vector2 = border;
			num += vector2.x;
			num2 += vector2.y;
			num3 -= vector2.z;
			num4 -= vector2.w;
			Transform transform = cachedTransform;
			mCorners[0] = transform.TransformPoint(num, num2, 0f);
			mCorners[1] = transform.TransformPoint(num, num4, 0f);
			mCorners[2] = transform.TransformPoint(num3, num4, 0f);
			mCorners[3] = transform.TransformPoint(num3, num2, 0f);
			return mCorners;
		}
	}

	public bool hasVertices
	{
		get
		{
			return mGeom != null && mGeom.hasVertices;
		}
	}

	public Vector2 pivotOffset
	{
		get
		{
			return NGUIMath.GetPivotOffset(pivot);
		}
	}

	public GameObject cachedGameObject
	{
		get
		{
			if (mGo == null)
			{
				mGo = base.gameObject;
			}
			return mGo;
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

	public virtual Material material
	{
		get
		{
			return null;
		}
		set
		{
			throw new NotImplementedException(string.Concat(GetType(), " has no material setter"));
		}
	}

	public virtual Texture mainTexture
	{
		get
		{
			Material material = this.material;
			return (!(material != null)) ? null : material.mainTexture;
		}
		set
		{
			throw new NotImplementedException(string.Concat(GetType(), " has no mainTexture setter"));
		}
	}

	public UIPanel panel
	{
		get
		{
			if (mPanel == null)
			{
				CreatePanel();
			}
			return mPanel;
		}
		set
		{
			mPanel = value;
		}
	}

	[Obsolete("There is no relative scale anymore. Widgets now have width and height instead")]
	public Vector2 relativeSize
	{
		get
		{
			return Vector2.one;
		}
	}

	public bool hasBoxCollider
	{
		get
		{
			BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
			return boxCollider != null;
		}
	}

	public virtual int minWidth
	{
		get
		{
			return 4;
		}
	}

	public virtual int minHeight
	{
		get
		{
			return 4;
		}
	}

	public virtual Vector4 border
	{
		get
		{
			return Vector4.zero;
		}
	}

	private void ResizeCollider()
	{
		if (NGUITools.IsActive(this))
		{
			BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
			if (boxCollider != null)
			{
				NGUITools.UpdateWidgetCollider(boxCollider, true);
			}
		}
	}

	public static int CompareFunc(UIWidget left, UIWidget right)
	{
		int num = UIPanel.CompareFunc(left.mPanel, right.mPanel);
		if (num == 0)
		{
			if (left.mDepth < right.mDepth)
			{
				return -1;
			}
			if (left.mDepth > right.mDepth)
			{
				return 1;
			}
			Material material = left.material;
			Material material2 = right.material;
			if (material == material2)
			{
				return 0;
			}
			if (material != null)
			{
				return -1;
			}
			if (material2 != null)
			{
				return 1;
			}
			return (material.GetInstanceID() >= material2.GetInstanceID()) ? 1 : (-1);
		}
		return num;
	}

	public Bounds CalculateBounds()
	{
		return CalculateBounds(null);
	}

	public Bounds CalculateBounds(Transform relativeParent)
	{
		if (relativeParent == null)
		{
			Vector3[] array = localCorners;
			Bounds result = new Bounds(array[0], Vector3.zero);
			for (int i = 1; i < 4; i++)
			{
				result.Encapsulate(array[i]);
			}
			return result;
		}
		Matrix4x4 worldToLocalMatrix = relativeParent.worldToLocalMatrix;
		Vector3[] array2 = worldCorners;
		Bounds result2 = new Bounds(worldToLocalMatrix.MultiplyPoint3x4(array2[0]), Vector3.zero);
		for (int j = 1; j < 4; j++)
		{
			result2.Encapsulate(worldToLocalMatrix.MultiplyPoint3x4(array2[j]));
		}
		return result2;
	}

	private void SetDirty()
	{
		if (drawCall != null)
		{
			drawCall.isDirty = true;
		}
		else if (isVisible && hasVertices)
		{
			UIPanel.SetDirty();
		}
	}

	protected void RemoveFromPanel()
	{
		if (mPanel != null)
		{
			drawCall = null;
			mPanel = null;
			SetDirty();
		}
	}

	protected virtual void OnValidate()
	{
		mChanged = true;
		if (mWidth < minWidth)
		{
			mWidth = minWidth;
		}
		if (mHeight < minHeight)
		{
			mHeight = minHeight;
		}
		if (autoResizeBoxCollider)
		{
			ResizeCollider();
		}
	}

	public void MarkAsChangedLite()
	{
		mChanged = true;
	}

	public virtual void MarkAsChanged()
	{
		if (!(this == null))
		{
			mChanged = true;
			if (mPanel != null && base.enabled && NGUITools.GetActive(base.gameObject) && !Application.isPlaying)
			{
				SetDirty();
				CheckLayer();
			}
		}
	}

	public void CreatePanel()
	{
		if (!(mPanel == null) || !base.enabled || !NGUITools.GetActive(base.gameObject))
		{
			return;
		}
		mPanel = UIPanel.Find(cachedTransform, mStarted);
		if (mPanel != null)
		{
			CheckLayer();
			mChanged = true;
			if (material != null)
			{
				UIPanel.SetDirty();
			}
		}
	}

	public void CheckLayer()
	{
		if (mPanel != null && mPanel.gameObject.layer != base.gameObject.layer)
		{
			Debug.LogWarning("You can't place widgets on a layer different than the UIPanel that manages them.\nIf you want to move widgets to a different layer, parent them to a new panel instead.", this);
			base.gameObject.layer = mPanel.gameObject.layer;
		}
	}

	public void ParentHasChanged()
	{
		if (mPanel != null)
		{
			UIPanel uIPanel = UIPanel.Find(cachedTransform);
			if (mPanel != uIPanel)
			{
				RemoveFromPanel();
				CreatePanel();
			}
		}
	}

	protected virtual void Awake()
	{
		mGo = base.gameObject;
		mPlayMode = Application.isPlaying;
	}

	protected virtual void OnEnable()
	{
		list.Add(this);
		mChanged = true;
		mPanel = null;
		if (mWidth == 100 && mHeight == 100 && cachedTransform.localScale.magnitude > 8f)
		{
			UpgradeFrom265();
			cachedTransform.localScale = Vector3.one;
		}
	}

	protected virtual void UpgradeFrom265()
	{
		Vector3 localScale = cachedTransform.localScale;
		mWidth = Mathf.Abs(Mathf.RoundToInt(localScale.x));
		mHeight = Mathf.Abs(Mathf.RoundToInt(localScale.y));
		if (GetComponent<BoxCollider>() != null)
		{
			NGUITools.AddWidgetCollider(base.gameObject, true);
		}
	}

	private void Start()
	{
		mStarted = true;
		OnStart();
		CreatePanel();
	}

	public virtual void Update()
	{
		if (mPanel == null)
		{
			CreatePanel();
		}
	}

	protected virtual void OnDisable()
	{
		list.Remove(this);
		RemoveFromPanel();
	}

	private void OnDestroy()
	{
		RemoveFromPanel();
	}

	private bool HasTransformChanged()
	{
		if (cachedTransform.hasChanged)
		{
			mTrans.hasChanged = false;
			return true;
		}
		return false;
	}

	public bool UpdateGeometry(UIPanel p, bool forceVisible)
	{
		if (p != null)
		{
			mPanel = p;
			bool flag = false;
			float num = finalAlpha;
			bool flag2 = num > 0.001f;
			bool flag3 = forceVisible || mVisibleByPanel;
			if (HasTransformChanged())
			{
				if (!mPanel.widgetsAreStatic)
				{
					mLocalToPanel = p.worldToLocal * cachedTransform.localToWorldMatrix;
					flag = true;
					Vector2 vector = pivotOffset;
					float num2 = (0f - vector.x) * (float)mWidth;
					float num3 = (0f - vector.y) * (float)mHeight;
					float x = num2 + (float)mWidth;
					float y = num3 + (float)mHeight;
					Transform transform = cachedTransform;
					Vector3 v = transform.TransformPoint(num2, num3, 0f);
					Vector3 v2 = transform.TransformPoint(x, y, 0f);
					v = p.worldToLocal.MultiplyPoint3x4(v);
					v2 = p.worldToLocal.MultiplyPoint3x4(v2);
					if (Vector3.SqrMagnitude(mOldV0 - v) > 1E-06f || Vector3.SqrMagnitude(mOldV1 - v2) > 1E-06f)
					{
						mChanged = true;
						mOldV0 = v;
						mOldV1 = v2;
					}
				}
				if (flag2 || mForceVisible != forceVisible)
				{
					mForceVisible = forceVisible;
					flag3 = forceVisible || mPanel.IsVisible(this);
				}
			}
			else if (flag2 && mForceVisible != forceVisible)
			{
				mForceVisible = forceVisible;
				flag3 = mPanel.IsVisible(this);
			}
			if (mVisibleByPanel != flag3)
			{
				mVisibleByPanel = flag3;
				mChanged = true;
			}
			if (mVisibleByPanel && mLastAlpha != num)
			{
				mChanged = true;
			}
			mLastAlpha = num;
			if (mChanged)
			{
				mChanged = false;
				if (material != null && isVisible)
				{
					bool result = mGeom.hasVertices;
					mGeom.Clear();
					OnFill(mGeom.verts, mGeom.uvs, mGeom.cols);
					if (mGeom.hasVertices)
					{
						if (!flag)
						{
							mLocalToPanel = p.worldToLocal * cachedTransform.localToWorldMatrix;
						}
						mGeom.ApplyTransform(mLocalToPanel);
						return true;
					}
					return result;
				}
				if (mGeom.hasVertices)
				{
					mGeom.Clear();
					return true;
				}
			}
		}
		return false;
	}

	public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
	{
		mGeom.WriteToBuffers(v, u, c, n, t);
	}

	public virtual void MakePixelPerfect()
	{
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.z = Mathf.Round(localPosition.z);
		localPosition.x = Mathf.Round(localPosition.x);
		localPosition.y = Mathf.Round(localPosition.y);
		cachedTransform.localPosition = localPosition;
		Vector3 localScale = cachedTransform.localScale;
		cachedTransform.localScale = new Vector3(Mathf.Sign(localScale.x), Mathf.Sign(localScale.y), 1f);
	}

	protected virtual void OnStart()
	{
	}

	public virtual void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
	}
}

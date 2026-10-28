using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Scroll Bar")]
public class UIScrollBar : UIWidgetContainer
{
	public enum Direction
	{
		Horizontal = 0,
		Vertical = 1
	}

	public delegate void OnDragFinished();

	public static UIScrollBar current;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public OnDragFinished onDragFinished;

	[HideInInspector]
	[SerializeField]
	private UISprite mBG;

	[HideInInspector]
	[SerializeField]
	private UISprite mFG;

	[SerializeField]
	[HideInInspector]
	private Direction mDir;

	[HideInInspector]
	[SerializeField]
	private bool mInverted;

	[SerializeField]
	[HideInInspector]
	private float mScroll;

	[HideInInspector]
	[SerializeField]
	private float mSize = 1f;

	private Transform mTrans;

	private bool mIsDirty;

	private Camera mCam;

	private Vector2 mScreenPos = Vector2.zero;

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

	public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				mCam = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
			return mCam;
		}
	}

	public UISprite background
	{
		get
		{
			return mBG;
		}
		set
		{
			if (mBG != value)
			{
				mBG = value;
				mIsDirty = true;
			}
		}
	}

	public UISprite foreground
	{
		get
		{
			return mFG;
		}
		set
		{
			if (mFG != value)
			{
				mFG = value;
				mIsDirty = true;
			}
		}
	}

	public Direction direction
	{
		get
		{
			return mDir;
		}
		set
		{
			if (mDir == value)
			{
				return;
			}
			mDir = value;
			mIsDirty = true;
			if (!(mBG != null))
			{
				return;
			}
			int width = mBG.width;
			int height = mBG.height;
			if ((mDir == Direction.Vertical && width > height) || (mDir == Direction.Horizontal && width < height))
			{
				mBG.width = height;
				mBG.height = width;
				ForceUpdate();
				if (mBG.GetComponent<Collider>() != null)
				{
					NGUITools.AddWidgetCollider(mBG.gameObject);
				}
				if (mFG.GetComponent<Collider>() != null)
				{
					NGUITools.AddWidgetCollider(mFG.gameObject);
				}
			}
		}
	}

	public bool inverted
	{
		get
		{
			return mInverted;
		}
		set
		{
			if (mInverted != value)
			{
				mInverted = value;
				mIsDirty = true;
			}
		}
	}

	public float value
	{
		get
		{
			return mScroll;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mScroll != num)
			{
				mScroll = num;
				mIsDirty = true;
				if (onChange != null)
				{
					current = this;
					EventDelegate.Execute(onChange);
					current = null;
				}
			}
		}
	}

	[Obsolete("Use 'value' instead")]
	public float scrollValue
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public float barSize
	{
		get
		{
			return mSize;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mSize != num)
			{
				mSize = num;
				mIsDirty = true;
				if (onChange != null)
				{
					current = this;
					EventDelegate.Execute(onChange);
					current = null;
				}
			}
		}
	}

	public float alpha
	{
		get
		{
			if (mFG != null)
			{
				return mFG.alpha;
			}
			if (mBG != null)
			{
				return mBG.alpha;
			}
			return 0f;
		}
		set
		{
			if (mFG != null)
			{
				mFG.alpha = value;
				if (mFG.GetComponent<Collider>() != null)
				{
					mFG.GetComponent<Collider>().enabled = mFG.alpha > 0.001f;
				}
			}
			if (mBG != null)
			{
				mBG.alpha = value;
				if (mBG.GetComponent<Collider>() != null)
				{
					mBG.GetComponent<Collider>().enabled = mBG.alpha > 0.001f;
				}
			}
		}
	}

	private void CenterOnPos(Vector2 localPos)
	{
		if (!(mBG == null) && !(mFG == null))
		{
			Bounds bounds = NGUIMath.CalculateRelativeInnerBounds(cachedTransform, mBG);
			Bounds bounds2 = NGUIMath.CalculateRelativeInnerBounds(cachedTransform, mFG);
			if (mDir == Direction.Horizontal)
			{
				float num = bounds.size.x - bounds2.size.x;
				float num2 = num * 0.5f;
				float num3 = bounds.center.x - num2;
				float num4 = ((!(num > 0f)) ? 0f : ((localPos.x - num3) / num));
				value = ((!mInverted) ? num4 : (1f - num4));
			}
			else
			{
				float num5 = bounds.size.y - bounds2.size.y;
				float num6 = num5 * 0.5f;
				float num7 = bounds.center.y - num6;
				float num8 = ((!(num5 > 0f)) ? 0f : (1f - (localPos.y - num7) / num5));
				value = ((!mInverted) ? num8 : (1f - num8));
			}
		}
	}

	private void Reposition(Vector2 screenPos)
	{
		Transform transform = cachedTransform;
		Plane plane = new Plane(transform.rotation * Vector3.back, transform.position);
		Ray ray = cachedCamera.ScreenPointToRay(screenPos);
		float enter;
		if (plane.Raycast(ray, out enter))
		{
			CenterOnPos(transform.InverseTransformPoint(ray.GetPoint(enter)));
		}
	}

	private void OnPressBackground(GameObject go, bool isPressed)
	{
		mCam = UICamera.currentCamera;
		Reposition(UICamera.lastTouchPosition);
		if (!isPressed && onDragFinished != null)
		{
			onDragFinished();
		}
	}

	private void OnDragBackground(GameObject go, Vector2 delta)
	{
		mCam = UICamera.currentCamera;
		Reposition(UICamera.lastTouchPosition);
	}

	private void OnPressForeground(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			mCam = UICamera.currentCamera;
			Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(mFG.cachedTransform);
			mScreenPos = mCam.WorldToScreenPoint(bounds.center);
		}
		else if (onDragFinished != null)
		{
			onDragFinished();
		}
	}

	private void OnDragForeground(GameObject go, Vector2 delta)
	{
		mCam = UICamera.currentCamera;
		Reposition(mScreenPos + UICamera.currentTouch.totalDelta);
	}

	private void Start()
	{
		if (background != null && background.GetComponent<Collider>() != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(background.gameObject);
			uIEventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener.onPress, new UIEventListener.BoolDelegate(OnPressBackground));
			uIEventListener.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener.onDrag, new UIEventListener.VectorDelegate(OnDragBackground));
		}
		if (foreground != null && foreground.GetComponent<Collider>() != null)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(foreground.gameObject);
			uIEventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener2.onPress, new UIEventListener.BoolDelegate(OnPressForeground));
			uIEventListener2.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener2.onDrag, new UIEventListener.VectorDelegate(OnDragForeground));
		}
		if (onChange != null)
		{
			current = this;
			EventDelegate.Execute(onChange);
			current = null;
		}
		ForceUpdate();
	}

	private void Update()
	{
		if (mIsDirty)
		{
			ForceUpdate();
		}
	}

	public void ForceUpdate()
	{
		mIsDirty = false;
		if (!(mBG != null) || !(mFG != null))
		{
			return;
		}
		mSize = Mathf.Clamp01(mSize);
		mScroll = Mathf.Clamp01(mScroll);
		Vector4 border = mBG.border;
		Vector4 border2 = mFG.border;
		Vector2 vector = new Vector2(Mathf.Max(0f, (float)mBG.width - border.x - border.z), Mathf.Max(0f, (float)mBG.height - border.y - border.w));
		float num = ((!mInverted) ? mScroll : (1f - mScroll));
		if (mDir == Direction.Horizontal)
		{
			Vector2 vector2 = new Vector2(vector.x * mSize, vector.y);
			mFG.pivot = UIWidget.Pivot.Left;
			mBG.pivot = UIWidget.Pivot.Left;
			mBG.cachedTransform.localPosition = Vector3.zero;
			mFG.cachedTransform.localPosition = new Vector3(Mathf.RoundToInt(border.x - border2.x + (vector.x - vector2.x) * num), 0f, 0f);
			mFG.width = Mathf.RoundToInt(vector2.x + border2.x + border2.z);
			mFG.height = Mathf.RoundToInt(vector2.y + border2.y + border2.w);
			if (mFG.GetComponent<Collider>() != null)
			{
				NGUITools.AddWidgetCollider(mFG.gameObject);
			}
		}
		else
		{
			Vector2 vector3 = new Vector2(vector.x, vector.y * mSize);
			mFG.pivot = UIWidget.Pivot.Top;
			mBG.pivot = UIWidget.Pivot.Top;
			mBG.cachedTransform.localPosition = Vector3.zero;
			mFG.cachedTransform.localPosition = new Vector3(0f, Mathf.RoundToInt(0f - border.y + border2.y - (vector.y - vector3.y) * num), 0f);
			mFG.width = Mathf.RoundToInt(vector3.x + border2.x + border2.z);
			mFG.height = Mathf.RoundToInt(vector3.y + border2.y + border2.w);
			if (mFG.GetComponent<Collider>() != null)
			{
				NGUITools.AddWidgetCollider(mFG.gameObject);
			}
		}
	}
}

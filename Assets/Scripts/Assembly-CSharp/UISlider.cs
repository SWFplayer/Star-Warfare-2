using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Slider")]
public class UISlider : UIWidgetContainer
{
	public enum Direction
	{
		Horizontal = 0,
		Vertical = 1
	}

	public static UISlider current;

	public Transform foreground;

	public Transform thumb;

	public Direction direction;

	public int numberOfSteps;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[HideInInspector]
	[SerializeField]
	private float rawValue = 1f;

	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	[HideInInspector]
	[SerializeField]
	private string functionName = "OnSliderChange";

	private BoxCollider mCol;

	private Transform mTrans;

	private Transform mFGTrans;

	private UIWidget mFGWidget;

	private UISprite mFGFilled;

	private bool mInitDone;

	private Vector2 mSize = Vector2.zero;

	private Vector2 mCenter = Vector3.zero;

	public float value
	{
		get
		{
			float num = rawValue;
			if (numberOfSteps > 1)
			{
				num = Mathf.Round(num * (float)(numberOfSteps - 1)) / (float)(numberOfSteps - 1);
			}
			return num;
		}
		set
		{
			Set(value, false);
		}
	}

	[Obsolete("Use 'value' instead")]
	public float sliderValue
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

	public Vector2 fullSize
	{
		get
		{
			return mSize;
		}
		set
		{
			if (!(mSize != value))
			{
				return;
			}
			mSize = value;
			if (foreground != null)
			{
				UIWidget component = foreground.GetComponent<UIWidget>();
				if (component != null)
				{
					component.width = Mathf.RoundToInt(value.x);
					component.height = Mathf.RoundToInt(value.y);
					Vector3[] localCorners = component.localCorners;
					mCenter = Vector3.Lerp(localCorners[0], localCorners[2], 0.5f);
				}
				else
				{
					mCenter = foreground.localPosition + (Vector3)value * 0.5f;
				}
			}
			ForceUpdate();
		}
	}

	public void Init()
	{
		mInitDone = true;
		if (foreground != null)
		{
			mFGWidget = foreground.GetComponent<UIWidget>();
			mFGFilled = ((!(mFGWidget != null)) ? null : (mFGWidget as UISprite));
			mFGTrans = foreground.transform;
			if (mSize == Vector2.zero)
			{
				UIWidget component = foreground.GetComponent<UIWidget>();
				mSize = ((!(component != null)) ? ((Vector2)foreground.localScale) : new Vector2(component.width, component.height));
			}
			if (mCenter == Vector2.zero)
			{
				UIWidget component2 = foreground.GetComponent<UIWidget>();
				if (component2 != null)
				{
					Vector3[] localCorners = component2.localCorners;
					mCenter = Vector3.Lerp(localCorners[0], localCorners[2], 0.5f);
				}
				else
				{
					mCenter = foreground.localPosition + foreground.localScale * 0.5f;
				}
			}
		}
		else if (mCol != null)
		{
			if (mSize == Vector2.zero)
			{
				mSize = mCol.size;
			}
			if (mCenter == Vector2.zero)
			{
				mCenter = mCol.center;
			}
		}
		else
		{
			Debug.LogWarning("UISlider expected to find a foreground object or a box collider to work with", this);
		}
	}

	private void Awake()
	{
		mTrans = base.transform;
		mCol = base.GetComponent<Collider>() as BoxCollider;
	}

	private void Start()
	{
		Init();
		if (EventDelegate.IsValid(onChange))
		{
			eventReceiver = null;
			functionName = null;
		}
		if (Application.isPlaying && thumb != null && thumb.GetComponent<Collider>() != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(thumb.gameObject);
			uIEventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener.onPress, new UIEventListener.BoolDelegate(OnPressThumb));
			uIEventListener.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener.onDrag, new UIEventListener.VectorDelegate(OnDragThumb));
		}
		Set(rawValue, true);
	}

	private void OnPress(bool pressed)
	{
		if (base.enabled && pressed && UICamera.currentTouchID != -100)
		{
			UpdateDrag();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled)
		{
			UpdateDrag();
		}
	}

	private void OnPressThumb(GameObject go, bool pressed)
	{
		if (base.enabled && pressed)
		{
			UpdateDrag();
		}
	}

	private void OnDragThumb(GameObject go, Vector2 delta)
	{
		if (base.enabled)
		{
			UpdateDrag();
		}
	}

	private void OnKey(KeyCode key)
	{
		if (!base.enabled)
		{
			return;
		}
		float num = ((!((float)numberOfSteps > 1f)) ? 0.125f : (1f / (float)(numberOfSteps - 1)));
		if (direction == Direction.Horizontal)
		{
			switch (key)
			{
			case KeyCode.LeftArrow:
				Set(rawValue - num, false);
				break;
			case KeyCode.RightArrow:
				Set(rawValue + num, false);
				break;
			}
		}
		else
		{
			switch (key)
			{
			case KeyCode.DownArrow:
				Set(rawValue - num, false);
				break;
			case KeyCode.UpArrow:
				Set(rawValue + num, false);
				break;
			}
		}
	}

	private void UpdateDrag()
	{
		if (!(mCol == null) && !(UICamera.currentCamera == null) && UICamera.currentTouch != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float enter;
			if (new Plane(mTrans.rotation * Vector3.back, mTrans.position).Raycast(ray, out enter))
			{
				Vector3 vector = mTrans.localPosition + (Vector3)(mCenter - mSize * 0.5f);
				Vector3 vector2 = mTrans.localPosition - vector;
				Vector3 vector3 = mTrans.InverseTransformPoint(ray.GetPoint(enter));
				Vector3 vector4 = vector3 + vector2;
				Set((direction != Direction.Horizontal) ? (vector4.y / mSize.y) : (vector4.x / mSize.x), false);
			}
		}
	}

	private void Set(float input, bool force)
	{
		if (!mInitDone)
		{
			Init();
		}
		float num = Mathf.Clamp01(input);
		if (num < 0.001f)
		{
			num = 0f;
		}
		float num2 = value;
		rawValue = num;
		float num3 = value;
		if (!force && num2 == num3)
		{
			return;
		}
		Vector3 localScale = mSize;
		if (direction == Direction.Horizontal)
		{
			localScale.x *= num3;
		}
		else
		{
			localScale.y *= num3;
		}
		if (mFGFilled != null && mFGFilled.type == UISprite.Type.Filled)
		{
			mFGFilled.fillAmount = num3;
		}
		else if (mFGWidget != null)
		{
			if (num3 > 0.001f)
			{
				mFGWidget.width = Mathf.RoundToInt(localScale.x);
				mFGWidget.height = Mathf.RoundToInt(localScale.y);
				mFGWidget.enabled = true;
			}
			else
			{
				mFGWidget.enabled = false;
			}
		}
		else if (foreground != null)
		{
			mFGTrans.localScale = localScale;
		}
		if (thumb != null)
		{
			Vector3 localPosition = thumb.localPosition;
			if (mFGFilled != null && mFGFilled.type == UISprite.Type.Filled)
			{
				if (mFGFilled.fillDirection == UISprite.FillDirection.Horizontal)
				{
					localPosition.x = ((!mFGFilled.invert) ? localScale.x : (mSize.x - localScale.x));
				}
				else if (mFGFilled.fillDirection == UISprite.FillDirection.Vertical)
				{
					localPosition.y = ((!mFGFilled.invert) ? localScale.y : (mSize.y - localScale.y));
				}
				else
				{
					Debug.LogWarning("Slider thumb is only supported with Horizontal or Vertical fill direction", this);
				}
			}
			else if (direction == Direction.Horizontal)
			{
				localPosition.x = localScale.x;
			}
			else
			{
				localPosition.y = localScale.y;
			}
			thumb.localPosition = localPosition;
		}
		current = this;
		if (EventDelegate.IsValid(onChange))
		{
			EventDelegate.Execute(onChange);
		}
		else if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
		{
			eventReceiver.SendMessage(functionName, num3, SendMessageOptions.DontRequireReceiver);
		}
		current = null;
	}

	public void ForceUpdate()
	{
		Set(rawValue, true);
	}
}

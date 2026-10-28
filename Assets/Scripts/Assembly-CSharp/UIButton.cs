using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	public static UIButton current;

	public Color disabledColor = Color.grey;

	public List<EventDelegate> onClick = new List<EventDelegate>();

	public bool isEnabled
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			Collider collider = base.GetComponent<Collider>();
			return (bool)collider && collider.enabled;
		}
		set
		{
			Collider collider = base.GetComponent<Collider>();
			if (collider != null)
			{
				collider.enabled = value;
			}
			base.enabled = value;
		}
	}

	protected override void OnEnable()
	{
		if (isEnabled)
		{
			if (mStarted)
			{
				if (mHighlighted)
				{
					base.OnEnable();
				}
				else
				{
					UpdateColor(true, false);
				}
			}
		}
		else
		{
			UpdateColor(false, true);
		}
	}

	protected override void OnDisable()
	{
		if (mStarted)
		{
			UpdateColor(false, false);
		}
	}

	public override void OnHover(bool isOver)
	{
		if (isEnabled)
		{
			base.OnHover(isOver);
		}
	}

	public override void OnPress(bool isPressed)
	{
		if (isEnabled)
		{
			base.OnPress(isPressed);
		}
	}

	private void OnClick()
	{
		if (isEnabled)
		{
			current = this;
			EventDelegate.Execute(onClick);
			current = null;
		}
	}

	public void UpdateColor(bool shouldBeEnabled, bool immediate)
	{
		if ((bool)tweenTarget)
		{
			if (!mStarted)
			{
				mStarted = true;
				Init();
			}
			Color color = ((!shouldBeEnabled) ? disabledColor : base.defaultColor);
			TweenColor tweenColor = TweenColor.Begin(tweenTarget, 0.15f, color);
			if (immediate)
			{
				tweenColor.color = color;
				tweenColor.enabled = false;
			}
		}
	}
}

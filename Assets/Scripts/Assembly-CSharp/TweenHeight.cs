using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Height")]
[RequireComponent(typeof(UIWidget))]
public class TweenHeight : UITweener
{
	public int from = 100;

	public int to = 100;

	public bool updateTable;

	private UIWidget mWidget;

	private UITable mTable;

	public UIWidget cachedWidget
	{
		get
		{
			if (mWidget == null)
			{
				mWidget = GetComponent<UIWidget>();
			}
			return mWidget;
		}
	}

	public int height
	{
		get
		{
			return cachedWidget.height;
		}
		set
		{
			cachedWidget.height = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		cachedWidget.height = Mathf.RoundToInt((float)from * (1f - factor) + (float)to * factor);
		if (!updateTable)
		{
			return;
		}
		if (mTable == null)
		{
			mTable = NGUITools.FindInParents<UITable>(base.gameObject);
			if (mTable == null)
			{
				updateTable = false;
				return;
			}
		}
		mTable.repositionNow = true;
	}

	public static TweenHeight Begin(UIWidget widget, float duration, int height)
	{
		TweenHeight tweenHeight = UITweener.Begin<TweenHeight>(widget.gameObject, duration);
		tweenHeight.from = widget.height;
		tweenHeight.to = height;
		if (duration <= 0f)
		{
			tweenHeight.Sample(1f, true);
			tweenHeight.enabled = false;
		}
		return tweenHeight;
	}
}

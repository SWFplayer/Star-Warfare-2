using System.Collections.Generic;
using UnityEngine;

public class UIWidgetSet : MonoBehaviour
{
	private List<UIWidget> mWidgets = new List<UIWidget>();

	private void Awake()
	{
		mWidgets.Clear();
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>(true);
		foreach (UIWidget item in componentsInChildren)
		{
			mWidgets.Add(item);
		}
	}

	public void SetAlpha(float alpha)
	{
		foreach (UIWidget mWidget in mWidgets)
		{
			mWidget.alpha = alpha;
		}
	}
}

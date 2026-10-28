using System;
using UnityEngine;

public class AchievementUIScript : MonoBehaviour
{
	public GameObject ExitButton;

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(ExitButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == ExitButton)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}

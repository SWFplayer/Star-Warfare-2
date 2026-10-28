using System;
using UnityEngine;

public class VidioPageScript : MonoBehaviour
{
	public GameObject Tip1;

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(Tip1);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClick));
	}

	public void OnClick(GameObject go)
	{
		if (go == Tip1)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OptionUIScript.mInstance.SetTip("LOC_VIDEO_TIPS_1");
		}
	}
}

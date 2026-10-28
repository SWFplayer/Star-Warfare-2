using UnityEngine;

public class HeadFramePage : MonoBehaviour
{
	public UITexture Head;

	private int mLastHeadFrame;

	private void OnEnable()
	{
		mLastHeadFrame = GameApp.GetInstance().GetUserState().GetCurrentTilteFrameID();
	}

	private void OnDisable()
	{
		if (mLastHeadFrame != GameApp.GetInstance().GetUserState().GetCurrentTilteFrameID())
		{
			GameApp.GetInstance().Save();
			Debug.Log("HeadFramePage save.");
		}
	}
}

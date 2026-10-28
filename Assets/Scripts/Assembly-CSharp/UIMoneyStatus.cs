using UnityEngine;

public class UIMoneyStatus : MonoBehaviour
{
	public UILabel Label_Cash;

	public UILabel Label_Mithril;

	private float mUpdateMoneyTime = 0.2f;

	private float mLastUpdateTime;

	private bool mNeedAutoUpdate = true;

	private void OnEnable()
	{
		mLastUpdateTime = Time.time;
		mNeedAutoUpdate = true;
		Refresh();
	}

	private void Update()
	{
		if (mNeedAutoUpdate && Time.time - mLastUpdateTime > mUpdateMoneyTime)
		{
			mLastUpdateTime = Time.time;
			Refresh();
		}
	}

	public void SetUpdate(bool needUpdate)
	{
		mNeedAutoUpdate = needUpdate;
	}

	public void Refresh()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState != null)
		{
			Label_Cash.text = (userState.GetCash() + userState.MainMissionRewardGold).ToString();
			Label_Mithril.text = (userState.GetMithril() + userState.MainMissionRewardMithril).ToString();
		}
	}

	private void OnClick()
	{
		if (IapScript.mInstance != null)
		{
			IapScript.mInstance.OpenPage(IapScript.IapType.Mithril);
		}
	}
}

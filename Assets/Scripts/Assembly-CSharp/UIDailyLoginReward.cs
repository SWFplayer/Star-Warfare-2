using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDailyLoginReward : MonoBehaviour
{
	[HideInInspector]
	public int mIndex;

	public GameObject selectBackground;

	public UISprite icon;

	public UILabel amountLabel;

	public UILabel receivedLable;

	public UISprite receivedSprite;

	public GameObject button;

	private List<UIWidget> mWidgets = new List<UIWidget>();

	private void Awake()
	{
		mWidgets.Clear();
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		foreach (UIWidget item in componentsInChildren)
		{
			mWidgets.Add(item);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetAlpha(float alpha)
	{
		foreach (UIWidget mWidget in mWidgets)
		{
			mWidget.alpha = alpha;
		}
	}

	public void Init(int index)
	{
		mIndex = index;
		DailyBonusManager.GetInstance().SetInfoToUI(this);
	}

	private void OnPress(bool isPress)
	{
		if (isPress)
		{
			UIDailyBonus.mInstance.SetNeedUpdate();
		}
	}

	public void UnSelect()
	{
		selectBackground.SetActive(false);
	}

	public void Select()
	{
		selectBackground.SetActive(true);
	}

	public void SetValue(int amount)
	{
		amountLabel.text = "X" + amount;
	}

	public void SetIcon(string iconName)
	{
		icon.spriteName = iconName;
	}

	public void SetButtonSprite(string buttonSpriteName)
	{
	}

	public void SetButton(bool received)
	{
		button.SetActive(true);
		if (!received)
		{
			button.GetComponent<Collider>().enabled = true;
			receivedLable.text = LocalizationManager.GetInstance().GetString("LOC_QIANDAO_BUTTON_1");
			receivedSprite.spriteName = "iapbtn_n";
			receivedSprite.gameObject.SetActive(true);
		}
		else
		{
			button.GetComponent<Collider>().enabled = false;
			button.GetComponent<UIImageButton>().normalSprite = "iapbtn_b";
			button.GetComponent<UIImageButton>().hoverSprite = "iapbtn_b";
			receivedLable.text = LocalizationManager.GetInstance().GetString("LOC_QIANDAO_BUTTON_2");
			receivedSprite.spriteName = "iapbtn_b";
			receivedSprite.gameObject.SetActive(true);
		}
		UIEventListener uIEventListener = UIEventListener.Get(button);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	public void InactiveButton()
	{
		button.SetActive(false);
	}

	public void OnButtonClick(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (go == button)
		{
			TryGetDailyLoginRewardRequest request = new TryGetDailyLoginRewardRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}
}

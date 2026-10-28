using System;
using UnityEngine;

public class VideoPageScript : MonoBehaviour
{
	public GameObject BloomOnBtn;

	public GameObject BloomOffBtn;

	public GameObject ColorCorrectionOnBtn;

	public GameObject ColorCorrectionOffBtn;

	public UISlider m_GraphicsQuality;

	public GameObject BloomOnSprite;

	public GameObject BloomOffSprite;

	public GameObject ColorCorrectionOnSprite;

	public GameObject ColorCorrectionOffSprite;

	private UserState State;

	private bool BloomOn;

	private bool ColorCorrectionOn;

	private float interval = 1f / 3f;

	private void Start()
	{
		State = GameApp.GetInstance().GetUserState();
		m_GraphicsQuality.numberOfSteps = 4;
		m_GraphicsQuality.value = (float)State.mGraphicQuality.mCapbility * interval;
		EventDelegate.Add(m_GraphicsQuality.onChange, OnChange);
		UIEventListener uIEventListener = UIEventListener.Get(BloomOnBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(BloomOffBtn);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(ColorCorrectionOnBtn);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(ColorCorrectionOffBtn);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClick));
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		BloomOn = State.mGraphicQuality.mSwitchBloom;
		ColorCorrectionOn = State.mGraphicQuality.mSwitchColorCorrection;
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (BloomOn)
		{
			BloomOnSprite.SetActive(true);
			BloomOffSprite.SetActive(false);
		}
		else
		{
			BloomOnSprite.SetActive(false);
			BloomOffSprite.SetActive(true);
		}
		if (ColorCorrectionOn)
		{
			ColorCorrectionOnSprite.SetActive(true);
			ColorCorrectionOffSprite.SetActive(false);
		}
		else
		{
			ColorCorrectionOffSprite.SetActive(true);
			ColorCorrectionOnSprite.SetActive(false);
		}
	}

	private void Update()
	{
	}

	private void OnChange()
	{
		DeviceCaps.Capbility capbility = DeviceCaps.Capbility.High;
		float num = (float)capbility * interval + 0.0001f;
		if (num >= m_GraphicsQuality.value)
		{
			State.mGraphicQuality.SetCapbility((DeviceCaps.Capbility)((m_GraphicsQuality.value + 0.001f) / interval));
			if (State.mGraphicQuality.mCapbility == DeviceCaps.Capbility.High)
			{
				State.mGraphicQuality.mSwitchBloom = true;
				State.mGraphicQuality.mSwitchColorCorrection = true;
			}
			else
			{
				State.mGraphicQuality.mSwitchBloom = false;
				State.mGraphicQuality.mSwitchColorCorrection = false;
			}
			if (State.mGraphicQuality.mCapbility == DeviceCaps.Capbility.Low)
			{
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_3"));
			}
			UpdateSprite();
		}
		else
		{
			m_GraphicsQuality.value = num;
		}
		Debug.Log("State.mGraphicQuality.mCapbility = " + State.mGraphicQuality.mCapbility);
		Debug.Log("m_GraphicsQuality.value = " + m_GraphicsQuality.value);
	}

	private void OnClick(GameObject go)
	{
		if (State.mGraphicQuality.mCapbility >= DeviceCaps.Capbility.Middle)
		{
			if (go == BloomOnBtn)
			{
				BloomOn = true;
				State.mGraphicQuality.mSwitchBloom = true;
			}
			else if (go == BloomOffBtn)
			{
				BloomOn = false;
				State.mGraphicQuality.mSwitchBloom = false;
			}
			else if (go == ColorCorrectionOnBtn)
			{
				ColorCorrectionOn = true;
				State.mGraphicQuality.mSwitchColorCorrection = true;
			}
			else if (go == ColorCorrectionOffBtn)
			{
				ColorCorrectionOn = false;
				State.mGraphicQuality.mSwitchColorCorrection = false;
			}
			UpdateSprite();
		}
	}
}

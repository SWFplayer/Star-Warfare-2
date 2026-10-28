using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlPageScript : MonoBehaviour
{
	private UserState State;

	private bool mGravityOn;

	private bool mItemHideOn;

	private byte TouchInputSensitivity;

	private bool mSkipDialog;

	private bool mAutoHide;

	private bool mChangeSide;

	private bool mSkipTutorial;

	private bool mPVEAutoAim;

	private bool mPVPAutoAim;

	private bool mSteadyModeOn;

	private bool mSmoothMoveOn;

	private bool mLeftRevert;

	private bool mRightRevert;

	public GameObject AutoHideOnBtn;

	public GameObject AutoHideOffBtn;

	public GameObject GravityOnBtn;

	public GameObject GravityOffBtn;

	public GameObject ItemHideOnBtn;

	public GameObject ItemHideOffBtn;

	public GameObject SkipDialogOnBtn;

	public GameObject SKipDialogOffBtn;

	public GameObject ChangeSideOnBtn;

	public GameObject ChangeSideOffBtn;

	public GameObject PVEAutoAimOnBtn;

	public GameObject PVEAutoAimOffBtn;

	public GameObject PVPAutoAimOnBtn;

	public GameObject PVPAutoAimOffBtn;

	public GameObject SteadyModeOnBtn;

	public GameObject SteadyModeOffBtn;

	public GameObject SmoothMoveOnBtn;

	public GameObject SmoothMoveOffBtn;

	public GameObject LeftRevertOnBtn;

	public GameObject LeftRevertOffBtn;

	public GameObject RightRevertOnBtn;

	public GameObject RightRevertOffBtn;

	public UISlider Sensitivity;

	public GameObject AutoHideOnSprite;

	public GameObject AutoHideOffSprite;

	public GameObject GravityOnSprite;

	public GameObject GravityOffSprite;

	public GameObject ItemHideOnSprite;

	public GameObject ItemHideOffSprite;

	public GameObject SkipDialogOnSprite;

	public GameObject SkipDialogOffSpite;

	public GameObject ChangeSideOnSprite;

	public GameObject ChangeSideOffSprite;

	public GameObject PVEAutoAimOnSprite;

	public GameObject PVEAutoAimOffSprite;

	public GameObject PVPAutoAimOnSprite;

	public GameObject PVPAutoAimOffSprite;

	public GameObject SteadyModeOnSprite;

	public GameObject SteadyModeOffSprite;

	public GameObject SmoothMoveOnSprite;

	public GameObject SmoothMoveOffSprite;

	public GameObject LeftRevertOnSprite;

	public GameObject LeftRevertOffSprite;

	public GameObject RightRevertOnSprite;

	public GameObject RightRevertOffSprite;

	public List<GameObject> Tips = new List<GameObject>();

	private void Start()
	{
		State = GameApp.GetInstance().GetUserState();
		TouchInputSensitivity = State.TouchInputSensitivity;
		Sensitivity.value = (float)(int)TouchInputSensitivity / 10f;
		mGravityOn = State.GetGravity();
		mItemHideOn = State.GetItemHide();
		mSkipDialog = State.GetSkipDialog();
		mAutoHide = State.GetAutoHide();
		mChangeSide = State.GetChangeSide();
		mSkipTutorial = State.GetSkipTutorial();
		mPVEAutoAim = State.GetPVEAutoAim();
		mPVPAutoAim = State.GetPVPAutoAim();
		mSteadyModeOn = State.GetSteadyModeOn();
		mSmoothMoveOn = State.GetSmoothMoveOn();
		mLeftRevert = State.GetHorizontalJoyStickRevert();
		mRightRevert = State.GetVerticalJoyStickRevert();
		EventDelegate.Add(Sensitivity.onChange, OnChange);
		UIEventListener uIEventListener = UIEventListener.Get(AutoHideOnBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(AutoHideOffBtn);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(GravityOnBtn);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(GravityOffBtn);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(ItemHideOnBtn);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(ItemHideOffBtn);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(SkipDialogOnBtn);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(SKipDialogOffBtn);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener9 = UIEventListener.Get(ChangeSideOnBtn);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener10 = UIEventListener.Get(ChangeSideOffBtn);
		uIEventListener10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener10.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener11 = UIEventListener.Get(PVEAutoAimOnBtn);
		uIEventListener11.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener11.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener12 = UIEventListener.Get(PVEAutoAimOffBtn);
		uIEventListener12.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener12.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener13 = UIEventListener.Get(PVPAutoAimOnBtn);
		uIEventListener13.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener13.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener14 = UIEventListener.Get(PVPAutoAimOffBtn);
		uIEventListener14.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener14.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener15 = UIEventListener.Get(SteadyModeOnBtn);
		uIEventListener15.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener15.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener16 = UIEventListener.Get(SteadyModeOffBtn);
		uIEventListener16.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener16.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener17 = UIEventListener.Get(SmoothMoveOnBtn);
		uIEventListener17.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener17.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener18 = UIEventListener.Get(SmoothMoveOffBtn);
		uIEventListener18.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener18.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener19 = UIEventListener.Get(LeftRevertOnBtn);
		uIEventListener19.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener19.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener20 = UIEventListener.Get(LeftRevertOffBtn);
		uIEventListener20.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener20.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener21 = UIEventListener.Get(RightRevertOnBtn);
		uIEventListener21.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener21.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener22 = UIEventListener.Get(RightRevertOffBtn);
		uIEventListener22.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener22.onClick, new UIEventListener.VoidDelegate(OnClick));
		foreach (GameObject tip in Tips)
		{
			UIEventListener uIEventListener23 = UIEventListener.Get(tip);
			uIEventListener23.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener23.onClick, new UIEventListener.VoidDelegate(OnClick));
		}
		UpdateSprite();
	}

	private void OnChange()
	{
		State.TouchInputSensitivity = (byte)Mathf.RoundToInt(Sensitivity.value * 10f);
	}

	private void UpdateSprite()
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (mGravityOn)
		{
			GravityOnSprite.SetActive(true);
			GravityOffSprite.SetActive(false);
		}
		else
		{
			GravityOffSprite.SetActive(true);
			GravityOnSprite.SetActive(false);
		}
		if (mItemHideOn)
		{
			ItemHideOnSprite.SetActive(true);
			ItemHideOffSprite.SetActive(false);
		}
		else
		{
			ItemHideOffSprite.SetActive(true);
			ItemHideOnSprite.SetActive(false);
		}
		if (mSkipDialog)
		{
			SkipDialogOnSprite.SetActive(true);
			SkipDialogOffSpite.SetActive(false);
		}
		else
		{
			SkipDialogOffSpite.SetActive(true);
			SkipDialogOnSprite.SetActive(false);
		}
		if (mAutoHide)
		{
			AutoHideOnSprite.SetActive(true);
			AutoHideOffSprite.SetActive(false);
		}
		else
		{
			AutoHideOffSprite.SetActive(true);
			AutoHideOnSprite.SetActive(false);
		}
		if (mChangeSide)
		{
			ChangeSideOnSprite.SetActive(true);
			ChangeSideOffSprite.SetActive(false);
		}
		else
		{
			ChangeSideOffSprite.SetActive(true);
			ChangeSideOnSprite.SetActive(false);
		}
		if (mPVEAutoAim)
		{
			PVEAutoAimOnSprite.SetActive(true);
			PVEAutoAimOffSprite.SetActive(false);
		}
		else
		{
			PVEAutoAimOnSprite.SetActive(false);
			PVEAutoAimOffSprite.SetActive(true);
		}
		if (mPVPAutoAim)
		{
			PVPAutoAimOnSprite.SetActive(true);
			PVPAutoAimOffSprite.SetActive(false);
		}
		else
		{
			PVPAutoAimOnSprite.SetActive(false);
			PVPAutoAimOffSprite.SetActive(true);
		}
		if (mSteadyModeOn)
		{
			SteadyModeOnSprite.SetActive(true);
			SteadyModeOffSprite.SetActive(false);
		}
		else
		{
			SteadyModeOnSprite.SetActive(false);
			SteadyModeOffSprite.SetActive(true);
		}
		if (mSmoothMoveOn)
		{
			SmoothMoveOnSprite.SetActive(true);
			SmoothMoveOffSprite.SetActive(false);
		}
		else
		{
			SmoothMoveOnSprite.SetActive(false);
			SmoothMoveOffSprite.SetActive(true);
		}
		if (mLeftRevert)
		{
			LeftRevertOnSprite.SetActive(true);
			LeftRevertOffSprite.SetActive(false);
		}
		else
		{
			LeftRevertOffSprite.SetActive(true);
			LeftRevertOnSprite.SetActive(false);
		}
		if (mRightRevert)
		{
			RightRevertOnSprite.SetActive(true);
			RightRevertOffSprite.SetActive(false);
		}
		else
		{
			RightRevertOffSprite.SetActive(true);
			RightRevertOnSprite.SetActive(false);
		}
	}

	private void OnClick(GameObject go)
	{
		if (go == GravityOnBtn)
		{
			mGravityOn = true;
			State.SetGravity(true);
		}
		else if (go == GravityOffBtn)
		{
			mGravityOn = false;
			State.SetGravity(false);
		}
		else if (go == ItemHideOnBtn)
		{
			mItemHideOn = true;
			State.SetItemHide(true);
		}
		else if (go == ItemHideOffBtn)
		{
			mItemHideOn = false;
			State.SetItemHide(false);
		}
		else if (go == SkipDialogOnBtn)
		{
			mSkipDialog = true;
			State.SetSkipDialog(true);
		}
		else if (go == SKipDialogOffBtn)
		{
			mSkipDialog = false;
			State.SetSkipDialog(false);
		}
		else if (go == AutoHideOnBtn)
		{
			mAutoHide = true;
			State.SetAutoHide(true);
		}
		else if (go == AutoHideOffBtn)
		{
			mAutoHide = false;
			State.SetAutoHide(false);
		}
		else if (go == ChangeSideOnBtn)
		{
			mChangeSide = true;
			State.SetChangeSide(true);
		}
		else if (go == ChangeSideOffBtn)
		{
			mChangeSide = false;
			State.SetChangeSide(false);
		}
		else if (go == PVEAutoAimOnBtn)
		{
			mPVEAutoAim = true;
			State.SetPVEAutoAim(true);
		}
		else if (go == PVEAutoAimOffBtn)
		{
			mPVEAutoAim = false;
			State.SetPVEAutoAim(false);
		}
		else if (go == PVPAutoAimOnBtn)
		{
			mPVPAutoAim = true;
			State.SetPVPAutoAim(true);
		}
		else if (go == PVPAutoAimOffBtn)
		{
			mPVPAutoAim = false;
			State.SetPVPAutoAim(false);
		}
		else if (go == SteadyModeOnBtn)
		{
			mSteadyModeOn = true;
			State.SetSteadyModeOn(true);
		}
		else if (go == SteadyModeOffBtn)
		{
			mSteadyModeOn = false;
			State.SetSteadyModeOn(false);
		}
		else if (go == SmoothMoveOnBtn)
		{
			mSmoothMoveOn = true;
			State.SetSmoothMoveOn(true);
		}
		else if (go == SmoothMoveOffBtn)
		{
			mSmoothMoveOn = false;
			State.SetSmoothMoveOn(false);
		}
		else if (go == LeftRevertOnBtn)
		{
			mLeftRevert = true;
			State.SetHorizontalJoyStickRevert(true);
		}
		else if (go == LeftRevertOffBtn)
		{
			mLeftRevert = false;
			State.SetHorizontalJoyStickRevert(false);
		}
		else if (go == RightRevertOnBtn)
		{
			mRightRevert = true;
			State.SetVerticalJoyStickRevert(true);
		}
		else if (go == RightRevertOffBtn)
		{
			mRightRevert = false;
			State.SetVerticalJoyStickRevert(false);
		}
		else if (Tips.Contains(go))
		{
			string tip = "LOC_CONTROL_TIPS_" + (Tips.IndexOf(go) + 1);
			OptionUIScript.mInstance.SetTip(tip);
		}
		UpdateSprite();
	}
}

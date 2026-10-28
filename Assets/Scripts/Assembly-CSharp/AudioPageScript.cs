using System;
using UnityEngine;

public class AudioPageScript : MonoBehaviour
{
	private UserState State;

	private bool MusicOn;

	private bool SoundOn;

	public GameObject MusicOnBtn;

	public GameObject MusicOffBtn;

	public GameObject SoundOnBtn;

	public GameObject SoundOffBtn;

	public UISlider MusicVolume;

	public UISlider SoundVolume;

	public GameObject MusicOnSprite;

	public GameObject MusicOffSprite;

	public GameObject SoundOnSprite;

	public GameObject SoundOffSprite;

	private void Start()
	{
		State = GameApp.GetInstance().GetUserState();
		MusicVolume.value = State.GetMusicVolume();
		SoundVolume.value = State.GetSoundVolume();
		MusicOn = State.GetPlayMusic();
		SoundOn = State.GetPlaySound();
		EventDelegate.Add(MusicVolume.onChange, OnChange);
		EventDelegate.Add(SoundVolume.onChange, OnChange);
		UIEventListener uIEventListener = UIEventListener.Get(MusicOnBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(MusicOffBtn);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(SoundOnBtn);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(SoundOffBtn);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClick));
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (MusicOn)
		{
			MusicOnSprite.SetActive(true);
			MusicOffSprite.SetActive(false);
		}
		else
		{
			MusicOnSprite.SetActive(false);
			MusicOffSprite.SetActive(true);
		}
		if (SoundOn)
		{
			SoundOnSprite.SetActive(true);
			SoundOffSprite.SetActive(false);
		}
		else
		{
			SoundOffSprite.SetActive(true);
			SoundOnSprite.SetActive(false);
		}
	}

	private void OnChange()
	{
		State.SetMusicVolume(MusicVolume.value);
		State.SetSoundVolume(SoundVolume.value);
	}

	private void OnClick(GameObject go)
	{
		if (go == MusicOnBtn)
		{
			MusicOn = true;
			State.SetPlayMusic(true);
		}
		else if (go == MusicOffBtn)
		{
			MusicOn = false;
			State.SetPlayMusic(false);
		}
		else if (go == SoundOnBtn)
		{
			SoundOn = true;
			State.SetPlaySound(true);
		}
		else if (go == SoundOffBtn)
		{
			SoundOn = false;
			State.SetPlaySound(false);
		}
		UpdateSprite();
	}
}

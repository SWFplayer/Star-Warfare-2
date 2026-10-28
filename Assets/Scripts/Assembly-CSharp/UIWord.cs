using System;
using UnityEngine;

public class UIWord : MonoBehaviour
{
	[SerializeField]
	private UILabel m_DialogLabel;

	[SerializeField]
	private float m_WordSpeed = 0.02f;

	[SerializeField]
	private float m_DialogDuration = 2f;

	[SerializeField]
	private GameObject m_CallBackWhenDialogEnd;

	[SerializeField]
	private string m_Function;

	private bool mIsRefreshDialog;

	private bool mIsCloseDialog;

	private string mDialogText;

	private int mIndexInDialog;

	private DateTime mLastUpdateTime;

	private void Awake()
	{
		EUserLanguage userLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
		if (userLanguage != EUserLanguage.CHINESE_SIMPLIFIED && userLanguage != EUserLanguage.CHINESE_TRADITIONAL)
		{
			m_WordSpeed *= 0.1f;
		}
	}

	private void Update()
	{
		if (mIsRefreshDialog)
		{
			if ((DateTime.Now - mLastUpdateTime).TotalSeconds > (double)m_WordSpeed)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_MISSION_TEXT);
				m_DialogLabel.text += mDialogText[mIndexInDialog];
				mLastUpdateTime = DateTime.Now;
				mIndexInDialog++;
				if (mIndexInDialog > mDialogText.Length - 1)
				{
					mIsRefreshDialog = false;
					mLastUpdateTime = DateTime.Now;
				}
			}
		}
		else if (!mIsCloseDialog && (DateTime.Now - mLastUpdateTime).TotalSeconds > (double)m_DialogDuration)
		{
			mIsCloseDialog = true;
			if (m_CallBackWhenDialogEnd != null)
			{
				m_CallBackWhenDialogEnd.SendMessage(m_Function, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void TextEnd()
	{
		m_DialogLabel.text = mDialogText;
		mIsRefreshDialog = false;
	}

	public void SetText(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			mIsCloseDialog = false;
			mIsRefreshDialog = true;
			mDialogText = text;
			mIndexInDialog = 0;
			mLastUpdateTime = DateTime.Now;
			m_DialogLabel.text = string.Empty;
		}
	}

	public bool IsPrintingText()
	{
		return mIsRefreshDialog;
	}

	public bool IsDialogClose()
	{
		return mIsCloseDialog;
	}
}

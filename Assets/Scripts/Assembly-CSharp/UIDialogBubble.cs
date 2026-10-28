using System.Collections.Generic;
using UnityEngine;

public class UIDialogBubble : ComponentUI
{
	[SerializeField]
	private UILabel m_NameLabel;

	[SerializeField]
	private UISprite m_Avatar;

	[SerializeField]
	private UIWord m_UIWord;

	[SerializeField]
	private bool m_DisableWhenDialogEnd = true;

	[SerializeField]
	private GameObject m_CallBackWhenDialogEnd;

	[SerializeField]
	private string m_Function;

	[SerializeField]
	private GameObject m_NextDialogButton;

	public UIPanel m_DialogOpenPanel;

	public GameObject m_DialogOpenLines;

	public float m_PanelOpenHeight = 140f;

	public float m_OpenPanelDuration = 0.3f;

	private List<string> mDialogList = new List<string>();

	private int mIndexInList;

	private bool m_PanelOpening;

	private void Awake()
	{
		if (m_NextDialogButton != null)
		{
			AddButtonListener(m_NextDialogButton);
		}
	}

	private void OnEnable()
	{
		if (mDialogList.Count == 0)
		{
			base.gameObject.SetActive(false);
			return;
		}
		mIndexInList = 0;
		m_UIWord.SetText(mDialogList[mIndexInList]);
	}

	private void NextDialog()
	{
		mIndexInList++;
		if (mIndexInList > mDialogList.Count - 1)
		{
			if (m_DisableWhenDialogEnd)
			{
				base.gameObject.SetActive(false);
			}
			if (m_CallBackWhenDialogEnd != null)
			{
				m_CallBackWhenDialogEnd.SendMessage(m_Function, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			m_UIWord.SetText(mDialogList[mIndexInList]);
		}
	}

	public void ShowDialog(UIDialogData data)
	{
		if (data == null)
		{
			return;
		}
		mDialogList = data.DialogTextList;
		if (string.IsNullOrEmpty(data.AvatarName))
		{
			m_Avatar.gameObject.SetActive(false);
		}
		else
		{
			m_Avatar.gameObject.SetActive(true);
			m_Avatar.spriteName = data.AvatarName;
			m_Avatar.MakePixelPerfect();
		}
		if (m_NameLabel != null)
		{
			if (string.IsNullOrEmpty(data.SpeakerName))
			{
				m_NameLabel.gameObject.SetActive(false);
			}
			else
			{
				m_NameLabel.gameObject.SetActive(true);
				m_NameLabel.text = data.SpeakerName;
			}
		}
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
		m_PanelOpening = true;
		m_DialogOpenLines.SetActive(true);
		UITweener[] componentsInChildren = m_DialogOpenLines.GetComponentsInChildren<UITweener>();
		UITweener[] array = componentsInChildren;
		foreach (UITweener uITweener in array)
		{
			uITweener.Reset();
			uITweener.PlayForward();
		}
	}

	protected override void OnClickThumb(GameObject go)
	{
		base.OnClickThumb(go);
		if (go.Equals(m_NextDialogButton))
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (m_UIWord.IsPrintingText())
			{
				m_UIWord.TextEnd();
			}
			else
			{
				NextDialog();
			}
		}
	}

	private void OnDisable()
	{
		Vector4 clipRange = m_DialogOpenPanel.clipRange;
		clipRange.Set(clipRange.x, clipRange.y, clipRange.z, 1f);
		m_DialogOpenPanel.clipRange = clipRange;
	}

	private void Update()
	{
		if (m_PanelOpening)
		{
			if (m_DialogOpenPanel.clipRange.w < m_PanelOpenHeight)
			{
				Vector4 clipRange = m_DialogOpenPanel.clipRange;
				clipRange.Set(clipRange.x, clipRange.y, clipRange.z, clipRange.w + RealTime.deltaTime * m_PanelOpenHeight / m_OpenPanelDuration);
				m_DialogOpenPanel.clipRange = clipRange;
			}
			else
			{
				m_DialogOpenLines.SetActive(false);
				m_PanelOpening = false;
				Vector4 clipRange2 = m_DialogOpenPanel.clipRange;
				clipRange2.Set(clipRange2.x, clipRange2.y, clipRange2.z, 0f);
				m_DialogOpenPanel.clipRange = clipRange2;
			}
		}
	}
}

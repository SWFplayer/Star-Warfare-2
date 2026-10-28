using System.Collections.Generic;
using UnityEngine;

public class UIStoryDialog : GameUI
{
	public const string BUNDLE_KEY_STORY_DIALOG = "StoryDialog";

	[SerializeField]
	private GameObject m_DialogSkipButton;

	[SerializeField]
	private UIDialogBubble m_LeftUIDialogBubble;

	[SerializeField]
	private UIDialogBubble m_RightUIDialogBubble;

	private List<UIDialogData> mDialogList = new List<UIDialogData>();

	private int mIndexInList;

	private void Awake()
	{
		if (m_DialogSkipButton != null)
		{
			AddButtonListener(m_DialogSkipButton);
		}
		base.gameObject.SetActive(false);
	}

	protected override void OnInit(GameUIBundle bundle)
	{
		base.OnInit(bundle);
		if (bundle != null)
		{
			object obj = bundle.Get("StoryDialog");
			if (obj != null)
			{
				ShowDialog((List<UIDialogData>)obj);
			}
		}
	}

	private void OnEnable()
	{
		mIndexInList = 0;
		ShowDialog(mIndexInList);
	}

	private void NextDialog()
	{
		mIndexInList++;
		if (mIndexInList > mDialogList.Count - 1)
		{
			CloseDialog();
		}
		else
		{
			ShowDialog(mIndexInList);
		}
	}

	private void ShowDialog(int index)
	{
		if (mDialogList[index].AvatarPos.Equals("Left"))
		{
			m_RightUIDialogBubble.gameObject.SetActive(false);
			m_LeftUIDialogBubble.ShowDialog(mDialogList[index]);
		}
		else
		{
			m_LeftUIDialogBubble.gameObject.SetActive(false);
			m_RightUIDialogBubble.ShowDialog(mDialogList[index]);
		}
	}

	public void ShowDialog(List<UIDialogData> dataList)
	{
		if (dataList != null && dataList.Count > 0)
		{
			mDialogList = dataList;
			base.gameObject.SetActive(true);
		}
	}

	protected override void OnClickThumb(GameObject go)
	{
		base.OnClickThumb(go);
		if (go.Equals(m_DialogSkipButton))
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			CloseDialog();
		}
	}

	private void CloseDialog()
	{
		GameUIManager.GetInstance().RemoveUI(this);
	}
}

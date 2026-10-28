using System.Collections.Generic;
using UnityEngine;

public class VSRoomItemScript : MonoBehaviour
{
	public RoomPage PageType;

	public int id;

	public UISprite Background;

	public string UnlightBackgroundName;

	public string HighlightBackgroundName;

	public UISprite Arrow;

	public string UnlightArrowName;

	public string HighlightArrowName;

	public UISprite Highlight;

	public UISprite MapIcon;

	private List<UIWidget> mWidgets = new List<UIWidget>();

	[HideInInspector]
	public string DisplayStringOnLabel;

	[HideInInspector]
	public byte mWinCondition;

	[HideInInspector]
	public short mWinValue;

	public Mode mPVEMapMode;

	public EDifficultMode mDiffMode;

	public UISprite mPVP_PVE_ProcessBar;

	public UISprite ModeLock;

	private bool mIsAvailabel = true;

	private void Awake()
	{
		if (PageType != RoomPage.ModeSelectPage)
		{
			mIsAvailabel = true;
			if (ModeLock != null)
			{
				ModeLock.gameObject.SetActive(false);
			}
		}
		mWidgets.Clear();
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		foreach (UIWidget item in componentsInChildren)
		{
			mWidgets.Add(item);
		}
		if (PageType == RoomPage.QuickJoinPVPPage && id == 13)
		{
			if (Lobby.GetInstance().EnableMutantMode)
			{
				MapIcon.spriteName = "pk_pic1";
				MapIcon.width = 280;
				MapIcon.height = 190;
			}
			else
			{
				MapIcon.spriteName = "b_lock";
				MapIcon.MakePixelPerfect();
				mIsAvailabel = false;
			}
		}
	}

	public void SetHighlight(bool isHighlight)
	{
		if (PageType == RoomPage.MapSelectPage || PageType == RoomPage.QuickJoinPVPPage || PageType == RoomPage.QuickJoinPVEPage)
		{
			Highlight.gameObject.SetActive(isHighlight);
			if (mPVP_PVE_ProcessBar != null)
			{
				mPVP_PVE_ProcessBar.spriteName = ((!isHighlight) ? "move_n" : "move_u");
			}
			return;
		}
		if (PageType == RoomPage.PageSelect)
		{
			Highlight.gameObject.SetActive(isHighlight);
			if (isHighlight)
			{
				SetAlpha(1f);
			}
			else
			{
				SetAlpha(0.5f);
			}
			return;
		}
		if (Background != null)
		{
			if (isHighlight)
			{
				Background.spriteName = HighlightBackgroundName;
			}
			else
			{
				Background.spriteName = UnlightBackgroundName;
			}
		}
		if (Arrow != null)
		{
			if (isHighlight)
			{
				Arrow.spriteName = HighlightArrowName;
			}
			else
			{
				Arrow.spriteName = UnlightArrowName;
			}
		}
	}

	public void SetAlpha(float alpha)
	{
		foreach (UIWidget mWidget in mWidgets)
		{
			mWidget.alpha = alpha;
		}
	}

	private void OnPress(bool isPress)
	{
		if ((PageType == RoomPage.MapSelectPage || PageType == RoomPage.QuickJoinPVPPage || PageType == RoomPage.QuickJoinPVEPage) && isPress)
		{
			VSRoomUIScript.mInstance.SetNeedUpdateDragContents();
		}
	}

	public void SetWinConditionValues(byte winCondition, byte winValueIndex)
	{
		if (PageType == RoomPage.WinConditionSelectPage)
		{
			mWinCondition = winCondition;
			id = winValueIndex;
			mWinValue = 0;
			string text = string.Empty;
			string text2 = string.Empty;
			if (id == 0)
			{
				text = string.Empty;
				text2 = LocalizationManager.GetInstance().GetString("LOC_PVP_WINCON_1");
			}
			else if (winCondition == UIConstant.SEARCH_ROOM_WIN_CONDITION[1])
			{
				text = LocalizationManager.GetInstance().GetString("LOC_PVP_WINCON_2");
				text2 = UIConstant.WIN_VALUE_FOR_SCORE[id - 1];
				mWinValue = UIConstant.SEARCH_ROOM_WIN_VALUE_FOR_SCORE[id];
			}
			else if (winCondition == UIConstant.SEARCH_ROOM_WIN_CONDITION[2])
			{
				text = LocalizationManager.GetInstance().GetString("LOC_PVP_WINCON_3");
				text2 = UIConstant.WIN_VALUE_FOR_TIMER[id - 1];
				mWinValue = UIConstant.SEARCH_ROOM_WIN_VALUE_FOR_TIMER[id];
			}
			DisplayStringOnLabel = text + " " + text2;
			base.transform.Find("Unit").GetComponent<UILabel>().text = text;
			base.transform.Find("Value").GetComponent<UILabel>().text = text2;
		}
	}

	public void SetMapIcon(string spriteName)
	{
		if (MapIcon != null)
		{
			MapIcon.spriteName = spriteName;
		}
	}

	public bool IsAvailabel()
	{
		return mIsAvailabel;
	}

	public void SetID(int id)
	{
		this.id = id;
		if (PageType == RoomPage.ModeSelectPage && id == 13 && !Lobby.GetInstance().EnableMutantMode)
		{
			if (ModeLock != null)
			{
				ModeLock.gameObject.SetActive(true);
			}
			mIsAvailabel = false;
		}
	}
}

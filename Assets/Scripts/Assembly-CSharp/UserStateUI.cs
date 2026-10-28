using System.Collections.Generic;
using UnityEngine;

public class UserStateUI
{
	public class HUDFightDialogManager
	{
		private UIDialogData mUIDialogData;

		private string GetDialogText(string locId)
		{
			string text = LocalizationManager.GetInstance().GetString(locId);
			return "[Dialog][Character][Avatar]Icon3[/Avatar][Name]Player[/Name][/Character][Text]" + text + "[/Text][/Dialog]";
		}

		public void PushMissionDialog(string locId)
		{
			mUIDialogData = UIDialogData.CreateDialog(GetDialogText(locId))[0];
		}

		public void Clear()
		{
			mUIDialogData = null;
		}

		public UIDialogData PopFightDialogData()
		{
			UIDialogData result = mUIDialogData;
			mUIDialogData = null;
			return result;
		}
	}

	public class UIStoryDialogManager
	{
		public List<UIDialogData> LoadDialog(string dialogId)
		{
			if (dialogId[1] == '-')
			{
				string text = string.Empty;
				GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.DIALOGUE_LIST, DataSheetName.MISSION_DIALOGUE);
				for (int i = 0; i < gameDataTable.Count; i++)
				{
					string text2 = gameDataTable.GetString(i, 0);
					if (text2 == dialogId)
					{
						string text3 = gameDataTable.GetString(i, 1);
						string text4 = gameDataTable.GetString(i, 2);
						string text5 = gameDataTable.GetString(i, 3);
						string key = gameDataTable.GetString(i, 4);
						string text6 = text;
						text = text6 + "[Dialog][Character][AvatarPos]" + text5 + "[/AvatarPos][Avatar]" + text3 + "[/Avatar][Name]" + text4 + "[/Name][/Character][Text]" + LocalizationManager.GetInstance().GetString(key) + "[/Text][/Dialog]";
					}
				}
				if (text.Length == 0)
				{
					Debug.LogError("Dialog " + dialogId + " doesn't exist in Dialogue xls!");
				}
				return UIDialogData.CreateDialog(text);
			}
			string text7 = string.Empty;
			GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TUTORIAL, DataSheetName.TUTORIAL_DIALOG);
			for (int j = 0; j < gameDataTable2.Count; j++)
			{
				string text8 = gameDataTable2.GetString(j, 0);
				if (text8 == dialogId)
				{
					string text9 = gameDataTable2.GetString(j, 1);
					string text10 = gameDataTable2.GetString(j, 2);
					string text11 = gameDataTable2.GetString(j, 3);
					string key2 = gameDataTable2.GetString(j, 4);
					string text6 = text7;
					text7 = text6 + "[Dialog][Character][AvatarPos]" + text11 + "[/AvatarPos][Avatar]" + text9 + "[/Avatar][Name]" + text10 + "[/Name][/Character][Text]" + LocalizationManager.GetInstance().GetString(key2) + "[/Text][/Dialog]";
				}
			}
			if (text7.Length == 0)
			{
				Debug.LogError("Dialog " + dialogId + " doesn't exist in Tutorial xls!");
			}
			return UIDialogData.CreateDialog(text7);
		}
	}

	private static UserStateUI Instance;

	private HUDFightDialogManager mHUDFightDialogManager = new HUDFightDialogManager();

	private UIStoryDialogManager mUIStoryDialogManager = new UIStoryDialogManager();

	public HUDFightDialogManager FightDialogManager
	{
		get
		{
			return mHUDFightDialogManager;
		}
	}

	public UIStoryDialogManager StoryDialogManager
	{
		get
		{
			return mUIStoryDialogManager;
		}
	}

	private UserStateUI()
	{
	}

	public static UserStateUI GetInstance()
	{
		if (Instance == null)
		{
			Instance = new UserStateUI();
		}
		return Instance;
	}

	public void Clear()
	{
		mHUDFightDialogManager.Clear();
	}
}

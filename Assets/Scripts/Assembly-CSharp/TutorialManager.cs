using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager
{
	private static TutorialManager mInstance;

	private List<TutorialData> mToDoList = new List<TutorialData>();

	public static TutorialManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new TutorialManager();
		}
		return mInstance;
	}

	public void Init()
	{
		SetTutorialWatched(TutorialTriggerType.FIRST_BOSS, true);
		SetTutorialWatched(TutorialTriggerType.REGISTER_TUTORIAL, true);
		SetTutorialWatched(TutorialTriggerType.VOID_TUTORIAL, true);
		SetTutorialWatched(TutorialTriggerType.VOID_COMPOSE, true);
	}

	public bool Trigger(string type)
	{
		if (type != TutorialTriggerType.FREE_WATCH && (GetTutorialWatched(type) || GameApp.GetInstance().GetUserState().GetSkipTutorial()))
		{
			return false;
		}
		mToDoList = new List<TutorialData>();
		if (TutorialScript.GetInstance() == null)
		{
			GameObject original = Resources.Load("Tutorial/TutorialUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		}
		TutorialScript.GetInstance().SetType(type);
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TUTORIAL, DataSheetName.TUTORIAL_ALL);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			if (gameDataTable.GetString(i, 0) == type)
			{
				TutorialData item = new TutorialData
				{
					type = (TutorialType)(int)Enum.Parse(typeof(TutorialType), gameDataTable.GetString(i, 1)),
					ID = gameDataTable.GetString(i, 2)
				};
				if (item.type == TutorialType.TEXTURE)
				{
					item.texturePath = gameDataTable.GetString(i, 3);
				}
				item.index = gameDataTable.GetInt(i, 4);
				mToDoList.Add(item);
			}
		}
		return true;
	}

	public List<TutorialData> GetToDoList()
	{
		return mToDoList;
	}

	public bool GetTutorialWatched(string type)
	{
		int num = int.Parse(type) - 1;
		if (num == 14)
		{
			return false;
		}
		return GameApp.GetInstance().GetUserState().GetTutorialWatched(num);
	}

	public void SetTutorialWatched(string type, bool watched)
	{
		int i = int.Parse(type) - 1;
		GameApp.GetInstance().GetUserState().SetTutorialWatched(watched, i);
	}
}

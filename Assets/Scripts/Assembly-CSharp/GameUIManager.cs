using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
	private static GameUIManager Instance;

	private List<GameUI> mUiList;

	private List<GameUIStatus> mNextStatusList = new List<GameUIStatus>();

	private List<GameUIStatus> mRemovedStatusList = new List<GameUIStatus>();

	private List<GameUIStatus> mHiddenStatusList = new List<GameUIStatus>();

	private List<GameUIBundle> mNextGameUIBundleList = new List<GameUIBundle>();

	private List<GameUIListener> mNextGameUIListenerList = new List<GameUIListener>();

	private void Awake()
	{
		mUiList = new List<GameUI>();
		Instance = this;
	}

	public static GameUIManager GetInstance()
	{
		return Instance;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void Update()
	{
		foreach (GameUIStatus mHiddenStatus in mHiddenStatusList)
		{
			if (mHiddenStatus != GameUIStatus.None)
			{
				GameUI uI = GetUI(mUiList, mHiddenStatus);
				if (uI != null)
				{
					uI.gameObject.SetActive(false);
				}
			}
		}
		mHiddenStatusList.Clear();
		foreach (GameUIStatus mRemovedStatus in mRemovedStatusList)
		{
			if (mRemovedStatus != GameUIStatus.None)
			{
				GameUI uI2 = GetUI(mUiList, mRemovedStatus);
				if (uI2 != null)
				{
					mUiList.Remove(uI2);
					Object.Destroy(uI2.gameObject);
				}
			}
		}
		mRemovedStatusList.Clear();
		for (int i = 0; i < mNextStatusList.Count; i++)
		{
			GameUIStatus gameUIStatus = mNextStatusList[i];
			GameUIBundle gameUIBundle = mNextGameUIBundleList[i];
			GameUIListener listener = mNextGameUIListenerList[i];
			if (mNextStatusList[0] == GameUIStatus.None)
			{
				continue;
			}
			GameUI uI3 = GetUI(mUiList, gameUIStatus);
			if (mUiList.Count >= 1 && mUiList[mUiList.Count - 1].Equals(uI3))
			{
				continue;
			}
			if (uI3 == null)
			{
				uI3 = CreateUI(gameUIStatus);
				uI3.InitUI(gameUIStatus);
				uI3.SetGameUIBundle(gameUIBundle);
				uI3.SetListener(listener);
				mUiList.Add(uI3);
				continue;
			}
			mUiList.Remove(uI3);
			if (!uI3.gameObject.activeSelf)
			{
				uI3.gameObject.SetActive(true);
			}
			uI3.SetGameUIBundle(gameUIBundle);
			mUiList.Add(uI3);
		}
		mNextStatusList.Clear();
		mNextGameUIBundleList.Clear();
	}

	private GameUI CreateUI(GameUIStatus gameUIStatus)
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		switch (gameUIStatus)
		{
		case GameUIStatus.UIRader:
			gameObject = ((!GameApp.GetInstance().GetGameMode().IsVSMode()) ? (Resources.Load("UI/HUD/MiniMap/Prefab/UIDemoMap") as GameObject) : (Resources.Load("UI/HUD/MiniMap/Prefab/PVPRadar") as GameObject));
			gameObject2 = Object.Instantiate(gameObject, Vector3.up * -1000f, Quaternion.identity) as GameObject;
			break;
		case GameUIStatus.HUD:
		{
			bool flag = false;
			float num = (float)Screen.width / (float)Screen.height;
			if (num <= 1.3833333f)
			{
				flag = true;
			}
			if (GameApp.GetInstance().GetGameMode().IsSingle() || GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				gameObject = ((!flag) ? (Resources.Load("UI/HUD/BattleHUD/Prefab/BattleHUD") as GameObject) : (Resources.Load("UI/HUD/BattleHUD/Prefab/BattleHUD_Pad") as GameObject));
				gameObject2 = Object.Instantiate(gameObject) as GameObject;
			}
			else if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				gameObject = ((!flag) ? (Resources.Load("UI/HUD/OnlineHUD/Prefab/OnlineHUD") as GameObject) : (Resources.Load("UI/HUD/OnlineHUD/Prefab/OnlineHUD_Pad") as GameObject));
				gameObject2 = Object.Instantiate(gameObject, Vector3.up * -1000f, Quaternion.identity) as GameObject;
			}
			break;
		}
		case GameUIStatus.StoryDialog:
			gameObject = Resources.Load("UI/HUD/FightDialog/Prefab/UIStoryDialog") as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
			break;
		}
		if (gameObject2 == null)
		{
			Debug.LogError(string.Concat("Can't find UI '", gameUIStatus, "'"));
			return null;
		}
		return gameObject2.GetComponent<GameUI>();
	}

	private GameUI GetUI(List<GameUI> list, GameUIStatus status)
	{
		foreach (GameUI item in list)
		{
			if (status == item.UIStatus)
			{
				return item;
			}
		}
		return null;
	}

	public void LoadUI(GameUIStatus status, GameUIBundle bundle, GameUIListener listener)
	{
		mNextStatusList.Add(status);
		mNextGameUIBundleList.Add(bundle);
		mNextGameUIListenerList.Add(listener);
	}

	public void LoadUI(GameUIStatus status, GameUIBundle bundle)
	{
		LoadUI(status, bundle, null);
	}

	public void LoadUI(GameUIStatus status)
	{
		LoadUI(status, null);
	}

	public void RemoveUI(GameUIStatus status)
	{
		mRemovedStatusList.Add(status);
	}

	public void RemoveUI(GameUI gameUI)
	{
		RemoveUI(gameUI.UIStatus);
	}

	public void HideUI(GameUIStatus status)
	{
		mHiddenStatusList.Add(status);
	}

	public void HideUI(GameUI gameUI)
	{
		HideUI(gameUI.UIStatus);
	}

	public GameUI GetGameUI(GameUIStatus status)
	{
		return GetUI(mUiList, status);
	}

	public void LoadHUD(HUDBase.HUDType hudType)
	{
		GameUIBundle gameUIBundle = new GameUIBundle();
		gameUIBundle.Add("HUDType", hudType);
		LoadUI(GameUIStatus.HUD, gameUIBundle);
	}

	public void LoadStoryDialog(List<UIDialogData> list)
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		UserState userState = GameApp.GetInstance().GetUserState();
		bool flag = true;
		if (gameWorld != null)
		{
			flag = gameWorld.GetFirstComplete();
		}
		bool skipDialog = userState.GetSkipDialog();
		if ((flag || !skipDialog) && userState.GetSelectedDifficultMode() == EDifficultMode.NORMAL && GameApp.GetInstance().GetGameMode().IsSingle())
		{
			GameUIBundle gameUIBundle = new GameUIBundle();
			gameUIBundle.Add("StoryDialog", list);
			LoadUI(GameUIStatus.StoryDialog, gameUIBundle);
		}
	}
}

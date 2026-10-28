using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour, UIMsgListener
{
	public const int STATISTIC_COUNT = 83;

	public static PlayerInfoUI mLocalInstance;

	public GameObject ExitButton;

	public GameObject StatisticPage;

	public GameObject TitlePage;

	public GameObject PhotoframePage;

	public GameObject TitleButton;

	public GameObject PrestigeInfoClose;

	public GameObject PrestigeButton;

	public GameObject PvpExpIapButton;

	public GameObject EditNameButton;

	public GameObject EditNamePage;

	public UILabel PlayerName;

	public UILabel PlayerTitle;

	public UILabel PlayerID;

	public UILabel LastLoginDate;

	public UILabel TotalPlayedTime;

	public UISprite RankIcon;

	public UISprite ExpForward;

	public UILabel RankLevel;

	public UILabel prestigeHint1;

	public GameObject PrestigeInfo;

	public GameObject Prestige;

	public UISprite PrestigeBg;

	public UISprite PrestigeLevel;

	public UILabel PrestigeCloseLabel;

	public UILabel[] PrestigeLevelInfoList;

	public UILabel MissionCompleteState;

	public UILabel WeaponCollectionState;

	public UILabel EquipCollectionState;

	public UILabel SwVoidCollectionState;

	public UIGrid DetailStatistics;

	public UIGrid TitleTransform;

	public GameObject StatisticTemplate;

	public GameObject TitleTemplate;

	public GameObject TitleSprite;

	public GameObject TitleExitButton;

	public GameObject FriendButton;

	public GameObject FriendButtonHighlight;

	public UILabel FriendButtonLabel;

	public GameObject AvatarFrame;

	public Camera Camera2D;

	public Camera Camera3D;

	public GameObject Portrait;

	public UITexture Head;

	public UISprite HeadFrame;

	public GameObject FrameGrid;

	public PvpExpIapUI PvpExpIap;

	private GameObject mModel;

	private Vector3 m_Rotation = new Vector3(0f, 128f, 0f);

	private Vector3 m_Pos;

	private Vector3 m_Scale = Vector3.one;

	private string mAnimationName;

	private bool mIsLocal;

	public static List<PlayerInfoUI> PlayerInfoUI_List = new List<PlayerInfoUI>();

	private AchievementState achievements;

	private UserState mUserstate;

	public List<UILabel> mStatistics = new List<UILabel>();

	private Dictionary<GameObject, int> TitleButtonList = new Dictionary<GameObject, int>();

	private GameObject SelectedTitle;

	private SocialPlayer mSocialPlayer;

	public GameObject HeadFramePage;

	public GameObject HeadFramePageExit;

	public List<GameObject> HeadFrames;

	private HeadFramePage mHeadFramePage;

	private List<UISprite> mHeadFrameSpriteList;

	private int mAccountID = -1;

	private bool canPrestigeLevelUp;

	private GameObject mAvatarObj;

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(ExitButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		FriendButtonLabel.text = LocalizationManager.GetInstance().GetString("LOC_INFO_ADDFRIEND");
		mIsLocal = false;
		int count = PlayerInfoUI_List.Count;
		if (count > 0)
		{
			PlayerInfoUI_List[count - 1].gameObject.SetActive(false);
		}
		PlayerInfoUI_List.Add(this);
		if (InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().ShareUICamera.enabled = false;
		}
		UIEventListener uIEventListener2 = UIEventListener.Get(FriendButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(Prestige);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(FriendButton);
		uIEventListener4.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener4.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener5 = UIEventListener.Get(PrestigeInfoClose);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(PrestigeButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(PvpExpIapButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(Portrait);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener9 = UIEventListener.Get(EditNameButton);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener10 = UIEventListener.Get(TitleButton);
		uIEventListener10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener10.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener11 = UIEventListener.Get(TitleExitButton);
		uIEventListener11.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener11.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener12 = UIEventListener.Get(HeadFramePageExit);
		uIEventListener12.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener12.onClick, new UIEventListener.VoidDelegate(OnClick));
		mHeadFramePage = base.gameObject.GetComponentsInChildren<HeadFramePage>(true)[0];
		TitleButton.GetComponent<Collider>().enabled = false;
		Portrait.GetComponent<Collider>().enabled = false;
		EditNameButton.SetActive(false);
		foreach (GameObject headFrame in HeadFrames)
		{
			UIEventListener uIEventListener13 = UIEventListener.Get(headFrame);
			uIEventListener13.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener13.onClick, new UIEventListener.VoidDelegate(OnClick));
			headFrame.SetActive(false);
		}
		int cameraDepth = 2;
		if (!GameApp.GetInstance().IsConnectedToServer() && FriendUI.mInstance == null)
		{
			cameraDepth = 7;
			PvpExpIapButton.SetActive(false);
			Prestige.GetComponent<Collider>().enabled = false;
			EditNameButton.SetActive(false);
			FriendButton.SetActive(false);
		}
		else
		{
			FriendButton.SetActive(GameApp.GetInstance().GetUserState().HasLoginOnline());
			if (FriendUI.mInstance != null)
			{
				FriendUI.mInstance.FriendUICamera.enabled = false;
				cameraDepth = Mathf.CeilToInt(FriendUI.mInstance.FriendUICamera.GetComponent<Camera>().depth + 1f);
			}
		}
		SetCameraDepth(cameraDepth);
	}

	private void Start()
	{
		StatisticPage.SetActive(true);
		TitlePage.SetActive(false);
		PhotoframePage.SetActive(false);
		TitleSprite.SetActive(false);
		HeadFramePage.SetActive(false);
	}

	private void OnEnable()
	{
		if (mModel != null)
		{
			mModel.GetComponent<Animation>().Play(mAnimationName);
		}
	}

	private void OnClick(GameObject go)
	{
		if (go == TitleButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			StatisticPage.SetActive(false);
			TitlePage.SetActive(true);
			PhotoframePage.SetActive(false);
			TitleSprite.SetActive(true);
			HeadFramePage.SetActive(false);
		}
		else if (go == EditNameButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			AvatarFrame.SetActive(false);
			EditNamePage.SetActive(true);
		}
		else if (TitleButtonList.ContainsKey(go))
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			foreach (GameObject key in TitleButtonList.Keys)
			{
				key.transform.Find("SelectedSprite").gameObject.SetActive(false);
			}
			go.transform.Find("SelectedSprite").gameObject.SetActive(true);
			int currentTitleId = TitleButtonList[go];
			mUserstate.SetCurrentTitleId(currentTitleId);
			RefreshTitle();
			GameApp.GetInstance().Save();
			StatisticPage.SetActive(true);
			TitlePage.SetActive(false);
			PhotoframePage.SetActive(false);
			TitleSprite.SetActive(false);
			HeadFramePage.SetActive(false);
		}
		else if (go == TitleExitButton || go == HeadFramePageExit)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			StatisticPage.SetActive(true);
			TitlePage.SetActive(false);
			PhotoframePage.SetActive(false);
			TitleSprite.SetActive(false);
			HeadFramePage.SetActive(false);
		}
		else if (HeadFrames.Contains(go))
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			HeadFrame.spriteName = go.GetComponent<UISprite>().spriteName;
			int result = 0;
			int.TryParse(go.name, out result);
			mUserstate.SetCurrentTitleFrameId(result);
			RefreshHeadFrame();
			mHeadFramePage.Head.transform.position = go.transform.position;
		}
		else if (go == Portrait)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			HeadFramePage.SetActive(true);
			TitlePage.SetActive(false);
			StatisticPage.SetActive(false);
			RefreshHeadFrame();
		}
	}

	private void OnDestroy()
	{
		PlayerInfoUI_List.Remove(this);
		int count = PlayerInfoUI_List.Count;
		if (count > 0)
		{
			PlayerInfoUI_List[count - 1].gameObject.SetActive(true);
		}
		if (FriendUI.mInstance != null)
		{
			FriendUI.mInstance.FriendUICamera.enabled = true;
		}
		if (InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().ShareUICamera.enabled = true;
		}
		if (mIsLocal)
		{
			mLocalInstance = null;
		}
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == ExitButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (GameApp.GetInstance().GetNetworkManager() != null)
			{
				UploadGlobalDataRequest request = new UploadGlobalDataRequest(GameApp.GetInstance().GetUserState());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			UnityEngine.Object.Destroy(base.gameObject);
			if (mIsLocal && LobbyScript.mInstance != null)
			{
				LobbyScript.mInstance.ShowLobbyUI();
			}
			if (!GameApp.GetInstance().IsConnectedToServer() && CustomizeUIScript.mInstance != null)
			{
				CustomizeUIScript.mInstance.gameObject.SetActive(true);
			}
			if (PVPResultPageUI.mInstance != null)
			{
				PVPResultPageUI.mInstance.SelfStatiticPage.AvatarFrame.SetActive(true);
				PVPResultPageUI.mInstance.SelfStatiticPage.ShowModels();
			}
		}
		else if (go == FriendButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mIsLocal)
			{
				GameObject original = Resources.Load("UI/Menu/FriendUI/Prefab/FriendUI") as GameObject;
				UnityEngine.Object.Instantiate(original);
			}
			else if (mAccountID > 0 && GameApp.GetInstance().GetUserState().HasLoginOnline())
			{
				SwFriendAddRequest request2 = new SwFriendAddRequest(mAccountID);
				GameApp.GetInstance().GetWWWManager().SendRequest(request2);
			}
		}
		else if (go == Prestige)
		{
			if (mIsLocal)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
				PrestigeInfo.SetActive(true);
				AvatarFrame.SetActive(false);
				if (canPrestigeLevelUp)
				{
					prestigeHint1.gameObject.SetActive(false);
					PrestigeButton.SetActive(true);
				}
				else
				{
					prestigeHint1.gameObject.SetActive(true);
					PrestigeButton.SetActive(false);
				}
			}
			else
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			}
		}
		else if (go == PrestigeInfoClose)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			PrestigeInfo.SetActive(false);
			AvatarFrame.SetActive(true);
			if (mAvatarObj != null && mAvatarObj.GetComponent<Animation>() != null)
			{
				mAvatarObj.GetComponent<Animation>().Play(mAnimationName);
			}
		}
		else if (go == PrestigeButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			PrestigeInfo.SetActive(false);
			AvatarFrame.SetActive(true);
			if (mAvatarObj != null && mAvatarObj.GetComponent<Animation>() != null)
			{
				mAvatarObj.GetComponent<Animation>().Play(mAnimationName);
			}
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_21"), 7);
			Debug.Log("Current Prestige = " + GameApp.GetInstance().GetUserState().GetOnlinePlayer()
				.GetPrestige());
			PlayerPrestigeRequest request3 = new PlayerPrestigeRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
		}
		else if (go == PvpExpIapButton && mIsLocal)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (PvpExpIap != null)
			{
				PvpExpIap.gameObject.SetActive(true);
				PvpExpIap.InitInfo("LOC_PVP_IAP_HINT0");
			}
		}
	}

	public void OnButtonPress(GameObject go, bool isPress)
	{
		if (go == FriendButton)
		{
			FriendButtonHighlight.SetActive(isPress);
		}
	}

	public void InitByUserState(UserState userState, bool isLocal)
	{
		if (!isLocal && GameApp.GetInstance().GetUserState().GetFriendDictionary()
			.ContainsKey(userState.GetOnlineUserId()))
		{
			InitByOnlineFriend(GameApp.GetInstance().GetUserState().GetFriendDictionary()[userState.GetOnlineUserId()]);
			return;
		}
		mIsLocal = isLocal;
		achievements = userState.Achievement;
		mUserstate = userState;
		CharacterShaderController.TempData = mUserstate.GetCurrentCustomize().mAvatarColorArray;
		if (isLocal)
		{
			mLocalInstance = this;
			mSocialPlayer = SocialManager.GetInstance().LocalSocialPlayer;
			TitleButton.GetComponent<Collider>().enabled = true;
			Portrait.GetComponent<Collider>().enabled = true;
			FriendButtonLabel.text = LocalizationManager.GetInstance().GetString("LOC_INFO_FRIENDLIST");
			if (userState.GetOnlineRegisted())
			{
				EditNameButton.SetActive(true);
			}
		}
		else if (!userState.DontMindIfRegisted())
		{
			FriendButton.SetActive(false);
		}
		short[] array = new short[6];
		short[] avatar = userState.GetAvatar();
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		Weapon weapon = userState.GetBattleWeapons()[0];
		array[5] = (short)weapon.GunID;
		ChangeAvatar(array, false);
		PlayerName.text = userState.GetOnlineNickname();
		if (isLocal)
		{
			PlayerID.gameObject.SetActive(false);
			AvatarFrame.transform.localPosition = new Vector3(AvatarFrame.transform.localPosition.x, -127f, AvatarFrame.transform.localPosition.z);
		}
		else
		{
			PlayerID.text = "ID: " + userState.GetOnlineUserId();
			PlayerID.gameObject.SetActive(true);
		}
		SetExp(userState.GetExp(), userState.GetOnlinePlayer().GetPrestige());
		RefreshTitle();
		RefreshHeadFrame();
		int currentTilteFrameID = mUserstate.GetCurrentTilteFrameID();
		mHeadFramePage.Head.transform.position = HeadFrames[currentTilteFrameID].transform.position;
		if (mIsLocal)
		{
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ACHIEVEMENT, DataSheetName.STATISTICS);
			for (int j = 0; j < 83; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(StatisticTemplate) as GameObject;
				gameObject.transform.parent = DetailStatistics.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponentsInChildren<UILabel>()[0].text = gameDataTable.GetString(j, 1);
				gameObject.GetComponentInChildren<UILocalizationX>().enabled = true;
				UILabel uILabel = gameObject.GetComponentsInChildren<UILabel>()[1];
				if (gameDataTable.GetString(j, 4) != string.Empty)
				{
					SkillsType sType = (SkillsType)(int)Enum.Parse(typeof(SkillsType), gameDataTable.GetString(j, 4));
					string text = gameDataTable.GetString(j, 5);
					if (text == "PERCENT")
					{
						uILabel.text = (userState.playerSkill.GetSkill(sType) * 100f).ToString() + '%';
					}
					else if (text == "INTEGER")
					{
						uILabel.text = userState.playerSkill.GetSkill(sType).ToString();
					}
				}
				mStatistics.Add(gameObject.GetComponentsInChildren<UILabel>()[1]);
			}
		}
		else
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(StatisticTemplate) as GameObject;
			gameObject2.transform.parent = DetailStatistics.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.GetComponentsInChildren<UILabel>()[0].overflowMethod = UILabel.Overflow.ResizeHeight;
			gameObject2.GetComponentsInChildren<UILabel>()[0].width = 288;
			gameObject2.GetComponentsInChildren<UILabel>()[0].text = LocalizationManager.GetInstance().GetString("LOC_STATISTICS_0");
			gameObject2.GetComponentsInChildren<UILabel>()[1].text = string.Empty;
		}
		DetailStatistics.GetComponent<UIGrid>().Reposition();
		SetLastLoginTime();
		SetMissionCompleteStatus();
		SetWeaponAndArmor();
		if (!isLocal)
		{
			return;
		}
		foreach (int title in userState.Achievement.GetTitleList())
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(TitleTemplate) as GameObject;
			gameObject3.transform.parent = TitleTransform.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localScale = Vector3.one;
			gameObject3.GetComponentsInChildren<UILabel>(true)[0].text = userState.TitleDictionary[title];
			gameObject3.GetComponentsInChildren<UILocalizationX>(true)[0].enabled = true;
			TitleButtonList.Add(gameObject3, title);
			if (userState.GetCurrentTitleID() == title)
			{
				gameObject3.transform.Find("SelectedSprite").gameObject.SetActive(true);
			}
			UIEventListener uIEventListener = UIEventListener.Get(gameObject3);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClick));
			if (title < HeadFrames.Count)
			{
				HeadFrames[title].SetActive(true);
			}
		}
		FrameGrid.GetComponent<UIGrid>().Reposition();
		TitleTransform.GetComponent<UIGrid>().Reposition();
		SetPerkNum();
		SetAchievementComplete();
		SetFavoriteWeapon();
		SetFavoriteMission();
		SetFavoritePVPMode();
		SetCashAndMat();
		SetMaxDamage();
		SetMaxGS();
		SetDeathNum();
		SetPVPStatus();
		SetWeaponStatus();
		SetKillEnemyStatus();
		SetPotionUsed();
	}

	public void InitByOnlineFriend(OnlineFriend friend)
	{
		CharacterShaderController.TempData = friend.GetCustomize().mAvatarColorArray;
		short[] array = new short[6];
		short[] mAvatarIdArray = friend.GetCustomize().mAvatarIdArray;
		for (int i = 0; i < mAvatarIdArray.Length; i++)
		{
			array[i] = mAvatarIdArray[i];
		}
		Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[friend.GetCustomize().GetFirstWeaponID()];
		array[5] = (short)weapon.GunID;
		ChangeAvatar(array, false);
		SetExp(friend.GetExp(), friend.GetPrestige());
		PlayerName.text = friend.GetName();
		PlayerID.text = "ID: " + friend.GetUserId();
		PlayerID.gameObject.SetActive(true);
		PlayerTitle.text = GameApp.GetInstance().GetUserState().TitleDictionary[friend.GetTitleId()];
		PlayerTitle.gameObject.GetComponent<UILocalizationX>().Refresh();
		int frameType = friend.GetFrameType();
		HeadFrame.spriteName = "head_" + string.Format("{0:00}", frameType);
		int prestige = friend.GetPrestige();
		HeadFrame.color = Global.HeadFrameColor[prestige];
		mSocialPlayer = friend.SocialPlayer;
		SocialManager.GetInstance().LoadSocialData(mSocialPlayer);
		PlayerSkill friendSkill = friend.GetFriendSkill();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ACHIEVEMENT, DataSheetName.STATISTICS);
		for (int j = 0; j < 83; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(StatisticTemplate) as GameObject;
			gameObject.transform.parent = DetailStatistics.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponentsInChildren<UILabel>()[0].text = gameDataTable.GetString(j, 1);
			gameObject.GetComponentInChildren<UILocalizationX>().enabled = true;
			UILabel uILabel = gameObject.GetComponentsInChildren<UILabel>()[1];
			if (gameDataTable.GetString(j, 4) != string.Empty)
			{
				SkillsType sType = (SkillsType)(int)Enum.Parse(typeof(SkillsType), gameDataTable.GetString(j, 4));
				string text = gameDataTable.GetString(j, 5);
				if (text == "PERCENT")
				{
					uILabel.text = (friendSkill.GetSkill(sType) * 100f).ToString() + '%';
				}
				else if (text == "INTEGER")
				{
					uILabel.text = friendSkill.GetSkill(sType).ToString();
				}
			}
			mStatistics.Add(gameObject.GetComponentsInChildren<UILabel>()[1]);
		}
		DetailStatistics.GetComponent<UIGrid>().Reposition();
		achievements = friend.GetAchievement();
		SetFourBlocksByFriend(friend);
		SetFriendPerkNum(friend);
		SetAchievementComplete();
		SetFavoriteWeapon();
		SetFavoriteMission();
		SetFavoritePVPMode();
		SetCashAndMat();
		SetMaxDamage();
		SetMaxGS();
		SetDeathNum();
		SetPVPStatus();
		SetFriendWeaponStatus(friend);
		SetKillEnemyStatus();
		SetPotionUsed();
		FriendButton.SetActive(false);
	}

	public void InitByRoomPlayer(RoomPlayer roomPlayer)
	{
		if (GameApp.GetInstance().GetUserState().GetFriendDictionary()
			.ContainsKey(roomPlayer.AccountId))
		{
			InitByOnlineFriend(GameApp.GetInstance().GetUserState().GetFriendDictionary()[roomPlayer.AccountId]);
			return;
		}
		SetAccountID(roomPlayer.AccountId);
		if (!roomPlayer.GetIsRegisted())
		{
			FriendButton.SetActive(false);
		}
		CharacterShaderController.TempData = roomPlayer.CustomizeData.mAvatarColorArray;
		short[] array = new short[6];
		short[] mAvatarIdArray = roomPlayer.CustomizeData.mAvatarIdArray;
		for (int i = 0; i < mAvatarIdArray.Length; i++)
		{
			array[i] = mAvatarIdArray[i];
		}
		Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[roomPlayer.CustomizeData.GetFirstWeaponID()];
		array[5] = (short)weapon.GunID;
		ChangeAvatar(array, false);
		SetExp(roomPlayer.EXP, roomPlayer.GetPrestige());
		PlayerName.text = roomPlayer.getPlayerName();
		PlayerID.text = "ID: " + roomPlayer.AccountId;
		PlayerID.gameObject.SetActive(true);
		PlayerTitle.text = GameApp.GetInstance().GetUserState().TitleDictionary[roomPlayer.GetTitleId()];
		PlayerTitle.gameObject.GetComponent<UILocalizationX>().Refresh();
		int frameType = roomPlayer.GetFrameType();
		HeadFrame.spriteName = "head_" + string.Format("{0:00}", frameType);
		int prestige = roomPlayer.GetPrestige();
		HeadFrame.color = Global.HeadFrameColor[prestige];
		mSocialPlayer = roomPlayer.SocialPlayer;
		SocialManager.GetInstance().LoadSocialData(mSocialPlayer);
		SetFourBlocksByRoomPlayer(roomPlayer);
		GameObject gameObject = UnityEngine.Object.Instantiate(StatisticTemplate) as GameObject;
		gameObject.transform.parent = DetailStatistics.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponentsInChildren<UILabel>()[0].overflowMethod = UILabel.Overflow.ResizeHeight;
		gameObject.GetComponentsInChildren<UILabel>()[0].width = 288;
		gameObject.GetComponentsInChildren<UILabel>()[0].text = LocalizationManager.GetInstance().GetString("LOC_STATISTICS_0");
		gameObject.GetComponentsInChildren<UILabel>()[1].text = string.Empty;
		DetailStatistics.GetComponent<UIGrid>().Reposition();
	}

	private void SetExp(int exp, int prestigeLevel)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		List<Rank> rankList = userState.GetRankList();
		int rankID = userState.GetRank(exp).rankID;
		RankIcon.spriteName = Rank.GetRankIconName(rankID);
		RankLevel.text = Rank.GetRankLevelString(rankID);
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_PERK_SIZE);
		int num = gameDataTable.GetInt(gameDataTable.Count - 2, 0);
		canPrestigeLevelUp = false;
		PrestigeBg.spriteName = "turn_0";
		if (prestigeLevel < num)
		{
			PrestigeLevel.spriteName = "alive_" + (prestigeLevel + 1);
			prestigeHint1.text = LocalizationManager.GetInstance().GetString("LOC_Prestige_hint1");
		}
		else
		{
			PrestigeLevel.spriteName = "alive_" + prestigeLevel;
			PrestigeBg.spriteName = "turn";
			prestigeHint1.text = LocalizationManager.GetInstance().GetString("LOC_ERROR_CODE_303");
		}
		for (int i = 0; i < PrestigeLevelInfoList.Length; i++)
		{
			if (prestigeLevel == i)
			{
				PrestigeLevelInfoList[i].color = Color.yellow;
			}
			else if (prestigeLevel > i)
			{
				PrestigeLevelInfoList[i].color = Color.cyan;
			}
			else
			{
				PrestigeLevelInfoList[i].color = Color.gray;
			}
		}
		if (rankID >= rankList.Count)
		{
			ExpForward.fillAmount = 1f;
			PvpExpIapButton.SetActive(false);
			if (mIsLocal && userState.GetOnlinePlayer().GetPrestige() < num)
			{
				canPrestigeLevelUp = true;
				PrestigeBg.spriteName = "turn";
			}
		}
		else
		{
			int exp2 = rankList[rankID - 1].exp;
			int exp3 = rankList[rankID].exp;
			PvpExpIapButton.SetActive(true);
			ExpForward.fillAmount = (float)(exp - exp2) / (float)(exp3 - exp2);
		}
	}

	public void ChangeAvatar(short[] avatar, bool isChangeWeapon)
	{
		ClearModels();
		Armor armor = GameApp.GetInstance().GetUserState().GetArmor(4, avatar[4]);
		bool flag = ((armor.GetSkill(SkillsType.FLY) > 0f) ? true : false);
		mAvatarObj = AvatarBuilder.GetInstance().CreateUIAvatar(avatar);
		mAvatarObj.transform.rotation = Quaternion.Euler(m_Rotation);
		Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[avatar[5]];
		Transform parent = mAvatarObj.transform.Find(BoneName.Weapon);
		if (weapon.GetWeaponType() == WeaponType.BowSword)
		{
			parent = mAvatarObj.transform.Find(BoneName.WeaponL);
		}
		string path = Global.WEAPON_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", avatar[5]);
		UnityEngine.Object original = Resources.Load(path);
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.AddComponent<MaterialChanger>();
		gameObject.GetComponent<MaterialChanger>().SetMaterialInfo();
		if (weapon.GetWeaponType() == WeaponType.DoublePistol || weapon.GetWeaponType() == WeaponType.ChargeDoublePistol)
		{
			Transform parent2 = mAvatarObj.transform.Find(BoneName.WeaponL);
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(original);
			gameObject2.transform.parent = parent2;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.AddComponent<MaterialChanger>();
			gameObject2.GetComponent<MaterialChanger>().SetMaterialInfo();
		}
		mAvatarObj.transform.localScale = m_Scale;
		SetGameObject(mAvatarObj);
		mAnimationName = AnimationString.Shop + "_" + weapon.GetAnimationSuffix() + "_" + AnimationString.Idle;
		mAvatarObj.GetComponent<Animation>()[mAnimationName].wrapMode = WrapMode.Loop;
		mAvatarObj.GetComponent<Animation>().Play(mAnimationName);
		SetModel(mAvatarObj);
		mAvatarObj.transform.parent = AvatarFrame.transform;
		mAvatarObj.transform.localPosition = Vector3.zero;
		mAvatarObj.transform.localRotation = Quaternion.Euler(m_Rotation);
		mAvatarObj.transform.localScale = m_Scale;
	}

	public void SetGameObject(GameObject playerObj)
	{
		SetLowerBodyAnimation(playerObj, "assault");
		SetLowerBodyAnimation(playerObj, "RPG");
		SetLowerBodyAnimation(playerObj, "shotgun");
		SetLowerBodyAnimation(playerObj, "fist");
	}

	public void SetLowerBodyAnimation(GameObject playerObj, string weaponSuffix)
	{
		if (playerObj.GetComponent<Animation>()[AnimationString.Shop + "_" + weaponSuffix + "_" + AnimationString.Idle] != null)
		{
			playerObj.GetComponent<Animation>()[AnimationString.Shop + "_" + weaponSuffix + "_" + AnimationString.Idle].wrapMode = WrapMode.Loop;
		}
	}

	public void ClearModels()
	{
		if (mModel != null)
		{
			UnityEngine.Object.Destroy(mModel);
			mModel = null;
		}
	}

	public void HideModels()
	{
		if (mModel != null)
		{
			mModel.SetActive(false);
		}
	}

	public void ShowModels()
	{
		if (mModel != null)
		{
			mModel.SetActive(true);
			mModel.GetComponent<Animation>().Play(mAnimationName);
		}
	}

	public void SetModel(GameObject obj)
	{
		mModel = obj;
		mModel.transform.position = m_Pos;
	}

	public GameObject GetModel()
	{
		return mModel;
	}

	private void SetCameraDepth(int depth)
	{
		Camera2D.depth = depth;
		Camera3D.depth = depth + 1;
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		byte eventId = whichMsg.EventId;
		if (eventId == 70)
		{
			switch (buttonId)
			{
			case UIMsg.UIMsgButton.Ok:
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_21"), 7);
				Debug.Log("Current Prestige = " + GameApp.GetInstance().GetUserState().GetOnlinePlayer()
					.GetPrestige());
				PlayerPrestigeRequest request = new PlayerPrestigeRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				break;
			}
			case UIMsg.UIMsgButton.Cancel:
				UIMsgBox.instance.CloseMessage();
				break;
			}
		}
	}

	public void PrestigeComplete(byte prestige, int exp)
	{
		GameApp.GetInstance().GetUserState().GetOnlinePlayer()
			.SetPrestige(prestige);
		GameApp.GetInstance().GetUserState().GetOnlinePlayer()
			.SetExp(exp);
		SetExp(exp, prestige);
		RefreshHeadFrame();
		Debug.Log("New Prestige = " + prestige);
	}

	private void SetFourBlocksByFriend(OnlineFriend friend)
	{
		LastLoginDate.text = string.Empty;
		if (friend.GetLastLoginTime().Ticks > 0)
		{
			LastLoginDate.text = friend.GetLastLoginTime().ToString("yyyy/MM/dd");
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(friend.GetTotalPlayTime());
		Debug.Log("time = " + timeSpan.ToString());
		TotalPlayedTime.text = string.Format("{0:G}:{1:D2}", timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes);
		MissionCompleteState.text = friend.GetMissionNum() + "/" + Global.TOTAL_ACTIVATED_STAGE * (Global.TOTAL_SUB_STAGE - 2) * 2;
		WeaponCollectionState.text = friend.GetWeaponNum().ToString() + '/' + 33.ToString();
		EquipCollectionState.text = friend.GetEquipNum().ToString() + '/' + (Global.TOTAL_ARMOR_NUM * 4 + Global.TOTAL_ARMOR_BAG_NUM).ToString();
		SwVoidCollectionState.text = friend.GetSwVoidNum().ToString() + '/' + (Global.VOID_TYPE_NUM * 2).ToString();
	}

	private void SetFourBlocksByRoomPlayer(RoomPlayer player)
	{
		LastLoginDate.text = string.Empty;
		if (player.GetLastLoginTime().Ticks > 0)
		{
			LastLoginDate.text = player.GetLastLoginTime().ToString("yyyy/MM/dd");
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(player.GetTotalPlayTime());
		Debug.Log("time = " + timeSpan.ToString());
		TotalPlayedTime.text = string.Format("{0:G}:{1:D2}", timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes);
		MissionCompleteState.text = player.GetMissionNum() + "/" + Global.TOTAL_ACTIVATED_STAGE * (Global.TOTAL_SUB_STAGE - 2) * 2;
		WeaponCollectionState.text = player.GetWeaponNum().ToString() + '/' + 33.ToString();
		EquipCollectionState.text = player.GetEquipNum().ToString() + '/' + (Global.TOTAL_ARMOR_NUM * 4 + Global.TOTAL_ARMOR_BAG_NUM).ToString();
		SwVoidCollectionState.text = player.GetSwVoidNum().ToString() + '/' + (Global.VOID_TYPE_NUM * 2).ToString();
	}

	private void SetLastLoginTime()
	{
		LastLoginDate.text = string.Empty;
		if (mUserstate.GetSyncRecordTime().Ticks > 0)
		{
			LastLoginDate.text = mUserstate.GetSyncRecordTime().ToString("yyyy/MM/dd");
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(mUserstate.GetTotalPlayTime());
		Debug.Log("time = " + timeSpan.ToString());
		TotalPlayedTime.text = string.Format("{0:G}:{1:D2}", timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes);
	}

	private void SetMissionCompleteStatus()
	{
		if (mIsLocal)
		{
			int unlockedStageId = mUserstate.GetUnlockedStageId(EDifficultMode.NORMAL);
			int unlockedSubStageId = mUserstate.GetUnlockedSubStageId(EDifficultMode.NORMAL);
			int unlockedStageId2 = mUserstate.GetUnlockedStageId(EDifficultMode.HEROIC);
			int unlockedSubStageId2 = mUserstate.GetUnlockedSubStageId(EDifficultMode.HEROIC);
			EDifficultMode unlockedDifficultMode = mUserstate.GetUnlockedDifficultMode();
			int num = unlockedStageId * (Global.TOTAL_SUB_STAGE - 2) + unlockedSubStageId - 1;
			if (unlockedDifficultMode == EDifficultMode.HEROIC)
			{
				num += unlockedStageId2 * (Global.TOTAL_SUB_STAGE - 2) + unlockedSubStageId2 - 1;
			}
			MissionCompleteState.text = num.ToString() + '/' + (Global.TOTAL_ACTIVATED_STAGE * (Global.TOTAL_SUB_STAGE - 2) * 2).ToString();
		}
		else
		{
			MissionCompleteState.text = mUserstate.GetMissionNum().ToString() + '/' + (Global.TOTAL_ACTIVATED_STAGE * (Global.TOTAL_SUB_STAGE - 2) * 2).ToString();
		}
	}

	private void SetWeaponAndArmor()
	{
		if (mIsLocal)
		{
			Dictionary<int, Weapon> weapons = mUserstate.GetWeapons();
			int num = 0;
			for (int i = 1; i <= 33; i++)
			{
				if (weapons[i].Level != 15 && weapons[i].Level != 0)
				{
					num++;
				}
			}
			WeaponCollectionState.text = num.ToString() + '/' + 33.ToString();
			List<List<Armor>> armor = mUserstate.GetArmor();
			num = 0;
			for (int j = 0; j <= 3; j++)
			{
				for (int k = 0; k < Global.TOTAL_ARMOR_NUM; k++)
				{
					if (armor[j][k].Level != 15 && armor[j][k].Level != 0)
					{
						num++;
					}
				}
			}
			List<Armor> list = mUserstate.GetArmor()[4];
			for (int l = 0; l < Global.TOTAL_ARMOR_BAG_NUM; l++)
			{
				if (list[l].Level != 15 && list[l].Level != 0)
				{
					num++;
				}
			}
			EquipCollectionState.text = num.ToString() + '/' + (Global.TOTAL_ARMOR_NUM * 4 + Global.TOTAL_ARMOR_BAG_NUM).ToString();
			Dictionary<ESwVoidType, int> dictionary = new Dictionary<ESwVoidType, int>();
			foreach (SwVoidSavedata value in mUserstate.swVoidList.Values)
			{
				if (!dictionary.ContainsKey(value.GetType()))
				{
					if (value.GetLevel() >= 3)
					{
						dictionary.Add(value.GetType(), 2);
					}
					else
					{
						dictionary.Add(value.GetType(), 1);
					}
				}
				else if (value.GetLevel() >= 3 && dictionary[value.GetType()] == 1)
				{
					dictionary.Remove(value.GetType());
					dictionary.Add(value.GetType(), 2);
				}
			}
			int num2 = 0;
			foreach (int value2 in dictionary.Values)
			{
				num2 += value2;
			}
			SwVoidCollectionState.text = num2.ToString() + '/' + (Global.VOID_TYPE_NUM * 2).ToString();
		}
		else
		{
			WeaponCollectionState.text = mUserstate.GetWeaponNum().ToString() + '/' + 33.ToString();
			EquipCollectionState.text = mUserstate.GetEquipNum().ToString() + '/' + (Global.TOTAL_ARMOR_NUM * 4 + Global.TOTAL_ARMOR_BAG_NUM).ToString();
			SwVoidCollectionState.text = mUserstate.GetSwVoidNum().ToString() + '/' + (Global.VOID_TYPE_NUM * 2).ToString();
		}
	}

	private void SetPerkNum()
	{
		List<PvpSkill> allPerk = mUserstate.GetPvpSkillManager().GetAllPerk();
		int num = 0;
		foreach (PvpSkill item in allPerk)
		{
			if (mUserstate.GetRank().rankID >= item.GetUnlockLevel())
			{
				num++;
			}
		}
		mStatistics[0].text = num.ToString();
	}

	private void SetFriendPerkNum(OnlineFriend friend)
	{
		List<PvpSkill> allPerk = GameApp.GetInstance().GetUserState().GetPvpSkillManager()
			.GetAllPerk();
		UserState userState = GameApp.GetInstance().GetUserState();
		int rankID = userState.GetRank(friend.GetExp()).rankID;
		int num = 0;
		foreach (PvpSkill item in allPerk)
		{
			if (rankID >= item.GetUnlockLevel())
			{
				num++;
			}
		}
		mStatistics[0].text = num.ToString();
	}

	private void SetAchievementComplete()
	{
		int num = 0;
		AchievementInfo[] array = achievements.GetAchievements();
		foreach (AchievementInfo achievementInfo in array)
		{
			if (achievementInfo.complete)
			{
				num++;
			}
		}
		mStatistics[1].text = num.ToString();
	}

	private void SetFavoriteWeapon()
	{
		int[] weaponUsed = achievements.GetWeaponUsed(0);
		int[] weaponUsed2 = achievements.GetWeaponUsed(1);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < 33; i++)
		{
			if (weaponUsed[i] + weaponUsed2[i] > num)
			{
				num = weaponUsed[i] + weaponUsed2[i];
				num2 = i;
			}
			if (weaponUsed[i] > num3)
			{
				num3 = weaponUsed[i];
				num4 = i;
			}
		}
		mStatistics[2].text = GameApp.GetInstance().GetUserState().GetWeapons()[num2 + 1].Name;
		mStatistics[2].GetComponent<UILocalizationX>().enabled = true;
		mStatistics[33].text = GameApp.GetInstance().GetUserState().GetWeapons()[num2 + 1].Name;
		mStatistics[33].GetComponent<UILocalizationX>().enabled = true;
		mStatistics[34].text = achievements.GetPVEMaxCombo().ToString();
	}

	private void SetFavoriteMission()
	{
		int tOTAL_STAGE = Global.TOTAL_STAGE;
		int tOTAL_SUB_STAGE = Global.TOTAL_SUB_STAGE;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int[] missionPlayed = achievements.GetMissionPlayed();
		int[] missionCompleted = achievements.GetMissionCompleted();
		int[] heroMissionPlayed = achievements.GetHeroMissionPlayed();
		int[] heroMissionCompleted = achievements.GetHeroMissionCompleted();
		for (int i = 0; i < tOTAL_STAGE * tOTAL_SUB_STAGE; i++)
		{
			num5 += missionCompleted[i] + heroMissionCompleted[i];
			if (missionCompleted[i] + heroMissionCompleted[i] > num3)
			{
				num3 = missionCompleted[i] + heroMissionCompleted[i];
				num4 = i;
			}
			if (i % tOTAL_SUB_STAGE == 0 && missionPlayed[i] + heroMissionCompleted[i] > num)
			{
				num = missionPlayed[i] + heroMissionCompleted[i];
				num2 = i;
			}
		}
		mStatistics[3].text = num2 / tOTAL_SUB_STAGE + 1 + " - 0";
		mStatistics[23].text = num4 / tOTAL_SUB_STAGE + 1 + " - " + num4 % tOTAL_SUB_STAGE;
		mStatistics[24].text = num5.ToString();
	}

	private void SetFavoritePVPMode()
	{
		int num = 0;
		int value = 0;
		int num2 = 0;
		int value2 = 0;
		int num3 = 0;
		int num4 = 0;
		float num5 = 0f;
		int[] vSModePlayed = achievements.GetVSModePlayed();
		int[] vSModeWins = achievements.GetVSModeWins();
		for (int i = 0; i < 6; i++)
		{
			if (vSModePlayed[i] > num)
			{
				num = vSModePlayed[i];
				value = i;
			}
			if (vSModeWins[i] > num2 && i != 5)
			{
				num2 = vSModeWins[i];
				value2 = i;
			}
			if (i != 5)
			{
				num3 += vSModeWins[i];
				num4 += vSModePlayed[i];
			}
		}
		if (num4 != 0)
		{
			num5 = (float)num3 / (float)num4;
		}
		mStatistics[19].text = ((AchievementVSMode)(int)Enum.ToObject(typeof(AchievementVSMode), value2)).ToString();
		mStatistics[4].text = ((AchievementVSMode)(int)Enum.ToObject(typeof(AchievementVSMode), value)).ToString();
		mStatistics[17].text = (Mathf.Round(num5 * 10000f) / 100f).ToString() + '%';
	}

	private void SetCashAndMat()
	{
		mStatistics[5].text = achievements.GetTotalCash().ToString();
		mStatistics[6].text = achievements.GetVoidMat().ToString();
	}

	private void SetMaxDamage()
	{
		mStatistics[7].text = achievements.GetMaxDamage().ToString();
	}

	private void SetMaxGS()
	{
		mStatistics[8].text = LocalizationManager.GetInstance().GetString("LOC_GS_COUNT");
	}

	private void SetDeathNum()
	{
		mStatistics[9].text = achievements.GetPVEDeath().ToString();
		mStatistics[10].text = achievements.GetPVPDeath().ToString();
	}

	private void SetPVPStatus()
	{
		mStatistics[35].text = achievements.GetDoubleKill().ToString();
		mStatistics[36].text = achievements.GetTripleKill().ToString();
		mStatistics[11].text = achievements.GetPlayerKills().ToString();
		mStatistics[12].text = achievements.GetFlagNum().ToString();
		mStatistics[14].text = achievements.GetFBNum().ToString();
		mStatistics[13].text = achievements.GetOccupationNum().ToString();
		mStatistics[15].text = achievements.GetComboKillNum().ToString();
		mStatistics[18].text = achievements.GetVipCount().ToString();
		mStatistics[37].text = achievements.GetWeaponTypeWins(1)[0].ToString();
		mStatistics[38].text = achievements.GetWeaponTypeWins(1)[1].ToString();
		mStatistics[39].text = achievements.GetWeaponTypeWins(1)[3].ToString();
		mStatistics[41].text = achievements.GetWeaponTypeWins(1)[4].ToString();
		mStatistics[40].text = achievements.GetWeaponTypeWins(1)[5].ToString();
		mStatistics[42].text = achievements.GetWeaponTypeWins(1)[2].ToString();
		for (int i = 0; i <= 4; i++)
		{
			mStatistics[44 + i].text = achievements.GetVSModeWins()[i].ToString();
		}
		mStatistics[43].text = achievements.GetMaxComboWins().ToString();
	}

	private void SetWeaponStatus()
	{
		float num = 0f;
		if (achievements.GetAttackCount() != 0)
		{
			num = (float)achievements.GetHitEnemyCount() / (float)achievements.GetAttackCount() * 100f;
		}
		mStatistics[16].text = num.ToString("#0.00") + '%';
		mStatistics[49].text = mUserstate.GetBattleWeapons()[0].Damage.ToString();
		mStatistics[50].text = (1f / mUserstate.GetBattleWeapons()[0].AttackFrequency).ToString("#0.00") + "/s";
	}

	private void SetFriendWeaponStatus(OnlineFriend friend)
	{
		float num = 0f;
		if (achievements.GetAttackCount() != 0)
		{
			num = (float)achievements.GetHitEnemyCount() / (float)achievements.GetAttackCount() * 100f;
		}
		mStatistics[16].text = num.ToString("#0.00") + '%';
		Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[friend.GetCustomize().GetFirstWeaponID()];
		mStatistics[49].text = weapon.Damage.ToString();
		mStatistics[50].text = (1f / weapon.AttackFrequency).ToString("#0.00") + "/s";
	}

	private void SetKillEnemyStatus()
	{
		int num = 0;
		for (int i = 0; i < achievements.GetNormalEnemyKills().Length; i++)
		{
			num += achievements.GetNormalEnemyKills()[i];
		}
		mStatistics[20].text = num.ToString();
		num = 0;
		for (int j = 0; j < achievements.GetEliteEnemyKills().Length; j++)
		{
			num += achievements.GetEliteEnemyKills()[j];
		}
		mStatistics[21].text = num.ToString();
		num = 0;
		for (int k = 0; k < Global.TOTAL_BOSS_STAGE - 1; k++)
		{
			num += achievements.GetBossKills()[k];
			mStatistics[25 + k].text = achievements.GetBossKills()[k].ToString();
		}
		mStatistics[22].text = num.ToString();
	}

	private void SetPotionUsed()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int[] flaskUsed = achievements.GetFlaskUsed();
		int[] itemUsed = achievements.GetItemUsed();
		for (int i = 0; i < flaskUsed.Length; i++)
		{
			num4 += flaskUsed[i];
			if (flaskUsed[i] > flaskUsed[num2])
			{
				num2 = i;
			}
		}
		for (int j = 0; j < itemUsed.Length; j++)
		{
			num3 += itemUsed[j];
			if (itemUsed[j] > itemUsed[num])
			{
				num = j;
			}
		}
		mStatistics[29].text = num3.ToString();
		mStatistics[30].text = num4.ToString();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.SHOP_ITEMS);
		mStatistics[31].text = LocalizationManager.GetInstance().GetString(gameDataTable.GetString(num + 12, 1));
		string text = LocalizationManager.GetInstance().GetString(gameDataTable.GetString(num2 * 3, 1));
		int num5 = text.IndexOf('(');
		if (num5 < 0)
		{
			num5 = text.IndexOf('（');
		}
		text = text.Substring(0, num5);
		mStatistics[32].text = text;
	}

	public void RefreshTitle()
	{
		PlayerTitle.text = mUserstate.GetPlayerTitle();
		PlayerTitle.gameObject.GetComponent<UILocalizationX>().Refresh();
	}

	private void Update()
	{
		SocialManager.UpdateHead(mSocialPlayer, Head);
		SocialManager.UpdateHead(mSocialPlayer, mHeadFramePage.Head);
	}

	private void RefreshHeadFrame()
	{
		if (mHeadFrameSpriteList == null)
		{
			mHeadFrameSpriteList = new List<UISprite>();
			foreach (GameObject headFrame in HeadFrames)
			{
				mHeadFrameSpriteList.Add(headFrame.GetComponent<UISprite>());
			}
		}
		GameWorld.RefreshHeadFrame(HeadFrame);
		foreach (UISprite mHeadFrameSprite in mHeadFrameSpriteList)
		{
			GameWorld.RefreshHeadFrameColor(mHeadFrameSprite);
		}
	}

	public void SetAccountID(int accoutID)
	{
		mAccountID = accoutID;
		if (GameApp.GetInstance().GetUserState().GetFriendDictionary()
			.ContainsKey(accoutID))
		{
			FriendButton.SetActive(false);
		}
	}
}

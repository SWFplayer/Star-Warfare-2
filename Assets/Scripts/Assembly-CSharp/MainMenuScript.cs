using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainMenuScript : MonoBehaviour, UIMsgListener
{
	public struct PhotoInfo
	{
		public string fileName;

		public long time;
	}

	public static MainMenuScript mInstance;

	public Material AtlasMaterial;

	public Material AtlasMaterial_Runtime;

	public GameObject AvatarPosition;

	public GameObject CampaignCollision;

	public GameObject MultiplayCollision;

	public GameObject GameCenterCollision;

	public GameObject CreditsCollision;

	public GameObject OptionCollision;

	public GameObject SwitchCollision;

	public float[] CampaignLightsIntensity;

	public Light[] CampaignLightsFront;

	public Light[] CampaignLightsUp;

	public Light[] CampaignLightsDown;

	public float[] MultiLightsIntensity;

	public Light[] MultiLightsFront;

	public Light[] MultiLightsUp;

	public Light[] MultiLightsDown;

	public GameObject LeftAnimationObject;

	public GameObject RightAnimationObject;

	public AnimationClip LeftDoorAnimation;

	public AnimationClip RightDoorAnimation;

	public GameObject EffectRoot;

	public NetworkManager networkMgr;

	private float mWaitToLoadDelay = 0.83f;

	private float mWaitToLoadTime = 2f;

	private float mStartConnectToGameServerTime;

	private float mClickTime;

	private bool mCampaignClicked;

	private bool mMultiplayClicked;

	private string mLoadLevelName = string.Empty;

	private bool mDelayPassed;

	private bool mStartConnectToGameServer;

	private GameObject mModel;

	protected bool enableFly;

	private IRecordset mRecordFromServer;

	public static bool mNeedToTurnRegister;

	private GameObject Achievement;

	private Timer AchievementLoadTimer;

	public bool bConnection { get; set; }

	public byte loadResult { get; set; }

	public bool bIsRegist { get; set; }

	public bool bNeedResetFriendTime { get; set; }

	private void Awake()
	{
		mInstance = this;
		GameApp.GetInstance().GetGameMode().TypeOfNetwork = NetworkType.Single;
		UIEventListener uIEventListener = UIEventListener.Get(CampaignCollision);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(ClickGameObject));
		UIEventListener uIEventListener2 = UIEventListener.Get(MultiplayCollision);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(ClickGameObject));
		UIEventListener uIEventListener3 = UIEventListener.Get(CreditsCollision);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(ClickGameObject));
		UIEventListener uIEventListener4 = UIEventListener.Get(GameCenterCollision);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(ClickGameObject));
		UIEventListener uIEventListener5 = UIEventListener.Get(OptionCollision);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(ClickGameObject));
		UIEventListener uIEventListener6 = UIEventListener.Get(SwitchCollision);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(ClickGameObject));
		if (AvatarPosition.transform.childCount > 0)
		{
			GameObject gameObject = AvatarPosition.transform.GetChild(0).gameObject;
			gameObject.transform.parent = null;
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		UIMsgBox.instance.CloseMessage();
		mInstance = null;
		GameApp.GetInstance().Save();
	}

	private void Start()
	{
		GameApp.GetInstance().CreateGameAppScript();
		bConnection = false;
		loadResult = 0;
		GameDataManager.GetInstance().Init();
		TimeManager.GetInstance().Init();
		TimeManager.GetInstance().setMaxLoopTimes(-1);
		TimeManager.GetInstance().setPeriod(3f);
		GameObject gameObject = GameObject.Find(AudioName.MENU_MUSIC_NAME);
		if (gameObject == null)
		{
			GameObject original = Resources.Load(AudioName.BGM_MENU_SINGLE) as GameObject;
			gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.name = AudioName.MENU_MUSIC_NAME;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<MenuMusicScript>();
		}
		InitUserState();
		InitLanguage();
		GameApp.GetInstance().GetUserState().Achievement.LateInit();
		MissionManager.GetInstance().Init();
		mCampaignClicked = false;
		mMultiplayClicked = false;
		CreateDefaultAvatar();
		if (UIMsgBox.instance == null)
		{
			GameObject original2 = Resources.Load("UI/Common/MessageBox/Prefab/MsgUI") as GameObject;
			UnityEngine.Object.Instantiate(original2, new Vector3(0f, -1500f, 0f), Quaternion.identity);
		}
		if (LoadingUIScript.mInstance == null)
		{
			GameObject original3 = Resources.Load("UI/Common/LoadingUI/Prefab/LoadingUI") as GameObject;
			UnityEngine.Object.Instantiate(original3, new Vector3(0f, 500f, 0f), Quaternion.identity);
		}
		mDelayPassed = false;
		mStartConnectToGameServer = false;
		InitEffectRoot();
		Vector3 localPosition = Camera.main.transform.localPosition;
		Quaternion localRotation = Camera.main.transform.localRotation;
		Camera.main.gameObject.AddComponent<AudioManager>();
		Camera.main.transform.localPosition = localPosition;
		Camera.main.transform.localRotation = localRotation;
		LeftAnimationObject.GetComponent<Animation>().AddClip(LeftDoorAnimation, "LeftDoor");
		LeftAnimationObject.GetComponent<Animation>()["LeftDoor"].speed = 2f;
		RightAnimationObject.GetComponent<Animation>().AddClip(RightDoorAnimation, "RightDoor");
		RightAnimationObject.GetComponent<Animation>()["RightDoor"].speed = 2f;
		RightAnimationObject.GetComponent<Animation>().AddClip(RightDoorAnimation, "RightDoorClose");
		RightAnimationObject.GetComponent<Animation>()["RightDoorClose"].time = RightAnimationObject.GetComponent<Animation>()["RightDoorClose"].length;
		RightAnimationObject.GetComponent<Animation>()["RightDoorClose"].speed = -2f;
		if (GameObject.Find("ApplicationPauseManager") == null)
		{
			GameObject original4 = Resources.Load("UI/Common/ApplicationPauseManager") as GameObject;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(original4) as GameObject;
			gameObject2.name = "ApplicationPauseManager";
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
		}
		if (mNeedToTurnRegister)
		{
			ClickGameObject(OptionCollision);
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.REGISTER_TUTORIAL);
		}
		RemovePhoto();
		Resources.UnloadUnusedAssets();
	}

	private void Update()
	{
		UpdateNetwork();
		if (bConnection)
		{
			networkMgr = GameApp.GetInstance().GetNetworkManager();
			if (networkMgr != null)
			{
				if (networkMgr.conn != null)
				{
					TimeManager.GetInstance().Loop();
				}
				else
				{
					bConnection = false;
				}
			}
			else
			{
				bConnection = false;
			}
		}
		if (mCampaignClicked)
		{
			if (!mDelayPassed && Time.time - mClickTime > mWaitToLoadDelay)
			{
				mDelayPassed = true;
				OpenCampaign();
			}
			else if (mDelayPassed && Time.time - mClickTime > mWaitToLoadTime + mWaitToLoadDelay)
			{
				if (TutorialManager.GetInstance().Trigger(TutorialTriggerType.GAME_STORY))
				{
					mCampaignClicked = false;
					return;
				}
				LoadingUIScript.mInstance.ShowAfterMissionTip(GameState.Playing, MissionType.NONE, false, false);
				Application.LoadLevel(mLoadLevelName);
			}
		}
		if (mMultiplayClicked && !mDelayPassed && Time.time - mClickTime > mWaitToLoadDelay)
		{
			mDelayPassed = true;
			OpenMulti();
			Login();
		}
		if (mStartConnectToGameServer && Time.time - mStartConnectToGameServerTime > Global.MAX_LOGIN_WAITING_TIME)
		{
			if (bIsRegist && GameApp.GetInstance().GetUserState().GetOnlineRegisted())
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 18);
				bIsRegist = false;
				mStartConnectToGameServerTime = 10000f;
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 18);
				mStartConnectToGameServerTime = 10000f;
			}
			if (networkMgr != null)
			{
				networkMgr.CloseConnection();
			}
		}
		if (AchievementLoadTimer != null && AchievementLoadTimer.Ready())
		{
			AchievementLoadTimer.Pause();
			UIMsgBox.instance.CloseMessage();
			Achievement.SetActive(true);
		}
	}

	public void OnLoginFail()
	{
		if (mMultiplayClicked)
		{
			CloseMulti();
		}
		mCampaignClicked = false;
		mMultiplayClicked = false;
		mDelayPassed = false;
		mStartConnectToGameServer = false;
	}

	public void ClickGameObject(GameObject go)
	{
		if (mCampaignClicked || mMultiplayClicked)
		{
			return;
		}
		if (go == CampaignCollision)
		{
			mCampaignClicked = true;
			mClickTime = Time.time;
			mLoadLevelName = "ShopAndCustomize2";
			AudioManager.GetInstance().PlaySound(AudioName.MENU_DOOR_OPEN);
		}
		else if (go == MultiplayCollision)
		{
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				if (GameApp.GetInstance().GetUserState().HasLoginOnline())
				{
					mMultiplayClicked = true;
					mClickTime = Time.time;
					AudioManager.GetInstance().PlaySound(AudioName.MENU_DOOR_OPEN);
				}
				else
				{
					UIMsgBox.instance.ShowMessage(mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_1"), 8, 23);
				}
			}
			else
			{
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
		}
		else if (go == GameCenterCollision)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (!UIMsgBox.instance.IsMessageShow())
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_ACHIEVEMENT_TIPS_3"), 7);
			}
			AchievementLoadTimer = new Timer(0.2f, false);
			GameObject original = Resources.Load("UI/Menu/AchievementUI/Prefab/AchievementUI") as GameObject;
			Achievement = (GameObject)UnityEngine.Object.Instantiate(original, new Vector3(0f, 1500f, 0f), Quaternion.identity);
			Achievement.SetActive(false);
		}
		else if (go == CreditsCollision)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		}
		else if (go == OptionCollision)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			GameObject original2 = Resources.Load("UI/Menu/OptionUI/Prefab/OptionUI") as GameObject;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original2, new Vector3(0f, 1500f, 0f), Quaternion.identity);
			gameObject.GetComponent<OptionUIScript>().SetIsInMainMenu(true);
		}
		else if (go == SwitchCollision)
		{
			ShaderManager.GetInstance().SwitchShader(mModel);
		}
	}

	public void OpenCampaign()
	{
		for (int i = 0; i < CampaignLightsFront.Length; i++)
		{
			CampaignLightsFront[i].GetComponent<BlinkLightScript>().enabled = true;
		}
		for (int j = 0; j < CampaignLightsUp.Length; j++)
		{
			CampaignLightsUp[j].GetComponent<BlinkLightScript>().enabled = true;
			CampaignLightsDown[j].GetComponent<BlinkLightScript>().enabled = true;
		}
		LeftAnimationObject.GetComponent<Animation>().Play("LeftDoor");
	}

	public void OpenMulti()
	{
		for (int i = 0; i < MultiLightsFront.Length; i++)
		{
			MultiLightsFront[i].GetComponent<BlinkLightScript>().enabled = true;
			MultiLightsFront[i].GetComponent<BlinkLightScript>().Fresh();
		}
		for (int j = 0; j < MultiLightsUp.Length; j++)
		{
			MultiLightsUp[j].GetComponent<BlinkLightScript>().enabled = true;
			MultiLightsUp[j].GetComponent<BlinkLightScript>().Fresh();
			MultiLightsDown[j].GetComponent<BlinkLightScript>().enabled = true;
			MultiLightsDown[j].GetComponent<BlinkLightScript>().Fresh();
		}
		RightAnimationObject.GetComponent<Animation>().Play("RightDoor");
	}

	public void CloseMulti()
	{
		for (int i = 0; i < MultiLightsFront.Length; i++)
		{
			MultiLightsFront[i].GetComponent<BlinkLightScript>().enabled = false;
		}
		for (int j = 0; j < MultiLightsUp.Length; j++)
		{
			MultiLightsUp[j].GetComponent<BlinkLightScript>().enabled = false;
			MultiLightsDown[j].GetComponent<BlinkLightScript>().enabled = false;
		}
		RightAnimationObject.GetComponent<Animation>().Play("RightDoorClose");
	}

	private void InitUserState()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (!userState.bInit)
		{
			loadResult = GameApp.GetInstance().Load();
			if (loadResult != 0)
			{
				userState.Init();
			}
			userState.InitLevelName();
			userState.InitRank();
			userState.bInit = true;
			GameApp.GetInstance().StartHttpRequestThread();
			SocialManager.GetInstance().InitSocialManager();
			LoginBackground();
		}
	}

	public void CreateDefaultAvatar()
	{
		short[] avatar = GameApp.GetInstance().GetUserState().GetAvatar();
		short[] array = new short[6];
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		Weapon weapon = GameApp.GetInstance().GetUserState().GetBattleWeapons()[0];
		array[5] = (short)weapon.GunID;
		ChangeAvatar(array);
	}

	public void ChangeAvatar(short[] avatar)
	{
		ClearModels();
		Armor armor = GameApp.GetInstance().GetUserState().GetArmor(4, avatar[4]);
		enableFly = ((armor.GetSkill(SkillsType.FLY) > 0f) ? true : false);
		Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[avatar[5]];
		GameObject gameObject = AvatarBuilder.GetInstance().CreateMainMenuAvatar(avatar);
		gameObject.transform.rotation = Quaternion.identity;
		Transform parent = gameObject.transform.Find(BoneName.Weapon);
		if (weapon.GetWeaponType() == WeaponType.BowSword)
		{
			parent = gameObject.transform.Find(BoneName.WeaponL);
			BowSword bowSword = weapon as BowSword;
			bowSword.SetInnerState(BowSword.EInnerState.EBow);
		}
		string path = Global.WEAPON_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", avatar[5]);
		UnityEngine.Object original = Resources.Load(path);
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(original);
		gameObject2.transform.parent = parent;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localRotation = Quaternion.identity;
		gameObject2.AddComponent<MaterialChanger>();
		gameObject2.GetComponent<MaterialChanger>().SetMaterialInfo();
		if (weapon.GetWeaponType() != WeaponType.DoublePistol && weapon.GetWeaponType() != WeaponType.ChargeDoublePistol)
		{
			weapon.UIInit(gameObject2, null);
		}
		else
		{
			Transform parent2 = gameObject.transform.Find(BoneName.WeaponL);
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(original);
			gameObject3.transform.parent = parent2;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.AddComponent<MaterialChanger>();
			gameObject3.GetComponent<MaterialChanger>().SetMaterialInfo();
			weapon.UIInit(gameObject2, gameObject3);
		}
		gameObject.transform.localScale = Vector3.one;
		SetGameObject(gameObject);
		string text = AnimationString.Start + "_" + weapon.GetAnimationSuffix() + "_" + AnimationString.Idle;
		gameObject.GetComponent<Animation>()[AnimationString.Start + "_" + weapon.GetAnimationSuffix() + "_" + AnimationString.Idle].wrapMode = WrapMode.Loop;
		gameObject.GetComponent<Animation>().Play(text);
		gameObject.transform.parent = AvatarPosition.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		mModel = gameObject;
		AvatarBuilder.SetModelLayer(mModel, PhysicsLayer.PLAYER);
	}

	public void ClearModels()
	{
		if (mModel != null)
		{
			UnityEngine.Object.Destroy(mModel);
			mModel = null;
		}
	}

	public void SetLowerBodyAnimation(GameObject playerObj, string weaponSuffix)
	{
		if (playerObj.GetComponent<Animation>()[AnimationString.Start + "_" + weaponSuffix + "_" + AnimationString.Idle] != null)
		{
			playerObj.GetComponent<Animation>()[AnimationString.Start + "_" + weaponSuffix + "_" + AnimationString.Idle].wrapMode = WrapMode.Loop;
		}
	}

	public void SetGameObject(GameObject playerObj)
	{
		SetLowerBodyAnimation(playerObj, "assault");
		SetLowerBodyAnimation(playerObj, "RPG");
		SetLowerBodyAnimation(playerObj, "shotgun");
		SetLowerBodyAnimation(playerObj, "fist");
	}

	private void UpdateNetwork()
	{
		networkMgr = GameApp.GetInstance().GetNetworkManager();
		if (networkMgr != null)
		{
			networkMgr.ProcessReceivedPackets();
		}
	}

	private void Login()
	{
		SwAccountLoginRequest request = new SwAccountLoginRequest(false);
		GameApp.GetInstance().GetWWWManager().SendRequest(request);
		if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
		}
	}

	public void ConnectToGameServer(string ip, int port, int userId, string date, EUserType userType, bool isRegist)
	{
		GameApp.GetInstance().DestoryNetWork();
		GameApp.GetInstance().CreateNetwork(ip, port);
		RoleLoginRequest request = new RoleLoginRequest(Lobby.GetInstance().GetUserName(), userId, date, (byte)userType, isRegist);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		UIMsgBox.instance.ShowMessage(mInstance, LocalizationManager.GetInstance().GetString("LOC_PVP_CONNECT"), 7);
		mStartConnectToGameServer = true;
		mStartConnectToGameServerTime = Time.time;
		Debug.Log("login game server :" + ip + ":" + port + ",   userType = " + userType);
	}

	public string GetRandomString(int length)
	{
		string empty = string.Empty;
		char c = (char)UnityEngine.Random.Range(65, 91);
		empty += c;
		for (int i = 0; i < length; i++)
		{
			c = (char)UnityEngine.Random.Range(65, 91);
			empty += c;
		}
		return empty;
	}

	public void LogicServerFailed()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_ERROR_CODE_113"), 1, 17);
	}

	public void LogicSavaDataInvalid()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_SAVE_DATA_CRASH"), 1, 92);
	}

	public void UpLoadData()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		userState.SetSyncRecordTime(Lobby.GetInstance().GetLoginTime());
		userState.CheckNeedResetFriendTime(bNeedResetFriendTime);
		GameApp.GetInstance().Save();
		UploadDataRequest request = new UploadDataRequest(userState, bIsRegist);
		networkMgr.SendRequest(request);
		if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_PVP_CONNECT"), 7, 61);
		}
	}

	public void UploadDataLogic()
	{
		if (mRecordFromServer != null && mRecordFromServer.GetRecordData().mSaveNum > GameApp.GetInstance().GetUserState().GetSaveNum())
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_28"), 1, 60);
			GameObject original = Resources.Load("UI/Menu/CompareRecord/Prefab/CompareRecordUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			CompareRecordUI component = gameObject.GetComponent<CompareRecordUI>();
			if (null != component)
			{
				component.IsRegisted = bIsRegist;
			}
			mStartConnectToGameServer = false;
		}
		else
		{
			UpLoadData();
			ClearRecordFromServer();
		}
	}

	public void OnUploadDataComplete(bool isRegist)
	{
		mStartConnectToGameServer = false;
		UIMsgBox.instance.CloseMessage();
		if (isRegist)
		{
			networkMgr = GameApp.GetInstance().GetNetworkManager();
			if (networkMgr != null)
			{
				networkMgr.CloseConnection();
			}
			if (LoginPageScript.mInstance.RegisterPage.activeSelf)
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 21);
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_30"), 1, 21);
			}
		}
		else
		{
			GotoMultiMenu();
		}
	}

	public void CorrectLogin()
	{
		bConnection = true;
	}

	public void GotoMultiMenu()
	{
		GameObject gameObject = GameObject.Find(AudioName.MENU_MUSIC_NAME);
		if (gameObject != null)
		{
			UnityEngine.Object.DestroyObject(gameObject);
		}
		Application.LoadLevel("MultiMenu");
	}

	public void SetRecordFromServer(IRecordset record)
	{
		mRecordFromServer = record;
	}

	public IRecordset GetRecordFromServer()
	{
		return mRecordFromServer;
	}

	public void ResetRecordFromServer()
	{
		if (mRecordFromServer != null)
		{
			mRecordFromServer.ResetUserState();
			GameApp.GetInstance().GetUserState().CheckNeedResetFriendTime(bNeedResetFriendTime);
			GameApp.GetInstance().Save();
		}
	}

	public void ClearRecordFromServer()
	{
		mRecordFromServer = null;
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 18 || whichMsg.EventId == 17)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (LoginPageScript.mInstance != null)
				{
					LoginPageScript.mInstance.Refresh();
				}
				if (GameApp.GetInstance().GetUserState().GetOnlineRegisted())
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
				}
				OnLoginFail();
			}
		}
		else if (whichMsg.EventId == 16)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (LoginPageScript.mInstance != null)
				{
					LoginPageScript.mInstance.Refresh();
				}
				if (GameApp.GetInstance().GetUserState().GetOnlineRegisted())
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
				}
				OnLoginFail();
			}
		}
		else if (whichMsg.EventId == 21)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (!TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.VOID_TUTORIAL))
				{
					Application.LoadLevel("ShopAndCustomize2");
				}
				if (LoginPageScript.mInstance != null)
				{
					LoginPageScript.mInstance.Refresh();
				}
				if (CompareRecordUI.mInstance != null)
				{
					CompareRecordUI.mInstance.gameObject.SetActive(false);
				}
				GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
			}
		}
		else if (whichMsg.EventId == 60)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 23)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				mNeedToTurnRegister = true;
				ClickGameObject(OptionCollision);
			}
			else
			{
				mMultiplayClicked = true;
				mClickTime = Time.time;
				AudioManager.GetInstance().PlaySound(AudioName.MENU_DOOR_OPEN);
			}
			UIMsgBox.instance.CloseMessage();
		}
		else if (whichMsg.EventId == 92)
		{
			Debug.Log("Send email.");
			UIMsgBox.instance.CloseMessage();
			string text = string.Empty;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				text = "[IOS]";
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				text = "[Android]";
			}
			else if (Application.platform == RuntimePlatform.WP8Player)
			{
				text = "[WP]";
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			string text2 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_4") + text;
			string text3 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_5");
			string text4 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_6");
			string text5 = "Version: " + GameApp.GetInstance().GetUserState().readVersion;
			string text6 = "ID: " + GameApp.GetInstance().GetUserState().GetOnlineUserId();
			string text7 = "Language: " + LocalizationManager.GetInstance().currentLanguage;
			string text8 = "Device: " + SystemInfo.deviceModel;
			string text9 = "System: " + SystemInfo.operatingSystem;
			string text10 = "UUID: " + GameApp.GetInstance().UUID;
			string text11 = "mailto:ios@ifreyr.com?subject=" + text2 + "&body=" + text3 + "%0A%0A%0A%0A%0A%0A" + text4 + "%0A" + text5 + "%0A" + text6 + "%0A" + text7 + "%0A" + text8 + "%0A" + text9 + "%0A" + text10;
			Application.OpenURL(text11.Replace(" ", "%20"));
		}
	}

	public void InitEffectRoot()
	{
		EffectRoot = GameObject.Find("_EffectRoot");
		if (EffectRoot == null)
		{
			EffectRoot = new GameObject("_EffectRoot");
			EffectRoot.AddComponent<EffectPool>();
			UnityEngine.Object.DontDestroyOnLoad(EffectRoot);
		}
	}

	private void InitLanguage()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string currentLanguage = ExternScript.GetCurrentLanguage();
			Debug.Log("The current language key is " + currentLanguage);
			if (GameApp.GetInstance().GetUserState().GetUserLanguage() == EUserLanguage.DEFAULT)
			{
				EUserLanguage currentLanguage2;
				switch (currentLanguage)
				{
				case "en":
					currentLanguage2 = EUserLanguage.ENGLISH;
					break;
				case "zh-Hant":
					currentLanguage2 = EUserLanguage.CHINESE_SIMPLIFIED;
					break;
				case "zh-Hans":
					currentLanguage2 = EUserLanguage.CHINESE_SIMPLIFIED;
					break;
				default:
					currentLanguage2 = EUserLanguage.CHINESE_SIMPLIFIED;
					break;
				}
				LocalizationManager.GetInstance().currentLanguage = currentLanguage2;
			}
		}
		else
		{
			LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
		}
		Debug.Log("The current language is " + LocalizationManager.GetInstance().currentLanguage);
	}

	private void RemovePhoto()
	{
		string path = Application.persistentDataPath + "/Documents";
		if (!Directory.Exists(path))
		{
			return;
		}
		int num = 50;
		string[] files = Directory.GetFiles(path, "*.png");
		if (files.Length > num)
		{
			List<PhotoInfo> list = new List<PhotoInfo>();
			string[] array = files;
			PhotoInfo item = default(PhotoInfo);
			foreach (string fileName in array)
			{
				FileInfo fileInfo = new FileInfo(fileName);
				item.fileName = fileName;
				item.time = fileInfo.CreationTime.Ticks;
				list.Add(item);
			}
			list.Sort(SortByTime);
			for (int j = 0; j < files.Length - num; j++)
			{
				File.Delete(list[j].fileName);
			}
		}
	}

	public static int SortByTime(PhotoInfo a, PhotoInfo b)
	{
		if (a.time - b.time < 0)
		{
			return -1;
		}
		if (a.time - b.time > 0)
		{
			return 1;
		}
		return 0;
	}

	public void UpdateOrDownloadDataOver()
	{
		ClearRecordFromServer();
		UIMsgBox.instance.CloseMessage();
		if (!bIsRegist)
		{
			GotoMultiMenu();
		}
	}

	private void LoginBackground()
	{
		long lastLoginTicks = GameApp.GetInstance().GetUserState().GetLastLoginTicks();
		long nextLoginInterval = GameApp.GetInstance().GetUserState().GetNextLoginInterval();
		long ticks = DateTime.Now.Ticks;
		Debug.Log("lastLoginTicks: " + lastLoginTicks + ", nextLoginInterval: " + nextLoginInterval + ", currentTicks: " + ticks);
		if (ticks - lastLoginTicks > nextLoginInterval)
		{
			SwBackgroundLoginRequest request = new SwBackgroundLoginRequest();
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
		}
	}
}

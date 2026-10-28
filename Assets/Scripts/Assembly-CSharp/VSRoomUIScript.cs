using System;
using System.Collections.Generic;
using UnityEngine;

public class VSRoomUIScript : MonoBehaviour, UIMsgListener
{
	public static VSRoomUIScript mInstance;

	public UILabel RoomTitle;

	public UILabel DoButtonLabel;

	public GameObject DoButton;

	public GameObject ReturnButton;

	public GameObject BackButton;

	public GameObject RefreshButton;

	public GameObject CreateInListButton;

	public UIGrid SelectButtonList;

	public VSRoomItemScript ModeSelectButton;

	public UILabel SelectModeName;

	public VSRoomItemScript MapSelectButton;

	public UILabel SelectMapName;

	public VSRoomItemScript WinConditionSelectButton;

	public UILabel SelectWinConditionName;

	public VSRoomItemScript RoomSetSelectButton;

	public VSRoomItemScript PasswordInputButton;

	public GameObject RoomSettingObject;

	public GameObject RoomSettingCommonObj;

	public GameObject RoomModeSelectPage;

	public GameObject RoomMapSelectPage;

	public GameObject RoomWinConditionSelectPage;

	public GameObject RoomSetPage;

	public GameObject RoomPasswordPage;

	public UIInput CreatePasswordInput;

	public GameObject RoomQuickGamePVPPage;

	public GameObject RoomQuickGamePVEPage;

	public GameObject RoomSearchPage;

	public GameObject RoomListPage;

	public GameObject RoomListTagCoop;

	public GameObject RoomListTagVS;

	public GameObject RoomListTagFriends;

	public GameObject TagCoopHighlight;

	public UILabel TagCoopLabel;

	public GameObject TagVsHighlight;

	public UILabel TagVsLabel;

	public GameObject TagFriendsHighlight;

	public UILabel TagFriendsLabel;

	public GameObject RoomWaitPartnerPage;

	public UISprite WaitPartnerRoomIcon;

	public UISprite DiffcultyIcon;

	public PartnerInfoScript[] WaitPlayers;

	public GameObject ModeTemplate;

	public UIDraggablePanel MapsList;

	public int MapsSpace;

	public GameObject MapTemplate;

	public GameObject RandomMapTemplate;

	public GameObject WinConditionTemplate;

	public GameObject RoomTemplate;

	public UIGrid RoomsGrid;

	public GameObject PointLeft;

	public GameObject PointLeftMiddle;

	public GameObject PointMiddle;

	public GameObject PointRightMiddle;

	public GameObject PointRight;

	public GameObject WaitSearchingObj;

	public UILabel Random_Tip;

	public UISprite[] LoadingSprites;

	public UIInput SearchInput;

	public UIDraggablePanel QuickGamePVPList;

	public GameObject Q_PVP_PointLeft;

	public GameObject Q_PVP_PointLeftMiddle;

	public GameObject Q_PVP_PointMiddle;

	public GameObject Q_PVP_PointRightMiddle;

	public GameObject Q_PVP_PointRight;

	public UIDraggablePanel QuickGamePVEList;

	public GameObject Q_PVE_PointLeft;

	public GameObject Q_PVE_PointLeftMiddle;

	public GameObject Q_PVE_PointMiddle;

	public GameObject Q_PVE_PointRightMiddle;

	public GameObject Q_PVE_PointRight;

	public RoomSetSelection MaxPlayerSetting;

	public RoomSetSelection MinLevelLimit;

	public RoomSetSelection MaxLevelLimit;

	protected RoomPage mCurrentPage;

	protected RoomRequestType mRequestType;

	protected VSTarget mVSTarget;

	protected VSRoomItemScript mSelectMap;

	protected VSRoomItemScript mSelectMode;

	protected VSRoomItemScript mSelectWinCondition;

	protected VSRoomItemScript mSelectQuickGamePVP;

	protected VSRoomItemScript mSelectQuickGamePVE;

	private List<VSRoomItemScript> m_Modes = new List<VSRoomItemScript>();

	private List<VSRoomItemScript> m_WinConditions = new List<VSRoomItemScript>();

	private List<VSRoomItemScript> m_Maps = new List<VSRoomItemScript>();

	private List<VSRoomItemScript> m_QuickGamePVPModes = new List<VSRoomItemScript>();

	private List<VSRoomItemScript> m_QuickGamePVEMaps = new List<VSRoomItemScript>();

	private float mCheckCenterChildTime = 0.015f;

	private float mLastCheckTime;

	protected UICenterOnChild mCenterOnChild;

	protected UICenterOnChild mCenterOnChild_Q_PVP;

	protected UICenterOnChild mCenterOnChild_Q_PVE;

	protected float XLeft;

	protected float XLeftMiddle;

	protected float XMiddle;

	protected float XRightMiddle;

	protected float XRight;

	protected VSTarget mRoomListTarget = VSTarget.ALL;

	private Color mRoolListTargetLabelColor = new Color(43f / 255f, 226f / 255f, 254f / 255f);

	private bool needUpdateMaps = true;

	private bool needUpdateQuickGamePVP = true;

	private bool needUpdateQuickGamePVE = true;

	private bool mIsModePageInited;

	private bool mIsMapPageInited;

	private bool mIsWinConditionPageInited;

	private bool mIsPasswordCreateInited;

	private bool mIsRoomSettingInited;

	private string RoomCreateTitle;

	private string RoomJoinTitle;

	private string RoomSearchTitle;

	private string RoomListTitle;

	private string RoomWaitPartnerTitle;

	private string RoomCreateButtonText;

	private string RoomQuickGameButtonText;

	private string RoomSearchButtonText;

	private string RoomListButtonText;

	private string RoomWaitPartnerText;

	private int mFirstLoadingSpriteIndex;

	private float mLoadingSpriteUpdateInterval = 0.07f;

	private float mLoadingSpriteUpdateTime;

	private List<RoomTemplateScript> m_Rooms = new List<RoomTemplateScript>();

	protected RoomTemplateScript mSelectRoomInList;

	private int mRoomMaxPlayer = 8;

	private int mCoopMaxPlayer = 3;

	private int mRoomMinLevelLimit = 1;

	private int mRoomMaxLevelLimit = 50;

	private bool mIsDoButtonClicked;

	private void Awake()
	{
		RoomCreateTitle = LocalizationManager.GetInstance().GetString("LOC_PVP_CREATE");
		RoomJoinTitle = LocalizationManager.GetInstance().GetString("LOC_PVP_QUICK_GAME");
		RoomSearchTitle = LocalizationManager.GetInstance().GetString("LOC_PVP_SEARCH");
		RoomListTitle = LocalizationManager.GetInstance().GetString("LOC_PVP_LIST");
		RoomWaitPartnerTitle = LocalizationManager.GetInstance().GetString("LOC_COOP_WAITROOM");
		RoomCreateButtonText = LocalizationManager.GetInstance().GetString("LOC_PVP_CREATE");
		RoomQuickGameButtonText = LocalizationManager.GetInstance().GetString("LOC_ICON_24");
		RoomSearchButtonText = LocalizationManager.GetInstance().GetString("LOC_PVP_SEARCH");
		RoomListButtonText = LocalizationManager.GetInstance().GetString("LOC_JOIN");
		RoomWaitPartnerText = LocalizationManager.GetInstance().GetString("LOC_ICON_24");
		mInstance = this;
		mCenterOnChild = MapsList.transform.GetChild(0).GetComponent<UICenterOnChild>();
		mCenterOnChild.onFinished = DragFinished;
		mCenterOnChild_Q_PVP = QuickGamePVPList.transform.GetChild(0).GetComponent<UICenterOnChild>();
		mCenterOnChild_Q_PVP.onFinished = DragFinished;
		mCenterOnChild_Q_PVE = QuickGamePVEList.transform.GetChild(0).GetComponent<UICenterOnChild>();
		mCenterOnChild_Q_PVE.onFinished = DragFinished;
		UIEventListener uIEventListener = UIEventListener.Get(DoButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(ReturnButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(BackButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(RefreshButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(CreateInListButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(ModeSelectButton.gameObject);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(MapSelectButton.gameObject);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(WinConditionSelectButton.gameObject);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener9 = UIEventListener.Get(RoomSetSelectButton.gameObject);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener10 = UIEventListener.Get(PasswordInputButton.gameObject);
		uIEventListener10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener10.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener11 = UIEventListener.Get(RoomListTagCoop);
		uIEventListener11.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener11.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener12 = UIEventListener.Get(RoomListTagVS);
		uIEventListener12.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener12.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener13 = UIEventListener.Get(RoomListTagFriends);
		uIEventListener13.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener13.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener14 = UIEventListener.Get(MaxPlayerSetting.gameObject);
		uIEventListener14.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener14.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener15 = UIEventListener.Get(MinLevelLimit.gameObject);
		uIEventListener15.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener15.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener16 = UIEventListener.Get(MaxLevelLimit.gameObject);
		uIEventListener16.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener16.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		RefreshButton.SetActive(false);
		CreateInListButton.SetActive(false);
		mRoomMaxPlayer = 8;
		mRoomMinLevelLimit = 1;
		mRoomMaxLevelLimit = 50;
		MaxPlayerSetting.Init(mRoomMaxPlayer);
		MinLevelLimit.Init(mRoomMinLevelLimit);
		MaxLevelLimit.Init(mRoomMaxLevelLimit);
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void OpenPage(RoomPage page, RoomRequestType requestType, VSTarget target)
	{
		if (mRequestType != requestType || mVSTarget != target)
		{
			mIsModePageInited = false;
			mIsMapPageInited = false;
			mIsWinConditionPageInited = false;
			mIsPasswordCreateInited = false;
			mIsRoomSettingInited = false;
		}
		base.gameObject.SetActive(true);
		RoomSettingObject.SetActive(false);
		RoomModeSelectPage.SetActive(false);
		RoomMapSelectPage.SetActive(false);
		RoomWinConditionSelectPage.SetActive(false);
		RoomSetPage.SetActive(false);
		RoomPasswordPage.SetActive(false);
		RoomListPage.SetActive(false);
		RoomSearchPage.SetActive(false);
		RoomWaitPartnerPage.SetActive(false);
		RoomQuickGamePVPPage.SetActive(false);
		mVSTarget = target;
		mRoomListTarget = target;
		if (mVSTarget == VSTarget.PVE)
		{
			MapSelectButton.gameObject.SetActive(true);
			ModeSelectButton.gameObject.SetActive(false);
			WinConditionSelectButton.gameObject.SetActive(false);
		}
		else if (mVSTarget == VSTarget.PVP)
		{
			ModeSelectButton.gameObject.SetActive(true);
			MapSelectButton.gameObject.SetActive(false);
			WinConditionSelectButton.gameObject.SetActive(false);
		}
		mRequestType = requestType;
		if (mRequestType == RoomRequestType.Create)
		{
			RoomTitle.text = RoomCreateTitle;
			DoButtonLabel.text = RoomCreateButtonText;
			PasswordInputButton.gameObject.SetActive(true);
		}
		else
		{
			RoomTitle.text = RoomJoinTitle;
			DoButtonLabel.text = RoomQuickGameButtonText;
			PasswordInputButton.gameObject.SetActive(false);
		}
		ChangePage(page);
	}

	public void ChangePage(RoomPage page)
	{
		mIsDoButtonClicked = false;
		switch (mCurrentPage)
		{
		case RoomPage.ModeSelectPage:
			ModeSelectButton.SetHighlight(false);
			RoomModeSelectPage.SetActive(false);
			break;
		case RoomPage.MapSelectPage:
			needUpdateMaps = false;
			MapSelectButton.SetHighlight(false);
			RoomMapSelectPage.SetActive(false);
			break;
		case RoomPage.WinConditionSelectPage:
			WinConditionSelectButton.SetHighlight(false);
			RoomWinConditionSelectPage.SetActive(false);
			break;
		case RoomPage.ListPage:
			RoomListPage.SetActive(false);
			BackButton.SetActive(true);
			RefreshButton.SetActive(false);
			CreateInListButton.SetActive(false);
			break;
		case RoomPage.SearchPage:
			RoomSearchPage.SetActive(false);
			SearchInput.gameObject.SetActive(false);
			WaitSearchingObj.SetActive(false);
			break;
		case RoomPage.RoomSetPage:
			RoomSetSelectButton.SetHighlight(false);
			RoomSetPage.SetActive(false);
			break;
		case RoomPage.PasswordCreatePage:
		case RoomPage.PasswordJoinPage:
			PasswordInputButton.SetHighlight(false);
			RoomPasswordPage.SetActive(false);
			break;
		case RoomPage.WaitParnterPage:
		{
			PartnerInfoScript[] waitPlayers = WaitPlayers;
			foreach (PartnerInfoScript partnerInfoScript in waitPlayers)
			{
				partnerInfoScript.Init(null);
			}
			RoomWaitPartnerPage.SetActive(false);
			DoButton.SetActive(true);
			break;
		}
		case RoomPage.QuickJoinPVPPage:
			needUpdateQuickGamePVP = false;
			RoomQuickGamePVPPage.SetActive(false);
			RoomSettingCommonObj.SetActive(true);
			break;
		case RoomPage.QuickJoinPVEPage:
			needUpdateQuickGamePVE = false;
			RoomQuickGamePVEPage.SetActive(false);
			RoomSettingCommonObj.SetActive(true);
			break;
		}
		switch (page)
		{
		case RoomPage.ModeSelectPage:
			if (!mIsModePageInited)
			{
				InitModes();
			}
			RoomSettingObject.SetActive(true);
			RoomModeSelectPage.SetActive(true);
			ModeSelectButton.SetHighlight(true);
			SelectButtonList.Reposition();
			RoomModeSelectPage.GetComponent<UIGrid>().Reposition();
			break;
		case RoomPage.MapSelectPage:
			mLastCheckTime = Time.time;
			needUpdateMaps = true;
			RoomSettingObject.SetActive(true);
			RoomMapSelectPage.SetActive(true);
			MapSelectButton.SetHighlight(true);
			if (!mIsMapPageInited)
			{
				InitMaps(0);
			}
			mCenterOnChild.Recenter();
			SelectButtonList.Reposition();
			break;
		case RoomPage.WinConditionSelectPage:
			if (!mIsWinConditionPageInited)
			{
				InitWinConditions();
			}
			RoomSettingObject.SetActive(true);
			RoomWinConditionSelectPage.SetActive(true);
			WinConditionSelectButton.SetHighlight(true);
			SelectButtonList.Reposition();
			RoomWinConditionSelectPage.GetComponent<UIGrid>().Reposition();
			break;
		case RoomPage.ListPage:
			RoomSettingObject.SetActive(false);
			RoomListPage.SetActive(true);
			RoomTitle.text = RoomListTitle;
			DoButtonLabel.text = RoomListButtonText;
			if (SearchInput.value == string.Empty)
			{
				UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_25"), 7);
				GetRoomListRequest request = new GetRoomListRequest(GetRoomType(), GameApp.GetInstance().GetUserState().GetRank()
					.rankID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			BackButton.SetActive(false);
			RefreshButton.SetActive(true);
			CreateInListButton.SetActive(true);
			switch (mRoomListTarget)
			{
			case VSTarget.ALL:
				TagCoopHighlight.SetActive(true);
				TagCoopLabel.color = Color.white;
				TagVsHighlight.SetActive(true);
				TagVsLabel.color = Color.white;
				TagFriendsHighlight.SetActive(true);
				TagFriendsLabel.color = Color.white;
				break;
			case VSTarget.PVE:
				TagCoopHighlight.SetActive(true);
				TagCoopLabel.color = Color.white;
				TagVsHighlight.SetActive(false);
				TagVsLabel.color = mRoolListTargetLabelColor;
				TagFriendsHighlight.SetActive(false);
				TagFriendsLabel.color = mRoolListTargetLabelColor;
				break;
			case VSTarget.PVP:
				TagCoopHighlight.SetActive(false);
				TagCoopLabel.color = mRoolListTargetLabelColor;
				TagVsHighlight.SetActive(true);
				TagVsLabel.color = Color.white;
				TagFriendsHighlight.SetActive(false);
				TagFriendsLabel.color = mRoolListTargetLabelColor;
				break;
			case VSTarget.FRIENDS:
				TagCoopHighlight.SetActive(false);
				TagCoopLabel.color = mRoolListTargetLabelColor;
				TagVsHighlight.SetActive(false);
				TagVsLabel.color = mRoolListTargetLabelColor;
				TagFriendsHighlight.SetActive(true);
				TagFriendsLabel.color = Color.white;
				break;
			}
			break;
		case RoomPage.SearchPage:
			RoomSettingObject.SetActive(false);
			RoomSearchPage.SetActive(true);
			SearchInput.gameObject.SetActive(true);
			WaitSearchingObj.SetActive(false);
			RoomTitle.text = RoomSearchTitle;
			DoButtonLabel.text = RoomSearchButtonText;
			break;
		case RoomPage.RoomSetPage:
			RoomSettingObject.SetActive(true);
			RoomSetPage.SetActive(true);
			RoomSetSelectButton.SetHighlight(true);
			SelectButtonList.Reposition();
			if (!mIsRoomSettingInited)
			{
				if (mVSTarget == VSTarget.PVP)
				{
					MaxPlayerSetting.Init(mRoomMaxPlayer);
					MinLevelLimit.Init(mRoomMinLevelLimit);
					MaxLevelLimit.Init(mRoomMaxLevelLimit);
				}
				else
				{
					MaxPlayerSetting.Init(mCoopMaxPlayer);
					MinLevelLimit.Init(mRoomMinLevelLimit);
					MaxLevelLimit.Init(mRoomMaxLevelLimit);
				}
				mIsRoomSettingInited = true;
			}
			break;
		case RoomPage.PasswordCreatePage:
			if (!mIsPasswordCreateInited)
			{
				CreatePasswordInput.value = string.Empty;
				mIsPasswordCreateInited = true;
			}
			RoomSettingObject.SetActive(true);
			RoomPasswordPage.SetActive(true);
			PasswordInputButton.SetHighlight(true);
			SelectButtonList.Reposition();
			break;
		case RoomPage.WaitParnterPage:
			RoomSettingObject.SetActive(false);
			RoomWaitPartnerPage.SetActive(true);
			RoomTitle.text = RoomWaitPartnerTitle;
			DoButtonLabel.text = RoomWaitPartnerText;
			if (!Lobby.GetInstance().IsMasterPlayer)
			{
				DoButton.SetActive(false);
			}
			break;
		case RoomPage.PasswordJoinPage:
			if (!mIsPasswordCreateInited)
			{
				CreatePasswordInput.value = string.Empty;
				mIsPasswordCreateInited = true;
			}
			MapSelectButton.gameObject.SetActive(false);
			ModeSelectButton.gameObject.SetActive(false);
			WinConditionSelectButton.gameObject.SetActive(false);
			RoomSettingObject.SetActive(true);
			RoomPasswordPage.SetActive(true);
			PasswordInputButton.SetHighlight(true);
			SelectButtonList.Reposition();
			DoButtonLabel.text = RoomQuickGameButtonText;
			break;
		case RoomPage.QuickJoinPVPPage:
			mCenterOnChild_Q_PVP.Recenter();
			mLastCheckTime = Time.time;
			needUpdateQuickGamePVP = true;
			InitQuickGamePVP();
			RoomSettingObject.SetActive(true);
			RoomQuickGamePVPPage.SetActive(true);
			RoomSettingCommonObj.SetActive(false);
			break;
		case RoomPage.QuickJoinPVEPage:
			mCenterOnChild_Q_PVE.Recenter();
			mLastCheckTime = Time.time;
			needUpdateQuickGamePVE = true;
			InitQuickGamePVE();
			RoomSettingObject.SetActive(true);
			RoomQuickGamePVEPage.SetActive(true);
			RoomSettingCommonObj.SetActive(false);
			break;
		}
		mCurrentPage = page;
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == DoButton && !mIsDoButtonClicked)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			mIsDoButtonClicked = true;
			if (mCurrentPage == RoomPage.WaitParnterPage)
			{
				if (Lobby.GetInstance().IsMasterPlayer)
				{
					LobbyScript.mInstance.StartGame();
				}
			}
			else if (mCurrentPage == RoomPage.ListPage)
			{
				if (mSelectRoomInList != null)
				{
					if (mSelectRoomInList.IsLevelFit())
					{
						if (mSelectRoomInList.HasPassword())
						{
							ChangePage(RoomPage.PasswordJoinPage);
							return;
						}
						JoinRoomRequest request = new JoinRoomRequest((short)mSelectRoomInList.RoomID, GameApp.GetInstance().GetUserState().GetRank()
							.rankID, TimeManager.GetInstance().Ping);
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
					else
					{
						UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_6"), 1, 41);
					}
				}
				else
				{
					mIsDoButtonClicked = false;
				}
			}
			else if (mCurrentPage == RoomPage.PasswordJoinPage)
			{
				if (CreatePasswordInput.value == mSelectRoomInList.GetPassword().ToString())
				{
					JoinRoomRequest request2 = new JoinRoomRequest((short)mSelectRoomInList.RoomID, GameApp.GetInstance().GetUserState().GetRank()
						.rankID, TimeManager.GetInstance().Ping);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
				else
				{
					UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_26"), 1, 20);
				}
			}
			else if (mCurrentPage == RoomPage.QuickJoinPVPPage)
			{
				byte b = 0;
				EDifficultMode diffMode = EDifficultMode.NORMAL;
				if (mSelectQuickGamePVP != null)
				{
					if (!mSelectQuickGamePVP.IsAvailabel())
					{
						UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_PVP_MUTANT_TIMELIMIT"));
						mIsDoButtonClicked = false;
						return;
					}
					b = (byte)mSelectQuickGamePVP.id;
					diffMode = mSelectQuickGamePVP.mDiffMode;
				}
				bool autoBalance = true;
				byte playerNumber = 12;
				if (b == 13)
				{
					autoBalance = false;
					playerNumber = 8;
				}
				QuickJoinAdvancedRequest request3 = new QuickJoinAdvancedRequest(0, GameApp.GetInstance().GetUserState().GetRank()
					.rankID, 0, 99, b, 0, UIConstant.SEARCH_ROOM_WIN_CONDITION[0], 0, autoBalance, playerNumber, TimeManager.GetInstance().Ping, diffMode);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
			}
			else if (mCurrentPage == RoomPage.QuickJoinPVEPage)
			{
				byte gameMode = 2;
				byte mapID = 1;
				EDifficultMode diffMode2 = EDifficultMode.NORMAL;
				if (mSelectQuickGamePVE != null)
				{
					gameMode = (byte)mSelectQuickGamePVE.mPVEMapMode;
					mapID = (byte)mSelectQuickGamePVE.id;
					diffMode2 = mSelectQuickGamePVE.mDiffMode;
				}
				QuickJoinAdvancedRequest request4 = new QuickJoinAdvancedRequest(1, GameApp.GetInstance().GetUserState().GetRank()
					.rankID, 0, 99, gameMode, mapID, 0, 0, false, 3, TimeManager.GetInstance().Ping, diffMode2);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request4);
			}
			else if (mRequestType == RoomRequestType.Create)
			{
				if (mVSTarget == VSTarget.PVP)
				{
					byte b2 = 0;
					byte mapID2 = 1;
					byte winCondition = UIConstant.SEARCH_ROOM_WIN_CONDITION[0];
					short winValue = 0;
					bool hasPassword = false;
					short pass = 0;
					MaxPlayerSetting.ConfirmCurrent();
					MinLevelLimit.ConfirmCurrent();
					MaxLevelLimit.ConfirmCurrent();
					if (mSelectMode != null)
					{
						Debug.Log(string.Concat("PageType = ", mSelectMode.PageType, "  id = ", mSelectMode.id, "  IsAvailable = ", mSelectMode.IsAvailabel()));
						if (!mSelectMode.IsAvailabel())
						{
							UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_PVP_MUTANT_TIMELIMIT"));
							mIsDoButtonClicked = false;
							return;
						}
						b2 = (byte)mSelectMode.id;
					}
					if (mSelectMap != null)
					{
						mapID2 = (byte)mSelectMap.id;
					}
					if (mSelectWinCondition != null)
					{
						winCondition = mSelectWinCondition.mWinCondition;
						winValue = mSelectWinCondition.mWinValue;
					}
					if (CreatePasswordInput.value != string.Empty)
					{
						hasPassword = true;
						pass = Convert.ToInt16(CreatePasswordInput.value);
					}
					bool isAutoBalance = true;
					if (b2 == 13)
					{
						isAutoBalance = false;
					}
					CreateVSRoomRequest request5 = new CreateVSRoomRequest(Lobby.GetInstance().GetUserName(), pass, (byte)MaxPlayerSetting.GetValue(), hasPassword, b2, mapID2, GameApp.GetInstance().GetUserState().GetRank()
						.rankID, TimeManager.GetInstance().Ping, (byte)MinLevelLimit.GetValue(), (byte)MaxLevelLimit.GetValue(), winCondition, winValue, isAutoBalance);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request5);
				}
				else
				{
					byte mode = 2;
					byte mapID3 = 1;
					bool hasPassword2 = false;
					short pass2 = 0;
					EDifficultMode eDifficultMode = EDifficultMode.NORMAL;
					if (mSelectMap != null)
					{
						mapID3 = (byte)mSelectMap.id;
						mode = (byte)mSelectMap.mPVEMapMode;
						eDifficultMode = mSelectMap.mDiffMode;
					}
					if (CreatePasswordInput.value != string.Empty)
					{
						hasPassword2 = true;
						pass2 = Convert.ToInt16(CreatePasswordInput.value);
					}
					CreateRoomRequest request6 = new CreateRoomRequest(Lobby.GetInstance().GetUserName(), pass2, 3, hasPassword2, mode, mapID3, GameApp.GetInstance().GetUserState().GetRank()
						.rankID, TimeManager.GetInstance().Ping, (byte)MinLevelLimit.GetValue(), (byte)MaxLevelLimit.GetValue(), (byte)eDifficultMode);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request6);
				}
			}
			else if (mCurrentPage == RoomPage.SearchPage)
			{
				SearchRoomRequest request7 = new SearchRoomRequest(SearchInput.value);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request7);
				SearchInput.gameObject.SetActive(false);
				WaitSearchingObj.SetActive(true);
				SetLoadingSprites(0);
				Random_Tip.text = GetRandomTip();
			}
			else
			{
				if (mCurrentPage != RoomPage.ModeSelectPage && mCurrentPage != RoomPage.MapSelectPage && mCurrentPage != RoomPage.WinConditionSelectPage)
				{
					return;
				}
				byte gameMode2 = 0;
				byte winCondition2 = UIConstant.SEARCH_ROOM_WIN_CONDITION[0];
				short winValue2 = 0;
				byte playerNumber2 = 12;
				EDifficultMode diffMode3 = EDifficultMode.NORMAL;
				if (mSelectMode != null)
				{
					gameMode2 = (byte)mSelectMode.id;
				}
				if (mSelectWinCondition != null)
				{
					winCondition2 = mSelectWinCondition.mWinCondition;
					winValue2 = mSelectWinCondition.mWinValue;
				}
				byte mapID4 = 1;
				if (mSelectMap != null)
				{
					mapID4 = (byte)mSelectMap.id;
				}
				if (mVSTarget == VSTarget.PVE)
				{
					mapID4 = 0;
					gameMode2 = 3;
					playerNumber2 = 4;
					if (mSelectMap != null)
					{
						mapID4 = (byte)mSelectMap.id;
						gameMode2 = (byte)mSelectMap.mPVEMapMode;
						diffMode3 = mSelectMap.mDiffMode;
					}
				}
				QuickJoinAdvancedRequest request8 = new QuickJoinAdvancedRequest((byte)mVSTarget, GameApp.GetInstance().GetUserState().GetRank()
					.rankID, 0, 99, gameMode2, mapID4, winCondition2, winValue2, true, playerNumber2, TimeManager.GetInstance().Ping, diffMode3);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request8);
			}
			return;
		}
		if (go == RefreshButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mCurrentPage == RoomPage.ListPage)
			{
				RefreshRoomList();
			}
			return;
		}
		if (go == RoomListTagCoop)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			if (mCurrentPage == RoomPage.ListPage)
			{
				switch (mRoomListTarget)
				{
				case VSTarget.ALL:
					TagVsHighlight.SetActive(false);
					TagVsLabel.color = mRoolListTargetLabelColor;
					TagFriendsHighlight.SetActive(false);
					TagFriendsLabel.color = mRoolListTargetLabelColor;
					mRoomListTarget = VSTarget.PVE;
					break;
				case VSTarget.PVP:
					TagCoopHighlight.SetActive(true);
					TagCoopLabel.color = Color.white;
					TagVsHighlight.SetActive(false);
					TagVsLabel.color = mRoolListTargetLabelColor;
					mRoomListTarget = VSTarget.PVE;
					break;
				case VSTarget.FRIENDS:
					TagCoopHighlight.SetActive(true);
					TagCoopLabel.color = Color.white;
					TagFriendsHighlight.SetActive(false);
					TagFriendsLabel.color = mRoolListTargetLabelColor;
					mRoomListTarget = VSTarget.PVE;
					break;
				}
				RefreshRoomList();
			}
			return;
		}
		if (go == RoomListTagVS)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			if (mCurrentPage == RoomPage.ListPage)
			{
				switch (mRoomListTarget)
				{
				case VSTarget.ALL:
					TagVsHighlight.SetActive(false);
					TagVsLabel.color = mRoolListTargetLabelColor;
					TagFriendsHighlight.SetActive(false);
					TagFriendsLabel.color = mRoolListTargetLabelColor;
					mRoomListTarget = VSTarget.PVP;
					break;
				case VSTarget.PVE:
					TagCoopHighlight.SetActive(false);
					TagCoopLabel.color = mRoolListTargetLabelColor;
					TagVsHighlight.SetActive(true);
					TagVsLabel.color = Color.white;
					mRoomListTarget = VSTarget.PVP;
					break;
				case VSTarget.FRIENDS:
					TagVsHighlight.SetActive(true);
					TagVsLabel.color = Color.white;
					TagFriendsHighlight.SetActive(false);
					TagFriendsLabel.color = mRoolListTargetLabelColor;
					mRoomListTarget = VSTarget.PVP;
					break;
				}
				RefreshRoomList();
			}
			return;
		}
		if (go == RoomListTagFriends)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			if (mCurrentPage == RoomPage.ListPage)
			{
				switch (mRoomListTarget)
				{
				case VSTarget.ALL:
					TagVsHighlight.SetActive(false);
					TagVsLabel.color = mRoolListTargetLabelColor;
					TagCoopHighlight.SetActive(false);
					TagCoopLabel.color = mRoolListTargetLabelColor;
					mRoomListTarget = VSTarget.FRIENDS;
					break;
				case VSTarget.PVE:
					TagCoopHighlight.SetActive(false);
					TagCoopLabel.color = mRoolListTargetLabelColor;
					TagFriendsHighlight.SetActive(true);
					TagFriendsLabel.color = Color.white;
					mRoomListTarget = VSTarget.FRIENDS;
					break;
				case VSTarget.PVP:
					TagVsHighlight.SetActive(false);
					TagVsLabel.color = mRoolListTargetLabelColor;
					TagFriendsHighlight.SetActive(true);
					TagFriendsLabel.color = Color.white;
					mRoomListTarget = VSTarget.FRIENDS;
					break;
				}
				RefreshRoomList();
			}
			return;
		}
		if (go == ReturnButton || go == BackButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			ReturnToLobby();
			return;
		}
		if (go == CreateInListButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mCurrentPage == RoomPage.ListPage)
			{
				switch (mRoomListTarget)
				{
				case VSTarget.PVE:
				case VSTarget.ALL:
					OpenPage(RoomPage.MapSelectPage, RoomRequestType.Create, VSTarget.PVE);
					break;
				case VSTarget.PVP:
					OpenPage(RoomPage.ModeSelectPage, RoomRequestType.Create, VSTarget.PVP);
					break;
				case VSTarget.FRIENDS:
					break;
				}
			}
			return;
		}
		if (go == ModeSelectButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(RoomPage.ModeSelectPage);
			return;
		}
		if (go == MapSelectButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(RoomPage.MapSelectPage);
			return;
		}
		if (go == WinConditionSelectButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(RoomPage.WinConditionSelectPage);
			return;
		}
		if (go == RoomSetSelectButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(RoomPage.RoomSetPage);
			return;
		}
		if (go == PasswordInputButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(RoomPage.PasswordCreatePage);
			return;
		}
		if (go == MaxPlayerSetting.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			MinLevelLimit.ConfirmCurrent();
			MaxLevelLimit.ConfirmCurrent();
			if (mVSTarget == VSTarget.PVP)
			{
				MaxPlayerSetting.OpenSelection(Global.MAX_PLAYER_SELECTIONS);
			}
			else
			{
				MaxPlayerSetting.OpenSelection(Global.MAX_COOP_PLAYERS);
			}
			return;
		}
		if (go == MinLevelLimit.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			MaxPlayerSetting.ConfirmCurrent();
			MaxLevelLimit.ConfirmCurrent();
			List<int> list = new List<int>();
			for (int i = 0; i < Global.PVP_LEVEL_LIMIT_SELECTIONS.Count; i++)
			{
				if (Global.PVP_LEVEL_LIMIT_SELECTIONS[i] < MaxLevelLimit.GetValue() && Global.PVP_LEVEL_LIMIT_SELECTIONS[i] <= GameApp.GetInstance().GetUserState().GetRank()
					.rankID)
				{
					list.Add(Global.PVP_LEVEL_LIMIT_SELECTIONS[i]);
				}
			}
			MinLevelLimit.OpenSelection(list);
			return;
		}
		if (go == MaxLevelLimit.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			MaxPlayerSetting.ConfirmCurrent();
			MinLevelLimit.ConfirmCurrent();
			List<int> list2 = new List<int>();
			for (int j = 0; j < Global.PVP_LEVEL_LIMIT_SELECTIONS.Count; j++)
			{
				if (Global.PVP_LEVEL_LIMIT_SELECTIONS[j] > MinLevelLimit.GetValue() && Global.PVP_LEVEL_LIMIT_SELECTIONS[j] >= GameApp.GetInstance().GetUserState().GetRank()
					.rankID)
				{
					list2.Add(Global.PVP_LEVEL_LIMIT_SELECTIONS[j]);
				}
			}
			MaxLevelLimit.OpenSelection(list2);
			return;
		}
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		VSRoomItemScript component = go.GetComponent<VSRoomItemScript>();
		if (component != null)
		{
			if (component.PageType == RoomPage.ModeSelectPage)
			{
				if (mSelectMode != null)
				{
					mSelectMode.SetHighlight(false);
				}
				mSelectMode = component;
				mSelectMode.SetHighlight(true);
				if (mSelectMode.id != 13)
				{
					RoomSetSelectButton.gameObject.SetActive(true);
					SelectButtonList.Reposition();
					return;
				}
				RoomSetSelectButton.gameObject.SetActive(false);
				MaxPlayerSetting.Init(mRoomMaxPlayer);
				MinLevelLimit.Init(mRoomMinLevelLimit);
				MaxLevelLimit.Init(mRoomMaxLevelLimit);
				SelectButtonList.Reposition();
			}
			else if (component.PageType == RoomPage.WinConditionSelectPage)
			{
				if (mSelectWinCondition != null)
				{
					mSelectWinCondition.SetHighlight(false);
				}
				mSelectWinCondition = component;
				mSelectWinCondition.SetHighlight(true);
			}
			else if (component.PageType != RoomPage.MapSelectPage)
			{
			}
			return;
		}
		RoomTemplateScript component2 = go.GetComponent<RoomTemplateScript>();
		if (!(component2 != null))
		{
			return;
		}
		if (component2 == mSelectRoomInList)
		{
			if (mIsDoButtonClicked)
			{
				return;
			}
			mIsDoButtonClicked = true;
			if (mSelectRoomInList.IsLevelFit())
			{
				if (mSelectRoomInList.HasPassword())
				{
					ChangePage(RoomPage.PasswordJoinPage);
					return;
				}
				JoinRoomRequest request9 = new JoinRoomRequest((short)component2.RoomID, GameApp.GetInstance().GetUserState().GetRank()
					.rankID, TimeManager.GetInstance().Ping);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request9);
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_6"), 1, 41);
			}
		}
		else
		{
			if (mSelectRoomInList != null)
			{
				mSelectRoomInList.SetHighlight(false);
			}
			mSelectRoomInList = component2;
			mSelectRoomInList.SetHighlight(true);
		}
	}

	public void DragFinished()
	{
		needUpdateMaps = false;
		needUpdateQuickGamePVP = false;
		needUpdateQuickGamePVE = false;
	}

	private void InitModes()
	{
		foreach (VSRoomItemScript mode in m_Modes)
		{
			UIEventListener uIEventListener = UIEventListener.Get(mode.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			UnityEngine.Object.DestroyImmediate(mode.gameObject);
		}
		m_Modes.Clear();
		if (mVSTarget != VSTarget.PVE)
		{
			for (int i = 0; i < 7; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(ModeTemplate) as GameObject;
				gameObject.transform.parent = RoomModeSelectPage.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				VSRoomItemScript component = gameObject.GetComponent<VSRoomItemScript>();
				m_Modes.Add(component);
				UIEventListener uIEventListener2 = UIEventListener.Get(gameObject);
				uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			}
			m_Modes[0].transform.Find("Label").GetComponent<UILabel>().text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_1");
			m_Modes[0].SetID(0);
			m_Modes[1].transform.Find("Label").GetComponent<UILabel>().text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_2");
			m_Modes[1].SetID(5);
			m_Modes[2].transform.Find("Label").GetComponent<UILabel>().text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_3");
			m_Modes[2].SetID(4);
			m_Modes[3].transform.Find("Label").GetComponent<UILabel>().text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_4");
			m_Modes[3].SetID(6);
			m_Modes[4].transform.Find("Label").GetComponent<UILabel>().text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_5");
			m_Modes[4].SetID(9);
			m_Modes[5].transform.Find("Label").GetComponent<UILabel>().text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_6");
			m_Modes[5].SetID(12);
			m_Modes[6].transform.Find("Label").GetComponent<UILabel>().text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_7");
			m_Modes[6].SetID(13);
		}
		mIsModePageInited = true;
	}

	private void InitWinConditions()
	{
		foreach (VSRoomItemScript winCondition in m_WinConditions)
		{
			UIEventListener uIEventListener = UIEventListener.Get(winCondition.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			UnityEngine.Object.DestroyImmediate(winCondition.gameObject);
		}
		m_WinConditions.Clear();
		if (mVSTarget == VSTarget.PVP)
		{
			for (int i = 0; i < 7; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(WinConditionTemplate) as GameObject;
				gameObject.transform.parent = RoomWinConditionSelectPage.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				VSRoomItemScript component = gameObject.GetComponent<VSRoomItemScript>();
				m_WinConditions.Add(component);
				UIEventListener uIEventListener2 = UIEventListener.Get(gameObject);
				uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			}
			m_WinConditions[0].SetWinConditionValues(UIConstant.SEARCH_ROOM_WIN_CONDITION[0], 0);
			m_WinConditions[1].SetWinConditionValues(UIConstant.SEARCH_ROOM_WIN_CONDITION[1], 1);
			m_WinConditions[2].SetWinConditionValues(UIConstant.SEARCH_ROOM_WIN_CONDITION[1], 2);
			m_WinConditions[3].SetWinConditionValues(UIConstant.SEARCH_ROOM_WIN_CONDITION[1], 3);
			m_WinConditions[4].SetWinConditionValues(UIConstant.SEARCH_ROOM_WIN_CONDITION[2], 1);
			m_WinConditions[5].SetWinConditionValues(UIConstant.SEARCH_ROOM_WIN_CONDITION[2], 2);
			m_WinConditions[6].SetWinConditionValues(UIConstant.SEARCH_ROOM_WIN_CONDITION[2], 3);
		}
		mIsWinConditionPageInited = true;
	}

	public void InitMapsFocusOn(int centerNumber)
	{
		InitMaps(centerNumber);
	}

	private void InitMaps(int centerNumber)
	{
		Vector4 vector = default(Vector4);
		vector = MapsList.GetComponent<UIPanel>().clipRange;
		vector.x = 0f;
		MapsList.GetComponent<UIPanel>().clipRange = vector;
		MapsList.transform.position = Vector3.zero;
		MapsList.transform.localPosition = new Vector3(0f, -60f, 0f);
		mCenterOnChild = MapsList.transform.GetChild(0).GetComponent<UICenterOnChild>();
		mCenterOnChild.onFinished = DragFinished;
		XLeft = PointLeft.transform.position.x;
		XLeftMiddle = PointLeftMiddle.transform.position.x;
		XMiddle = PointMiddle.transform.position.x;
		XRightMiddle = PointRightMiddle.transform.position.x;
		XRight = PointRight.transform.position.x;
		m_Maps.Clear();
		for (int i = 0; i < MapsList.transform.GetChild(0).childCount; i++)
		{
			m_Maps.Add(MapsList.transform.GetChild(0).GetChild(i).GetComponent<VSRoomItemScript>());
			m_Maps[i].transform.localPosition = new Vector3((i - centerNumber) * MapsSpace, 0f, 0f);
		}
		mIsMapPageInited = true;
		mCenterOnChild.Recenter();
	}

	public void InitRoomList(List<Room> rooms)
	{
		mIsDoButtonClicked = false;
		if (mCurrentPage == RoomPage.SearchPage)
		{
			ChangePage(RoomPage.ListPage);
		}
		UIMsgBox.instance.CloseMessage();
		mSelectRoomInList = null;
		foreach (RoomTemplateScript room in m_Rooms)
		{
			UIEventListener uIEventListener = UIEventListener.Get(room.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			UnityEngine.Object.DestroyImmediate(room.gameObject);
		}
		m_Rooms.Clear();
		foreach (Room room2 in rooms)
		{
			Mode gameMode = (Mode)room2.getGameMode();
			VSTarget vSTarget = (GameApp.GetInstance().GetGameMode().IsCoopMode(gameMode) ? VSTarget.PVE : VSTarget.PVP);
			if (mRoomListTarget == vSTarget || mRoomListTarget == VSTarget.ALL || mRoomListTarget == VSTarget.FRIENDS)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(RoomTemplate) as GameObject;
				gameObject.transform.parent = RoomsGrid.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				RoomTemplateScript component = gameObject.GetComponent<RoomTemplateScript>();
				component.Init(room2.getRoomID(), vSTarget, false, gameMode, room2.getMaxPlayer(), room2.getJoinedPlayer(), room2.getMapID(), room2.getRoomName(), room2.isHasPassword(), room2.getRoomPassword(), room2.getPing(), room2.getDifficultyMode(), room2.getMinJoinRankID(), room2.getMaxJoinRankID());
				m_Rooms.Add(component);
				UIEventListener uIEventListener2 = UIEventListener.Get(gameObject);
				uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			}
		}
		RoomsGrid.Reposition();
	}

	private void InitQuickGamePVP()
	{
		QuickGamePVPList.transform.localPosition = new Vector3(0f, 40f, 0f);
		Vector4 vector = default(Vector4);
		vector = QuickGamePVPList.GetComponent<UIPanel>().clipRange;
		vector.x = 0f;
		QuickGamePVPList.GetComponent<UIPanel>().clipRange = vector;
		QuickGamePVPList.transform.position = Vector3.zero;
		m_QuickGamePVPModes.Clear();
		for (int i = 0; i < QuickGamePVPList.transform.GetChild(0).childCount; i++)
		{
			m_QuickGamePVPModes.Add(QuickGamePVPList.transform.GetChild(0).GetChild(i).GetComponent<VSRoomItemScript>());
		}
		mCenterOnChild_Q_PVP = QuickGamePVPList.transform.GetChild(0).GetComponent<UICenterOnChild>();
		mCenterOnChild_Q_PVP.onFinished = DragFinished;
		XLeft = Q_PVP_PointLeft.transform.position.x;
		XLeftMiddle = Q_PVP_PointLeftMiddle.transform.position.x;
		XMiddle = Q_PVP_PointMiddle.transform.position.x;
		XRightMiddle = Q_PVP_PointRightMiddle.transform.position.x;
		XRight = Q_PVP_PointRight.transform.position.x;
	}

	private void InitQuickGamePVE()
	{
		QuickGamePVEList.transform.localPosition = new Vector3(0f, 40f, 0f);
		Vector4 vector = default(Vector4);
		vector = QuickGamePVEList.GetComponent<UIPanel>().clipRange;
		vector.x = 0f;
		QuickGamePVEList.GetComponent<UIPanel>().clipRange = vector;
		QuickGamePVEList.transform.position = Vector3.zero;
		m_QuickGamePVEMaps.Clear();
		for (int i = 0; i < QuickGamePVEList.transform.GetChild(0).childCount; i++)
		{
			m_QuickGamePVEMaps.Add(QuickGamePVEList.transform.GetChild(0).GetChild(i).GetComponent<VSRoomItemScript>());
		}
		mCenterOnChild_Q_PVE = QuickGamePVEList.transform.GetChild(0).GetComponent<UICenterOnChild>();
		mCenterOnChild_Q_PVE.onFinished = DragFinished;
		XLeft = Q_PVE_PointLeft.transform.position.x;
		XLeftMiddle = Q_PVE_PointLeftMiddle.transform.position.x;
		XMiddle = Q_PVE_PointMiddle.transform.position.x;
		XRightMiddle = Q_PVE_PointRightMiddle.transform.position.x;
		XRight = Q_PVE_PointRight.transform.position.x;
	}

	private void Update()
	{
		if (mCurrentPage == RoomPage.MapSelectPage)
		{
			UpdateMaps();
		}
		else if (mCurrentPage == RoomPage.SearchPage)
		{
			UpdateWaitSearching();
		}
		else if (mCurrentPage == RoomPage.WaitParnterPage)
		{
			UpdateWaitingParnter();
		}
		else if (mCurrentPage == RoomPage.QuickJoinPVPPage)
		{
			UpdateQuickGamePVP();
		}
		else if (mCurrentPage == RoomPage.QuickJoinPVEPage)
		{
			UpdateQuickGamePVE();
		}
	}

	private void UpdateMaps()
	{
		if (needUpdateMaps)
		{
			ArrangeMaps();
		}
		if (!(Time.time - mLastCheckTime > mCheckCenterChildTime))
		{
			return;
		}
		mLastCheckTime = Time.time;
		if (!(mCenterOnChild.centeredObject != null))
		{
			return;
		}
		VSRoomItemScript component = mCenterOnChild.centeredObject.GetComponent<VSRoomItemScript>();
		if (component != null && component.PageType == RoomPage.MapSelectPage && component != mSelectMap)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			if (mSelectMap != null)
			{
				mSelectMap.SetHighlight(false);
			}
			mSelectMap = component;
			mSelectMap.SetHighlight(true);
		}
	}

	public void ArrangeMaps()
	{
		for (int i = 0; i < m_Maps.Count; i++)
		{
			float x = m_Maps[i].transform.position.x;
			float num = 0f;
			float num2 = 0f;
			float num3 = 1f;
			float num4 = 1f;
			float num5 = 1f;
			float num6 = 1f;
			bool flag = false;
			if (XLeft <= x && x <= XLeftMiddle)
			{
				num = XLeft;
				num2 = XLeftMiddle;
				num3 = 0.5f;
				num4 = 0.7f;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else if (XLeftMiddle < x && x <= XMiddle)
			{
				num = XLeftMiddle;
				num2 = XMiddle;
				num3 = 0.7f;
				num4 = 1f;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XMiddle < x && x <= XRightMiddle)
			{
				num = XMiddle;
				num2 = XRightMiddle;
				num3 = 0.7f;
				num4 = 1f;
				flag = true;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XRightMiddle < x && x <= XRight)
			{
				num = XRightMiddle;
				num2 = XRight;
				num3 = 0.5f;
				num4 = 0.7f;
				flag = true;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else
			{
				num = x;
				num2 = x + 1f;
				num3 = 0.5f;
				num4 = 0.5f;
				num5 = 0.3f;
				num6 = 0.3f;
			}
			float t = ((!flag) ? ((x - num) / (num2 - num)) : (1f - (x - num) / (num2 - num)));
			float num7 = Mathf.Lerp(num3, num4, t);
			float alpha = Mathf.Lerp(num5, num6, t);
			m_Maps[i].transform.localScale = Vector3.one * num7;
			m_Maps[i].SetAlpha(alpha);
		}
	}

	public void SetNeedUpdateDragContents()
	{
		needUpdateMaps = true;
		needUpdateQuickGamePVP = true;
		needUpdateQuickGamePVE = true;
	}

	private void UpdateWaitSearching()
	{
		if (Time.time - mLoadingSpriteUpdateTime >= mLoadingSpriteUpdateInterval)
		{
			SetLoadingSprites(mFirstLoadingSpriteIndex++);
		}
	}

	private void SetLoadingSprites(int firstIndex)
	{
		int num = 0;
		for (int i = 0; i < LoadingSprites.Length; i++)
		{
			num = (firstIndex + i) % UIConstant.LOADING_SPRITE_NAMES.Length;
			if (UIConstant.LOADING_SPRITE_NAMES[num].Equals("loading_null"))
			{
				LoadingSprites[i].gameObject.SetActive(false);
				continue;
			}
			LoadingSprites[i].gameObject.SetActive(true);
			LoadingSprites[i].spriteName = UIConstant.LOADING_SPRITE_NAMES[num];
			LoadingSprites[i].MakePixelPerfect();
		}
		mLoadingSpriteUpdateTime = Time.time;
	}

	private string GetRandomTip()
	{
		string empty = string.Empty;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TIP_LIST, DataSheetName.TIP_RANDOM);
		int row = UnityEngine.Random.Range(0, gameDataTable.Count - 1);
		return gameDataTable.GetString(row, 1);
	}

	public RoomPage GetCurrentPage()
	{
		return mCurrentPage;
	}

	private void UpdateWaitingParnter()
	{
	}

	public void ResetParnterPage()
	{
		RoomPlayer[] array = null;
		Room currentRoom = Lobby.GetInstance().GetCurrentRoom();
		if (currentRoom != null)
		{
			array = currentRoom.GetAllPlayers();
			for (int i = 0; i < WaitPlayers.Length; i++)
			{
				WaitPlayers[i].Init(null);
				if (array != null && i < array.Length)
				{
					WaitPlayers[i].Init(array[i]);
				}
			}
			byte mapID = currentRoom.getMapID();
			if (mapID == 0)
			{
				WaitPartnerRoomIcon.spriteName = "LOC_LEVEL_MAP_0_BOSS";
			}
			else
			{
				WaitPartnerRoomIcon.spriteName = "LOC_LEVEL_MAP_" + ((mapID - 1) % Global.TOTAL_ACTIVATED_STAGE + 1);
				if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
				{
					WaitPartnerRoomIcon.spriteName += "_BOSS";
				}
			}
			WaitPartnerRoomIcon.MakePixelPerfect();
			if (Lobby.GetInstance().DifficultMode == EDifficultMode.NORMAL)
			{
				DiffcultyIcon.spriteName = "normal";
			}
			else if (Lobby.GetInstance().DifficultMode == EDifficultMode.HEROIC)
			{
				DiffcultyIcon.spriteName = "hard";
			}
		}
		if (Lobby.GetInstance().IsMasterPlayer)
		{
			DoButton.SetActive(true);
		}
	}

	private void UpdateQuickGamePVP()
	{
		if (needUpdateQuickGamePVP)
		{
			ArrangeQuickGamePVPs();
		}
		if (!(Time.time - mLastCheckTime > mCheckCenterChildTime))
		{
			return;
		}
		mLastCheckTime = Time.time;
		if (!(mCenterOnChild_Q_PVP.centeredObject != null))
		{
			return;
		}
		VSRoomItemScript component = mCenterOnChild_Q_PVP.centeredObject.GetComponent<VSRoomItemScript>();
		if (component != null && component.PageType == RoomPage.QuickJoinPVPPage && component != mSelectQuickGamePVP)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			if (mSelectQuickGamePVP != null)
			{
				mSelectQuickGamePVP.SetHighlight(false);
			}
			mSelectQuickGamePVP = component;
			mSelectQuickGamePVP.SetHighlight(true);
		}
	}

	public void ArrangeQuickGamePVPs()
	{
		for (int i = 0; i < m_QuickGamePVPModes.Count; i++)
		{
			float x = m_QuickGamePVPModes[i].transform.position.x;
			float num = 0f;
			float num2 = 0f;
			float num3 = 1f;
			float num4 = 1f;
			float num5 = 1f;
			float num6 = 1f;
			bool flag = false;
			if (XLeft <= x && x <= XLeftMiddle)
			{
				num = XLeft;
				num2 = XLeftMiddle;
				num3 = 0.5f;
				num4 = 0.7f;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else if (XLeftMiddle < x && x <= XMiddle)
			{
				num = XLeftMiddle;
				num2 = XMiddle;
				num3 = 0.7f;
				num4 = 1f;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XMiddle < x && x <= XRightMiddle)
			{
				num = XMiddle;
				num2 = XRightMiddle;
				num3 = 0.7f;
				num4 = 1f;
				flag = true;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XRightMiddle < x && x <= XRight)
			{
				num = XRightMiddle;
				num2 = XRight;
				num3 = 0.5f;
				num4 = 0.7f;
				flag = true;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else
			{
				num = x;
				num2 = x + 1f;
				num3 = 0.5f;
				num4 = 0.5f;
				num5 = 0.3f;
				num6 = 0.3f;
			}
			float t = ((!flag) ? ((x - num) / (num2 - num)) : (1f - (x - num) / (num2 - num)));
			float num7 = Mathf.Lerp(num3, num4, t);
			float alpha = Mathf.Lerp(num5, num6, t);
			m_QuickGamePVPModes[i].transform.localScale = Vector3.one * num7;
			m_QuickGamePVPModes[i].SetAlpha(alpha);
		}
	}

	private void UpdateQuickGamePVE()
	{
		if (needUpdateQuickGamePVE)
		{
			ArrangeQuickGamePVEs();
		}
		if (!(Time.time - mLastCheckTime > mCheckCenterChildTime))
		{
			return;
		}
		mLastCheckTime = Time.time;
		if (!(mCenterOnChild_Q_PVE.centeredObject != null))
		{
			return;
		}
		VSRoomItemScript component = mCenterOnChild_Q_PVE.centeredObject.GetComponent<VSRoomItemScript>();
		if (component != null && component.PageType == RoomPage.QuickJoinPVEPage && component != mSelectQuickGamePVE)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			if (mSelectQuickGamePVE != null)
			{
				mSelectQuickGamePVE.SetHighlight(false);
			}
			mSelectQuickGamePVE = component;
			mSelectQuickGamePVE.SetHighlight(true);
		}
	}

	public void ArrangeQuickGamePVEs()
	{
		for (int i = 0; i < m_QuickGamePVEMaps.Count; i++)
		{
			float x = m_QuickGamePVEMaps[i].transform.position.x;
			float num = 0f;
			float num2 = 0f;
			float num3 = 1f;
			float num4 = 1f;
			float num5 = 1f;
			float num6 = 1f;
			bool flag = false;
			if (XLeft <= x && x <= XLeftMiddle)
			{
				num = XLeft;
				num2 = XLeftMiddle;
				num3 = 0.5f;
				num4 = 0.7f;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else if (XLeftMiddle < x && x <= XMiddle)
			{
				num = XLeftMiddle;
				num2 = XMiddle;
				num3 = 0.7f;
				num4 = 1f;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XMiddle < x && x <= XRightMiddle)
			{
				num = XMiddle;
				num2 = XRightMiddle;
				num3 = 0.7f;
				num4 = 1f;
				flag = true;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XRightMiddle < x && x <= XRight)
			{
				num = XRightMiddle;
				num2 = XRight;
				num3 = 0.5f;
				num4 = 0.7f;
				flag = true;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else
			{
				num = x;
				num2 = x + 1f;
				num3 = 0.5f;
				num4 = 0.5f;
				num5 = 0.3f;
				num6 = 0.3f;
			}
			float t = ((!flag) ? ((x - num) / (num2 - num)) : (1f - (x - num) / (num2 - num)));
			float num7 = Mathf.Lerp(num3, num4, t);
			float alpha = Mathf.Lerp(num5, num6, t);
			m_QuickGamePVEMaps[i].transform.localScale = Vector3.one * num7;
			m_QuickGamePVEMaps[i].SetAlpha(alpha);
		}
	}

	public void ReturnToLobby()
	{
		if (mCurrentPage == RoomPage.WaitParnterPage)
		{
			LeaveRoomRequest request = new LeaveRoomRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			Lobby.GetInstance().IsMasterPlayer = false;
			Lobby.GetInstance().SetCurrentRoomID(-1);
		}
		base.gameObject.SetActive(false);
		LobbyScript.mInstance.ShowLobbyPage();
	}

	private byte GetRoomType()
	{
		byte result = 0;
		if (mRoomListTarget == VSTarget.ALL)
		{
			result = 0;
		}
		else if (mRoomListTarget == VSTarget.PVP)
		{
			result = 1;
		}
		else if (mRoomListTarget == VSTarget.PVE)
		{
			result = 2;
		}
		else if (mRoomListTarget == VSTarget.FRIENDS)
		{
			result = 3;
		}
		else
		{
			Debug.LogError("mRoomListTarget wrong.");
		}
		return result;
	}

	public void RefreshRoomList()
	{
		UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_25"), 7);
		if (SearchInput.value == string.Empty)
		{
			GetRoomListRequest request = new GetRoomListRequest(GetRoomType(), GameApp.GetInstance().GetUserState().GetRank()
				.rankID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		else
		{
			SearchRoomRequest request2 = new SearchRoomRequest(SearchInput.value);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
		}
		if (mRoomListTarget == VSTarget.FRIENDS)
		{
			CreateInListButton.SetActive(false);
		}
		else
		{
			CreateInListButton.SetActive(true);
		}
	}

	public void OnCreateRoomFail()
	{
		mIsDoButtonClicked = false;
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_4"));
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 41 && buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			mIsDoButtonClicked = false;
			RefreshRoomList();
		}
	}
}

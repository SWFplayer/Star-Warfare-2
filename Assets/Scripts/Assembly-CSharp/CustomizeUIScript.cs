using System;
using UnityEngine;

public class CustomizeUIScript : MonoBehaviour, UIMsgListener
{
	public static CustomizeUIScript mInstance;

	public TweenPosition TweenPanel;

	public GameObject EmptyBackground;

	public GameObject EmptyColider_Left;

	public GameObject EmptyColider_Right;

	public GameObject SelectCloseButton;

	public GameObject LeftButtons;

	public GameObject RightButtons;

	public SelectItemPageScript SelectItemPage;

	public Transform SelectionPositionLeft;

	public Transform SelectionPositionRight;

	public CustomizeItemButton[] Weapons;

	public CustomizeItemButton[] Items;

	public CustomizeItemButton[] Equips;

	public GameObject FBButton;

	public GameObject TTButton;

	public GameObject PlayerInfoButton;

	public SuitStateScript SuitState;

	public GameObject SwVoidButton;

	public Transform SwVoidInit;

	public UISprite SwVoidIcon;

	protected SwVoidMenuScript swVoidMenu;

	public PvpExpIapUI pvpExpIap;

	public CustomizePlayerStatus SecondaryStatusObj;

	private CustomizeItemButton mSelectedButton;

	private UserState mUserState;

	private bool mIsInitialized;

	private bool mIsInterMission;

	private bool mIsInLoadout;

	private int mTargetWeaponID;

	private void Awake()
	{
		mInstance = this;
		SelectItemPage.gameObject.SetActive(false);
		mUserState = GameApp.GetInstance().GetUserState();
		mUserState.EquippedWeaponUnlockPatch();
		mIsInterMission = false;
		mIsInLoadout = false;
		if (InterMissionScript.GetInstance() != null)
		{
			mIsInterMission = true;
			SecondaryStatusObj.gameObject.SetActive(false);
		}
		else if (LoadoutUIScript.Instance != null)
		{
			mIsInLoadout = true;
			SecondaryStatusObj.gameObject.SetActive(true);
		}
		Init();
		mIsInitialized = false;
		UIEventListener uIEventListener = UIEventListener.Get(FBButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener2 = UIEventListener.Get(TTButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener3 = UIEventListener.Get(PlayerInfoButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		if (GameApp.GetInstance().IsConnectedToServer())
		{
			PlayerInfoButton.SetActive(false);
		}
		if (!AndroidConstant.isFacebook())
		{
			FBButton.SetActive(false);
		}
		TTButton.SetActive(false);
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void OnEnable()
	{
		if (mIsInLoadout)
		{
			short[] array = new short[6];
			short[] avatar = GameApp.GetInstance().GetUserState().GetAvatar();
			for (int i = 0; i < avatar.Length; i++)
			{
				array[i] = avatar[i];
			}
			Weapon weapon = GameApp.GetInstance().GetUserState().GetBattleWeapons()[0];
			array[5] = (short)weapon.GetGunID();
			UserState userState = GameApp.GetInstance().GetUserState();
			userState.GetCurrentCustomize().mAvatarColorArray[0].mCurrentEquipID = (byte)array[0];
			userState.GetCurrentCustomize().mAvatarColorArray[1].mCurrentEquipID = (byte)array[1];
			userState.GetCurrentCustomize().mAvatarColorArray[2].mCurrentEquipID = (byte)array[2];
			userState.GetCurrentCustomize().mAvatarColorArray[3].mCurrentEquipID = (byte)array[3];
		}
	}

	protected void Init()
	{
		short[] battleWeaponIDs = mUserState.GetBattleWeaponIDs();
		for (int i = 0; i < 3; i++)
		{
			if (i < Weapons.Length)
			{
				int itemID = 0;
				if (i < battleWeaponIDs.Length)
				{
					itemID = battleWeaponIDs[i];
				}
				Weapons[i].Init(itemID);
				UIEventListener uIEventListener = UIEventListener.Get(Weapons[i].gameObject);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
			}
		}
		short[] battleItemIDs = mUserState.GetBattleItemIDs();
		for (int j = 0; j < 3; j++)
		{
			if (j < Items.Length)
			{
				int itemID2 = 0;
				if (j < battleItemIDs.Length)
				{
					itemID2 = battleItemIDs[j];
				}
				Items[j].Init(itemID2);
				UIEventListener uIEventListener2 = UIEventListener.Get(Items[j].gameObject);
				uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
			}
		}
		short[] avatar = mUserState.GetAvatar();
		for (int k = 0; k < Global.AVATAR_PART_NUM; k++)
		{
			if (k < Equips.Length)
			{
				Equips[k].Init(avatar[k]);
				UIEventListener uIEventListener3 = UIEventListener.Get(Equips[k].gameObject);
				uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClickButton));
			}
		}
		SuitState.RefreshFromEquiped();
		short[] array = new short[6];
		for (int l = 0; l < avatar.Length; l++)
		{
			array[l] = avatar[l];
		}
		if (mTargetWeaponID != 0)
		{
			array[5] = (short)mTargetWeaponID;
		}
		else
		{
			array[5] = (short)mUserState.GetBattleWeapons()[0].GunID;
		}
		if (mIsInterMission)
		{
			InterMissionScript.GetInstance().StatusObj.SetStatus(array, array);
		}
		else if (mIsInLoadout)
		{
			SecondaryStatusObj.SetStatus(array, array);
		}
		RefreshSwVoidIcon();
		UIEventListener uIEventListener4 = UIEventListener.Get(EmptyColider_Left);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener5 = UIEventListener.Get(EmptyColider_Right);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener6 = UIEventListener.Get(SelectCloseButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener7 = UIEventListener.Get(SwVoidButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnClickButton));
	}

	public void ResetAvatar()
	{
		if (mIsInterMission)
		{
			if (InterMissionScript.GetInstance() != null)
			{
				InterMissionScript.GetInstance().ReturnCurrentEquip();
			}
		}
		else if (mIsInLoadout)
		{
			if (LoadoutUIScript.Instance != null)
			{
				LoadoutUIScript.Instance.LoadoutPage.RefreshLoadout();
			}
			short[] avatar = GameApp.GetInstance().GetUserState().GetAvatar();
			short[] array = new short[6];
			for (int i = 0; i < avatar.Length; i++)
			{
				array[i] = avatar[i];
			}
			if (GetTargetWeaponID() != 0)
			{
				array[5] = (short)GetTargetWeaponID();
			}
			else
			{
				array[5] = (short)GameApp.GetInstance().GetUserState().GetBattleWeapons()[0].GunID;
			}
			SecondaryStatusObj.SetStatus(array, array);
		}
	}

	private void OnClickButton(GameObject go)
	{
		if (go == EmptyColider_Left || go == EmptyColider_Right || go == SelectCloseButton)
		{
			if (SelectItemPage.gameObject.activeSelf)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
				SelectEmpty();
			}
			return;
		}
		if (go == SwVoidButton)
		{
			WaitForSwVoid();
			SwVoidMenuScript.TryShow();
			return;
		}
		if (go == FBButton)
		{
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				ScreenCapture.CaptureScreenshot("tempscreens.png");
				if (!FacebookTools.isSessionValid())
				{
					ApplicationPauseManager.mInstance.IsPostingToFacebook = true;
					ApplicationPauseManager.mInstance.currentType = ApplicationPauseManager.EPostingType.CUSTOMIZE;
					FacebookTools.CallFBLogin();
				}
				else
				{
					ApplicationPauseManager.mInstance.StartCoroutine(ApplicationPauseManager.mInstance.postToFacebook(ApplicationPauseManager.EPostingType.CUSTOMIZE));
				}
			}
			return;
		}
		if (go == TTButton)
		{
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				ScreenCapture.CaptureScreenshot("tempscreens.png");
				ApplicationPauseManager.mInstance.StartCoroutine(ApplicationPauseManager.mInstance.postToTwitter(ApplicationPauseManager.EPostingType.CUSTOMIZE));
			}
			return;
		}
		if (go == PlayerInfoButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			GameObject original = Resources.Load("UI/Menu/PlayerInfo/Prefab/PlayerInfoUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			PlayerInfoUI component = gameObject.GetComponent<PlayerInfoUI>();
			component.InitByUserState(GameApp.GetInstance().GetUserState(), true);
			base.gameObject.SetActive(false);
			return;
		}
		CustomizeItemButton component2 = go.GetComponent<CustomizeItemButton>();
		if (component2 != null)
		{
			if (mSelectedButton == component2)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
				SelectEmpty();
				return;
			}
			if (mSelectedButton != null)
			{
				mSelectedButton.Canceled();
			}
			if (component2.ItemType == CustomizeItemType.Bag)
			{
				if (mIsInterMission)
				{
					InterMissionScript.GetInstance().ShowAvatarBag(true);
				}
			}
			else if (mSelectedButton != null && mSelectedButton.ItemType == CustomizeItemType.Bag && mIsInterMission)
			{
				InterMissionScript.GetInstance().ShowAvatarBag(false);
			}
			mSelectedButton = component2;
			component2.Clicked();
		}
		else
		{
			Debug.Log("No script!?");
		}
	}

	public void SelectEmpty()
	{
		SelectItemPage.gameObject.SetActive(false);
		if (mSelectedButton != null)
		{
			mSelectedButton.Canceled();
		}
		mSelectedButton = null;
		LeftButtons.SetActive(true);
		RightButtons.SetActive(true);
		SuitState.SetHighlight(-1);
		short[] avatar = mUserState.GetAvatar();
		short[] array = new short[6];
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		if (mTargetWeaponID != 0)
		{
			array[5] = (short)mTargetWeaponID;
		}
		else
		{
			array[5] = (short)mUserState.GetBattleWeapons()[0].GunID;
		}
		if (mIsInterMission)
		{
			InterMissionScript.GetInstance().StatusObj.SetStatus(array, array);
		}
		else if (mIsInLoadout)
		{
			SecondaryStatusObj.SetStatus(array, array);
		}
		SuitState.RefreshFromEquiped();
		ResetAvatar();
	}

	public CustomizeItemButton GetSelectedButton()
	{
		return mSelectedButton;
	}

	public void OpenSelectItemPage(bool isLeftSide, CustomizeItemType itemType)
	{
		if (isLeftSide)
		{
			LeftButtons.SetActive(false);
			SelectItemPage.transform.position = SelectionPositionLeft.position;
			SelectItemPage.transform.rotation = SelectionPositionLeft.rotation;
		}
		else
		{
			RightButtons.SetActive(false);
			SelectItemPage.transform.position = SelectionPositionRight.position;
			SelectItemPage.transform.rotation = SelectionPositionRight.rotation;
		}
		SelectItemPage.gameObject.SetActive(false);
		SelectItemPage.gameObject.SetActive(true);
		SelectItemPage.OpenPage(itemType);
	}

	public void Refresh()
	{
		SelectEmpty();
		short[] battleWeaponIDs = mUserState.GetBattleWeaponIDs();
		for (int i = 0; i < 3; i++)
		{
			if (i < Weapons.Length)
			{
				int itemID = 0;
				if (i < battleWeaponIDs.Length)
				{
					itemID = battleWeaponIDs[i];
				}
				Weapons[i].Init(itemID);
			}
		}
		short[] battleItemIDs = mUserState.GetBattleItemIDs();
		for (int j = 0; j < 3; j++)
		{
			if (j < Items.Length)
			{
				int itemID2 = 0;
				if (j < battleItemIDs.Length)
				{
					itemID2 = battleItemIDs[j];
				}
				Items[j].Init(itemID2);
			}
		}
		short[] avatar = mUserState.GetAvatar();
		for (int k = 0; k < Global.AVATAR_PART_NUM; k++)
		{
			if (k < Equips.Length)
			{
				Equips[k].Init(avatar[k]);
			}
		}
		SuitState.RefreshFromEquiped();
		short[] array = new short[6];
		for (int l = 0; l < avatar.Length; l++)
		{
			array[l] = avatar[l];
		}
		if (mTargetWeaponID != 0)
		{
			array[5] = (short)mTargetWeaponID;
		}
		else
		{
			array[5] = (short)mUserState.GetBattleWeapons()[0].GunID;
		}
		if (mIsInterMission)
		{
			InterMissionScript.GetInstance().StatusObj.SetStatus(array, array);
		}
		else if (mIsInLoadout)
		{
			SecondaryStatusObj.SetStatus(array, array);
		}
		RefreshSwVoidIcon();
	}

	public void ConfirmEquips(short[] avatars)
	{
		mUserState.SetAvatar(avatars);
		GameApp.GetInstance().Save();
		Refresh();
	}

	public void Show()
	{
		TweenPanel.PlayForward();
		Refresh();
		SuitState.RefreshFromEquiped();
	}

	public void Hide()
	{
		SelectEmpty();
		TweenPanel.PlayReverse();
	}

	private void Update()
	{
		if (!mIsInitialized)
		{
			mIsInitialized = true;
			if (mIsInterMission)
			{
				Hide();
			}
		}
	}

	public bool GetInitialized()
	{
		return mIsInitialized;
	}

	public void ShowSwVoidMenu()
	{
		if (swVoidMenu == null && SwVoidMenuScript.mInstance == null)
		{
			GameObject original = Resources.Load("UI/Menu/SwVoid/Prefab/SwVoidUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, SwVoidInit.position, SwVoidInit.rotation) as GameObject;
			swVoidMenu = gameObject.GetComponent<SwVoidMenuScript>();
		}
		else if (swVoidMenu == null)
		{
			swVoidMenu = SwVoidMenuScript.mInstance;
			SwVoidMenuScript.mInstance.ResetSwVoidSaveDateInEachIcon();
		}
		else
		{
			SwVoidMenuScript.mInstance.ResetSwVoidSaveDateInEachIcon();
		}
		if (!SwVoidMenuScript.mInstance.gameObject.activeSelf)
		{
			SwVoidMenuScript.mInstance.gameObject.SetActive(true);
		}
		StopWaiting();
	}

	public void RefreshTempSuitState(int id)
	{
		int itemType = (int)mSelectedButton.ItemType;
		if (0 <= itemType && itemType <= 3)
		{
			Armor armor = mUserState.GetArmor(itemType, id);
			if (armor != null)
			{
				mInstance.SuitState.Refresh(itemType, armor.SetElement, armor.SuitId);
			}
		}
	}

	public bool IsInterMission()
	{
		return mIsInterMission;
	}

	public bool IsInLoadout()
	{
		return mIsInLoadout;
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId != 23)
		{
			return;
		}
		if (buttonId == UIMsg.UIMsgButton.Ok)
		{
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				LoadoutUIScript.Instance.LoadoutPage.LeaveCustomize();
				LoadoutUIScript.Instance.SaveCurrentCustomize();
				LoadoutUIScript.Instance.HideLoadoutUI();
				LobbyScript.mInstance.ShowLobbyUI();
				UploadLoadoutRequest request = new UploadLoadoutRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				UploadStoreDataRequest request2 = new UploadStoreDataRequest(GameApp.GetInstance().GetUserState());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				GameObject original = Resources.Load("UI/Menu/OptionUI/Prefab/OptionUI") as GameObject;
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
				gameObject.GetComponent<OptionUIScript>().SetIsInMainMenu(true);
				LoginPageScript.mTurnToRegister = true;
				UIMsgBox.instance.CloseMessage();
			}
			else
			{
				LoginPageScript.mTurnToRegister = true;
				Application.LoadLevel("MainMenu");
				UIMsgBox.instance.CloseMessage();
			}
		}
		else
		{
			UIMsgBox.instance.CloseMessage();
		}
	}

	public void RefreshSwVoidIcon()
	{
		if (mUserState.SwVoidIdInUsing != 0)
		{
			if (mUserState.swVoidList.ContainsKey(mUserState.SwVoidIdInUsing))
			{
				SwVoidSavedata swVoidSavedata = mUserState.swVoidList[mUserState.SwVoidIdInUsing];
				SwVoidIcon.spriteName = SwVoidDataManager.GetIconNameOfSwVoid(swVoidSavedata.GetType(), swVoidSavedata.GetLevel());
				SwVoidIcon.gameObject.SetActive(true);
			}
			else
			{
				SwVoidIcon.gameObject.SetActive(false);
			}
		}
		else
		{
			SwVoidIcon.gameObject.SetActive(false);
		}
	}

	public void WaitForSwVoid()
	{
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, string.Empty, 9, 53);
	}

	public void StopWaiting()
	{
		if (UIMsgBox.instance.GetCurrentMessageEventID() == 53)
		{
			UIMsgBox.instance.CloseMessage();
		}
	}

	public int GetTargetWeaponID()
	{
		return mTargetWeaponID;
	}

	public void SetTargetWeaponID(int id)
	{
		mTargetWeaponID = id;
	}

	public void CheckWeaponUpgradePush()
	{
		mUserState.GeneratePushUpgradeWeaponID();
		if (mUserState.GetWeaponUpgradePushID() > 0)
		{
			mUserState.SetTodayUpgradeHasPushed(true);
			GameObject original = Resources.Load("UI/Menu/IAP/Prefab/WeaponUpgradePush") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			WeaponUpgradePushScript component = gameObject.GetComponent<WeaponUpgradePushScript>();
			component.Init(mUserState.GetWeapons()[mUserState.GetWeaponUpgradePushID()]);
		}
	}
}

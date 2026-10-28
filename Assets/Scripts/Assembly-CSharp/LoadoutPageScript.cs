using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutPageScript : MonoBehaviour, UIMsgListener
{
	private const int INIT_PARAMS_NUM = 4;

	public LoadoutTabScript CoopTabScript;

	public List<LoadoutTabScript> VsTabScriptList;

	public List<LoadoutSkillScript> PvpSkillList;

	public List<LoadoutSkillScript> AwardSkillList;

	public GameObject SkillRoot;

	public List<UISprite> WeaponIconList;

	public List<UISprite> ItemIconList;

	public UISprite SwVoidIcon;

	public List<UISprite> SwVoidSkillList;

	public UIDraggablePanel DragPanel;

	public GameObject DropdownButton;

	public GameObject SkillBlock;

	public UIGrid AvatarParamGrid;

	public GameObject AvatarParamTemplete;

	public UILabel PlayerName;

	public UILabel PlayerTitle;

	public SuitStateScript SuitState;

	public GameObject CustomizeCollider;

	public Transform LoadoutAvatarPosition;

	public Transform CustomizeAvatarPosition;

	public bool IsForPVPRebirth;

	public TweenAlpha TipTween;

	private GameObject AvatarFrame;

	private List<AvatarParamScript> mAvatarParamList = new List<AvatarParamScript>();

	private GameObject mModel;

	private Vector3 m_Rotation = new Vector3(0f, 128f, 0f);

	private Vector3 m_Pos;

	private Vector3 m_Scale = Vector3.one;

	private bool mCoopTabSelected;

	private UserState mUserState;

	private string mAnimationName;

	private int mSelectedVSTabId;

	private float mTipTime = 5f;

	private float mTipStartTime;

	private bool mIsShowTip;

	private void Awake()
	{
		mUserState = GameApp.GetInstance().GetUserState();
		PlayerName.text = Lobby.GetInstance().GetUserName();
		AvatarFrame = GameObject.Find("AvatarFrame");
		UIEventListener uIEventListener = UIEventListener.Get(DropdownButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(SkillBlock);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(CoopTabScript.gameObject);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(CustomizeCollider);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		foreach (LoadoutTabScript vsTabScript in VsTabScriptList)
		{
			UIEventListener uIEventListener5 = UIEventListener.Get(vsTabScript.gameObject);
			uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		}
		int onlineCustomizeSize = mUserState.GetOnlinePlayer().GetOnlineCustomizeSize();
		for (int i = onlineCustomizeSize; i < VsTabScriptList.Count; i++)
		{
			VsTabScriptList[i].gameObject.SetActive(false);
		}
		if (!IsForPVPRebirth && !UnlockListPageScript.IsGoToCustomize)
		{
			SelectVsTab(0);
			mCoopTabSelected = false;
			return;
		}
		if (IsForPVPRebirth && GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			SelectCoopTab();
		}
		else
		{
			SelectVsTab(mSelectedVSTabId);
			mCoopTabSelected = false;
		}
		RefreshLoadout();
		UnlockListPageScript.IsGoToCustomize = false;
	}

	public void RefreshLoadout()
	{
		RefreshModels();
		RefreshParams();
		RefreshIcons();
		RefreshSkill();
		RefreshTitle();
		SuitState.RefreshFromEquiped();
	}

	private void RefreshSkill()
	{
		if (mCoopTabSelected)
		{
			SkillRoot.SetActive(false);
			return;
		}
		SkillRoot.SetActive(true);
		OnlineCustomizeData currentCustomize = mUserState.GetCurrentCustomize();
		for (int i = 0; i < PvpSkillList.Count; i++)
		{
			if (i < currentCustomize.mPvpSkillIdArray.Length)
			{
				PvpSkillList[i].SetUnlocked();
				PvpSkillList[i].SetUnselected();
				PvpSkillList[i].SetSkill(mUserState.GetPvpSkillManager().GetPerkById(currentCustomize.mPvpSkillIdArray[i]));
			}
			else
			{
				PvpSkillList[i].SetLocked();
			}
		}
		for (int j = 0; j < AwardSkillList.Count; j++)
		{
			if (j < currentCustomize.mAwardSkillIdArray.Length)
			{
				AwardSkillList[j].SetUnlocked();
				AwardSkillList[j].SetUnselected();
				AwardSkillList[j].SetSkill(mUserState.GetPvpSkillManager().GetKillStreakById(currentCustomize.mAwardSkillIdArray[j]));
			}
			else
			{
				AwardSkillList[j].SetLocked();
			}
		}
	}

	private void ClearParams()
	{
		foreach (AvatarParamScript mAvatarParam in mAvatarParamList)
		{
			mAvatarParam.Recycle();
			mAvatarParam.Hide();
		}
	}

	private void RefreshIcons()
	{
		OnlineCustomizeData currentCustomize = mUserState.GetCurrentCustomize();
		for (int i = 0; i < WeaponIconList.Count; i++)
		{
			if (i + 1 < currentCustomize.mWeaponIdArray.Length && currentCustomize.mWeaponIdArray[i + 1] != 0 && currentCustomize.mWeaponIdArray[i + 1] != currentCustomize.GetFirstWeaponID())
			{
				WeaponIconList[i].gameObject.SetActive(true);
				string text = UIConstant.EQUIPS_ICON_STRING_PREFIX[5];
				text += currentCustomize.mWeaponIdArray[i + 1];
				WeaponIconList[i].spriteName = text;
			}
			else
			{
				WeaponIconList[i].gameObject.SetActive(false);
			}
		}
		for (int j = 0; j < ItemIconList.Count; j++)
		{
			if (j < currentCustomize.mItemIdArray.Length && currentCustomize.mItemIdArray[j] != 0)
			{
				ItemIconList[j].gameObject.SetActive(true);
				ItemIconList[j].spriteName = mUserState.GetItems()[currentCustomize.mItemIdArray[j]].GetIconName();
			}
			else
			{
				ItemIconList[j].gameObject.SetActive(false);
			}
		}
		if (mUserState.SwVoidIdInUsing != 0)
		{
			SwVoidIcon.gameObject.SetActive(true);
			foreach (UISprite swVoidSkill in SwVoidSkillList)
			{
				swVoidSkill.gameObject.SetActive(true);
			}
			if (!mUserState.swVoidList.ContainsKey(mUserState.SwVoidIdInUsing))
			{
				return;
			}
			SwVoidSavedata swVoidSavedata = mUserState.swVoidList[mUserState.SwVoidIdInUsing];
			SwVoidIcon.spriteName = SwVoidDataManager.GetIconNameOfSwVoid(swVoidSavedata.GetType(), swVoidSavedata.GetLevel());
			for (int k = 0; k < SwVoidSkillList.Count; k++)
			{
				if (swVoidSavedata.isPassiveSkillShow(k))
				{
					short passiveSkillId = swVoidSavedata.GetPassiveSkillId(k);
					SwVoidSkillList[k].spriteName = SwVoidDataManager.GetIconNameOfPassiveSkill(passiveSkillId);
					if (swVoidSavedata.isPassiveSkillActive(k))
					{
						SwVoidSkillList[k].alpha = 1f;
					}
					else
					{
						SwVoidSkillList[k].alpha = 0.5f;
					}
				}
				else
				{
					SwVoidSkillList[k].spriteName = "skill_any";
				}
			}
			return;
		}
		SwVoidIcon.gameObject.SetActive(false);
		foreach (UISprite swVoidSkill2 in SwVoidSkillList)
		{
			swVoidSkill2.gameObject.SetActive(false);
		}
	}

	private void RefreshParams()
	{
		ClearParams();
		Refresh4FinalParas();
		RefreshSkillBonusParas();
		DropdownButton.SetActive(true);
		AvatarParamGrid.Reposition();
	}

	private void Refresh4FinalParas()
	{
		for (int i = 0; i < 4; i++)
		{
			if (i >= mAvatarParamList.Count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(AvatarParamTemplete) as GameObject;
				gameObject.name = AvatarParamTemplete.name + i;
				gameObject.transform.parent = AvatarParamGrid.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				AvatarParamScript component = gameObject.GetComponent<AvatarParamScript>();
				mAvatarParamList.Add(component);
			}
			if (i < 4)
			{
				mAvatarParamList[i].Show();
			}
			else
			{
				mAvatarParamList[i].Hide();
			}
		}
		Weapon weapon = mUserState.GetBattleWeapons()[0];
		float finalDamageInUI = weapon.GetFinalDamageInUI(mUserState.playerSkill);
		mAvatarParamList[0].Set(LocalizationManager.GetInstance().GetString("LOC_CUSTOMIZE_POW"), finalDamageInUI.ToString());
		float num = 1f / (weapon.AttackFrenquencyInit / (1f + mUserState.playerSkill.GetSkill(SkillsType.SHOOT_SPEED) + mUserState.playerSkill.GetSkill(weapon.AccType)));
		mAvatarParamList[1].Set(LocalizationManager.GetInstance().GetString("LOC_CUSTOMIZE_FIRERATE"), num.ToString("#0.00"));
		float skill = mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH);
		skill *= 1f + mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE);
		mAvatarParamList[2].Set(LocalizationManager.GetInstance().GetString("LOC_CUSTOMIZE_HP"), skill.ToString());
		float num2 = 6f + mUserState.playerSkill.GetSkill(SkillsType.SPEED_BOOTH);
		mAvatarParamList[3].Set(LocalizationManager.GetInstance().GetString("LOC_CUSTOMIZE_SPD"), num2.ToString());
	}

	private void RefreshSkillBonusParas()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PLAYER_STATUS_ORDER, DataSheetName.STATUS_ORDER);
		int num = 4;
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			SkillsType sType = SkillsType.HP_BOOTH;
			string value = gameDataTable.GetString(i, 1);
			if (Enum.IsDefined(typeof(SkillsType), value))
			{
				sType = (SkillsType)(int)Enum.Parse(typeof(SkillsType), value, true);
			}
			else
			{
				Debug.LogError("No Skill Type!");
			}
			float skill = mUserState.playerSkill.GetSkill(sType);
			if (skill != 0f)
			{
				string paraName = gameDataTable.GetString(i, 2);
				int num2 = gameDataTable.GetInt(i, 3);
				string valueString = skill.ToString();
				if (num2 == 1)
				{
					valueString = skill * 100f + "%";
				}
				if (num >= mAvatarParamList.Count)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(AvatarParamTemplete) as GameObject;
					gameObject.name = AvatarParamTemplete.name + num;
					gameObject.transform.parent = AvatarParamGrid.transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
					AvatarParamScript component = gameObject.GetComponent<AvatarParamScript>();
					mAvatarParamList.Add(component);
				}
				mAvatarParamList[num].Set(paraName, valueString);
				mAvatarParamList[num].Hide();
				num++;
			}
		}
	}

	private void RefreshModels()
	{
		short[] array = new short[6];
		if (mCoopTabSelected)
		{
			short[] avatar = GameApp.GetInstance().GetUserState().GetAvatar();
			for (int i = 0; i < avatar.Length; i++)
			{
				array[i] = avatar[i];
			}
			Weapon weapon = GameApp.GetInstance().GetUserState().GetBattleWeapons()[0];
			array[5] = (short)weapon.GunID;
		}
		else
		{
			OnlineCustomizeData currentCustomize = mUserState.GetCurrentCustomize();
			for (int j = 0; j < Global.AVATAR_PART_NUM; j++)
			{
				array[j] = currentCustomize.mAvatarIdArray[j];
			}
			array[5] = currentCustomize.GetFirstWeaponID();
		}
		if (CustomizeUIScript.mInstance != null)
		{
			CustomizeUIScript.mInstance.SelectItemPage.ResetAvatar(array);
		}
		ChangeAvatar(array, false);
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == DropdownButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			DropdownButton.SetActive(false);
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PLAYER_STATUS_ORDER, DataSheetName.STATUS_ORDER);
			for (int i = 4; i < mAvatarParamList.Count; i++)
			{
				if (mAvatarParamList[i].NeedShow())
				{
					mAvatarParamList[i].Show();
				}
			}
			AvatarParamGrid.Reposition();
			return;
		}
		if (go == SkillBlock)
		{
			if (IsForPVPRebirth)
			{
				TipForPVPRebirth();
			}
			else if (LoadoutUIScript.Instance != null)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
				LoadoutUIScript.Instance.OpenSkillPage();
			}
			return;
		}
		if (go == CoopTabScript.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			SelectCoopTab();
			RefreshLoadout();
			return;
		}
		if (go == CustomizeCollider)
		{
			if (IsForPVPRebirth)
			{
				TipForPVPRebirth();
				return;
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			HideLoadoutPageWithoutModel();
			if (CustomizeUIScript.mInstance == null)
			{
				GameObject original = Resources.Load("UI/Menu/Bag/Prefab/CustomizeUI") as GameObject;
				GameObject gameObject = UnityEngine.Object.Instantiate(original, Vector3.up * 1000f, Quaternion.identity) as GameObject;
				CustomizeUIScript.mInstance.Refresh();
				CustomizeUIScript.mInstance.SuitState.RefreshFromEquiped();
			}
			else
			{
				CustomizeUIScript.mInstance.Refresh();
			}
			CustomizeUIScript.mInstance.gameObject.SetActive(true);
			CustomizeUIScript.mInstance.CheckWeaponUpgradePush();
			AvatarFrame.transform.position = CustomizeAvatarPosition.position;
			return;
		}
		for (int j = 0; j < VsTabScriptList.Count; j++)
		{
			if (go == VsTabScriptList[j].gameObject)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
				SelectVsTab(j);
				RefreshLoadout();
				break;
			}
		}
	}

	private void Update()
	{
		if (IsForPVPRebirth && mIsShowTip && Time.time - mTipStartTime >= mTipTime)
		{
			TipTween.PlayReverse();
			mIsShowTip = false;
		}
	}

	private void SelectCoopTab()
	{
		mCoopTabSelected = true;
		mUserState.LoadCurrentCustomize(true, 0);
		CoopTabScript.OnSelected();
		foreach (LoadoutTabScript vsTabScript in VsTabScriptList)
		{
			vsTabScript.OnDeselected();
		}
	}

	private void SelectVsTab(int index)
	{
		mCoopTabSelected = false;
		if (LoadoutUIScript.Instance != null)
		{
			LoadoutUIScript.Instance.CurrentVsTabId = index;
		}
		mSelectedVSTabId = index;
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.SetPVPNextRebirthLoadoutID(index);
		}
		mUserState.LoadCurrentCustomize(false, index);
		for (int i = 0; i < VsTabScriptList.Count; i++)
		{
			if (i == index)
			{
				VsTabScriptList[i].OnSelected();
			}
			else
			{
				VsTabScriptList[i].OnDeselected();
			}
		}
		CoopTabScript.OnDeselected();
	}

	public void ChangeAvatar(short[] avatar, bool isChangeWeapon)
	{
		ClearModels(true);
		Armor armor = GameApp.GetInstance().GetUserState().GetArmor(4, avatar[4]);
		bool flag = ((armor.GetSkill(SkillsType.FLY) > 0f) ? true : false);
		GameObject gameObject = AvatarBuilder.GetInstance().CreateUIAvatar(avatar, false);
		gameObject.transform.rotation = Quaternion.Euler(m_Rotation);
		Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[avatar[5]];
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
		if (weapon.GetWeaponType() == WeaponType.DoublePistol || weapon.GetWeaponType() == WeaponType.ChargeDoublePistol)
		{
			path = Global.WEAPON_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", avatar[5]);
			original = Resources.Load(path);
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(original);
			gameObject3.AddComponent<MaterialChanger>();
			gameObject3.GetComponent<MaterialChanger>().SetMaterialInfo();
			Transform parent2 = gameObject.transform.Find(BoneName.WeaponL);
			gameObject3.transform.parent = parent2;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
		}
		gameObject.transform.localScale = m_Scale;
		AvatarBuilder.SetModelLayer(gameObject, PhysicsLayer.PLAYER);
		SetGameObject(gameObject);
		mAnimationName = AnimationString.Shop + "_" + weapon.GetAnimationSuffix() + "_" + AnimationString.Idle;
		gameObject.GetComponent<Animation>()[mAnimationName].wrapMode = WrapMode.Loop;
		gameObject.GetComponent<Animation>().Play(mAnimationName);
		SetModel(gameObject);
		gameObject.transform.parent = AvatarFrame.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.Euler(m_Rotation);
		gameObject.transform.localScale = m_Scale;
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

	public void ClearModels(bool ShowGlow)
	{
		if (mModel != null)
		{
			if (ShowGlow)
			{
				GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.SHOP_GLOW).CreateObject(mModel.transform.position, mModel.transform.forward, Quaternion.identity);
				gameObject.transform.localScale = Vector3.one * 0.6f;
				NGUITools.SetLayer(gameObject, PhysicsLayer.PLAYER);
			}
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

	public void ShowLoadoutPage()
	{
		if (mCoopTabSelected)
		{
			SelectCoopTab();
		}
		else
		{
			SelectVsTab(mSelectedVSTabId);
		}
		RefreshLoadout();
		base.gameObject.SetActive(true);
		PlayerName.text = Lobby.GetInstance().GetUserName();
	}

	public void HideLoadoutPage()
	{
		ClearModels(false);
		DragPanel.ResetPosition();
		base.gameObject.SetActive(false);
	}

	public void HideLoadoutPageWithoutModel()
	{
		DragPanel.ResetPosition();
		base.gameObject.SetActive(false);
	}

	public void SaveCurrentCustomize()
	{
		if (LoadoutUIScript.Instance != null)
		{
			mUserState.SaveCurrentCustomize(mCoopTabSelected, LoadoutUIScript.Instance.CurrentVsTabId);
		}
	}

	public void ReadyToLeaveCustomize()
	{
		Debug.Log("ready To leave");
		if (isCurrentCustomizeComplete())
		{
			LeaveCustomize();
		}
		else
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_VOID_UNEQUIPED_TIPS"), 2, 80);
		}
	}

	public void LeaveCustomize()
	{
		SaveCurrentCustomize();
		if (CustomizeUIScript.mInstance != null)
		{
			CustomizeUIScript.mInstance.gameObject.SetActive(false);
		}
		AvatarFrame.transform.position = LoadoutAvatarPosition.position;
		ShowLoadoutPage();
	}

	public int GetSelectedVSTabId()
	{
		return mSelectedVSTabId;
	}

	public bool IsCoopTabSelected()
	{
		return mCoopTabSelected;
	}

	public bool isCurrentCustomizeComplete()
	{
		if (mUserState.GetCurrentCustomize().mSwVoidId == 0)
		{
			return false;
		}
		return true;
	}

	public void RefreshTitle()
	{
		if (!(PlayerTitle == null))
		{
			PlayerTitle.text = mUserState.GetPlayerTitle();
			PlayerTitle.gameObject.GetComponent<UILocalizationX>().Refresh();
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 80)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				LeaveCustomize();
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
	}

	private void OnEnable()
	{
		if (IsForPVPRebirth)
		{
			TipTween.PlayReverse();
			ShowModels();
			return;
		}
		int onlineCustomizeSize = mUserState.GetOnlinePlayer().GetOnlineCustomizeSize();
		for (int i = 0; i < onlineCustomizeSize; i++)
		{
			VsTabScriptList[i].gameObject.SetActive(true);
		}
		for (int j = onlineCustomizeSize; j < VsTabScriptList.Count; j++)
		{
			VsTabScriptList[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < VsTabScriptList.Count; k++)
		{
			if (k == mSelectedVSTabId)
			{
				VsTabScriptList[k].OnSelected();
			}
			else
			{
				VsTabScriptList[k].OnDeselected();
			}
		}
	}

	private void TipForPVPRebirth()
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
		TipTween.Reset();
		TipTween.PlayForward();
		mTipStartTime = Time.time;
		mIsShowTip = true;
	}
}

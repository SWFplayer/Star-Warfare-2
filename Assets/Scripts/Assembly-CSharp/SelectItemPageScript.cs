using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectItemPageScript : MonoBehaviour, UIMsgListener
{
	private const int TAKE_OFF_ID = -2;

	public static SelectItemPageScript mInstance;

	public UIDraggablePanel Weapon_Item_List;

	public GameObject Weapon_Item_Template;

	public UIDraggablePanel Equips_List;

	public GameObject Equips_Template;

	public GameObject TakeOffWeapon_ItemTemplate;

	public GameObject TakeOffWeapon_Item;

	public GameObject Buttons;

	public GameObject EquipButton;

	public UILabel EquipLabel;

	public GameObject UpgradeButton;

	public UISprite UpgradeCurrency;

	public UILabel UpgradePrice;

	public GameObject UpgradeIcon;

	public GameObject UpgradePriceObj;

	public GameObject MaxLevelObj;

	public TweenAlpha EquipTween;

	public TweenAlpha UpgradeTween;

	public GameObject ChangeColorButton;

	public GameObject PaintObj;

	public UISliderColors PreviewColorCurrent;

	public GameObject ResetColorButton;

	public GameObject CancelColorButton;

	public GameObject ConfirmColorButton;

	public GameObject CloseColorButton;

	public GameObject AllSuitCheckObj;

	public GameObject CheckIcon;

	public FloatColorButton[] FloatColors;

	public float ColorMin;

	public float ColorMax = 1f;

	private bool mIsPainting;

	private int mPaintingStartFrame;

	private int mPaintingFrameLock = 6;

	private UISlider SliderCurrent;

	private bool mHasChangedColorToggle;

	private FloatColorButton mSelectedFloatColor;

	private int mSelectedColorModeID;

	private CustomizeItemType mItemType;

	private GameObject mSelectedItem;

	private UserState mUserState;

	private short[] m_baseEquipSel = new short[6];

	private short[] m_equipSel = new short[6];

	public CustomizeItemType CurrentItemType
	{
		get
		{
			return mItemType;
		}
	}

	private FloatColorButton SelectedFloatColor
	{
		get
		{
			return mSelectedFloatColor;
		}
		set
		{
			if (mSelectedFloatColor != null)
			{
				mSelectedFloatColor.SetHighlight(false);
			}
			mSelectedFloatColor = value;
			if (mSelectedFloatColor != null)
			{
				mSelectedFloatColor.SetHighlight(true);
			}
		}
	}

	private void Awake()
	{
		mInstance = this;
		mUserState = GameApp.GetInstance().GetUserState();
		Weapon_Item_List.gameObject.SetActive(false);
		Equips_List.gameObject.SetActive(false);
		PaintObj.SetActive(false);
		mSelectedItem = null;
		UIEventListener uIEventListener = UIEventListener.Get(EquipButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
		UIEventListener uIEventListener2 = UIEventListener.Get(UpgradeButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
		UIEventListener uIEventListener3 = UIEventListener.Get(ChangeColorButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
		UIEventListener uIEventListener4 = UIEventListener.Get(ResetColorButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
		UIEventListener uIEventListener5 = UIEventListener.Get(CancelColorButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
		UIEventListener uIEventListener6 = UIEventListener.Get(ConfirmColorButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
		UIEventListener uIEventListener7 = UIEventListener.Get(CloseColorButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
		UIEventListener uIEventListener8 = UIEventListener.Get(AllSuitCheckObj);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
		bool flag = true;
		if (AndroidConstant.version == AndroidConstant.Version.Kindle && GameApp.GetInstance().IapColorPackStatus == 0)
		{
			flag = false;
		}
		if (flag)
		{
			for (int i = 0; i < FloatColors.Length; i++)
			{
				UIEventListener uIEventListener9 = UIEventListener.Get(FloatColors[i].gameObject);
				uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(FloatColorButtonClicked));
			}
		}
		else
		{
			for (int j = 0; j < FloatColors.Length; j++)
			{
				FloatColors[j].gameObject.SetActive(false);
			}
		}
		InitEquipSel();
		mIsPainting = false;
		mPaintingStartFrame = 0;
		SliderCurrent = PreviewColorCurrent.GetComponent<UISlider>();
		mHasChangedColorToggle = false;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Update()
	{
		if (PaintObj.activeSelf)
		{
			float colorValue = Mathf.PingPong(Time.time, 1f);
			UpdateColor(colorValue);
		}
	}

	public void OpenPage(CustomizeItemType itemType)
	{
		SetItemStatus("---", 0);
		if (CustomizeUIScript.mInstance.GetSelectedButton().mItemID != 0)
		{
			EquipLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIPED");
		}
		else
		{
			EquipLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_SELECT");
		}
		EquipTween.gameObject.SetActive(false);
		UpgradeTween.gameObject.SetActive(false);
		UpgradeButton.GetComponent<UIImageButton>().target.alpha = 0.4f;
		switch (itemType)
		{
		case CustomizeItemType.Head:
		case CustomizeItemType.Body:
		case CustomizeItemType.Arm:
		case CustomizeItemType.Foot:
		case CustomizeItemType.Bag:
		{
			Equips_List.gameObject.SetActive(true);
			List<Armor> list2 = new List<Armor>();
			List<List<Armor>> armor = mUserState.GetArmor();
			List<Armor> list3 = armor[(int)itemType];
			for (int j = 0; j < list3.Count; j++)
			{
				if (list3[j].Level != 0 && list3[j].Level != 15)
				{
					list2.Add(list3[j]);
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate(Equips_Template) as GameObject;
				gameObject3.transform.parent = Equips_List.transform;
				gameObject3.transform.localPosition = Vector3.zero;
				gameObject3.transform.localRotation = Quaternion.identity;
				gameObject3.transform.localScale = Vector3.one;
				SelectItemButton component3 = gameObject3.GetComponent<SelectItemButton>();
				component3.Init(itemType, list2[k].GetArmorID(), CustomizeUIScript.mInstance.IsInLoadout() && !LoadoutUIScript.Instance.LoadoutPage.IsCoopTabSelected());
				if (component3.ItemID == CustomizeUIScript.mInstance.GetSelectedButton().mItemID)
				{
					SetSelect(component3);
				}
				UIEventListener uIEventListener5 = UIEventListener.Get(gameObject3);
				uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
			}
			Equips_List.ResetPosition();
			Equips_List.GetComponent<UIGrid>().Reposition();
			if (itemType != CustomizeItemType.Bag)
			{
				CustomizeUIScript.mInstance.SuitState.SetHighlight((int)itemType);
				ChangeColorButton.SetActive(true);
			}
			else
			{
				CustomizeUIScript.mInstance.SuitState.SetHighlight(-1);
				ChangeColorButton.SetActive(false);
			}
			CustomizeUIScript.mInstance.SuitState.AutoShowDetail(mItemType, itemType);
			break;
		}
		case CustomizeItemType.Weapon:
		{
			Weapon_Item_List.gameObject.SetActive(true);
			ChangeColorButton.SetActive(false);
			List<Weapon> list = new List<Weapon>();
			foreach (Weapon value in mUserState.GetWeapons().Values)
			{
				if (value.Level != 0 && value.Level != 15)
				{
					ScrollEquipsScript.SortWeaponByDisplayOrder(value, list);
				}
			}
			if (mUserState.GetBattleWeapons().Count > 1 && CustomizeUIScript.mInstance.GetSelectedButton().mItemID != 0)
			{
				TakeOffWeapon_Item = UnityEngine.Object.Instantiate(TakeOffWeapon_ItemTemplate) as GameObject;
				TakeOffWeapon_Item.transform.parent = Weapon_Item_List.transform;
				TakeOffWeapon_Item.transform.localPosition = Vector3.zero;
				TakeOffWeapon_Item.transform.localRotation = Quaternion.identity;
				TakeOffWeapon_Item.transform.localScale = Vector3.one;
				UIEventListener uIEventListener3 = UIEventListener.Get(TakeOffWeapon_Item);
				uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Weapon_Item_Template) as GameObject;
				gameObject2.transform.parent = Weapon_Item_List.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.transform.localScale = Vector3.one;
				SelectItemButton component2 = gameObject2.GetComponent<SelectItemButton>();
				component2.Init(itemType, list[i].GunID, CustomizeUIScript.mInstance.IsInLoadout() && !LoadoutUIScript.Instance.LoadoutPage.IsCoopTabSelected());
				if (component2.ItemID == CustomizeUIScript.mInstance.GetSelectedButton().mItemID)
				{
					SetSelect(component2);
					if (list[i].Level != 0 && list[i].Level < Global.MAX_LEVEL_WEAPONW)
					{
						UpgradeTween.gameObject.SetActive(true);
						UpgradeTween.Reset();
						UpgradeButton.GetComponent<UIImageButton>().target.alpha = 1f;
						EquipTween.gameObject.SetActive(false);
					}
				}
				UIEventListener uIEventListener4 = UIEventListener.Get(gameObject2);
				uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
			}
			Weapon_Item_List.ResetPosition();
			Weapon_Item_List.GetComponent<UIGrid>().Reposition();
			int mItemID = CustomizeUIScript.mInstance.GetSelectedButton().mItemID;
			if (mItemID != 0)
			{
				m_baseEquipSel[(int)CustomizeUIScript.mInstance.GetSelectedButton().ItemType] = (short)mItemID;
				m_equipSel[(int)CustomizeUIScript.mInstance.GetSelectedButton().ItemType] = (short)mItemID;
				if (CustomizeUIScript.mInstance.IsInterMission())
				{
					InterMissionScript.GetInstance().ChangeAvatar(m_equipSel, true);
				}
				else if (CustomizeUIScript.mInstance.IsInLoadout())
				{
					LoadoutUIScript.Instance.LoadoutPage.ChangeAvatar(m_equipSel, true);
				}
				CustomizeUIScript.mInstance.RefreshTempSuitState(CustomizeUIScript.mInstance.GetSelectedButton().mItemID);
				if (CustomizeUIScript.mInstance.IsInterMission())
				{
					InterMissionScript.GetInstance().StatusObj.SetStatus(m_baseEquipSel, m_baseEquipSel);
				}
				else if (CustomizeUIScript.mInstance.IsInLoadout())
				{
					CustomizeUIScript.mInstance.SecondaryStatusObj.SetStatus(m_baseEquipSel, m_baseEquipSel);
				}
			}
			CustomizeUIScript.mInstance.SuitState.AutoShowDetail(mItemType, itemType);
			break;
		}
		case CustomizeItemType.Item:
		{
			Weapon_Item_List.gameObject.SetActive(true);
			ChangeColorButton.SetActive(false);
			SetItemStatus("---", 0);
			if (CustomizeUIScript.mInstance.GetSelectedButton().mItemID != 0)
			{
				TakeOffWeapon_Item = UnityEngine.Object.Instantiate(TakeOffWeapon_ItemTemplate) as GameObject;
				TakeOffWeapon_Item.transform.parent = Weapon_Item_List.transform;
				TakeOffWeapon_Item.transform.localPosition = Vector3.zero;
				TakeOffWeapon_Item.transform.localRotation = Quaternion.identity;
				TakeOffWeapon_Item.transform.localScale = Vector3.one;
				UIEventListener uIEventListener = UIEventListener.Get(TakeOffWeapon_Item);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
			}
			Dictionary<int, Item> items = GameApp.GetInstance().GetUserState().GetItems();
			Debug.Log("IIIIIIIIIIIIItem.count = " + items.Count);
			foreach (Item value2 in items.Values)
			{
				if (value2.GetItemType() == Item.ItemType.Potion && value2.GetCount() > 0)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(Weapon_Item_Template) as GameObject;
					gameObject.transform.parent = Weapon_Item_List.transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
					SelectItemButton component = gameObject.GetComponent<SelectItemButton>();
					component.Init(itemType, value2.GetItemID(), false);
					if (component.ItemID == CustomizeUIScript.mInstance.GetSelectedButton().mItemID)
					{
						SetSelect(component);
					}
					UIEventListener uIEventListener2 = UIEventListener.Get(gameObject);
					uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
				}
			}
			Weapon_Item_List.ResetPosition();
			Weapon_Item_List.GetComponent<UIGrid>().Reposition();
			CustomizeUIScript.mInstance.SuitState.AutoShowDetail(mItemType, itemType);
			break;
		}
		}
		mItemType = itemType;
	}

	private void SetSelect(SelectItemButton button)
	{
		button.SelectObj.gameObject.SetActive(true);
		button.SelectFrame.gameObject.SetActive(true);
		button.SelectFrame.GetComponent<TweenWidth>().Reset();
		button.SelectFrame.GetComponent<TweenWidth>().PlayForward();
		button.SelectFrame.GetComponent<TweenHeight>().Reset();
		button.SelectFrame.GetComponent<TweenHeight>().PlayForward();
		mSelectedItem = button.gameObject;
		SetItemStatus("---", 0);
		RefreshItemStatus(button.ItemID);
	}

	private void SelectItemButtonClicked(GameObject go)
	{
		SelectItemButton selectItemButton = null;
		if (mSelectedItem != null)
		{
			selectItemButton = mSelectedItem.GetComponent<SelectItemButton>();
		}
		SelectItemButton component = go.GetComponent<SelectItemButton>();
		if (go == EquipButton)
		{
			if (selectItemButton != null)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_EQUIPPED);
				if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Weapon)
				{
					if (CustomizeUIScript.mInstance.IsInterMission())
					{
						InterMissionScript.GetInstance().ConfirmEquipAnimation(true);
					}
					int mItemID = CustomizeUIScript.mInstance.GetSelectedButton().mItemID;
					int itemID = selectItemButton.ItemID;
					for (int i = 0; i < mUserState.GetBattleWeaponIDs().Length; i++)
					{
						if (itemID == mUserState.GetBattleWeaponIDs()[i])
						{
							mUserState.SetBattleWeaponID(i, mItemID);
							break;
						}
					}
					mUserState.SetBattleWeaponID(CustomizeUIScript.mInstance.GetSelectedButton().ButtonId, itemID);
				}
				else if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Item)
				{
					int mItemID2 = CustomizeUIScript.mInstance.GetSelectedButton().mItemID;
					int itemID2 = selectItemButton.ItemID;
					for (int j = 0; j < mUserState.GetBattleItemIDs().Length; j++)
					{
						if (itemID2 == mUserState.GetBattleItemIDs()[j])
						{
							mUserState.SetBattleItemID(j, mItemID2);
							break;
						}
					}
					mUserState.SetBattleItemID(CustomizeUIScript.mInstance.GetSelectedButton().ButtonId, itemID2);
				}
				ConfirmSelection();
			}
			else if (mSelectedItem == TakeOffWeapon_Item)
			{
				if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Weapon)
				{
					AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
					mUserState.SetBattleWeaponID(CustomizeUIScript.mInstance.GetSelectedButton().ButtonId, 0);
				}
				else if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Item)
				{
					AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
					mUserState.SetBattleItemID(CustomizeUIScript.mInstance.GetSelectedButton().ButtonId, 0);
				}
				ConfirmSelection();
			}
			if (mUserState.GetBattleWeapons().Count == 3 && mUserState.GetArmor().Count == 5 && mUserState.SwVoidIdInUsing != 0)
			{
				mUserState.Achievement.CheckAchievement_FullyArmored();
			}
		}
		else if (go == UpgradeButton)
		{
			if (selectItemButton != null)
			{
				if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Weapon)
				{
					int itemID3 = selectItemButton.ItemID;
					Weapon weapon = mUserState.GetWeapons()[itemID3];
					if (weapon.Level != 0 && weapon.Level < Global.MAX_LEVEL_WEAPONW)
					{
						if (mUserState.GetCash() >= weapon.GetDamageUpgradePrice())
						{
							AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
							byte level = weapon.Level;
							mUserState.Buy(weapon.GetDamageUpgradePrice());
							weapon.Upgrade();
							RefreshItemStatus(itemID3);
							GameApp.GetInstance().Save();
							selectItemButton.Refresh();
							if (CustomizeUIScript.mInstance.IsInterMission())
							{
								InterMissionScript.GetInstance().StatusObj.SetStatus(m_baseEquipSel, m_equipSel);
							}
							else if (CustomizeUIScript.mInstance.IsInLoadout())
							{
								CustomizeUIScript.mInstance.SecondaryStatusObj.SetStatus(m_baseEquipSel, m_equipSel);
							}
							byte level2 = weapon.Level;
							GameObject original = Resources.Load("UI/Menu/LevelUpWindow/Prefab/WeaponLevelUp") as GameObject;
							GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
							WeaponLevelUpWindow component2 = gameObject.GetComponent<WeaponLevelUpWindow>();
							component2.Init(weapon, level, level2);
						}
						else
						{
							UIMsgBox.instance.ShowMoneyNotEnoughMessage(this);
						}
					}
					else
					{
						AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
					}
				}
				else
				{
					AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
				}
			}
			else
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			}
		}
		else if (go == TakeOffWeapon_Item)
		{
			if (selectItemButton != null)
			{
				selectItemButton.SelectObj.gameObject.SetActive(false);
				selectItemButton.SelectFrame.gameObject.SetActive(false);
			}
			else if (mSelectedItem == TakeOffWeapon_Item)
			{
				if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Weapon)
				{
					AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
					mUserState.SetBattleWeaponID(CustomizeUIScript.mInstance.GetSelectedButton().ButtonId, 0);
				}
				else if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Item)
				{
					AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
					mUserState.SetBattleItemID(CustomizeUIScript.mInstance.GetSelectedButton().ButtonId, 0);
				}
				ConfirmSelection();
				return;
			}
			GameObject gameObject2 = TakeOffWeapon_Item.transform.Find("Selected").gameObject;
			gameObject2.SetActive(true);
			gameObject2.GetComponent<TweenWidth>().Reset();
			gameObject2.GetComponent<TweenWidth>().PlayForward();
			gameObject2.GetComponent<TweenHeight>().Reset();
			gameObject2.GetComponent<TweenHeight>().PlayForward();
			mSelectedItem = TakeOffWeapon_Item;
		}
		else if (go == ChangeColorButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
			CustomizeItemType itemType = CustomizeUIScript.mInstance.GetSelectedButton().ItemType;
			if (itemType == CustomizeItemType.Item || !(mSelectedItem != TakeOffWeapon_Item))
			{
				return;
			}
			if (!mIsPainting)
			{
				mIsPainting = true;
				PaintObj.SetActive(true);
				Buttons.SetActive(false);
				int itemID4 = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
				float num = 0f;
				Color color = default(Color);
				num = (int)mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mSliderValue;
				color = mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mColor;
				if (mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mMode > 1)
				{
					SelectedFloatColor = FloatColors[mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mMode - 2];
					PreviewColorCurrent.sprite.gameObject.SetActive(false);
				}
				else if (mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mMode == 1)
				{
					SelectedFloatColor = null;
					PreviewColorCurrent.sprite.gameObject.SetActive(true);
				}
				else
				{
					SelectedFloatColor = null;
					num = 0f;
					color = new Color(1f, 0f, 0f);
					PreviewColorCurrent.sprite.gameObject.SetActive(true);
				}
				if (mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mCurrentEquipID != m_equipSel[(int)itemType])
				{
					SelectedFloatColor = null;
					num = 0f;
					color = new Color(1f, 0f, 0f);
					PreviewColorCurrent.sprite.gameObject.SetActive(true);
					CheckIcon.SetActive(false);
				}
				mPaintingStartFrame = 0;
				PreviewColorCurrent.sprite.color = color;
				SliderCurrent.value = num / 100f;
			}
			else
			{
				HidePaintObj();
				CustomizeUIScript.mInstance.ResetAvatar();
			}
		}
		else if (go == ResetColorButton)
		{
			PreviewColorCurrent.sprite.gameObject.SetActive(true);
			SliderCurrent.value = 0f;
			SelectedFloatColor = null;
			ResetColor();
		}
		else if (go == CancelColorButton || go == CloseColorButton)
		{
			HidePaintObj();
			CustomizeUIScript.mInstance.ResetAvatar();
		}
		else if (go == ConfirmColorButton)
		{
			if (CheckIcon.activeSelf)
			{
				ConfirmAllColor();
			}
			else
			{
				ConfirmCurrentColor();
			}
			mHasChangedColorToggle = false;
			HidePaintObj();
			GameApp.GetInstance().Save();
		}
		else if (go == AllSuitCheckObj)
		{
			CheckIcon.SetActive(!CheckIcon.activeSelf);
		}
		else
		{
			if (!(component != null))
			{
				return;
			}
			if (component.LockObj != null && component.LockObj.activeSelf)
			{
				CustomizeUIScript.mInstance.pvpExpIap.gameObject.SetActive(true);
				CustomizeUIScript.mInstance.pvpExpIap.InitInfo("LOC_PVP_IAP_HINT1");
				return;
			}
			if (CustomizeUIScript.mInstance.GetSelectedButton().mItemID == component.ItemID)
			{
				EquipLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIPED");
				EquipTween.gameObject.SetActive(false);
			}
			else
			{
				EquipLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIP");
				EquipTween.gameObject.SetActive(true);
				EquipTween.Reset();
				UpgradeTween.gameObject.SetActive(false);
				UpgradeButton.GetComponent<UIImageButton>().target.alpha = 0.4f;
			}
			if (selectItemButton == component)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_EQUIPPED);
				if (CustomizeUIScript.mInstance.IsInterMission())
				{
					InterMissionScript.GetInstance().ConfirmEquipAnimation(true);
				}
				if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Weapon)
				{
					int mItemID3 = CustomizeUIScript.mInstance.GetSelectedButton().mItemID;
					int itemID5 = component.ItemID;
					for (int k = 0; k < mUserState.GetBattleWeaponIDs().Length; k++)
					{
						if (itemID5 == mUserState.GetBattleWeaponIDs()[k])
						{
							mUserState.SetBattleWeaponID(k, mItemID3);
							break;
						}
					}
					mUserState.SetBattleWeaponID(CustomizeUIScript.mInstance.GetSelectedButton().ButtonId, itemID5);
				}
				else if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Item)
				{
					int mItemID4 = CustomizeUIScript.mInstance.GetSelectedButton().mItemID;
					int itemID6 = component.ItemID;
					for (int l = 0; l < mUserState.GetBattleItemIDs().Length; l++)
					{
						if (itemID6 == mUserState.GetBattleItemIDs()[l])
						{
							mUserState.SetBattleItemID(l, mItemID4);
							break;
						}
					}
					mUserState.SetBattleItemID(CustomizeUIScript.mInstance.GetSelectedButton().ButtonId, itemID6);
				}
				ConfirmSelection();
				return;
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
			RefreshItemStatus(component.ItemID);
			if (selectItemButton != null)
			{
				selectItemButton.SelectObj.gameObject.SetActive(false);
				selectItemButton.SelectFrame.gameObject.SetActive(false);
			}
			else if (mSelectedItem == TakeOffWeapon_Item)
			{
				TakeOffWeapon_Item.transform.Find("Selected").gameObject.SetActive(false);
			}
			component.SelectObj.SetActive(true);
			component.SelectFrame.gameObject.SetActive(true);
			component.SelectFrame.GetComponent<TweenWidth>().Reset();
			component.SelectFrame.GetComponent<TweenWidth>().PlayForward();
			component.SelectFrame.GetComponent<TweenHeight>().Reset();
			component.SelectFrame.GetComponent<TweenHeight>().PlayForward();
			mSelectedItem = go;
			if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType != CustomizeItemType.Item)
			{
				UpdateAvatar(component.ItemID);
			}
			if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType != CustomizeItemType.Weapon)
			{
				return;
			}
			Weapon weapon2 = mUserState.GetWeapons()[component.ItemID];
			if (CustomizeUIScript.mInstance.GetSelectedButton().mItemID == component.ItemID)
			{
				if (weapon2.Level != 0 && weapon2.Level < Global.MAX_LEVEL_WEAPONW)
				{
					UpgradeTween.gameObject.SetActive(true);
					UpgradeTween.Reset();
					UpgradeButton.GetComponent<UIImageButton>().target.alpha = 1f;
				}
				else
				{
					UpgradeTween.gameObject.SetActive(false);
					UpgradeButton.GetComponent<UIImageButton>().target.alpha = 0.4f;
				}
			}
			else
			{
				UpgradeTween.gameObject.SetActive(false);
				if (weapon2.Level != 0 && weapon2.Level < Global.MAX_LEVEL_WEAPONW)
				{
					UpgradeButton.GetComponent<UIImageButton>().target.alpha = 1f;
				}
				else
				{
					UpgradeButton.GetComponent<UIImageButton>().target.alpha = 0.4f;
				}
			}
		}
	}

	public void ClearSelectButtons()
	{
		HidePaintObj();
		int childCount = Weapon_Item_List.transform.childCount;
		int childCount2 = Equips_List.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			UIEventListener uIEventListener = UIEventListener.Get(Weapon_Item_List.transform.GetChild(0).gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
			UnityEngine.Object.DestroyImmediate(Weapon_Item_List.transform.GetChild(0).gameObject);
		}
		for (int j = 0; j < childCount2; j++)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(Equips_List.transform.GetChild(0).gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener2.onClick, new UIEventListener.VoidDelegate(SelectItemButtonClicked));
			UnityEngine.Object.DestroyImmediate(Equips_List.transform.GetChild(0).gameObject);
		}
		mSelectedItem = null;
		TakeOffWeapon_Item = TakeOffWeapon_ItemTemplate;
	}

	public void UpdateAvatar(int id)
	{
		bool isChangeWeapon = false;
		m_equipSel[(int)CustomizeUIScript.mInstance.GetSelectedButton().ItemType] = (short)id;
		if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Weapon)
		{
			isChangeWeapon = true;
		}
		if (CustomizeUIScript.mInstance.IsInterMission())
		{
			InterMissionScript.GetInstance().ChangeAvatar(m_equipSel, isChangeWeapon);
		}
		else if (CustomizeUIScript.mInstance.IsInLoadout())
		{
			LoadoutUIScript.Instance.LoadoutPage.ChangeAvatar(m_equipSel, isChangeWeapon);
		}
		CustomizeUIScript.mInstance.RefreshTempSuitState(id);
		if (CustomizeUIScript.mInstance.IsInterMission())
		{
			InterMissionScript.GetInstance().StatusObj.SetStatus(m_baseEquipSel, m_equipSel);
		}
		else if (CustomizeUIScript.mInstance.IsInLoadout())
		{
			CustomizeUIScript.mInstance.SecondaryStatusObj.SetStatus(m_baseEquipSel, m_equipSel);
		}
	}

	public void ConfirmSelection()
	{
		short[] array = new short[Global.AVATAR_PART_NUM];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = m_equipSel[i];
			m_baseEquipSel[i] = m_equipSel[i];
		}
		bool flag = true;
		short[] battleWeaponIDs = mUserState.GetBattleWeaponIDs();
		foreach (short num in battleWeaponIDs)
		{
			if (num == m_equipSel[5])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			m_equipSel[5] = (short)mUserState.GetBattleWeapons()[0].GunID;
		}
		m_baseEquipSel[5] = m_equipSel[5];
		CustomizeUIScript.mInstance.SetTargetWeaponID(m_equipSel[5]);
		CustomizeUIScript.mInstance.ConfirmEquips(array);
	}

	public void OnDisable()
	{
		ClearSelectButtons();
	}

	public void ChangeCurrentColor()
	{
		if (mPaintingStartFrame <= mPaintingFrameLock)
		{
			mPaintingStartFrame++;
		}
		if (!mIsPainting || CheckIcon.activeSelf || mPaintingStartFrame <= mPaintingFrameLock)
		{
			return;
		}
		PreviewColorCurrent.sprite.gameObject.SetActive(true);
		SelectedFloatColor = null;
		CustomizeItemType itemType = CustomizeUIScript.mInstance.GetSelectedButton().ItemType;
		int itemID = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
		GameObject gameObject = null;
		if (CustomizeUIScript.mInstance.IsInterMission())
		{
			gameObject = InterMissionScript.GetInstance().GetModel();
		}
		else if (CustomizeUIScript.mInstance.IsInLoadout())
		{
			gameObject = LoadoutUIScript.Instance.LoadoutPage.GetModel();
		}
		if (!(gameObject != null))
		{
			return;
		}
		ColorMax = Mathf.Clamp(ColorMax, 0f, 1f);
		ColorMin = Mathf.Clamp(ColorMin, 0f, ColorMax);
		byte b = (byte)(SliderCurrent.value * 100f);
		Color color = PreviewColorCurrent.sprite.color;
		color.r = Mathf.Clamp(color.r, ColorMin, ColorMax);
		color.g = Mathf.Clamp(color.g, ColorMin, ColorMax);
		color.b = Mathf.Clamp(color.b, ColorMin, ColorMax);
		color.a = 1f;
		float value = SliderCurrent.value;
		Transform transform = null;
		if (itemType != CustomizeItemType.Weapon)
		{
			transform = gameObject.transform.Find(UIConstant.SUB_AVATAR[(int)itemType]);
			CharacterShaderController characterShaderController = transform.GetComponent<CharacterShaderController>();
			if (characterShaderController == null)
			{
				characterShaderController = transform.GetComponentInChildren<CharacterShaderController>();
				transform = ((!(characterShaderController != null)) ? null : characterShaderController.transform);
			}
			if (transform != null)
			{
				mHasChangedColorToggle |= characterShaderController.SetUserCustomColor(true);
				characterShaderController.UpdateColor(value, color);
				SetColorMode(1);
				mSelectedColorModeID = 1;
			}
		}
	}

	public void ChangeAllColor()
	{
		if (mPaintingStartFrame <= mPaintingFrameLock)
		{
			mPaintingStartFrame++;
		}
		if (!mIsPainting || !CheckIcon.activeSelf || mPaintingStartFrame <= mPaintingFrameLock)
		{
			return;
		}
		PreviewColorCurrent.sprite.gameObject.SetActive(true);
		SelectedFloatColor = null;
		CustomizeItemType itemType = CustomizeUIScript.mInstance.GetSelectedButton().ItemType;
		GameObject gameObject = null;
		if (CustomizeUIScript.mInstance.IsInterMission())
		{
			gameObject = InterMissionScript.GetInstance().GetModel();
		}
		else if (CustomizeUIScript.mInstance.IsInLoadout())
		{
			gameObject = LoadoutUIScript.Instance.LoadoutPage.GetModel();
		}
		if (gameObject != null)
		{
			ColorMax = Mathf.Clamp(ColorMax, 0f, 1f);
			ColorMin = Mathf.Clamp(ColorMin, 0f, ColorMax);
			byte b = (byte)(SliderCurrent.value * 100f);
			Color color = PreviewColorCurrent.sprite.color;
			color.r = Mathf.Clamp(color.r, ColorMin, ColorMax);
			color.g = Mathf.Clamp(color.g, ColorMin, ColorMax);
			color.b = Mathf.Clamp(color.b, ColorMin, ColorMax);
			color.a = 1f;
			float value = SliderCurrent.value;
			SetColorMode(1);
			mSelectedColorModeID = 1;
			Transform transform = gameObject.transform.Find(UIConstant.SUB_AVATAR[0]);
			if (transform != null && transform.GetComponent<Renderer>().material.HasProperty("_paintColor"))
			{
				mHasChangedColorToggle |= transform.GetComponent<CharacterShaderController>().SetUserCustomColor(true);
				transform.GetComponent<CharacterShaderController>().UpdateColor(value, color);
			}
			Transform transform2 = gameObject.transform.Find(UIConstant.SUB_AVATAR[1]);
			if (transform2 != null && transform2.GetComponent<Renderer>().material.HasProperty("_paintColor"))
			{
				mHasChangedColorToggle |= transform2.GetComponent<CharacterShaderController>().SetUserCustomColor(true);
				transform2.GetComponent<CharacterShaderController>().UpdateColor(value, color);
			}
			Transform transform3 = gameObject.transform.Find(UIConstant.SUB_AVATAR[2]);
			if (transform3 != null && transform3.GetComponent<Renderer>().material.HasProperty("_paintColor"))
			{
				mHasChangedColorToggle |= transform3.GetComponent<CharacterShaderController>().SetUserCustomColor(true);
				transform3.GetComponent<CharacterShaderController>().UpdateColor(value, color);
			}
			Transform transform4 = gameObject.transform.Find(UIConstant.SUB_AVATAR[3]);
			if (transform4 != null && transform4.GetComponent<Renderer>().material.HasProperty("_paintColor"))
			{
				mHasChangedColorToggle |= transform4.GetComponent<CharacterShaderController>().SetUserCustomColor(true);
				transform4.GetComponent<CharacterShaderController>().UpdateColor(value, color);
			}
		}
	}

	public void ConfirmCurrentColor()
	{
		CustomizeItemType itemType = CustomizeUIScript.mInstance.GetSelectedButton().ItemType;
		int itemID = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
		ColorMax = Mathf.Clamp(ColorMax, 0f, 1f);
		ColorMin = Mathf.Clamp(ColorMin, 0f, ColorMax);
		byte mSliderValue = (byte)(SliderCurrent.value * 100f);
		Color color = PreviewColorCurrent.sprite.color;
		color.r = Mathf.Clamp(color.r, ColorMin, ColorMax);
		color.g = Mathf.Clamp(color.g, ColorMin, ColorMax);
		color.b = Mathf.Clamp(color.b, ColorMin, ColorMax);
		color.a = 1f;
		float value = SliderCurrent.value;
		mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mColor = color;
		mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mSliderValue = mSliderValue;
		mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mMode = (byte)mSelectedColorModeID;
		mUserState.GetCurrentCustomize().mAvatarColorArray[(int)itemType].mCurrentEquipID = (byte)m_equipSel[(int)itemType];
	}

	public void ConfirmAllColor()
	{
		CustomizeItemType itemType = CustomizeUIScript.mInstance.GetSelectedButton().ItemType;
		int itemID = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
		ColorMax = Mathf.Clamp(ColorMax, 0f, 1f);
		ColorMin = Mathf.Clamp(ColorMin, 0f, ColorMax);
		byte mSliderValue = (byte)(SliderCurrent.value * 100f);
		Color color = PreviewColorCurrent.sprite.color;
		color.r = Mathf.Clamp(color.r, ColorMin, ColorMax);
		color.g = Mathf.Clamp(color.g, ColorMin, ColorMax);
		color.b = Mathf.Clamp(color.b, ColorMin, ColorMax);
		color.a = 1f;
		float value = SliderCurrent.value;
		mUserState.GetCurrentCustomize().mAvatarColorArray[0].mColor = color;
		mUserState.GetCurrentCustomize().mAvatarColorArray[0].mSliderValue = mSliderValue;
		mUserState.GetCurrentCustomize().mAvatarColorArray[0].mMode = (byte)mSelectedColorModeID;
		mUserState.GetCurrentCustomize().mAvatarColorArray[0].mCurrentEquipID = (byte)m_equipSel[0];
		mUserState.GetCurrentCustomize().mAvatarColorArray[1].mColor = color;
		mUserState.GetCurrentCustomize().mAvatarColorArray[1].mSliderValue = mSliderValue;
		mUserState.GetCurrentCustomize().mAvatarColorArray[1].mMode = (byte)mSelectedColorModeID;
		mUserState.GetCurrentCustomize().mAvatarColorArray[1].mCurrentEquipID = (byte)m_equipSel[1];
		mUserState.GetCurrentCustomize().mAvatarColorArray[2].mColor = color;
		mUserState.GetCurrentCustomize().mAvatarColorArray[2].mSliderValue = mSliderValue;
		mUserState.GetCurrentCustomize().mAvatarColorArray[2].mMode = (byte)mSelectedColorModeID;
		mUserState.GetCurrentCustomize().mAvatarColorArray[2].mCurrentEquipID = (byte)m_equipSel[2];
		mUserState.GetCurrentCustomize().mAvatarColorArray[3].mColor = color;
		mUserState.GetCurrentCustomize().mAvatarColorArray[3].mSliderValue = mSliderValue;
		mUserState.GetCurrentCustomize().mAvatarColorArray[3].mMode = (byte)mSelectedColorModeID;
		mUserState.GetCurrentCustomize().mAvatarColorArray[3].mCurrentEquipID = (byte)m_equipSel[3];
	}

	public void HidePaintObj()
	{
		mIsPainting = false;
		PaintObj.SetActive(false);
		Buttons.SetActive(true);
		if (mHasChangedColorToggle)
		{
			ResetColor();
			mHasChangedColorToggle = false;
		}
	}

	public void ResetColor()
	{
		mSelectedColorModeID = 0;
		if (CheckIcon.activeSelf)
		{
			int itemID = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
			GameObject gameObject = null;
			if (CustomizeUIScript.mInstance.IsInterMission() && InterMissionScript.GetInstance() != null)
			{
				gameObject = InterMissionScript.GetInstance().GetModel();
			}
			else if (CustomizeUIScript.mInstance.IsInLoadout())
			{
				gameObject = LoadoutUIScript.Instance.LoadoutPage.GetModel();
			}
			if (gameObject != null)
			{
				Transform transform = gameObject.transform.Find(UIConstant.SUB_AVATAR[0]);
				if (transform != null && transform.GetComponent<Renderer>().material.HasProperty("_paintColor"))
				{
					transform.GetComponent<CharacterShaderController>().SetUserCustomColor(false);
					transform.GetComponent<CharacterShaderController>().changeColorType = 0;
				}
				Transform transform2 = gameObject.transform.Find(UIConstant.SUB_AVATAR[1]);
				if (transform2 != null && transform2.GetComponent<Renderer>().material.HasProperty("_paintColor"))
				{
					transform2.GetComponent<CharacterShaderController>().SetUserCustomColor(false);
					transform2.GetComponent<CharacterShaderController>().changeColorType = 0;
				}
				Transform transform3 = gameObject.transform.Find(UIConstant.SUB_AVATAR[2]);
				if (transform3 != null && transform3.GetComponent<Renderer>().material.HasProperty("_paintColor"))
				{
					transform3.GetComponent<CharacterShaderController>().SetUserCustomColor(false);
					transform3.GetComponent<CharacterShaderController>().changeColorType = 0;
				}
				Transform transform4 = gameObject.transform.Find(UIConstant.SUB_AVATAR[3]);
				if (transform4 != null && transform4.GetComponent<Renderer>().material.HasProperty("_paintColor"))
				{
					transform4.GetComponent<CharacterShaderController>().SetUserCustomColor(false);
					transform4.GetComponent<CharacterShaderController>().changeColorType = 0;
				}
			}
			return;
		}
		CustomizeItemType itemType = CustomizeUIScript.mInstance.GetSelectedButton().ItemType;
		int itemID2 = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
		GameObject gameObject2 = null;
		if (CustomizeUIScript.mInstance.IsInterMission() && InterMissionScript.GetInstance() != null)
		{
			gameObject2 = InterMissionScript.GetInstance().GetModel();
		}
		else if (CustomizeUIScript.mInstance.IsInLoadout() && LoadoutUIScript.Instance != null)
		{
			gameObject2 = LoadoutUIScript.Instance.LoadoutPage.GetModel();
		}
		if (!(gameObject2 != null))
		{
			return;
		}
		Transform transform5 = null;
		if (itemType != CustomizeItemType.Weapon && itemType != CustomizeItemType.Bag)
		{
			transform5 = gameObject2.transform.Find(UIConstant.SUB_AVATAR[(int)itemType]);
			CharacterShaderController characterShaderController = transform5.GetComponent<CharacterShaderController>();
			if (characterShaderController == null)
			{
				characterShaderController = transform5.GetComponentInChildren<CharacterShaderController>();
			}
			if (characterShaderController != null)
			{
				characterShaderController.SetUserCustomColor(false);
				characterShaderController.changeColorType = 0;
			}
		}
	}

	public void SetColorMode(int mode)
	{
		if (CheckIcon.activeSelf)
		{
			int itemID = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
			GameObject gameObject = null;
			if (CustomizeUIScript.mInstance.IsInterMission())
			{
				gameObject = InterMissionScript.GetInstance().GetModel();
			}
			else if (CustomizeUIScript.mInstance.IsInLoadout())
			{
				gameObject = LoadoutUIScript.Instance.LoadoutPage.GetModel();
			}
			if (gameObject != null)
			{
				Transform transform = gameObject.transform.Find(UIConstant.SUB_AVATAR[0]);
				if (transform != null && transform.GetComponent<Renderer>().material.HasProperty("_paintColor"))
				{
					transform.GetComponent<CharacterShaderController>().changeColorType = mode;
					mHasChangedColorToggle |= transform.GetComponent<CharacterShaderController>().SetUserCustomColor(true);
				}
				Transform transform2 = gameObject.transform.Find(UIConstant.SUB_AVATAR[1]);
				if (transform2 != null && transform2.GetComponent<Renderer>().material.HasProperty("_paintColor"))
				{
					transform2.GetComponent<CharacterShaderController>().changeColorType = mode;
					mHasChangedColorToggle |= transform2.GetComponent<CharacterShaderController>().SetUserCustomColor(true);
				}
				Transform transform3 = gameObject.transform.Find(UIConstant.SUB_AVATAR[2]);
				if (transform3 != null && transform3.GetComponent<Renderer>().material.HasProperty("_paintColor"))
				{
					transform3.GetComponent<CharacterShaderController>().changeColorType = mode;
					mHasChangedColorToggle |= transform3.GetComponent<CharacterShaderController>().SetUserCustomColor(true);
				}
				Transform transform4 = gameObject.transform.Find(UIConstant.SUB_AVATAR[3]);
				if (transform4 != null && transform4.GetComponent<Renderer>().material.HasProperty("_paintColor"))
				{
					transform4.GetComponent<CharacterShaderController>().changeColorType = mode;
					mHasChangedColorToggle |= transform4.GetComponent<CharacterShaderController>().SetUserCustomColor(true);
				}
			}
			return;
		}
		int itemID2 = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
		CustomizeItemType itemType = CustomizeUIScript.mInstance.GetSelectedButton().ItemType;
		GameObject gameObject2 = null;
		if (CustomizeUIScript.mInstance.IsInterMission())
		{
			gameObject2 = InterMissionScript.GetInstance().GetModel();
		}
		else if (CustomizeUIScript.mInstance.IsInLoadout())
		{
			gameObject2 = LoadoutUIScript.Instance.LoadoutPage.GetModel();
		}
		if (!(gameObject2 != null))
		{
			return;
		}
		Transform transform5 = null;
		if (itemType != CustomizeItemType.Weapon)
		{
			transform5 = gameObject2.transform.Find(UIConstant.SUB_AVATAR[(int)itemType]);
			CharacterShaderController characterShaderController = transform5.GetComponent<CharacterShaderController>();
			if (characterShaderController == null)
			{
				characterShaderController = transform5.GetComponentInChildren<CharacterShaderController>();
			}
			if (characterShaderController != null)
			{
				characterShaderController.changeColorType = mode;
				mHasChangedColorToggle |= characterShaderController.SetUserCustomColor(true);
			}
		}
	}

	private void FloatColorButtonClicked(GameObject go)
	{
		for (int i = 0; i < FloatColors.Length; i++)
		{
			if (go == FloatColors[i].gameObject)
			{
				if (mUserState.GetMultiColorUnlocked())
				{
					SelectedFloatColor = FloatColors[i];
					PreviewColorCurrent.sprite.gameObject.SetActive(false);
					mSelectedColorModeID = i + 2;
					CustomizeItemType itemType = CustomizeUIScript.mInstance.GetSelectedButton().ItemType;
					int itemID = mSelectedItem.GetComponent<SelectItemButton>().ItemID;
					SetColorMode(mSelectedColorModeID);
					mPaintingStartFrame = 0;
				}
				else
				{
					UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_IAP_COLOR_PACKAGE1_HINT"), 2, 40);
				}
			}
		}
	}

	public void InitEquipSel()
	{
		short[] avatar = GameApp.GetInstance().GetUserState().GetAvatar();
		for (int i = 0; i < avatar.Length; i++)
		{
			m_equipSel[i] = avatar[i];
			m_baseEquipSel[i] = avatar[i];
		}
		Weapon weapon = GameApp.GetInstance().GetUserState().GetBattleWeapons()[0];
		m_equipSel[5] = (short)weapon.GetGunID();
		m_baseEquipSel[5] = (short)weapon.GetGunID();
	}

	public void SetItemStatus(string price, int value)
	{
		UpgradeIcon.SetActive(true);
		UpgradePriceObj.SetActive(true);
		MaxLevelObj.SetActive(false);
		if (mUserState.GetCash() >= value)
		{
			UpgradePrice.text = UIConstant.CURRENCY_COLOR_GOLD_PREFIX + price + UIConstant.COLOR_POSTFIX;
		}
		else
		{
			UpgradePrice.text = UIConstant.CURRENCY_COLOR_NOT_ENOUGH_PREFIX + price + UIConstant.COLOR_POSTFIX;
		}
	}

	public void RefreshItemStatus(int itemID)
	{
		if (CustomizeUIScript.mInstance.GetSelectedButton().ItemType == CustomizeItemType.Weapon)
		{
			if (itemID != 0)
			{
				Weapon weapon = mUserState.GetWeapons()[itemID];
				if (weapon.GetGunID() == itemID)
				{
					string text = "---";
					if (weapon.Level < Global.MAX_LEVEL_WEAPONW)
					{
						text = weapon.GetDamageUpgradePrice().ToString();
						SetItemStatus(text, weapon.GetDamageUpgradePrice());
					}
					else
					{
						UpgradeIcon.SetActive(false);
						UpgradePriceObj.SetActive(false);
						MaxLevelObj.SetActive(true);
					}
				}
			}
			else
			{
				SetItemStatus("---", 0);
			}
		}
		else
		{
			SetItemStatus("---", 0);
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 4)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				IapScript.mInstance.OpenPage(IapScript.IapType.Gold);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 40)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				IapScript.mInstance.OpenPage(IapScript.IapType.Nothing);
				IapScript.mInstance.IsGotoColorPack = true;
				IapScript.mInstance.OpenPage(IapScript.IapType.Package);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else
		{
			if (whichMsg.EventId != 81)
			{
				return;
			}
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (LoadoutUIScript.Instance != null && LoadoutUIScript.Instance.gameObject.activeSelf)
				{
					LoadoutUIScript.Instance.Leave();
				}
				if (LobbyScript.mInstance != null && LobbyScript.mInstance.gameObject.activeSelf)
				{
					LobbyScript.mInstance.OnButtonClick(LobbyScript.mInstance.ShopButton);
				}
				if (ShopUIScript.GetInstance() == null)
				{
					ShopUIScript.IsGoingToExpItemPage = true;
				}
				else
				{
					ShopUIScript.GetInstance().StartCoroutine(ShopUIScript.GetInstance().ChangeToExpItemPage());
				}
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
	}

	private void UpdateColor(float colorValue)
	{
		int num = 2;
		FloatColorButton[] floatColors = FloatColors;
		foreach (FloatColorButton floatColorButton in floatColors)
		{
			switch (num)
			{
			case 2:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_1[0], Global.COLORSET_1[1], colorValue);
				break;
			case 3:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_2[0], Global.COLORSET_2[1], colorValue);
				break;
			case 4:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_3[0], Global.COLORSET_3[1], colorValue);
				break;
			case 5:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_4[0], Global.COLORSET_4[1], colorValue);
				break;
			case 6:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_5[0], Global.COLORSET_5[1], colorValue);
				break;
			case 7:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_6[0], Global.COLORSET_6[1], colorValue);
				break;
			case 8:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_7[0], Global.COLORSET_7[1], colorValue);
				break;
			case 9:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_8[0], Global.COLORSET_8[1], colorValue);
				break;
			case 10:
				floatColorButton.ColorIcon.color = Color.Lerp(Global.COLORSET_9[0], Global.COLORSET_9[1], colorValue);
				break;
			}
			num++;
		}
	}

	public void ResetAvatar(short[] equips)
	{
		m_equipSel = equips;
	}
}

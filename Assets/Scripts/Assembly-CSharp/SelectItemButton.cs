using System.Collections.Generic;
using UnityEngine;

public class SelectItemButton : MonoBehaviour
{
	public UISprite Icon;

	public GameObject SelectObj;

	public UISprite SelectFrame;

	public UISprite SuitStateSprite;

	public UISprite SuitStateSpriteBackground;

	public GameObject LockObj;

	public UILabel LockWarning;

	public UILabel ItemCount;

	protected int mItemID;

	public UILabel Name;

	public WeaponArmorStatusScript Status;

	public UILabel WeaponLevel;

	public UILabel ItemDescription;

	private CustomizeItemType mItemType;

	private bool mCheckLock;

	private int mUnlockLevel;

	private UIDragPanelContents mDragPanelContent;

	public int ItemID
	{
		get
		{
			return mItemID;
		}
	}

	private void Awake()
	{
		mDragPanelContent = GetComponent<UIDragPanelContents>();
	}

	private void Update()
	{
		if (!(mDragPanelContent != null))
		{
			return;
		}
		if (mDragPanelContent.draggablePanel.panel.IsVisible(base.transform.position))
		{
			if (!base.GetComponent<Collider>().enabled)
			{
				base.GetComponent<Collider>().enabled = true;
			}
		}
		else if (base.GetComponent<Collider>().enabled)
		{
			base.GetComponent<Collider>().enabled = false;
		}
	}

	public void Init(CustomizeItemType itemType, int itemID, bool checkLock)
	{
		if (Icon != null)
		{
			mItemID = itemID;
			mItemType = itemType;
			mCheckLock = checkLock;
			if (itemType < CustomizeItemType.Item)
			{
				string text = UIConstant.EQUIPS_ICON_STRING_PREFIX[(int)itemType];
				text += mItemID;
				Icon.spriteName = text;
				if (SuitStateSprite != null && SuitStateSpriteBackground != null)
				{
					if (itemType < CustomizeItemType.Bag)
					{
						SuitStateSprite.gameObject.SetActive(true);
						SuitStateSpriteBackground.gameObject.SetActive(true);
						List<Armor> list = GameApp.GetInstance().GetUserState().GetArmor()[(int)itemType];
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].GetArmorID() != mItemID)
							{
								continue;
							}
							SuitStateSprite.spriteName = "suits_" + list[i].SetElement;
							Name.text = list[i].Name;
							if (checkLock)
							{
								int rankID = GameApp.GetInstance().GetUserState().GetRank()
									.rankID;
								if (rankID < list[i].UnlockPVPLevel)
								{
									LockObj.SetActive(true);
									LockWarning.text = LocalizationManager.GetInstance().GetString("LOC_ICON_26", list[i].UnlockPVPLevel);
									mUnlockLevel = list[i].UnlockPVPLevel;
								}
							}
							break;
						}
					}
					else
					{
						List<Armor> list2 = GameApp.GetInstance().GetUserState().GetArmor()[(int)itemType];
						for (int j = 0; j < list2.Count; j++)
						{
							if (list2[j].GetArmorID() != mItemID)
							{
								continue;
							}
							Name.text = list2[j].Name;
							if (checkLock)
							{
								int rankID2 = GameApp.GetInstance().GetUserState().GetRank()
									.rankID;
								if (rankID2 < list2[j].UnlockPVPLevel)
								{
									LockObj.SetActive(true);
									LockWarning.text = LocalizationManager.GetInstance().GetString("LOC_ICON_26", list2[j].UnlockPVPLevel);
									mUnlockLevel = list2[j].UnlockPVPLevel;
								}
							}
						}
						SuitStateSprite.gameObject.SetActive(false);
						SuitStateSpriteBackground.gameObject.SetActive(false);
					}
				}
				else if (itemType == CustomizeItemType.Weapon)
				{
					Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[mItemID];
					Name.text = weapon.Info;
					WeaponLevel.text = "LV " + weapon.Level;
					if (checkLock)
					{
						int rankID3 = GameApp.GetInstance().GetUserState().GetRank()
							.rankID;
						if (rankID3 < weapon.UnlockPVPLevel)
						{
							LockObj.SetActive(true);
							LockWarning.text = LocalizationManager.GetInstance().GetString("LOC_ICON_26", weapon.UnlockPVPLevel);
							WeaponLevel.gameObject.SetActive(false);
							mUnlockLevel = weapon.UnlockPVPLevel;
						}
					}
				}
				if (ItemCount != null)
				{
					ItemCount.gameObject.SetActive(false);
				}
			}
			else if (itemType == CustomizeItemType.Item)
			{
				Status.gameObject.SetActive(false);
				Item item = GameApp.GetInstance().GetUserState().GetItems()[ItemID];
				Icon.spriteName = item.GetIconName();
				Icon.MakePixelPerfect();
				Name.text = item.GetItemName();
				ItemDescription.text = item.GetItemDescription();
				if (ItemCount != null)
				{
					ItemCount.gameObject.SetActive(true);
					ItemCount.text = item.GetCount().ToString();
				}
			}
		}
		SetStatus(itemType);
	}

	private void SetStatus(CustomizeItemType itemType)
	{
		Status.ClearStatus();
		int num = 0;
		float currentValue = 0f;
		int num2 = 0;
		float currentValue2 = 0f;
		if (itemType < CustomizeItemType.Weapon)
		{
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.EQUIPMENT);
			int num3 = GameApp.GetInstance().GetUserState().GetAvatar()[(int)itemType];
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				int num4 = gameDataTable.GetInt(i, 2);
				if (num4 == (int)itemType)
				{
					int num5 = gameDataTable.GetInt(i, 11);
					if (num5 == num3)
					{
						num2 = gameDataTable.GetInt(i, 3);
						currentValue2 = gameDataTable.GetFloat(i, 5);
						num = gameDataTable.GetInt(i, 4);
						break;
					}
				}
			}
			int num6 = mItemID;
			int num7 = 0;
			float newValue = 0f;
			int num8 = 0;
			for (int j = 0; j < gameDataTable.Count - 1; j++)
			{
				int num9 = gameDataTable.GetInt(j, 2);
				if (num9 == (int)itemType)
				{
					int num10 = gameDataTable.GetInt(j, 11);
					if (num10 == num6)
					{
						num7 = gameDataTable.GetInt(j, 3);
						newValue = gameDataTable.GetFloat(j, 5);
						num8 = gameDataTable.GetInt(j, 4);
						break;
					}
				}
			}
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.HP, num2, num7, false);
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.Speed, currentValue2, newValue, false);
			Status.SetPercentStatus(CustomizePlayerStatus.PlayerStatus.Power, num, num8, false);
		}
		else
		{
			if (itemType != CustomizeItemType.Weapon)
			{
				return;
			}
			GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
			int num11 = CustomizeUIScript.mInstance.GetSelectedButton().mItemID;
			for (int k = 0; k < gameDataTable2.Count - 1; k++)
			{
				int num12 = gameDataTable2.GetInt(k, 1);
				if (num12 == num11)
				{
					num = Mathf.CeilToInt(GameApp.GetInstance().GetUserState().GetWeapons()[num11].SimpleDamage());
					currentValue = gameDataTable2.GetFloat(k, 3);
					currentValue2 = gameDataTable2.GetFloat(k, 11);
					break;
				}
			}
			int num13 = mItemID;
			int num14 = 0;
			float newValue2 = 0f;
			float newValue3 = 0f;
			for (int l = 0; l < gameDataTable2.Count - 1; l++)
			{
				int num15 = gameDataTable2.GetInt(l, 1);
				if (num15 == num13)
				{
					num14 = Mathf.CeilToInt(GameApp.GetInstance().GetUserState().GetWeapons()[num13].SimpleDamage());
					newValue2 = gameDataTable2.GetFloat(l, 3);
					newValue3 = gameDataTable2.GetFloat(l, 11);
					break;
				}
			}
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.Power, num, num14, false);
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.FireRate, currentValue, newValue2, false);
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.Speed, currentValue2, newValue3, false);
		}
	}

	public void Refresh()
	{
		Init(mItemType, mItemID, mCheckLock);
	}

	public int GetUnlockLevel()
	{
		return mUnlockLevel;
	}
}

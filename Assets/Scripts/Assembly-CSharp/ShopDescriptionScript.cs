using System.Collections.Generic;
using UnityEngine;

public class ShopDescriptionScript : MonoBehaviour
{
	public ShopUIScript ShopUI;

	public UILabel EquipName;

	public UILabel EquipDescription;

	public GameObject EquipDescOBJ;

	public GameObject ItemDescOBJ;

	public UILabel ItemDescription;

	public UILabel ItemFlaskWarning;

	public UILabel ItemCount;

	public UISprite EffectLine;

	public UISprite WeaponDamageTypeIcon;

	public Transform DamageTypeIconDefaultPosition;

	public Transform DamageTypeIconEnglishPosition;

	public GameObject StatusIconObjs;

	public Transform StatusIconDefaultPosition;

	public Transform StatusIconEnglishPosition;

	public WeaponArmorStatusScript Status;

	private bool needUpdateEffectLine;

	public void UpdateDescription()
	{
		Status.ClearStatus();
		if (EquipDescription.transform.parent != null && EquipDescription.transform.parent.GetComponent<UIDraggablePanel>() != null)
		{
			EquipDescription.transform.parent.GetComponent<UIDraggablePanel>().ResetPosition();
		}
		string empty = string.Empty;
		string text = string.Empty;
		string text2 = "x";
		bool flag = false;
		bool flag2 = false;
		UserState userState = GameApp.GetInstance().GetUserState();
		WeaponDamageTypeIcon.spriteName = string.Empty;
		if (LocalizationManager.GetInstance().currentLanguage == EUserLanguage.ENGLISH)
		{
			StatusIconObjs.transform.localPosition = StatusIconEnglishPosition.localPosition;
		}
		else
		{
			StatusIconObjs.transform.localPosition = StatusIconDefaultPosition.localPosition;
		}
		if (ShopUI.CurrentEquipTag < 5)
		{
			EquipDescOBJ.SetActive(true);
			ItemDescOBJ.SetActive(false);
			Armor armor = userState.GetArmor(ShopUI.CurrentEquipTag, ShopUI.CurrentEquipID);
			empty = armor.Name;
			text = LocalizationManager.GetInstance().GetString(armor.Description);
			if (armor.Level != 0 && armor.Level != 15)
			{
				flag = true;
				if (armor.GetArmorID() == userState.GetAvatar()[ShopUI.CurrentEquipTag])
				{
					flag2 = true;
				}
			}
			if (ShopUI.CurrentEquipTag < 4)
			{
				ShopUI.SuitState.Refresh(ShopUI.CurrentEquipTag, armor.SetElement, armor.SuitId);
			}
		}
		else if (ShopUI.CurrentEquipTag == 5)
		{
			EquipDescOBJ.SetActive(true);
			ItemDescOBJ.SetActive(false);
			Weapon weapon = userState.GetWeapons()[ShopUI.CurrentEquipID];
			empty = weapon.Name;
			text = LocalizationManager.GetInstance().GetString(weapon.Description);
			if (weapon.Level != 0 && weapon.Level != 15)
			{
				flag = true;
				short[] battleWeaponIDs = userState.GetBattleWeaponIDs();
				for (int i = 0; i < battleWeaponIDs.Length; i++)
				{
					if (weapon.GetGunID() == battleWeaponIDs[i])
					{
						flag2 = true;
						break;
					}
				}
			}
			WeaponDamageTypeIcon.gameObject.SetActive(true);
			switch (weapon.GetDamageType())
			{
			case PlayerDamageType.BULLET:
				WeaponDamageTypeIcon.spriteName = "small_1";
				break;
			case PlayerDamageType.ENERGY:
				WeaponDamageTypeIcon.spriteName = "small_2";
				break;
			case PlayerDamageType.EXPLOSION:
				WeaponDamageTypeIcon.spriteName = "small_3";
				break;
			case PlayerDamageType.MELEE:
				WeaponDamageTypeIcon.spriteName = "small_4";
				break;
			default:
				WeaponDamageTypeIcon.gameObject.SetActive(false);
				break;
			}
		}
		else if (ShopUI.CurrentEquipTag == 6)
		{
			EquipDescOBJ.SetActive(false);
			ItemDescOBJ.SetActive(true);
			Dictionary<int, Item> items = userState.GetItems();
			List<ShopItem> shopItemList = ShopItemManager.GetInstance().GetShopItemList();
			if (ShopUI.CurrentEquipID < shopItemList.Count)
			{
				ShopItem shopItem = shopItemList[ShopUI.CurrentEquipID];
				empty = shopItem.GetName();
				text = shopItem.GetDescription();
				Item item = GameApp.GetInstance().GetUserState().GetItems()[shopItem.GetItemID()];
				ItemFlaskWarning.gameObject.SetActive(item.GetItemType() == Item.ItemType.Flask);
				text2 += item.GetCount();
			}
			else
			{
				Debug.LogError("Error: Shop ItemID = " + ShopUI.CurrentEquipID);
			}
		}
		EquipName.text = LocalizationManager.GetInstance().GetString(empty);
		if (EquipName.gameObject.GetComponent<TypewriterEffect>() != null)
		{
			Object.Destroy(EquipName.gameObject.GetComponent<TypewriterEffect>());
		}
		TypewriterEffect typewriterEffect = EquipName.gameObject.AddComponent<TypewriterEffect>();
		typewriterEffect.charsPerSecond = 10;
		if (ShopUI.CurrentEquipTag < 6)
		{
			EquipDescription.text = text;
			if (EquipDescription.gameObject.GetComponent<TypewriterEffect>() != null)
			{
				Object.Destroy(EquipDescription.gameObject.GetComponent<TypewriterEffect>());
			}
			EquipDescription.gameObject.AddComponent<TypewriterEffect>();
		}
		else if (ShopUI.CurrentEquipTag == 6)
		{
			ItemDescription.text = text;
			if (ItemDescription.gameObject.GetComponent<TypewriterEffect>() != null)
			{
				Object.Destroy(ItemDescription.gameObject.GetComponent<TypewriterEffect>());
			}
			ItemDescription.gameObject.AddComponent<TypewriterEffect>();
			ItemCount.text = text2;
		}
		if (flag)
		{
			ShopUI.BuyEquip_Label.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIP");
			if (flag2)
			{
				ShopUI.BuyEquip_Label.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIPED");
			}
		}
		else
		{
			ShopUI.BuyEquip_Label.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_BUY");
		}
		int num = 0;
		float currentValue = 0f;
		int num2 = 0;
		float currentValue2 = 0f;
		if (ShopUI.CurrentEquipTag < 5)
		{
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.EQUIPMENT);
			int num3 = userState.GetAvatar()[ShopUI.CurrentEquipTag];
			for (int j = 0; j < gameDataTable.Count - 1; j++)
			{
				int num4 = gameDataTable.GetInt(j, 2);
				if (num4 == ShopUI.CurrentEquipTag)
				{
					int num5 = gameDataTable.GetInt(j, 11);
					if (num5 == num3)
					{
						num2 = gameDataTable.GetInt(j, 3);
						currentValue2 = gameDataTable.GetFloat(j, 5);
						num = gameDataTable.GetInt(j, 4);
						break;
					}
				}
			}
			int currentEquipID = ShopUI.CurrentEquipID;
			int num6 = 0;
			int num7 = 0;
			float newValue = 0f;
			for (int k = 0; k < gameDataTable.Count - 1; k++)
			{
				int num8 = gameDataTable.GetInt(k, 2);
				if (num8 == ShopUI.CurrentEquipTag)
				{
					int num9 = gameDataTable.GetInt(k, 11);
					if (num9 == currentEquipID)
					{
						num6 = gameDataTable.GetInt(k, 3);
						newValue = gameDataTable.GetFloat(k, 5);
						num7 = gameDataTable.GetInt(k, 4);
						break;
					}
				}
			}
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.HP, num2, num6, true);
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.Speed, currentValue2, newValue, true);
			Status.SetPercentStatus(CustomizePlayerStatus.PlayerStatus.Power, num, num7, true);
		}
		else
		{
			if (ShopUI.CurrentEquipTag != 5)
			{
				return;
			}
			GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
			int gunID = userState.GetBattleWeapons()[0].GunID;
			for (int l = 0; l < gameDataTable2.Count - 1; l++)
			{
				int num10 = gameDataTable2.GetInt(l, 1);
				if (num10 == gunID)
				{
					num = Mathf.CeilToInt(GameApp.GetInstance().GetUserState().GetWeapons()[gunID].SimpleDamage());
					currentValue = gameDataTable2.GetFloat(l, 3);
					currentValue2 = gameDataTable2.GetFloat(l, 11);
					break;
				}
			}
			int currentEquipID2 = ShopUI.CurrentEquipID;
			int num11 = 0;
			float newValue2 = 0f;
			float newValue3 = 0f;
			for (int m = 0; m < gameDataTable2.Count - 1; m++)
			{
				int num12 = gameDataTable2.GetInt(m, 1);
				if (num12 == currentEquipID2)
				{
					num11 = Mathf.CeilToInt(GameApp.GetInstance().GetUserState().GetWeapons()[currentEquipID2].SimpleDamage());
					newValue2 = gameDataTable2.GetFloat(m, 3);
					newValue3 = gameDataTable2.GetFloat(m, 11);
					break;
				}
			}
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.Power, num, num11, true);
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.FireRate, currentValue, newValue2, true);
			Status.SetStatus(CustomizePlayerStatus.PlayerStatus.Speed, currentValue2, newValue3, true);
		}
	}

	public void UpdateEffectLine()
	{
		needUpdateEffectLine = true;
	}

	private void Update()
	{
		if (needUpdateEffectLine)
		{
			if (EffectLine.fillAmount < 1f)
			{
				EffectLine.fillAmount += RealTime.deltaTime * 1.5f;
			}
			else
			{
				needUpdateEffectLine = false;
			}
		}
	}
}

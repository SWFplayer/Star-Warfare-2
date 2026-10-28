using System.Collections.Generic;
using UnityEngine;

public class CustomizeItemButton : MonoBehaviour
{
	public UISprite Icon;

	public UISprite SelectedBackground;

	public CustomizeItemType ItemType;

	public int ButtonId;

	public UISprite SuitStateSprite;

	public UISprite SuitStateSpriteBackground;

	public UILabel ItemCount;

	public GameObject ItemEmptyWarning;

	public int mItemID { get; set; }

	private void Awake()
	{
		SelectedBackground.gameObject.SetActive(false);
	}

	public void Init(int itemID)
	{
		mItemID = itemID;
		if (itemID == 0)
		{
			if (ItemType == CustomizeItemType.Weapon)
			{
				Icon.spriteName = "type_w";
			}
			else if (ItemType == CustomizeItemType.Item)
			{
				Icon.spriteName = "type_i";
			}
		}
		else
		{
			string text = UIConstant.EQUIPS_ICON_STRING_PREFIX[(int)ItemType];
			text += mItemID;
			Icon.spriteName = text;
		}
		if (SuitStateSprite != null && SuitStateSpriteBackground != null)
		{
			if (ItemType < CustomizeItemType.Bag)
			{
				SuitStateSprite.gameObject.SetActive(true);
				SuitStateSpriteBackground.gameObject.SetActive(true);
				List<Armor> list = GameApp.GetInstance().GetUserState().GetArmor()[(int)ItemType];
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].GetArmorID() == mItemID)
					{
						SuitStateSprite.spriteName = "suits_" + list[i].SetElement;
						break;
					}
				}
			}
			else
			{
				SuitStateSprite.gameObject.SetActive(false);
				SuitStateSpriteBackground.gameObject.SetActive(false);
			}
		}
		if (!(ItemCount != null) || !(ItemEmptyWarning != null))
		{
			return;
		}
		if (ItemType == CustomizeItemType.Item && mItemID > 0)
		{
			Item item = GameApp.GetInstance().GetUserState().GetItems()[mItemID];
			Icon.spriteName = item.GetIconName();
			if (item.GetCount() > 0)
			{
				ItemEmptyWarning.SetActive(false);
			}
			else
			{
				ItemEmptyWarning.SetActive(true);
			}
			ItemCount.gameObject.SetActive(false);
			ItemCount.text = Mathf.Min(item.GetCount(), item.GetEquipCount()).ToString();
		}
		else
		{
			ItemCount.gameObject.SetActive(false);
			ItemEmptyWarning.SetActive(false);
		}
	}

	public void Clicked()
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
		switch (ItemType)
		{
		case CustomizeItemType.Head:
		case CustomizeItemType.Body:
		case CustomizeItemType.Arm:
		case CustomizeItemType.Foot:
		case CustomizeItemType.Bag:
			CustomizeUIScript.mInstance.OpenSelectItemPage(true, ItemType);
			break;
		case CustomizeItemType.Weapon:
		case CustomizeItemType.Item:
			CustomizeUIScript.mInstance.OpenSelectItemPage(false, ItemType);
			break;
		}
		SelectedBackground.gameObject.SetActive(true);
	}

	public void Canceled()
	{
		SelectedBackground.gameObject.SetActive(false);
	}
}

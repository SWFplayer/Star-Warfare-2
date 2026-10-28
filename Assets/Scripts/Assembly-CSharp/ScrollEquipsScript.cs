using System.Collections.Generic;
using UnityEngine;

public class ScrollEquipsScript : MonoBehaviour
{
	public ShopUIScript ShopUI;

	public Camera UICamera;

	public GameObject EquipTemplate;

	public GameObject Weapon_ItemTemplate;

	public GameObject PointLeft;

	public GameObject PointLeftMiddle;

	public GameObject PointMiddle;

	public GameObject PointRightMiddle;

	public GameObject PointRight;

	public Vector3 StartPosition = new Vector3(204f, 0f, 0f);

	public int ItemSpace = 102;

	protected Vector3 CurrentEquipObjPosition = Vector3.zero;

	protected float XLeft;

	protected float XLeftMiddle;

	protected float XMiddle;

	protected float XRightMiddle;

	protected float XRight;

	protected float UpdateDepthTime = 0.017f;

	protected float LastUpdateDepthTime;

	protected int LastCenterChildID = -1;

	protected float mEquipScaleForUI = 80f;

	protected float mEquipHideZOffset = 50f;

	private short[] m_equipSel = new short[7];

	private bool mDragging;

	private bool needUpdate = true;

	private UserState mUserState;

	public void Init()
	{
		mUserState = GameApp.GetInstance().GetUserState();
		CreateClone();
	}

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		XLeft = PointLeft.transform.position.x;
		XLeftMiddle = PointLeftMiddle.transform.position.x;
		XMiddle = PointMiddle.transform.position.x;
		XRightMiddle = PointRightMiddle.transform.position.x;
		XRight = PointRight.transform.position.x;
		LastUpdateDepthTime = Time.time;
		if (InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().ChangeAvatar(m_equipSel, true);
		}
		base.gameObject.GetComponent<UICenterOnChild>().onFinished = DragFinished;
	}

	private void Update()
	{
		if (needUpdate)
		{
			ArrangeItems();
		}
	}

	public void ShowEquips(int tag)
	{
		if (tag != 5 && InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().ChangeAvatar(m_equipSel, true);
		}
		LastCenterChildID = -1;
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
		}
		GetComponent<UIDraggablePanel>().ResetPosition();
		SetNeedUpdate();
		if (tag < 5)
		{
			List<List<Armor>> armor = mUserState.GetArmor();
			List<Armor> list = armor[tag];
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject = Object.Instantiate(EquipTemplate) as GameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = new Vector3(j * ItemSpace, 0f, 0f);
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				int index = j % list.Count;
				ShopItemContainter component = gameObject.GetComponent<ShopItemContainter>();
				component.EquipIcon.spriteName = UIConstant.EQUIPS_ICON_STRING_PREFIX[tag] + list[index].GetArmorID();
				component.EquipIcon.depth = 0;
				component.EquipIcon.MakePixelPerfect();
				component.EquipIcon.transform.localScale = Vector3.one * 0.5f;
				component.LockIcon.depth = 1;
				component.EquipID = list[index].GetArmorID();
				component.SetPrice(list[index], tag);
				component.SetSuitStateSprite(list[index]);
				if (list[j].Level == 0)
				{
					component.SetLock();
				}
				if (list[index].GetArmorID() == m_equipSel[tag])
				{
					CurrentEquipObjPosition = gameObject.transform.localPosition;
				}
			}
		}
		else
		{
			switch (tag)
			{
			case 5:
			{
				Dictionary<int, Weapon> weapons = mUserState.GetWeapons();
				List<Weapon> list2 = new List<Weapon>();
				foreach (Weapon value in weapons.Values)
				{
					SortWeaponByDisplayOrder(value, list2);
				}
				int num2 = 0;
				bool flag = false;
				for (int k = 0; k < list2.Count; k++)
				{
					Weapon weapon = list2[k];
					if ((ShopUI.ShowWeaponType != PlayerDamageType.BULLET || weapon.GetDamageType() == PlayerDamageType.BULLET) && (ShopUI.ShowWeaponType != PlayerDamageType.ENERGY || weapon.GetDamageType() == PlayerDamageType.ENERGY) && (ShopUI.ShowWeaponType != PlayerDamageType.EXPLOSION || weapon.GetDamageType() == PlayerDamageType.EXPLOSION) && (ShopUI.ShowWeaponType != PlayerDamageType.MELEE || weapon.GetDamageType() == PlayerDamageType.MELEE || weapon.GunID == 14))
					{
						GameObject gameObject3 = Object.Instantiate(Weapon_ItemTemplate) as GameObject;
						gameObject3.transform.parent = base.transform;
						gameObject3.transform.localPosition = new Vector3(num2 * ItemSpace, 0f, 0f);
						gameObject3.transform.localRotation = Quaternion.identity;
						gameObject3.transform.localScale = Vector3.one;
						int gunID = weapon.GunID;
						ShopItemContainter component3 = gameObject3.GetComponent<ShopItemContainter>();
						component3.EquipIcon.spriteName = UIConstant.EQUIPS_ICON_STRING_PREFIX[tag] + gunID;
						component3.EquipIcon.depth = 0;
						component3.EquipIcon.MakePixelPerfect();
						component3.EquipIcon.transform.localScale = Vector3.one * 0.5f;
						component3.LockIcon.depth = 1;
						component3.EquipID = weapons[gunID].GunID;
						component3.SetPrice(weapons[gunID]);
						if (weapon.Level == 0)
						{
							component3.SetLock();
						}
						if (weapon.GunID == m_equipSel[tag])
						{
							CurrentEquipObjPosition = gameObject3.transform.localPosition;
							flag = true;
						}
						num2++;
					}
				}
				if (!flag)
				{
					CurrentEquipObjPosition = Vector3.zero;
				}
				break;
			}
			case 6:
			{
				Dictionary<int, Item> items = mUserState.GetItems();
				List<ShopItem> shopItemList = ShopItemManager.GetInstance().GetShopItemList();
				int num = 0;
				foreach (ShopItem item in shopItemList)
				{
					GameObject gameObject2 = Object.Instantiate(Weapon_ItemTemplate) as GameObject;
					gameObject2.transform.parent = base.transform;
					gameObject2.transform.localPosition = new Vector3(num * ItemSpace, 0f, 0f);
					gameObject2.transform.localRotation = Quaternion.identity;
					gameObject2.transform.localScale = Vector3.one;
					ShopItemContainter component2 = gameObject2.GetComponent<ShopItemContainter>();
					component2.EquipIcon.spriteName = item.GetIconName();
					component2.EquipIcon.depth = 0;
					component2.EquipIcon.MakePixelPerfect();
					component2.EquipIcon.transform.localScale = Vector3.one * 0.5f;
					component2.LockIcon.depth = 1;
					component2.EquipID = num;
					component2.SetPrice(item);
					num++;
				}
				CurrentEquipObjPosition = Vector3.zero;
				break;
			}
			}
		}
		GetComponent<UIDraggablePanel>().ResetPosition();
		for (int l = 0; l < base.transform.childCount; l++)
		{
			base.transform.GetChild(l).localPosition += StartPosition - CurrentEquipObjPosition;
		}
		ShopUI.Description.UpdateEffectLine();
	}

	protected void ArrangeItems()
	{
		int num = 0;
		float num2 = 1f;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			float x = base.transform.GetChild(i).position.x;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 1f;
			float num6 = 1f;
			bool flag = false;
			if (XLeft <= x && x <= XLeftMiddle)
			{
				num3 = XLeft;
				num4 = XLeftMiddle;
				num5 = 1f;
				num6 = 1.3f;
			}
			else if (XLeftMiddle < x && x <= XMiddle)
			{
				num3 = XLeftMiddle;
				num4 = XMiddle;
				num5 = 1.3f;
				num6 = 1.9f;
			}
			else if (XMiddle < x && x <= XRightMiddle)
			{
				num3 = XMiddle;
				num4 = XRightMiddle;
				num5 = 1.3f;
				num6 = 1.9f;
				flag = true;
			}
			else if (XRightMiddle < x && x <= XRight)
			{
				num3 = XRightMiddle;
				num4 = XRight;
				num5 = 1f;
				num6 = 1.3f;
				flag = true;
			}
			else
			{
				num3 = x;
				num4 = x + 1f;
				num5 = 1f;
				num6 = 1f;
			}
			float t = ((!flag) ? ((x - num3) / (num4 - num3)) : (1f - (x - num3) / (num4 - num3)));
			t = Mathf.Lerp(num5, num6, t);
			base.transform.GetChild(i).localScale = Vector3.one * t;
			base.transform.GetChild(i).GetComponent<ShopItemContainter>().LockObj.transform.localScale = Vector3.one / t;
			if (base.transform.GetChild(i).GetComponent<ShopItemContainter>().NeedShowPVPUnlockTip())
			{
				base.transform.GetChild(i).GetComponent<ShopItemContainter>().PVPUnlockTip.transform.localScale = Vector3.one / t;
			}
			if (t >= num2)
			{
				num2 = t;
				num = i;
			}
		}
		if (!(Time.time - LastUpdateDepthTime >= UpdateDepthTime))
		{
			return;
		}
		if (LastCenterChildID != num && num < base.transform.childCount)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			ShopItemContainter component = base.transform.GetChild(num).GetComponent<ShopItemContainter>();
			component.Background.depth = 2;
			component.EquipIcon.depth = 3;
			component.LockIcon.depth = 4;
			component.WidgetSet.SetAlpha(1f);
			if (component.LockObj.activeSelf)
			{
				component.UnlockTip.gameObject.SetActive(true);
				component.EquipIcon.alpha = 0.5f;
			}
			else if (!component.OwnedTag.gameObject.activeSelf)
			{
				component.CheckMoneyOrMithrilEnough();
			}
			if (!component.OwnedTag.gameObject.activeSelf)
			{
				component.PriceObj.SetActive(true);
			}
			if (component.NeedShowPVPUnlockTip())
			{
				component.PVPUnlockTip.gameObject.SetActive(true);
			}
			if (LastCenterChildID >= 0 && LastCenterChildID < base.transform.childCount)
			{
				ShopItemContainter component2 = base.transform.GetChild(LastCenterChildID).GetComponent<ShopItemContainter>();
				component2.Background.depth = -1;
				component2.EquipIcon.depth = 0;
				component2.LockIcon.depth = 1;
				component2.PriceObj.SetActive(false);
				component2.UnlockTip.gameObject.SetActive(false);
				if (component2.NeedShowPVPUnlockTip())
				{
					component2.PVPUnlockTip.gameObject.SetActive(false);
				}
				component2.WidgetSet.SetAlpha(0.5f);
			}
			LastCenterChildID = num;
			OnEquipSelected(base.transform.GetChild(num).GetComponent<ShopItemContainter>().EquipID);
		}
		LastUpdateDepthTime = Time.time;
	}

	public void RefreshAllGoods()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			ShopItemContainter component = base.transform.GetChild(i).GetComponent<ShopItemContainter>();
			component.CheckMoneyOrMithrilEnough();
		}
	}

	public void OnEquipSelected(int equipID)
	{
		ShopUI.CurrentEquipID = equipID;
		if (ShopUI.CurrentEquipTag < m_equipSel.Length)
		{
			m_equipSel[ShopUI.CurrentEquipTag] = (short)equipID;
		}
		ShopUI.Description.UpdateDescription();
		short[] array = new short[6];
		short[] avatar = mUserState.GetAvatar();
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		array[5] = (short)mUserState.GetBattleWeapons()[0].GunID;
		if (InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().StatusObj.SetStatus(array, m_equipSel);
		}
	}

	public void CreateClone()
	{
		short[] avatar = mUserState.GetAvatar();
		for (int i = 0; i < avatar.Length; i++)
		{
			m_equipSel[i] = avatar[i];
		}
		Weapon weapon = mUserState.GetBattleWeapons()[0];
		m_equipSel[5] = (short)weapon.GetGunID();
	}

	public void DragFinished()
	{
		bool isChangeWeapon = false;
		if (ShopUI.CurrentEquipTag == 5)
		{
			isChangeWeapon = true;
		}
		if (InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().ChangeAvatar(m_equipSel, isChangeWeapon);
		}
		ShopUI.Description.UpdateEffectLine();
		needUpdate = false;
	}

	public void SetEquipPositionRotationInShop(Transform equipTrans, int tag)
	{
		switch (tag)
		{
		case 0:
			equipTrans.localPosition += new Vector3(0f, -135f, 0f);
			equipTrans.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
			break;
		case 1:
			equipTrans.localPosition += new Vector3(0f, -82f, 0f);
			equipTrans.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
			break;
		case 2:
			equipTrans.localPosition += new Vector3(0f, -60f, 0f);
			equipTrans.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
			break;
		case 3:
			equipTrans.localPosition += new Vector3(0f, -27f, 0f);
			equipTrans.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
			break;
		case 4:
			equipTrans.localPosition += new Vector3(0f, 16f, -20f);
			break;
		case 5:
			equipTrans.localPosition += new Vector3(0f, -10f, 0f);
			break;
		}
	}

	public void SetNeedUpdate()
	{
		needUpdate = true;
	}

	public static bool SortWeaponByDisplayOrder(Weapon weapon, List<Weapon> weapons)
	{
		if (weapons.Count == 0)
		{
			weapons.Add(weapon);
		}
		else
		{
			int num = 0;
			int num2 = weapons.Count - 1;
			int num3 = (num + num2) / 2;
			if (weapon.DisplayOrder <= weapons[num].DisplayOrder)
			{
				weapons.Insert(num, weapon);
			}
			else if (weapon.DisplayOrder >= weapons[num2].DisplayOrder)
			{
				weapons.Insert(num2 + 1, weapon);
			}
			else
			{
				while (num2 - num > 1)
				{
					float num4 = (int)weapons[num3].DisplayOrder;
					if ((float)(int)weapon.DisplayOrder == num4)
					{
						num = num3;
						break;
					}
					if ((float)(int)weapon.DisplayOrder > num4)
					{
						num = num3;
					}
					else
					{
						num2 = num3;
					}
					num3 = (num + num2) / 2;
				}
				weapons.Insert(num + 1, weapon);
			}
		}
		return true;
	}

	public void RefreshAvatar()
	{
		if (InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().ChangeAvatar(m_equipSel, true);
		}
	}

	public void BuyOver()
	{
		ShopItemContainter component = base.transform.GetChild(LastCenterChildID).GetComponent<ShopItemContainter>();
		if (component != null)
		{
			component.OwnedTag.gameObject.SetActive(true);
			component.PriceObj.SetActive(false);
		}
	}

	public void ResetAvatar(short[] equips)
	{
		m_equipSel = equips;
	}
}

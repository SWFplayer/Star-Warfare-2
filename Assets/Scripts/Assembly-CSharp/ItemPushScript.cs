using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPushScript : MonoBehaviour, UIMsgListener
{
	public static ItemPushScript mInstance;

	protected UICenterOnChild mCenterOnChild;

	public GameObject GoodsList;

	public UIGrid ItemGrid;

	public GameObject GoodsTemplate;

	public GameObject ExitBtn;

	public GameObject GoBtn;

	private ItemGoodsScript lastGoods;

	private int[] ItemHPList = new int[3] { 2500, 5000, 15000 };

	protected float XLeft;

	protected float XLeftMiddle;

	protected float XMiddle;

	protected float XRightMiddle;

	protected float XRight;

	[HideInInspector]
	public List<ItemGoodsScript> m_Goods = new List<ItemGoodsScript>();

	public GameObject GlowEffect;

	public GameObject PointLeft;

	public GameObject PointLeftMiddle;

	public GameObject PointMiddle;

	public GameObject PointRightMiddle;

	public GameObject PointRight;

	private bool needArrange = true;

	private void Start()
	{
		mInstance = this;
		mCenterOnChild = ItemGrid.GetComponent<UICenterOnChild>();
		List<int> needItemList = GetNeedItemList();
		foreach (int item in needItemList)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(GoodsTemplate) as GameObject;
			gameObject.transform.parent = ItemGrid.transform;
			gameObject.transform.localScale = Vector3.one;
			ItemGoodsScript component = gameObject.GetComponent<ItemGoodsScript>();
			component.Init(item);
			m_Goods.Add(component);
		}
		ItemGrid.Reposition();
		UIEventListener uIEventListener = UIEventListener.Get(ExitBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClicked));
		UIEventListener uIEventListener2 = UIEventListener.Get(GoBtn);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClicked));
	}

	private List<int> GetNeedItemList()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		int nullItemNum = userState.GetNullItemNum();
		List<int> list = new List<int>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		switch (nullItemNum)
		{
		case 1:
			num = UnityEngine.Random.Range(1, 3);
			list.Add(num);
			if (!IsThereHPPotion())
			{
				list.Add(GetHPPotionPushID());
			}
			else if (!IsThereDamagePotion())
			{
				list.Add(GetDamagePotionPushID());
			}
			else
			{
				num = UnityEngine.Random.Range(18, 27);
				list.Add(num);
			}
			list.Add(3);
			break;
		case 2:
			num2 = 0;
			num3 = 0;
			if (!IsThereHPPotion())
			{
				num3 = GetHPPotionPushID();
				num2 = (IsThereDamagePotion() ? UnityEngine.Random.Range(18, 27) : GetDamagePotionPushID());
			}
			else
			{
				num3 = GetDamagePotionPushID();
				num2 = UnityEngine.Random.Range(18, 27);
			}
			list.Add(num2);
			list.Add(num3);
			list.Add(UnityEngine.Random.Range(1, 4));
			break;
		case 3:
			list.Add(GetDamagePotionPushID());
			list.Add(GetHPPotionPushID());
			list.Add(UnityEngine.Random.Range(18, 27));
			break;
		}
		return list;
	}

	public bool IsThereHPPotion()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		short[] battleItemIDs = userState.GetBattleItemIDs();
		short[] array = battleItemIDs;
		foreach (short num in array)
		{
			if (num >= 5 && num <= 9)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsThereDamagePotion()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		short[] battleItemIDs = userState.GetBattleItemIDs();
		short[] array = battleItemIDs;
		foreach (short num in array)
		{
			if (num == GetDamagePotionPushID())
			{
				return true;
			}
		}
		return false;
	}

	public int GetHPPotionPushID()
	{
		int num = 6;
		UserState userState = GameApp.GetInstance().GetUserState();
		float skill = userState.playerSkill.GetSkill(SkillsType.HP_BOOTH);
		skill *= 1f + userState.playerSkill.GetSkill(SkillsType.HP_UP_RATE);
		int[] itemHPList = ItemHPList;
		foreach (int num2 in itemHPList)
		{
			if (skill >= (float)num2)
			{
				num++;
				continue;
			}
			break;
		}
		return num;
	}

	public int GetDamagePotionPushID()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		PlayerDamageType damageType = userState.GetBattleWeapons()[0].GetDamageType();
		return (int)(9 + damageType);
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void ArrangeGoods()
	{
		for (int i = 0; i < m_Goods.Count; i++)
		{
			float x = m_Goods[i].transform.position.x;
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
				num3 = 0.7f;
				num4 = 0.85f;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else if (XLeftMiddle < x && x <= XMiddle)
			{
				num = XLeftMiddle;
				num2 = XMiddle;
				num3 = 0.85f;
				num4 = 1f;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XMiddle < x && x <= XRightMiddle)
			{
				num = XMiddle;
				num2 = XRightMiddle;
				num3 = 0.85f;
				num4 = 1f;
				flag = true;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XRightMiddle < x && x <= XRight)
			{
				num = XRightMiddle;
				num2 = XRight;
				num3 = 0.7f;
				num4 = 0.85f;
				flag = true;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else
			{
				num = x;
				num2 = x + 1f;
				num3 = 0.7f;
				num4 = 0.7f;
				num5 = 0.3f;
				num6 = 0.3f;
			}
			float t = ((!flag) ? ((x - num) / (num2 - num)) : (1f - (x - num) / (num2 - num)));
			float num7 = Mathf.Lerp(num3, num4, t);
			float alpha = Mathf.Lerp(num5, num6, t);
			m_Goods[i].transform.localScale = Vector3.one * num7;
			m_Goods[i].SetAlpha(alpha);
		}
	}

	private void OnButtonClicked(GameObject go)
	{
		if (go == ExitBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (go == GoBtn)
		{
			MissionSelectUIScript.m_Instance.OKClicked();
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 3)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				IapScript.mInstance.OpenPage(IapScript.IapType.Mithril);
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			}
		}
		else if (whichMsg.EventId == 4)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				IapScript.mInstance.OpenPage(IapScript.IapType.Gold);
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			}
		}
		else if (buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		}
	}

	private void OnDragFinished()
	{
		needArrange = false;
		if (lastGoods != null)
		{
			lastGoods.UnSelect();
		}
		lastGoods = mCenterOnChild.centeredObject.GetComponent<ItemGoodsScript>();
		lastGoods.Select();
	}

	public void ShowEffect(float x)
	{
		Vector3 position = GlowEffect.transform.position;
		position.x = x;
		GlowEffect.transform.position = position;
		GlowEffect.SetActive(false);
		GlowEffect.SetActive(true);
	}

	public void SetNeedUpdate()
	{
		needArrange = true;
	}
}

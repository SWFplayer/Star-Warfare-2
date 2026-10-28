using System.Collections.Generic;
using UnityEngine;

public class StateBuffContent : MonoBehaviour
{
	public UISprite BuffIcon;

	public UISprite BuffFrame;

	public UISprite CountDownMask;

	public UILabel FoldCount;

	private List<UIWidget> mWidgets = new List<UIWidget>();

	private StateBuff.EBuffType mEBuffType;

	private AdvancedSkill mSkill;

	private Item mItem;

	private bool mIsPressed;

	private void Start()
	{
		mIsPressed = false;
		mWidgets.Clear();
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		foreach (UIWidget item in componentsInChildren)
		{
			mWidgets.Add(item);
		}
		FoldCount.text = string.Empty;
	}

	private void Update()
	{
		if (mEBuffType == StateBuff.EBuffType.SKILL && mSkill != null)
		{
			CountDownMask.fillAmount = mSkill.ActivedTimePercentage();
			if (mSkill.GetStackTimes() > 0)
			{
				FoldCount.text = mSkill.GetStackTimes().ToString();
			}
			else
			{
				FoldCount.text = string.Empty;
			}
		}
		else if (mEBuffType == StateBuff.EBuffType.SHIELD)
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			CountDownMask.fillAmount = (float)localPlayer.ItemShield / (float)localPlayer.MaxItemShield;
		}
		else if (mEBuffType == StateBuff.EBuffType.MACHINE_GUN_STACK)
		{
			Player localPlayer2 = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer2.GetWeapon().GetWeaponType() == WeaponType.MachineGun)
			{
				FoldCount.text = ((MachineGun)localPlayer2.GetWeapon()).GetStackNum().ToString();
			}
		}
		else if (mEBuffType == StateBuff.EBuffType.NIGHTMARE_CHARGE)
		{
			Player localPlayer3 = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer3.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol)
			{
				FoldCount.text = ((ChargeDoublePistol)localPlayer3.GetWeapon()).GetCharge().ToString();
			}
		}
		else
		{
			if (mEBuffType != StateBuff.EBuffType.SUPER_SHIELD)
			{
				return;
			}
			Player localPlayer4 = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (mSkill != null)
			{
				if (!mSkill.IsActived())
				{
					FoldCount.text = string.Empty;
					CountDownMask.fillAmount = 0f;
				}
				else
				{
					float num = mSkill.ActivedTimePercentage();
					FoldCount.text = (num * 100f).ToString("#0.00");
					CountDownMask.fillAmount = 1f - num;
				}
			}
		}
	}

	public void InitForFlask(UIAtlas atlas, int buffId, Item item)
	{
		mEBuffType = StateBuff.EBuffType.FLASK;
		mItem = item;
		BuffIcon.atlas = atlas;
		BuffIcon.spriteName = "item_" + (12 + 3 * buffId);
		BuffIcon.width = 65;
		BuffIcon.height = 80;
	}

	public void InitForShield(UIAtlas atlas)
	{
		mEBuffType = StateBuff.EBuffType.SHIELD;
		BuffIcon.atlas = atlas;
		BuffIcon.spriteName = "BuffICON1";
		BuffIcon.MakePixelPerfect();
		BuffIcon.width = 50;
		BuffIcon.height = 50;
	}

	public void InitForMachineGun(UIAtlas atlas)
	{
		mEBuffType = StateBuff.EBuffType.MACHINE_GUN_STACK;
		FoldCount.text = "0";
		BuffIcon.atlas = atlas;
		BuffIcon.spriteName = "BuffICON3";
		BuffIcon.MakePixelPerfect();
		BuffIcon.width = 50;
		BuffIcon.height = 50;
	}

	public void InitForChargeGun(UIAtlas atlas)
	{
		mEBuffType = StateBuff.EBuffType.NIGHTMARE_CHARGE;
		FoldCount.text = "0";
		BuffIcon.atlas = atlas;
		BuffIcon.spriteName = "BuffICON4";
		BuffIcon.MakePixelPerfect();
		BuffIcon.width = 50;
		BuffIcon.height = 50;
	}

	public void InitForSuperShield(UIAtlas atlas, AdvancedSkill skill)
	{
		mEBuffType = StateBuff.EBuffType.SUPER_SHIELD;
		FoldCount.text = string.Empty;
		BuffIcon.atlas = atlas;
		BuffIcon.spriteName = "BuffICON5";
		BuffIcon.MakePixelPerfect();
		BuffIcon.width = 50;
		BuffIcon.height = 50;
		mSkill = skill;
	}

	public void InitForSkill(UIAtlas atlas, AdvancedSkill skill)
	{
		mEBuffType = StateBuff.EBuffType.SKILL;
		mSkill = skill;
		BuffIcon.atlas = atlas;
		BuffIcon.spriteName = skill.GetIconName();
		BuffIcon.MakePixelPerfect();
		BuffIcon.width = 60;
		BuffIcon.height = 56;
		if (skill.GetBuffState() == AdvancedSkill.EBuffState.BUFF)
		{
			BuffFrame.spriteName = "buff_1";
		}
		else if (skill.GetBuffState() == AdvancedSkill.EBuffState.DEBUFF)
		{
			BuffFrame.spriteName = "buff_2";
		}
	}

	public void SetAlpha(float alpha)
	{
		foreach (UIWidget mWidget in mWidgets)
		{
			mWidget.alpha *= alpha;
		}
	}

	private void OnPress(bool isPressed)
	{
		mIsPressed = isPressed;
		if (!(StateBuff.mInstance != null))
		{
			return;
		}
		if (isPressed)
		{
			if (mEBuffType == StateBuff.EBuffType.SKILL && mSkill != null)
			{
				StateBuff.mInstance.ShowDescription(mSkill.GetBuffDesc(), BuffFrame.spriteName);
			}
			else if (mEBuffType == StateBuff.EBuffType.FLASK && mItem != null)
			{
				StateBuff.mInstance.ShowDescription(mItem.GetBuffDesc(), BuffFrame.spriteName);
			}
			else if (mEBuffType == StateBuff.EBuffType.SHIELD)
			{
				StateBuff.mInstance.ShowDescription(LocalizationManager.GetInstance().GetString("LOC_BUFF_TIPS_0"), BuffFrame.spriteName);
			}
			else if (mEBuffType == StateBuff.EBuffType.MACHINE_GUN_STACK)
			{
				StateBuff.mInstance.ShowDescription(LocalizationManager.GetInstance().GetString("LOC_BUFF_TIPS_12"), BuffFrame.spriteName);
			}
			else if (mEBuffType == StateBuff.EBuffType.NIGHTMARE_CHARGE)
			{
				StateBuff.mInstance.ShowDescription(LocalizationManager.GetInstance().GetString("LOC_BUFF_TIPS_14"), BuffFrame.spriteName);
			}
			else if (mEBuffType == StateBuff.EBuffType.SUPER_SHIELD)
			{
				StateBuff.mInstance.ShowDescription(LocalizationManager.GetInstance().GetString("LOC_BUFF_TIPS_15"), BuffFrame.spriteName);
			}
		}
		else
		{
			StateBuff.mInstance.HideDescription();
		}
	}

	private void OnDestroy()
	{
		if (mIsPressed)
		{
			StateBuff.mInstance.HideDescription();
		}
	}
}

using UnityEngine;

public class PushCompareScript : MonoBehaviour
{
	public enum ECompareType
	{
		HP = 0,
		DPS = 1,
		FireRate = 2,
		Speed = 3,
		Damage = 4
	}

	public ECompareType type;

	public UISprite BaseSprite;

	public UISprite UpSprite;

	public UILabel BaseValueLabel;

	public UILabel UpValueLabel;

	private IapScript.IapPackageType packageType;

	public void Init(IapScript.IapPackageType iapPackageType)
	{
		packageType = iapPackageType;
		UserState userState = GameApp.GetInstance().GetUserState();
		TempPlayerSkill tempPlayerSkill = new TempPlayerSkill();
		tempPlayerSkill.SetSkillFromUserState(GameApp.GetInstance().GetUserState());
		short[] avatar = userState.GetAvatar();
		short[] array = new short[6];
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		array[5] = (short)userState.GetBattleWeapons()[0].GunID;
		short[] array2 = new short[6];
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_PACKAGE);
		array2[0] = (short)(gameDataTable.GetInt((int)packageType, 4) / 10);
		array2[1] = (short)(gameDataTable.GetInt((int)packageType, 5) / 10);
		array2[2] = (short)(gameDataTable.GetInt((int)packageType, 6) / 10);
		array2[3] = (short)(gameDataTable.GetInt((int)packageType, 7) / 10);
		array2[4] = (short)(gameDataTable.GetInt((int)packageType, 8) / 10);
		array2[5] = (short)gameDataTable.GetInt((int)packageType, 10);
		if (array2[4] == 0)
		{
			array2[4] = array[4];
		}
		tempPlayerSkill.ChangeEquipment(array, array2, GameApp.GetInstance().GetUserState());
		switch (type)
		{
		case ECompareType.HP:
		{
			float skill = userState.playerSkill.GetSkill(SkillsType.HP_BOOTH);
			skill *= 1f + userState.playerSkill.GetSkill(SkillsType.HP_UP_RATE);
			float skill2 = tempPlayerSkill.GetSkill(SkillsType.HP_BOOTH);
			skill2 *= 1f + tempPlayerSkill.GetSkill(SkillsType.HP_UP_RATE);
			if (skill2 < skill)
			{
				base.gameObject.SetActive(false);
				break;
			}
			float num6 = skill2 / skill;
			UpSprite.fillAmount = BaseSprite.fillAmount * num6;
			UpValueLabel.text = "+" + (int)(skill2 - skill);
			BaseValueLabel.text = ((int)skill).ToString();
			break;
		}
		case ECompareType.DPS:
		{
			Weapon weapon = userState.GetWeapons()[array[5]];
			Weapon weapon2 = userState.GetWeapons()[array2[5]];
			float finalDamageInUI = weapon.GetFinalDamageInUI(userState.playerSkill);
			float num = 1f / (weapon.AttackFrenquencyInit / (1f + userState.playerSkill.GetSkill(SkillsType.SHOOT_SPEED) + userState.playerSkill.GetSkill(weapon.AccType)));
			float num2 = finalDamageInUI * num;
			float finalDamageInUI2 = weapon2.GetFinalDamageInUI(tempPlayerSkill);
			float num3 = 1f / (weapon2.AttackFrenquencyInit / (1f + tempPlayerSkill.GetSkill(SkillsType.SHOOT_SPEED) + tempPlayerSkill.GetSkill(weapon2.AccType)));
			float num4 = finalDamageInUI2 * num3;
			if (num4 < num2)
			{
				base.gameObject.SetActive(false);
				break;
			}
			float num5 = num4 / num2;
			UpSprite.fillAmount = BaseSprite.fillAmount * num5;
			UpValueLabel.text = "+" + ((int)(num4 - num2)).ToString("#0");
			BaseValueLabel.text = ((int)num2).ToString("#0");
			break;
		}
		case ECompareType.FireRate:
			break;
		case ECompareType.Speed:
			break;
		}
	}

	public void Init(short trialWeaponID)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		TempPlayerSkill tempPlayerSkill = new TempPlayerSkill();
		tempPlayerSkill.SetSkillFromUserState(GameApp.GetInstance().GetUserState());
		short[] avatar = userState.GetAvatar();
		short[] array = new short[6];
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		array[5] = userState.GetCurrentCustomize().mWeaponIdArray[0];
		short[] array2 = new short[6];
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_PACKAGE);
		array2[0] = array[0];
		array2[1] = array[1];
		array2[2] = array[2];
		array2[3] = array[3];
		array2[4] = array[4];
		array2[5] = trialWeaponID;
		tempPlayerSkill.ChangeEquipment(array, array2, GameApp.GetInstance().GetUserState());
		Weapon weapon = userState.GetWeapons()[array[5]];
		Weapon weapon2 = userState.GetWeapons()[array2[5]];
		switch (type)
		{
		case ECompareType.HP:
		{
			float skill = userState.playerSkill.GetSkill(SkillsType.HP_BOOTH);
			skill *= 1f + userState.playerSkill.GetSkill(SkillsType.HP_UP_RATE);
			float skill2 = tempPlayerSkill.GetSkill(SkillsType.HP_BOOTH);
			skill2 *= 1f + tempPlayerSkill.GetSkill(SkillsType.HP_UP_RATE);
			if (skill2 <= skill)
			{
				base.gameObject.SetActive(false);
				break;
			}
			float num9 = skill2 / skill;
			UpSprite.fillAmount = BaseSprite.fillAmount * num9;
			UpValueLabel.text = "+" + (int)(skill2 - skill);
			BaseValueLabel.text = ((int)skill).ToString();
			break;
		}
		case ECompareType.DPS:
		{
			float finalDamageInUI = weapon.GetFinalDamageInUI(userState.playerSkill);
			float num4 = 1f / (weapon.AttackFrenquencyInit / (1f + userState.playerSkill.GetSkill(SkillsType.SHOOT_SPEED) + userState.playerSkill.GetSkill(weapon.AccType)));
			float num5 = finalDamageInUI * num4;
			float finalDamageInUI2 = weapon2.GetFinalDamageInUI(tempPlayerSkill);
			float num6 = 1f / (weapon2.AttackFrenquencyInit / (1f + tempPlayerSkill.GetSkill(SkillsType.SHOOT_SPEED) + tempPlayerSkill.GetSkill(weapon2.AccType)));
			float num7 = finalDamageInUI2 * num6;
			if (num7 <= num5)
			{
				base.gameObject.SetActive(false);
				break;
			}
			float num8 = num7 / num5;
			UpSprite.fillAmount = BaseSprite.fillAmount * num8;
			UpValueLabel.text = "+" + ((int)(num7 - num5)).ToString("#0");
			BaseValueLabel.text = ((int)num5).ToString("#0");
			break;
		}
		case ECompareType.FireRate:
		{
			float num10 = 1f / (weapon.AttackFrenquencyInit / (1f + userState.playerSkill.GetSkill(SkillsType.SHOOT_SPEED) + userState.playerSkill.GetSkill(weapon.AccType)));
			float num11 = 1f / (weapon2.AttackFrenquencyInit / (1f + tempPlayerSkill.GetSkill(SkillsType.SHOOT_SPEED) + tempPlayerSkill.GetSkill(weapon2.AccType)));
			if (num11 <= num10)
			{
				base.gameObject.SetActive(false);
				break;
			}
			float num12 = num11 / num10;
			UpSprite.fillAmount = BaseSprite.fillAmount * num12;
			UpValueLabel.text = "+" + (num11 - num10).ToString("#0.00");
			BaseValueLabel.text = num10.ToString("#0.00");
			break;
		}
		case ECompareType.Speed:
		{
			float num = 6f + userState.playerSkill.GetSkill(SkillsType.SPEED_BOOTH);
			float num2 = 6f + tempPlayerSkill.GetSkill(SkillsType.SPEED_BOOTH);
			if (num2 <= num)
			{
				base.gameObject.SetActive(false);
				break;
			}
			float num3 = num2 / num;
			UpSprite.fillAmount = BaseSprite.fillAmount * num3;
			UpValueLabel.text = "+" + (num2 - num).ToString("#0.0");
			BaseValueLabel.text = num.ToString("#0.0");
			break;
		}
		}
	}

	public void InitForWeaponUpgradePush(Weapon weapon, int oldLevel, int newLevel)
	{
		int gunID = weapon.GunID;
		switch (type)
		{
		case ECompareType.Damage:
		{
			float damageByLevel = weapon.GetDamageByLevel(oldLevel);
			float damageByLevel2 = weapon.GetDamageByLevel(newLevel);
			float damageByLevel3 = weapon.GetDamageByLevel(Global.MAX_LEVEL_WEAPONW);
			BaseSprite.fillAmount = damageByLevel / damageByLevel3;
			UpSprite.fillAmount = damageByLevel2 / damageByLevel3;
			BaseValueLabel.text = Mathf.CeilToInt(damageByLevel).ToString();
			UpValueLabel.text = "+" + Mathf.CeilToInt(damageByLevel2 - damageByLevel);
			break;
		}
		case ECompareType.FireRate:
		case ECompareType.Speed:
		{
			float num = 1f;
			float num2 = 0f;
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				int num3 = gameDataTable.GetInt(i, 1);
				if (num3 == gunID)
				{
					num = 1f / gameDataTable.GetFloat(i, 3);
					num2 = gameDataTable.GetFloat(i, 11);
					break;
				}
			}
			if (type == ECompareType.FireRate)
			{
				float num4 = 14f;
				BaseSprite.fillAmount = num / num4;
				UpSprite.fillAmount = 0f;
				BaseValueLabel.text = num.ToString("#0.00");
				UpValueLabel.text = string.Empty;
			}
			else if (type == ECompareType.Speed)
			{
				float num5 = -2f;
				float num6 = 2f;
				BaseSprite.fillAmount = (num2 - num5) / (num6 - num5);
				UpSprite.fillAmount = 0f;
				BaseValueLabel.text = num2.ToString("#0.0");
				UpValueLabel.text = string.Empty;
			}
			break;
		}
		}
	}
}

using UnityEngine;

public class CustomizePlayerStatus : MonoBehaviour
{
	public enum PlayerStatus
	{
		Power = 0,
		FireRate = 1,
		HP = 2,
		Speed = 3,
		StatusCount = 4
	}

	public UISprite[] NormalSprites;

	public UISprite[] IncreaceSprites;

	public UISprite[] DecreaseSprites;

	public UILabel[] PreviewValues;

	private static float[] MaxValues = new float[4] { 500f, 12f, 430f, 14f };

	private TempPlayerSkill tempPlayerSkill = new TempPlayerSkill();

	public void RefreshStatus(PlayerStatus type, float currentValue, float newValue)
	{
		string text = "[01fdfe]";
		string cOLOR_POSTFIX = UIConstant.COLOR_POSTFIX;
		if (currentValue >= newValue)
		{
			NormalSprites[(int)type].fillAmount = GetStatusFillAmount(type, newValue);
			IncreaceSprites[(int)type].fillAmount = GetStatusFillAmount(type, newValue);
			DecreaseSprites[(int)type].fillAmount = GetStatusFillAmount(type, currentValue);
			if (currentValue > newValue)
			{
				text = UIConstant.EQUIP_COMPARE_BAD_PREFIX;
			}
		}
		else
		{
			NormalSprites[(int)type].fillAmount = GetStatusFillAmount(type, currentValue);
			IncreaceSprites[(int)type].fillAmount = GetStatusFillAmount(type, newValue);
			DecreaseSprites[(int)type].fillAmount = GetStatusFillAmount(type, currentValue);
			text = UIConstant.EQUIP_COMPARE_GOOD_PREFIX;
		}
		if (type == PlayerStatus.FireRate)
		{
			PreviewValues[(int)type].text = text + newValue.ToString("#0.00") + cOLOR_POSTFIX;
		}
		else
		{
			PreviewValues[(int)type].text = text + newValue + cOLOR_POSTFIX;
		}
	}

	public float GetStatusFillAmount(PlayerStatus type, float value)
	{
		float num = 0f;
		switch (type)
		{
		case PlayerStatus.HP:
			return Mathf.Pow(value, 0.55f) / MaxValues[(int)type];
		case PlayerStatus.Power:
			if (value <= 100f)
			{
				return value / 400f;
			}
			if (value <= 350f)
			{
				return (value - 100f) / 1000f + 0.25f;
			}
			if (value <= 1500f)
			{
				return (value - 350f) / 4600f + 0.5f;
			}
			return (value - 1500f) / 14400f + 0.75f;
		default:
			return value / MaxValues[(int)type];
		}
	}

	public void SetStatus(short[] currentStatus, short[] newStatus)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 6f;
		float num8 = 6f;
		UserState userState = GameApp.GetInstance().GetUserState();
		if (currentStatus.Equals(newStatus))
		{
			tempPlayerSkill.SetSkillFromUserState(GameApp.GetInstance().GetUserState());
		}
		else
		{
			tempPlayerSkill.ChangeEquipment(currentStatus, newStatus, GameApp.GetInstance().GetUserState());
		}
		Weapon weapon = userState.GetWeapons()[currentStatus[5]];
		Weapon weapon2 = userState.GetWeapons()[newStatus[5]];
		num = weapon.GetFinalDamageInUI(userState.playerSkill);
		num3 = 1f / (weapon.AttackFrenquencyInit / (1f + userState.playerSkill.GetSkill(SkillsType.SHOOT_SPEED) + userState.playerSkill.GetSkill(weapon.AccType)));
		num7 += userState.playerSkill.GetSkill(SkillsType.SPEED_BOOTH);
		num2 = weapon2.GetFinalDamageInUI(tempPlayerSkill);
		num4 = 1f / (weapon2.AttackFrenquencyInit / (1f + tempPlayerSkill.GetSkill(SkillsType.SHOOT_SPEED) + tempPlayerSkill.GetSkill(weapon2.AccType)));
		num8 += tempPlayerSkill.GetSkill(SkillsType.SPEED_BOOTH);
		num5 = userState.playerSkill.GetSkill(SkillsType.HP_BOOTH);
		num5 *= 1f + userState.playerSkill.GetSkill(SkillsType.HP_UP_RATE);
		num6 = tempPlayerSkill.GetSkill(SkillsType.HP_BOOTH);
		num6 *= 1f + tempPlayerSkill.GetSkill(SkillsType.HP_UP_RATE);
		RefreshStatus(PlayerStatus.Power, num, num2);
		RefreshStatus(PlayerStatus.FireRate, num3, num4);
		RefreshStatus(PlayerStatus.HP, num5, num6);
		RefreshStatus(PlayerStatus.Speed, num7, num8);
	}
}

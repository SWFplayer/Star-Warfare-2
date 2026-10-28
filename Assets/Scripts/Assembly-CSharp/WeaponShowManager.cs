using System.Collections.Generic;
using UnityEngine;

public class WeaponShowManager
{
	private static WeaponShowManager mInstance;

	private List<float> shotgun_show1 = new List<float> { 2.23f, 2.6f };

	private List<float> shotgun_show2 = new List<float> { 2.23f, 2.6f };

	private List<float> assault_show1 = new List<float> { 1.67f, 3.27f };

	private List<float> assault_show2 = new List<float>();

	private List<float> RPG_show1 = new List<float> { 4.18f, 4.5f };

	private List<float> RPG_show2 = new List<float>();

	private List<float> doubleP_show1 = new List<float> { 1.4f, 2.62f };

	private List<float> doubleP_show2 = new List<float>();

	private List<float> mBow_show1 = new List<float> { 1.03f, 1.5f };

	private List<float> mBow_show2 = new List<float>();

	private List<float> Sniper_show1 = new List<float> { 1.36f, 1.5f };

	private List<float> Sniper_show2 = new List<float>();

	private List<float> Fist_Show1 = new List<float> { 1f, 2.5f };

	private List<float> Fist_Show2 = new List<float>();

	private List<float> Railgun_Show1 = new List<float> { 1f, 2.5f };

	private List<float> Railgun_Show2 = new List<float>();

	private List<float> Sword_Show = new List<float> { 0.8f, 4.5f };

	public static WeaponShowManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new WeaponShowManager();
		}
		return mInstance;
	}

	public List<float> GetTimeList(Weapon weapon, int showNum)
	{
		switch (showNum)
		{
		case 1:
			switch (weapon.GetAnimationSuffix())
			{
			case "shotgun":
				return new List<float>(shotgun_show1);
			case "assault":
				return new List<float>(assault_show1);
			case "RPG":
				return new List<float>(RPG_show1);
			case "DoubleP":
				return new List<float>(doubleP_show1);
			case "LaserBow":
				return new List<float>(mBow_show1);
			case "Sniper":
				return new List<float>(Sniper_show1);
			case "fist":
				return new List<float>(Fist_Show1);
			case "RailGun":
			case "jiatelin":
				return new List<float>(Railgun_Show1);
			case "LaserSword":
				return new List<float>(Sword_Show);
			default:
				Debug.Log("Weapon Show Manager doesn't contain data for " + weapon.GetAnimationSuffix());
				return new List<float>();
			}
		case 2:
			switch (weapon.GetAnimationSuffix())
			{
			case "shotgun":
				return new List<float>(shotgun_show2);
			case "assault":
				return new List<float>(assault_show2);
			case "RPG":
				return new List<float>(RPG_show2);
			case "DoubleP":
				return new List<float>(doubleP_show2);
			case "LaserBow":
				return new List<float>(mBow_show2);
			case "Sniper":
				return new List<float>(Sniper_show2);
			case "fist":
				return new List<float>(Fist_Show2);
			case "RailGun":
			case "jiatelin":
				return new List<float>(Railgun_Show2);
			case "LaserSword":
				return new List<float>(Sword_Show);
			default:
				Debug.Log("Weapon Show Manager doesn't contain data for " + weapon.GetAnimationSuffix());
				return new List<float>();
			}
		case 0:
			return new List<float>();
		default:
			return new List<float>();
		}
	}
}

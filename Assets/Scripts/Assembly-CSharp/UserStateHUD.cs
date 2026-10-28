using System.Collections.Generic;
using UnityEngine;

public class UserStateHUD
{
	public class JoystickHUD
	{
		private float moveJoystickRatio;

		public bool IsFixed { get; set; }

		public Vector2[] TouchPos { get; set; }

		public bool isMoveJoystickPressed { get; set; }

		public bool isPressedToRotateCamera { get; set; }

		public Vector2 MoveCenter { get; set; }

		public Vector2 ShootCenter { get; set; }

		public float Radius { get; set; }

		public float MoveJoystickRatio
		{
			get
			{
				return moveJoystickRatio;
			}
		}

		public JoystickHUD()
		{
			IsFixed = false;
			isMoveJoystickPressed = false;
			isPressedToRotateCamera = false;
			MoveCenter = Vector2.zero;
			ShootCenter = Vector2.zero;
			TouchPos = new Vector2[2];
			TouchPos[0] = Vector2.zero;
			TouchPos[1] = Vector2.zero;
			Radius = 0f;
			moveJoystickRatio = 0.3f;
		}
	}

	public class InfoBoxHUD
	{
		public class InfoHUD
		{
			public enum InfoType
			{
				PopInfo = 0,
				PrivateAchieve = 1,
				PrivateAchieveWithoutNum = 2,
				Broadcasting = 3,
				Warning = 4,
				ChangeTeam = 5,
				Mutant = 6
			}

			public string StrInfo1 { get; set; }

			public string StrInfo2 { get; set; }

			public int NumberInfo { get; set; }

			public string IconString { get; set; }

			public new InfoType GetType { get; set; }
		}

		private List<InfoHUD> list = new List<InfoHUD>();

		public void SetPrivateAchieve(string str, int bonus)
		{
			SetInfo(str, bonus, InfoHUD.InfoType.PrivateAchieve);
		}

		public void SetPrivateAchieveWithoutNum(string str)
		{
			SetInfo(str, 0, InfoHUD.InfoType.PrivateAchieveWithoutNum);
		}

		public void SetBroadcasting(string str)
		{
			SetInfo(str, 0, InfoHUD.InfoType.Broadcasting);
		}

		public void SetWarning(string str)
		{
			SetInfo(str, 0, InfoHUD.InfoType.Warning);
		}

		public void PushInfo(string str)
		{
			SetInfo(str, 0, InfoHUD.InfoType.PopInfo);
		}

		public void SetChangeTeam(string str)
		{
			SetInfo(str, 0, InfoHUD.InfoType.ChangeTeam);
		}

		public void SetMutantInfo(string str)
		{
			SetInfo(str, 0, InfoHUD.InfoType.Mutant);
		}

		public void PushFirstBloodInfo(Player killer, InfoHUD.InfoType type)
		{
			string text = LocalizationManager.GetInstance().GetString("LOC_PVP_1ST_BLOOD");
			string text2 = LocalizationManager.GetInstance().ModifyTeamNameColor(killer.GetDisplayName(), killer.Team, killer.GetSeatID());
			SetInfo(text2 + text, killer.VSStatistics.GetExpDataAfterExpBooth(PlayerVSStatistics.EExpType.FIRST_BLOOD), type);
		}

		public void PushKillInfo(Player killer, Player killed, byte attackerType, byte weaponID)
		{
			string text = LocalizationManager.GetInstance().GetString("LOC_PVP_A_KILL_B");
			string str = LocalizationManager.GetInstance().ModifyTeamNameColor(killer.GetName(), killer.Team, killer.GetSeatID());
			string str2 = LocalizationManager.GetInstance().ModifyTeamNameColor(killed.GetName(), killed.Team, killed.GetSeatID());
			string iconString = "weapons_" + killer.GetWeapon().GunID;
			switch (attackerType)
			{
			case 1:
				if (killer.mVehicle != null)
				{
					switch (killer.mVehicle.VehicleType)
					{
					case EVehicleType.UNCC_TURRET:
						iconString = GetOtherKillIcon(11);
						break;
					case EVehicleType.RPG_TURRET:
						iconString = GetOtherKillIcon(4);
						break;
					case EVehicleType.ATV_MINI:
						iconString = GetOtherKillIcon(7);
						break;
					case EVehicleType.ATV_LASER:
						iconString = GetOtherKillIcon(13);
						break;
					case EVehicleType.LONG_SWORD:
					case EVehicleType.LONG_SWORD_CANNON:
						iconString = "streak_16";
						break;
					}
				}
				break;
			case 2:
				if (killer.mSwVoid != null)
				{
					switch (killer.mSwVoid.mType)
					{
					case ESwVoidType.DEMOMAN:
						iconString = (killer.mSwVoid.IsHighLevel() ? "void_askill_02" : "void_askill_01");
						break;
					case ESwVoidType.SOLDIER:
						iconString = (killer.mSwVoid.IsHighLevel() ? "void_askill_06" : "void_askill_05");
						break;
					case ESwVoidType.SCOUT:
						iconString = (killer.mSwVoid.IsHighLevel() ? "void_askill_08" : "void_askill_07");
						break;
					case ESwVoidType.HEAVY:
						iconString = (killer.mSwVoid.IsHighLevel() ? "void_askill_10" : "void_askill_09");
						break;
					}
				}
				break;
			case 3:
				switch (weaponID)
				{
				case 13:
					iconString = GetOtherKillIcon(6);
					break;
				case 16:
					iconString = GetOtherKillIcon(9);
					break;
				case 17:
					iconString = "gen_perk_06";
					break;
				case 4:
					iconString = "pickup_02";
					break;
				case 75:
					iconString = "pickup_01";
					break;
				case 24:
					iconString = "set_skill_12";
					break;
				case 23:
					iconString = "item_mine";
					break;
				case 27:
					iconString = "set_skill_14";
					break;
				}
				break;
			case 4:
				if (weaponID == 0)
				{
					int num = killer.BossIndex + 1;
					iconString = "head_boss_0" + num;
				}
				else
				{
					iconString = "void_pskill_07";
				}
				break;
			}
			SetInfo(str, str2, 0, iconString, InfoHUD.InfoType.PopInfo);
		}

		private string GetOtherKillIcon(int skillID)
		{
			string result = string.Empty;
			PvpSkill killStreakById = GameApp.GetInstance().GetUserState().GetPvpSkillManager()
				.GetKillStreakById(skillID);
			if (killStreakById != null)
			{
				result = killStreakById.GetIconName();
			}
			return result;
		}

		public void PushMulitKillInfo(Player killer, int count, InfoHUD.InfoType type)
		{
			string text = LocalizationManager.GetInstance().GetString("LOC_PVP_KILL_STREAK").Replace("%d", string.Empty + count);
			string text2 = LocalizationManager.GetInstance().ModifyTeamNameColor(killer.GetDisplayName(), killer.Team, killer.GetSeatID());
			if (killer.IsBoss)
			{
				SetInfo(text2 + text, PlayerVSStatistics.GetExpData(PlayerVSStatistics.EExpType.PLAYER_BOSS_MULTI_KILL), type);
			}
			else
			{
				SetInfo(text2 + text, PlayerVSStatistics.GetExpData(PlayerVSStatistics.EExpType.MULTI_KILL), type);
			}
		}

		public void SetInfo(string str, int number, InfoHUD.InfoType type)
		{
			SetInfo(str, string.Empty, number, type);
		}

		public void SetInfo(string str1, string str2, int number, InfoHUD.InfoType type)
		{
			SetInfo(str1, str2, number, string.Empty, type);
		}

		public void SetInfo(string str1, string str2, int number, string iconString, InfoHUD.InfoType type)
		{
			InfoHUD infoHUD = new InfoHUD();
			infoHUD.StrInfo1 = str1;
			infoHUD.StrInfo2 = str2;
			infoHUD.GetType = type;
			infoHUD.IconString = iconString;
			infoHUD.NumberInfo = number;
			list.Add(infoHUD);
		}

		public InfoHUD PopInfo()
		{
			if (Count() > 0)
			{
				InfoHUD result = list[0];
				list.RemoveAt(0);
				return result;
			}
			return null;
		}

		public int Count()
		{
			return list.Count;
		}

		public void Clear()
		{
			list.Clear();
		}
	}

	private static UserStateHUD instance;

	private JoystickHUD mJoystickHUD;

	private InfoBoxHUD mInfoBoxHUD;

	public bool Reload { get; set; }

	public JoystickHUD Joystick
	{
		get
		{
			return mJoystickHUD;
		}
	}

	public InfoBoxHUD InfoBox
	{
		get
		{
			return mInfoBoxHUD;
		}
	}

	private UserStateHUD()
	{
		mJoystickHUD = new JoystickHUD();
		mInfoBoxHUD = new InfoBoxHUD();
	}

	public static UserStateHUD GetInstance()
	{
		if (instance == null)
		{
			instance = new UserStateHUD();
		}
		return instance;
	}

	public void Init()
	{
		Clear();
	}

	public void Clear()
	{
		Debug.Log("Clear");
		Debug.Log("***************************HUDClear");
	}

	public int GetUserHp()
	{
		return GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.Hp;
	}

	public int GetUserMaxHp()
	{
		return GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.MaxHp;
	}

	public int GetUserExp()
	{
		return GameApp.GetInstance().GetUserState().GetExp();
	}

	public Transform GetUserTransform()
	{
		return GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetTransform();
	}

	public bool IsUserDead()
	{
		return GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.State == Player.DEAD_STATE;
	}
}

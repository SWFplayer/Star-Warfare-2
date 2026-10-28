using System;
using UnityEngine;

public class PlayerInfo
{
	public int channelID;

	public int accountID;

	public byte seatID;

	public bool isRegisted;

	public byte bagIdOfWeapon;

	public OnlineCustomizeData coopCustomize;

	public bool pvpSpawned;

	public int pvpCustomizeSize;

	public byte pvpSkillGridSize;

	public byte currentPvpCustomizeID;

	public OnlineCustomizeData[] pvpCustomizeDataArray;

	public int maxHP;

	public int hp;

	public byte[] battleWeaponList = new byte[3];

	public byte[] battleItemList = new byte[3];

	public byte[] armors = new byte[Global.AVATAR_PART_NUM];

	public string playerName = string.Empty;

	public SocialPlayerData SocialPlayerData;

	public int Exp;

	public byte TitleId;

	public byte HeadFrame;

	public byte Prestige;

	public int MissionNum;

	public int WeaponNum;

	public int EquipNum;

	public int SwVoidNum;

	public int TotalPlayTime;

	public DateTime LastLoginTime;

	public Vector3 Position;

	public float RotationY;
}

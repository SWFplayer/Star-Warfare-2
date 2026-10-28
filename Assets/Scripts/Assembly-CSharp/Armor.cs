using System.Collections.Generic;
using UnityEngine;

public class Armor
{
	public enum EArmorParamName
	{
		NAME = 0,
		ID = 1,
		TYPE = 2,
		HP = 3,
		ATTACK = 4,
		SPEED = 5,
		MONEY = 6,
		EXP = 7,
		CAN_FLY = 8,
		SHARE_RATE = 9,
		SHOOT_PUNISH_REDUCE = 10,
		SUIT_ID = 11,
		SET_ELEMENT = 12,
		BAG_SKILL_ID = 13,
		UNLOCK_LEVEL = 14,
		UNLOCK_DIFFCULTY = 15,
		UNLOCK_STAGE = 16,
		UNLOCK_SUB_STAGE = 17,
		DESCRIPTION = 18,
		PRICE = 19,
		MITHRIL = 20
	}

	public static string ArmorPriceVersion = "100";

	protected byte mUniqueID;

	protected BodyType armorType;

	protected byte armorID;

	protected byte level;

	protected bool canUseInPVP;

	protected GameObject equipment;

	protected string name;

	protected Vector3 center;

	protected SkillGroup skillGroup;

	protected string description;

	protected byte unlockPVPLevel;

	protected LevelID unlockLevel;

	private string _price;

	private string _mithril;

	protected byte bagNum;

	protected byte setElement;

	protected byte suitId;

	protected string BuyVer;

	protected int price
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_price, "shuliang");
		}
		set
		{
			_price = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	protected int mithril
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mithril, "shuliang");
		}
		set
		{
			_mithril = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	public byte Level
	{
		get
		{
			return level;
		}
		set
		{
			level = value;
			if (value == 1)
			{
				BuyVer = ArmorPriceVersion;
			}
		}
	}

	public bool CanUseInPVP
	{
		get
		{
			return canUseInPVP;
		}
		set
		{
			canUseInPVP = value;
		}
	}

	public GameObject Equipment
	{
		get
		{
			return equipment;
		}
		set
		{
			equipment = value;
		}
	}

	public Vector3 Center
	{
		get
		{
			return center;
		}
		set
		{
			center = value;
		}
	}

	public byte UnlockPVPLevel
	{
		get
		{
			return unlockPVPLevel;
		}
	}

	public LevelID UnlockLevel
	{
		get
		{
			return unlockLevel;
		}
	}

	public int Price
	{
		get
		{
			return price;
		}
	}

	public int Mithril
	{
		get
		{
			return mithril;
		}
	}

	public string Name
	{
		get
		{
			return LocalizationManager.GetInstance().GetString(name);
		}
	}

	public byte BagNum
	{
		get
		{
			return bagNum;
		}
	}

	public byte SetElement
	{
		get
		{
			return setElement;
		}
	}

	public byte SuitId
	{
		get
		{
			return suitId;
		}
	}

	public string Description
	{
		get
		{
			return description;
		}
	}

	public Armor(BodyType type, int id)
	{
		armorID = (byte)id;
		armorType = type;
	}

	public float GetSkill(SkillsType sType)
	{
		return 0f;
	}

	public void CreateArmor()
	{
		string path = ((armorType != BodyType.Bag) ? (Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", armorID) + "/" + UIConstant.AVATAR_DIR[(int)armorType]) : (Global.BAG_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.BAG_PREFIX + string.Format("{0:D2}", armorID)));
		Object original = Resources.Load(path);
		equipment = Object.Instantiate(original) as GameObject;
	}

	public void LoadConfig(byte ownedState, int resID)
	{
		skillGroup = new SkillGroup();
		skillGroup.source = armorType.ToString();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.EQUIPMENT);
		if (gameDataTable != null)
		{
			name = gameDataTable.GetString(resID, 0);
			float val = gameDataTable.GetFloat(resID, 3);
			AddSkill(SkillsType.HP_BOOTH, val);
			val = gameDataTable.GetFloat(resID, 4);
			AddSkill(SkillsType.ALL_DAMAGE, val);
			val = gameDataTable.GetFloat(resID, 5);
			AddSkill(SkillsType.SPEED_BOOTH, val);
			val = gameDataTable.GetFloat(resID, 6);
			AddSkill(SkillsType.MONEY_BOOTH, val);
			val = gameDataTable.GetFloat(resID, 7);
			AddSkill(SkillsType.EXP_BOOTH, val);
			val = gameDataTable.GetFloat(resID, 8);
			AddSkill(SkillsType.FLY, val);
			val = gameDataTable.GetFloat(resID, 9);
			AddSkill(SkillsType.SHARE_RATE, val);
			val = gameDataTable.GetFloat(resID, 10);
			AddSkill(SkillsType.SHOOT_PUNISH_REDUCE, val);
			unlockPVPLevel = (byte)gameDataTable.GetInt(resID, 14);
			description = gameDataTable.GetString(resID, 18);
			price = gameDataTable.GetInt(resID, 19);
			mithril = gameDataTable.GetInt(resID, 20);
			suitId = (byte)gameDataTable.GetInt(resID, 11);
			if (armorType != BodyType.Bag)
			{
				setElement = (byte)gameDataTable.GetInt(resID, 12);
			}
			mUniqueID = (byte)gameDataTable.GetInt(resID, 1);
			unlockPVPLevel = (byte)gameDataTable.GetInt(resID, 14);
			unlockLevel.Difficult = (EDifficultMode)gameDataTable.GetInt(resID, 15);
			unlockLevel.StageID = (byte)gameDataTable.GetInt(resID, 16);
			unlockLevel.SubStageID = (byte)gameDataTable.GetInt(resID, 17);
			int num = gameDataTable.GetInt(resID, 13);
			if (num != 0)
			{
				skillGroup.InitSkillInfo(num);
			}
		}
	}

	public void AddSkill(SkillsType type, float val)
	{
		if (val != 0f)
		{
			skillGroup.AddStaticSkill(type, val);
		}
	}

	public void TakeOn(PlayerSkill playerSkill)
	{
		skillGroup.InitToPlayerSkill(playerSkill);
	}

	public void TakeOff()
	{
		skillGroup.UninitFromPlayerSkill();
	}

	public byte GetArmorID()
	{
		return armorID;
	}

	public void SetLevel(byte value)
	{
		level = value;
	}

	public void IrregularSetLevel(byte value, string VerString)
	{
		level = value;
		BuyVer = VerString;
	}

	public List<Skill> GetSkills()
	{
		return skillGroup.GetSkills();
	}

	public SkillGroup GetSkillGroup()
	{
		return skillGroup;
	}

	public BodyType GetArmorType()
	{
		return armorType;
	}

	public byte GetUniqueID()
	{
		return mUniqueID;
	}

	public string GetBuyVer()
	{
		return BuyVer;
	}

	public void SetBuyVer(string buyVer)
	{
		BuyVer = buyVer;
	}
}

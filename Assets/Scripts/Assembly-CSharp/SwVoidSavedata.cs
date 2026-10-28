using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SwVoidSavedata
{
	private string name;

	private int id;

	private ESwVoidType mType;

	private byte mLevel;

	private List<SwVoidAttribute> attributes = new List<SwVoidAttribute>();

	private short[] passiveSkillIds = new short[2];

	private string[] AttributeDes;

	private string[] AttributeNextDes;

	private SkillGroup mSkillGroup;

	public SwVoidSavedata()
	{
		id = 0;
	}

	public SwVoidSavedata(SwVoidResponseData swVoidResponseData)
	{
		mType = (ESwVoidType)swVoidResponseData.type;
		mLevel = swVoidResponseData.level;
		passiveSkillIds = swVoidResponseData.skillIdArray;
		id = swVoidResponseData.id;
		SwVoidAttribute[] attributeArray = swVoidResponseData.attributeArray;
		foreach (SwVoidAttribute item in attributeArray)
		{
			attributes.Add(item);
		}
		CalculateInfoFromTypeAndLevel();
	}

	public void Save(BinaryWriter bw)
	{
		bw.Write((byte)mType);
		bw.Write(mLevel);
		bw.Write((byte)attributes.Count);
		for (int i = 0; i < attributes.Count; i++)
		{
			bw.Write(attributes[i].id);
			bw.Write(attributes[i].value);
		}
		bw.Write((byte)passiveSkillIds.Length);
		for (int j = 0; j < passiveSkillIds.Length; j++)
		{
			bw.Write(passiveSkillIds[j]);
		}
	}

	public void Load(BinaryReader br, int id)
	{
		this.id = id;
		mType = (ESwVoidType)br.ReadByte();
		mLevel = br.ReadByte();
		byte b = br.ReadByte();
		attributes = new List<SwVoidAttribute>();
		for (int i = 0; i < b; i++)
		{
			SwVoidAttribute item = new SwVoidAttribute(br.ReadByte(), br.ReadInt16());
			attributes.Add(item);
		}
		byte b2 = br.ReadByte();
		passiveSkillIds = new short[b2];
		for (int j = 0; j < b2; j++)
		{
			passiveSkillIds[j] = br.ReadInt16();
		}
		CalculateInfoFromTypeAndLevel();
	}

	public string GetName()
	{
		if (name != null)
		{
			return name;
		}
		name = SwVoidDataManager.GetFinalNameFromIndex(mType, mLevel);
		return name;
	}

	public bool isMaxLevel()
	{
		if (mLevel == 6)
		{
			return true;
		}
		return false;
	}

	private void CalculateInfoFromTypeAndLevel()
	{
		name = SwVoidDataManager.GetFinalNameFromIndex(mType, mLevel);
	}

	public int GetId()
	{
		return id;
	}

	public new ESwVoidType GetType()
	{
		return mType;
	}

	public byte GetTypeInByte()
	{
		return (byte)mType;
	}

	public byte GetLevel()
	{
		return mLevel;
	}

	public void LevelUp()
	{
		mLevel++;
		CalculateInfoFromTypeAndLevel();
	}

	public void SetAttribute(byte id, short value)
	{
		SwVoidAttribute item = new SwVoidAttribute(id, value);
		attributes.Add(item);
	}

	public void SetAttribute(int index, byte id, short value)
	{
		if (attributes.Count < index)
		{
			SwVoidAttribute swVoidAttribute = attributes[index];
			swVoidAttribute.id = id;
			swVoidAttribute.value = value;
		}
		else
		{
			Debug.Log("there is no such attribute");
		}
	}

	public void ChangeAttributeValue(int index, short value)
	{
		SwVoidAttribute swVoidAttribute = attributes[index];
		swVoidAttribute.value = value;
	}

	public byte GetAttributeId(int index)
	{
		if (attributes.Count < index)
		{
			return 0;
		}
		return attributes[index].id;
	}

	public string GetAttributeName(int index)
	{
		return SwVoidDataManager.GetAttributeNameFromId(GetAttributeId(index));
	}

	public short GetAttributeValue(int index)
	{
		if (attributes.Count < index)
		{
			return 0;
		}
		return attributes[index].value;
	}

	public List<SwVoidAttribute> GetAttributes()
	{
		return attributes;
	}

	public void SetPassiveSkill(short[] passiveSkills)
	{
		passiveSkillIds = passiveSkills;
	}

	public short[] GetPassiveSkillIds()
	{
		return passiveSkillIds;
	}

	public short GetPassiveSkillId(int index)
	{
		return passiveSkillIds[index];
	}

	public bool isPassiveSkillActive(int index)
	{
		return mLevel >= 2 * (index + 1);
	}

	public bool isPassiveSkillShow(int index)
	{
		return mLevel >= 2 * (index + 1) - 1;
	}

	public string GetAttributeDescription(int index)
	{
		if (AttributeDes == null)
		{
			RefreshAttributeDes();
		}
		return AttributeDes[index];
	}

	public string GetAttributeNextDescription(int index)
	{
		if (AttributeNextDes == null)
		{
			RefreshAttributeDes();
		}
		return AttributeNextDes[index];
	}

	public void RefreshAttributeDes()
	{
		if (AttributeDes == null)
		{
			AttributeDes = new string[2];
		}
		if (AttributeNextDes == null)
		{
			AttributeNextDes = new string[2];
		}
		for (int i = 0; i < 2; i++)
		{
			string empty = string.Empty;
			empty = GetAttributeName(i);
			empty += " + ";
			empty += GetAttributeValue(i);
			empty += " %";
			AttributeDes[i] = empty;
			if (!isMaxLevel())
			{
				AttributeNextDes[i] = SwVoidDataManager.GetDescriptionOfAttribueUpgrade(attributes[i].id, (byte)(mLevel + 1));
			}
		}
	}

	public void Resolve()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		for (byte b = 1; b == mLevel; b++)
		{
			short[] materialNeedId = SwVoidDataManager.GetMaterialNeedId((byte)mType, b);
			short[] materialNeedNum = SwVoidDataManager.GetMaterialNeedNum((byte)mType, b);
			for (int i = 0; i < materialNeedId.Length; i++)
			{
				short num = materialNeedNum[i];
				num >>= 1;
				if (num > 0)
				{
					short materialId = materialNeedId[i];
					SwVoidDataManager.AddMaterial(materialId, num);
				}
			}
		}
	}

	public void InitFromRemote(byte type, byte level, List<SwVoidAttribute> attributeList, short[] passiveSkillid)
	{
		mType = (ESwVoidType)type;
		mLevel = level;
		attributes = attributeList;
		for (int i = 0; i < passiveSkillIds.Length; i++)
		{
			if (i < passiveSkillid.Length)
			{
				passiveSkillIds[i] = passiveSkillid[i];
			}
			else
			{
				passiveSkillIds[i] = 0;
			}
		}
		CalculateInfoFromTypeAndLevel();
	}

	public List<Skill> GetInitSkill()
	{
		List<Skill> list = new List<Skill>();
		foreach (SwVoidAttribute attribute in attributes)
		{
			Skill skill = new Skill();
			skill.skillType = SwVoidDataManager.GetSkillTypeFromAttributeId(attribute.id);
			skill.data = attribute.value;
			list.Add(skill);
		}
		return list;
	}

	public void InitSkillGroup()
	{
		mSkillGroup = new SkillGroup();
		mSkillGroup.source = "SwVoid " + mType;
		foreach (SwVoidAttribute attribute in attributes)
		{
			mSkillGroup.AddStaticSkill(SwVoidDataManager.GetSkillTypeFromAttributeId(attribute.id), attribute.value);
		}
		if (isPassiveSkillActive(0))
		{
			mSkillGroup.InitSkillInfo(SwVoidDataManager.GetPassiveSkillIdInfoId(passiveSkillIds[0]));
		}
		if (isPassiveSkillActive(1))
		{
			mSkillGroup.InitSkillInfo(SwVoidDataManager.GetPassiveSkillIdInfoId(passiveSkillIds[1]));
		}
	}

	public void TakeOn(PlayerSkill playerSkill)
	{
		if (playerSkill == null)
		{
			Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
		}
		if (mSkillGroup == null)
		{
			InitSkillGroup();
		}
		mSkillGroup.InitToPlayerSkill(playerSkill);
	}

	public void TakeOff()
	{
		if (mSkillGroup != null)
		{
			mSkillGroup.UninitFromPlayerSkill();
		}
		else
		{
			Debug.Log("skillGroup is NULL : " + id);
		}
	}

	public void SetId(int id)
	{
		this.id = id;
	}

	public bool IsHighLevel()
	{
		return mLevel >= SwVoidParameter.HIGH_LEVEL;
	}
}

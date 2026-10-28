using System.Collections.Generic;
using System.IO;

public class SwVoidSavedataRec100
{
	private SwVoidSavedata mSwVoidSavedate;

	public SwVoidSavedataRec100(SwVoidSavedata swVoidSavedata)
	{
		mSwVoidSavedate = swVoidSavedata;
	}

	public void Save(BinaryWriter bw)
	{
		bw.Write((byte)mSwVoidSavedate.GetType());
		bw.Write(mSwVoidSavedate.GetLevel());
		bw.Write((byte)mSwVoidSavedate.GetAttributes().Count);
		for (int i = 0; i < mSwVoidSavedate.GetAttributes().Count; i++)
		{
			bw.Write(mSwVoidSavedate.GetAttributes()[i].id);
			bw.Write(mSwVoidSavedate.GetAttributes()[i].value);
		}
		bw.Write((byte)mSwVoidSavedate.GetPassiveSkillIds().Length);
		for (int j = 0; j < mSwVoidSavedate.GetPassiveSkillIds().Length; j++)
		{
			bw.Write(mSwVoidSavedate.GetPassiveSkillIds()[j]);
		}
	}

	public void Load(BinaryReader br)
	{
		byte type = br.ReadByte();
		byte level = br.ReadByte();
		byte b = br.ReadByte();
		List<SwVoidAttribute> list = new List<SwVoidAttribute>();
		for (int i = 0; i < b; i++)
		{
			SwVoidAttribute item = new SwVoidAttribute(br.ReadByte(), br.ReadInt16());
			list.Add(item);
		}
		byte b2 = br.ReadByte();
		short[] array = new short[b2];
		for (int j = 0; j < b2; j++)
		{
			array[j] = br.ReadInt16();
		}
		mSwVoidSavedate.InitFromRemote(type, level, list, array);
	}
}

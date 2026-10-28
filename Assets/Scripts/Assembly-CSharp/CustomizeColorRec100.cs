using UnityEngine;

public class CustomizeColorRec100
{
	private CustomizeColorData mData;

	public CustomizeColorRec100(CustomizeColorData data)
	{
		mData = data;
	}

	public void Save(BytesBuffer bb)
	{
		bb.AddByte((byte)mData.customHeadColor.Length);
		for (int i = 0; i < mData.customHeadColor.Length; i++)
		{
			bb.AddBool(mData.headChangeColorToggle[i]);
			bb.AddByte(mData.headChangeColorSlideValue[i]);
			bb.AddShort((short)(mData.customHeadColor[i].x * 100f));
			bb.AddShort((short)(mData.customHeadColor[i].y * 100f));
			bb.AddShort((short)(mData.customHeadColor[i].z * 100f));
			bb.AddShort((short)(mData.customHeadColor[i].w * 100f));
			bb.AddByte(mData.headChangeColorMode[i]);
		}
		bb.AddByte((byte)mData.customBodyColor.Length);
		for (int j = 0; j < mData.customBodyColor.Length; j++)
		{
			bb.AddBool(mData.bodyChangeColorToggle[j]);
			bb.AddByte(mData.bodyChangeColorSlideValue[j]);
			bb.AddShort((short)(mData.customBodyColor[j].x * 100f));
			bb.AddShort((short)(mData.customBodyColor[j].y * 100f));
			bb.AddShort((short)(mData.customBodyColor[j].z * 100f));
			bb.AddShort((short)(mData.customBodyColor[j].w * 100f));
			bb.AddByte(mData.bodyChangeColorMode[j]);
		}
		bb.AddByte((byte)mData.customHandColor.Length);
		for (int k = 0; k < mData.customHandColor.Length; k++)
		{
			bb.AddBool(mData.handChangeColorToggle[k]);
			bb.AddByte(mData.handChangeColorSlideValue[k]);
			bb.AddShort((short)(mData.customHandColor[k].x * 100f));
			bb.AddShort((short)(mData.customHandColor[k].y * 100f));
			bb.AddShort((short)(mData.customHandColor[k].z * 100f));
			bb.AddShort((short)(mData.customHandColor[k].w * 100f));
			bb.AddByte(mData.handChangeColorMode[k]);
		}
		bb.AddByte((byte)mData.customFootColor.Length);
		for (int l = 0; l < mData.customFootColor.Length; l++)
		{
			bb.AddBool(mData.footChangeColorToggle[l]);
			bb.AddByte(mData.footChangeColorSlideValue[l]);
			bb.AddShort((short)(mData.customFootColor[l].x * 100f));
			bb.AddShort((short)(mData.customFootColor[l].y * 100f));
			bb.AddShort((short)(mData.customFootColor[l].z * 100f));
			bb.AddShort((short)(mData.customFootColor[l].w * 100f));
			bb.AddByte(mData.footChangeColorMode[l]);
		}
	}

	public void Load(BytesBuffer bb)
	{
		byte b = 0;
		b = bb.ReadByte();
		for (int i = 0; i < b; i++)
		{
			mData.headChangeColorToggle[i] = bb.ReadBool();
			mData.headChangeColorSlideValue[i] = bb.ReadByte();
			short num = bb.ReadShort();
			short num2 = bb.ReadShort();
			short num3 = bb.ReadShort();
			short num4 = bb.ReadShort();
			mData.customHeadColor[i] = new Vector4((float)num / 100f, (float)num2 / 100f, (float)num3 / 100f, (float)num4 / 100f);
			mData.headChangeColorMode[i] = bb.ReadByte();
		}
		b = bb.ReadByte();
		for (int j = 0; j < b; j++)
		{
			mData.bodyChangeColorToggle[j] = bb.ReadBool();
			mData.bodyChangeColorSlideValue[j] = bb.ReadByte();
			short num5 = bb.ReadShort();
			short num6 = bb.ReadShort();
			short num7 = bb.ReadShort();
			short num8 = bb.ReadShort();
			mData.customBodyColor[j] = new Vector4((float)num5 / 100f, (float)num6 / 100f, (float)num7 / 100f, (float)num8 / 100f);
			mData.bodyChangeColorMode[j] = bb.ReadByte();
		}
		b = bb.ReadByte();
		for (int k = 0; k < b; k++)
		{
			mData.handChangeColorToggle[k] = bb.ReadBool();
			mData.handChangeColorSlideValue[k] = bb.ReadByte();
			short num9 = bb.ReadShort();
			short num10 = bb.ReadShort();
			short num11 = bb.ReadShort();
			short num12 = bb.ReadShort();
			mData.customHandColor[k] = new Vector4((float)num9 / 100f, (float)num10 / 100f, (float)num11 / 100f, (float)num12 / 100f);
			mData.handChangeColorMode[k] = bb.ReadByte();
		}
		b = bb.ReadByte();
		for (int l = 0; l < b; l++)
		{
			mData.footChangeColorToggle[l] = bb.ReadBool();
			mData.footChangeColorSlideValue[l] = bb.ReadByte();
			short num13 = bb.ReadShort();
			short num14 = bb.ReadShort();
			short num15 = bb.ReadShort();
			short num16 = bb.ReadShort();
			mData.customFootColor[l] = new Vector4((float)num13 / 100f, (float)num14 / 100f, (float)num15 / 100f, (float)num16 / 100f);
			mData.footChangeColorMode[l] = bb.ReadByte();
		}
	}
}

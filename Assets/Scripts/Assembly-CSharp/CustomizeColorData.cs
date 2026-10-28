using UnityEngine;

public class CustomizeColorData
{
	public bool[] headChangeColorToggle = new bool[Global.TOTAL_ARMOR_HEAD_NUM + 1];

	public bool[] bodyChangeColorToggle = new bool[Global.TOTAL_ARMOR_BODY_NUM + 1];

	public bool[] handChangeColorToggle = new bool[Global.TOTAL_ARMOR_ARM_NUM + 1];

	public bool[] footChangeColorToggle = new bool[Global.TOTAL_ARMOR_FOOT_NUM + 1];

	public byte[] headChangeColorSlideValue = new byte[Global.TOTAL_ARMOR_HEAD_NUM + 1];

	public byte[] bodyChangeColorSlideValue = new byte[Global.TOTAL_ARMOR_BODY_NUM + 1];

	public byte[] handChangeColorSlideValue = new byte[Global.TOTAL_ARMOR_ARM_NUM + 1];

	public byte[] footChangeColorSlideValue = new byte[Global.TOTAL_ARMOR_FOOT_NUM + 1];

	public Vector4[] customHeadColor = new Vector4[Global.TOTAL_ARMOR_HEAD_NUM + 1];

	public Vector4[] customBodyColor = new Vector4[Global.TOTAL_ARMOR_BODY_NUM + 1];

	public Vector4[] customHandColor = new Vector4[Global.TOTAL_ARMOR_ARM_NUM + 1];

	public Vector4[] customFootColor = new Vector4[Global.TOTAL_ARMOR_FOOT_NUM + 1];

	public byte[] headChangeColorMode = new byte[Global.TOTAL_ARMOR_HEAD_NUM + 1];

	public byte[] bodyChangeColorMode = new byte[Global.TOTAL_ARMOR_BODY_NUM + 1];

	public byte[] handChangeColorMode = new byte[Global.TOTAL_ARMOR_ARM_NUM + 1];

	public byte[] footChangeColorMode = new byte[Global.TOTAL_ARMOR_FOOT_NUM + 1];
}

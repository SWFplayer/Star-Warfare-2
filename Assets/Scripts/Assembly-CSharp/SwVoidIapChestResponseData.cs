using System.Collections.Generic;

public class SwVoidIapChestResponseData : SwVoidBaseResponseData
{
	public int result;

	public Dictionary<short, short> materialList;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			materialList = new Dictionary<short, short>();
			byte b = bb.ReadByte();
			for (int i = 0; i < b; i++)
			{
				short key = bb.ReadShort();
				short value = bb.ReadShort();
				materialList.Add(key, value);
			}
		}
	}
}

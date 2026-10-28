using System.Collections.Generic;

public class SwVoidIapChestRequest : Request
{
	protected byte chestLevel;

	protected byte size;

	protected List<short> wishMaterialList;

	public SwVoidIapChestRequest(IapScript.IapPackageType type, List<short> _wishMaterialList)
	{
		requestID = 64;
		switch (type)
		{
		case IapScript.IapPackageType.CopperChest:
			chestLevel = 1;
			break;
		case IapScript.IapPackageType.SilverChest:
			chestLevel = 2;
			break;
		case IapScript.IapPackageType.GoldChest:
			chestLevel = 3;
			break;
		}
		size = (byte)_wishMaterialList.Count;
		wishMaterialList = new List<short>();
		for (int i = 0; i < _wishMaterialList.Count; i++)
		{
			wishMaterialList.Add(_wishMaterialList[i]);
		}
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(2 + size * 2);
		bytesBuffer.AddByte(chestLevel);
		bytesBuffer.AddByte(size);
		for (int i = 0; i < size; i++)
		{
			bytesBuffer.AddShort(wishMaterialList[i]);
		}
		return bytesBuffer;
	}
}

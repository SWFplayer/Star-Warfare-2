using System.Collections.Generic;

public class SwVoidResolveResponseData : SwVoidBaseResponseData
{
	public int result;

	public int resolvedVoidId;

	public SwVoidMaterialResponseData[] materialArray;

	public Dictionary<short, short> returnMaterials;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			resolvedVoidId = bb.ReadInt();
			byte b = bb.ReadByte();
			materialArray = new SwVoidMaterialResponseData[b];
			for (byte b2 = 0; b2 < b; b2++)
			{
				materialArray[b2] = default(SwVoidMaterialResponseData);
				materialArray[b2].type = bb.ReadShort();
				materialArray[b2].number = bb.ReadShort();
			}
			byte b3 = bb.ReadByte();
			returnMaterials = new Dictionary<short, short>();
			for (byte b4 = 0; b4 < b3; b4++)
			{
				short key = bb.ReadShort();
				short value = bb.ReadShort();
				returnMaterials.Add(key, value);
			}
		}
	}
}

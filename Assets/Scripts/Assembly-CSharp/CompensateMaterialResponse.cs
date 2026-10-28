using System.Collections.Generic;

public class CompensateMaterialResponse : Response
{
	private byte size;

	private Dictionary<short, short> materialList;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		size = bytesBuffer.ReadByte();
		materialList = new Dictionary<short, short>();
		for (int i = 0; i < size; i++)
		{
			materialList.Add(bytesBuffer.ReadShort(), bytesBuffer.ReadShort());
		}
	}

	public override void ProcessLogic()
	{
		GameApp.GetInstance().GetUserState().SetCompensateMaterialList(materialList);
		if (LobbyScript.mInstance != null && LobbyScript.mInstance.gameObject != null && LobbyScript.mInstance.gameObject.activeSelf)
		{
			LobbyScript.mInstance.ShowCompensateMaterial(materialList);
		}
	}
}

using UnityEngine;

public class SwVoidSpawnResponse : Response
{
	private int swVoidOwnerId;

	protected byte swVoidType;

	protected byte swVoidLevel;

	protected byte[] swVoidAttributeId;

	protected short[] swVoidAttributeValue;

	protected short[] swVoidPassiveSkillId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		Debug.Log("No this Response Anymore!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
		swVoidOwnerId = bytesBuffer.ReadInt();
		swVoidType = bytesBuffer.ReadByte();
		swVoidLevel = bytesBuffer.ReadByte();
		swVoidAttributeId = new byte[2];
		swVoidAttributeId[0] = bytesBuffer.ReadByte();
		swVoidAttributeId[1] = bytesBuffer.ReadByte();
		swVoidAttributeValue = new short[2];
		swVoidAttributeValue[0] = bytesBuffer.ReadShort();
		swVoidAttributeValue[1] = bytesBuffer.ReadShort();
		swVoidPassiveSkillId = new short[2];
		swVoidPassiveSkillId[0] = bytesBuffer.ReadShort();
		swVoidPassiveSkillId[1] = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null && (gameWorld.State == GameState.Playing || !GameApp.GetInstance().GetGameMode().IsVSMode()) && gameWorld.GetLocalPlayer().GetChannelID() == swVoidOwnerId)
		{
			gameWorld.SpawnLocalSwVoid();
		}
	}
}

public class QuickJoinNoRoomResponse : Response
{
	private byte mGameMode;

	private byte mMapID;

	private byte mWinCondition;

	private short mWinValue;

	private byte mOnlineMode;

	private byte mRandomWinCondition;

	private short mRandomWinValue;

	private byte mDiffMode;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mGameMode = bytesBuffer.ReadByte();
		mMapID = bytesBuffer.ReadByte();
		mWinCondition = bytesBuffer.ReadByte();
		mWinValue = bytesBuffer.ReadShort();
		mOnlineMode = bytesBuffer.ReadByte();
		mRandomWinCondition = bytesBuffer.ReadByte();
		mRandomWinValue = bytesBuffer.ReadShort();
		mDiffMode = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GlobalData.CreateRoomData.mOnlineMode = mOnlineMode;
		GlobalData.CreateRoomData.mGameMode = mGameMode;
		GlobalData.CreateRoomData.mMapID = mMapID;
		GlobalData.CreateRoomData.mWinCondition = mWinCondition;
		GlobalData.CreateRoomData.mWinValue = mWinValue;
		GlobalData.CreateRoomData.mRandomWinCondition = mRandomWinCondition;
		GlobalData.CreateRoomData.mRandomWinValue = mRandomWinValue;
		GlobalData.CreateRoomData.mDiffMode = mDiffMode;
		string key = ((!GameApp.GetInstance().GetGameMode().IsVsMode(mGameMode)) ? "LOC_ROOM_Q2" : "LOC_ROOM_Q1");
		UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString(key), 2, 90);
	}
}

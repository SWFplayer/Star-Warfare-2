public class LeaveRoomResponse : Response
{
	private byte mType;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mType = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		if (mType == 0)
		{
			if (VSRoomUIScript.mInstance != null && VSRoomUIScript.mInstance.GetCurrentPage() == RoomPage.WaitParnterPage)
			{
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_27"));
				VSRoomUIScript.mInstance.ReturnToLobby();
			}
		}
		else if (mType == 1 && VSRoomUIScript.mInstance != null && VSRoomUIScript.mInstance.GetCurrentPage() == RoomPage.WaitParnterPage)
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_COOP_BOSS_KICKED"));
			VSRoomUIScript.mInstance.ReturnToLobby();
		}
	}
}

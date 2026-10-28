internal class ServerExceptionResponse : Response
{
	private string command;

	private string exception;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		command = bytesBuffer.ReadStringShortLength();
		exception = bytesBuffer.ReadStringShortLength();
	}

	public override void ProcessLogic()
	{
		GameApp.GetInstance().GetNetworkManager().CloseConnection();
		string exMessage = "Server Exception!!!\n\n" + command + "\n\n" + exception;
		SwHandlerExceptionRequest request = new SwHandlerExceptionRequest(GameApp.GetInstance().GetUserState().GetOnlineUserId(), exMessage);
		GameApp.GetInstance().GetWWWManager().SendRequest(request);
		UIMsgBox.instance.ShowException(UIMsgBox.instance);
	}
}

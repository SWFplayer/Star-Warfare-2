public class PvpExpIapResponse : Response
{
	private int mErrorCode;

	private byte pretige;

	private int exp;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mErrorCode = bytesBuffer.ReadInt();
		pretige = bytesBuffer.ReadByte();
		exp = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		UIMsgBox.instance.CloseMessage();
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, LocalizationManager.GetInstance().GetString("LOC_PVP_IAP_HINT2"), 1, 7);
		GameApp.GetInstance().GetUserState().GetOnlinePlayer()
			.SetPrestige(pretige);
		GameApp.GetInstance().GetUserState().GetOnlinePlayer()
			.SetExp(exp);
		if (PlayerInfoUI.mLocalInstance != null && PlayerInfoUI.mLocalInstance.gameObject.activeSelf)
		{
			PlayerInfoUI.mLocalInstance.PrestigeComplete(pretige, exp);
		}
		else if (CustomizeUIScript.mInstance != null && CustomizeUIScript.mInstance.gameObject.activeSelf)
		{
			CustomizeUIScript.mInstance.Refresh();
		}
		else if (ShopUIScript.GetInstance() != null)
		{
			ShopUIScript.GetInstance().RefreshLockIcon();
		}
	}
}

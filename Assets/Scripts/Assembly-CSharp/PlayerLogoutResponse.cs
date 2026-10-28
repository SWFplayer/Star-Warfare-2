using UnityEngine;

internal class PlayerLogoutResponse : Response
{
	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
	}

	public override void ProcessLogic()
	{
		GameApp.GetInstance().GetNetworkManager().CloseConnection();
		GameObject gameObject = GameObject.Find("GameUI");
		if (gameObject != null)
		{
			InGameUIScript component = gameObject.GetComponent<InGameUIScript>();
			if (component != null)
			{
				UIMsgBox.instance.ShowMessage(component, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_39"), 1, 39);
				return;
			}
		}
		UIMsgBox.instance.ShowForceLogoutMessage();
	}
}

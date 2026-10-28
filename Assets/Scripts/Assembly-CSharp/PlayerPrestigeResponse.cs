using UnityEngine;

public class PlayerPrestigeResponse : Response
{
	private int mErrorCode;

	private byte pretige;

	private int exp;

	private byte mCustomizeSize;

	private OnlineCustomizeData[] mCustomizeDataArray;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mErrorCode = bytesBuffer.ReadInt();
		pretige = bytesBuffer.ReadByte();
		exp = bytesBuffer.ReadInt();
		if (mErrorCode == SwErrorCode.SUCCESS)
		{
			mCustomizeSize = bytesBuffer.ReadByte();
			mCustomizeDataArray = new OnlineCustomizeData[mCustomizeSize];
			for (int i = 0; i < mCustomizeSize; i++)
			{
				mCustomizeDataArray[i] = new OnlineCustomizeData();
				OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(mCustomizeDataArray[i]);
				onlineCustomizeRec.ReadData(bytesBuffer);
			}
		}
	}

	public override void ProcessLogic()
	{
		Debug.Log("mErrorCode = " + mErrorCode);
		UIMsgBox.instance.CloseMessage();
		if (PlayerInfoUI.mLocalInstance != null)
		{
			if (mErrorCode == SwErrorCode.SUCCESS)
			{
				OnlineCustomizeData[] array = new OnlineCustomizeData[mCustomizeDataArray.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new OnlineCustomizeData(mCustomizeDataArray[i]);
				}
				GameApp.GetInstance().GetUserState().GetOnlinePlayer()
					.SetOnlineCustomizeArray(array);
				PlayerInfoUI.mLocalInstance.PrestigeComplete(pretige, exp);
			}
			else
			{
				UIMsgBox.instance.ShowErrorCodeMessage(mErrorCode);
			}
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(mErrorCode);
		}
	}
}

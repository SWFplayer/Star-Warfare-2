using System.Collections.Generic;

public class SwFriendGetRequest : SwFriendRequest
{
	private List<OnlineFriendResponseData> mReceiveFriendList = new List<OnlineFriendResponseData>();

	private List<OnlineFriendResponseData> mReceiveRequestList = new List<OnlineFriendResponseData>();

	private string mSendLoginTime;

	public SwFriendGetRequest(string loginTime)
	{
		mSendLoginTime = loginTime;
		mSign = Hash(mSendUserId + mSendLoginTime);
	}

	protected override string GetUrl()
	{
		return "http://" + mFriendServerAddress + "/Sw2/SwFriendGet?userId=" + mSendUserId + "&userName=" + ToBase64String(GameApp.GetInstance().GetUserState().GetOnlineNickname()) + "&loginTime=" + ToBase64String(mSendLoginTime) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			mReceiveFriendList = new List<OnlineFriendResponseData>();
			byte b = bb.ReadByte();
			for (byte b2 = 0; b2 < b; b2++)
			{
				OnlineFriendResponseData onlineFriendResponseData = new OnlineFriendResponseData();
				onlineFriendResponseData.ReadData(bb);
				mReceiveFriendList.Add(onlineFriendResponseData);
			}
			mReceiveRequestList = new List<OnlineFriendResponseData>();
			byte b3 = bb.ReadByte();
			for (byte b4 = 0; b4 < b3; b4++)
			{
				OnlineFriendResponseData onlineFriendResponseData2 = new OnlineFriendResponseData();
				onlineFriendResponseData2.ReadRequestData(bb);
				mReceiveRequestList.Add(onlineFriendResponseData2);
			}
		}
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode != SwErrorCode.SUCCESS)
		{
			return;
		}
		Dictionary<int, OnlineFriend> dictionary = new Dictionary<int, OnlineFriend>();
		foreach (OnlineFriendResponseData mReceiveFriend in mReceiveFriendList)
		{
			OnlineFriend onlineFriend = new OnlineFriend(mReceiveFriend);
			dictionary.Add(onlineFriend.GetUserId(), onlineFriend);
		}
		GameApp.GetInstance().GetUserState().SetFriendDictionary(dictionary);
		List<OnlineFriend> list = new List<OnlineFriend>();
		foreach (OnlineFriendResponseData mReceiveRequest in mReceiveRequestList)
		{
			OnlineFriend item = new OnlineFriend(mReceiveRequest);
			list.Add(item);
		}
		GameApp.GetInstance().GetUserState().SetRequestList(list);
		if (LobbyScript.mInstance != null)
		{
			LobbyScript.mInstance.CheckFriendNotify();
		}
	}
}

using System;
using UnityEngine;

public class RoleLoginResponse : Response
{
	private bool mSuccess;

	private bool mDataValid;

	private int mChannelId;

	private bool mIsRegist;

	private EUserType mUserType;

	private bool mNeedResetFriendTime;

	private bool mFirstBossChallenge;

	private bool mEnableMutantMode;

	private int mSaveNum;

	private bool mSendDownloadData;

	private bool mCanUploadData;

	private string mName;

	private int mExp;

	private byte mPrestige;

	private byte mCustomizeSize;

	private byte mPvpSkillGridSize;

	private byte mCustomizeId;

	private OnlineCustomizeData[] mCustomizeDataArray;

	private IRecordset mRecord;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mSuccess = bytesBuffer.ReadBool();
		if (!mSuccess)
		{
			return;
		}
		mDataValid = true;
		mChannelId = bytesBuffer.ReadInt();
		mIsRegist = bytesBuffer.ReadBool();
		mUserType = (EUserType)bytesBuffer.ReadByte();
		mNeedResetFriendTime = bytesBuffer.ReadBool();
		mFirstBossChallenge = bytesBuffer.ReadBool();
		mEnableMutantMode = bytesBuffer.ReadBool();
		mName = bytesBuffer.ReadString();
		mExp = bytesBuffer.ReadInt();
		mPrestige = bytesBuffer.ReadByte();
		mCustomizeSize = bytesBuffer.ReadByte();
		mPvpSkillGridSize = bytesBuffer.ReadByte();
		mCustomizeId = bytesBuffer.ReadByte();
		mCustomizeDataArray = new OnlineCustomizeData[mCustomizeSize];
		for (int i = 0; i < mCustomizeSize; i++)
		{
			mCustomizeDataArray[i] = new OnlineCustomizeData();
			OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(mCustomizeDataArray[i]);
			onlineCustomizeRec.ReadData(bytesBuffer);
		}
		if (mUserType != EUserType.USER && mUserType != EUserType.GUEST)
		{
			return;
		}
		mSaveNum = bytesBuffer.ReadInt();
		mSendDownloadData = bytesBuffer.ReadBool();
		mCanUploadData = bytesBuffer.ReadBool();
		UserState userState = GameApp.GetInstance().GetUserState();
		if (!mSendDownloadData)
		{
			return;
		}
		mRecord = RecordFactory.CreateRecord(userState.readVersion = bytesBuffer.ReadString(), userState);
		try
		{
			mRecord.LoadBytesBuffer(bytesBuffer);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.StackTrace);
			Debug.Log("Invalid binary data!!! ");
			mDataValid = false;
		}
	}

	public override void ProcessLogic()
	{
		if (!mSuccess)
		{
			GameApp.GetInstance().DestoryNetWork();
			UIMsgBox.instance.CloseMessage();
			if (MainMenuScript.mInstance != null)
			{
				MainMenuScript.mInstance.LogicServerFailed();
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.LogicServerFailed();
			}
			return;
		}
		if (!mDataValid)
		{
			GameApp.GetInstance().DestoryNetWork();
			UIMsgBox.instance.CloseMessage();
			if (MainMenuScript.mInstance != null)
			{
				MainMenuScript.mInstance.LogicSavaDataInvalid();
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.LogicSavaDataInvalid();
			}
			return;
		}
		TimeManager.GetInstance().LastSynTime = Time.time;
		Lobby.GetInstance().SetChannelID(mChannelId);
		Lobby.GetInstance().FirstBossChallenge = mFirstBossChallenge;
		Lobby.GetInstance().EnableMutantMode = mEnableMutantMode;
		OnlinePlayer onlinePlayer = new OnlinePlayer();
		onlinePlayer.SetExp(mExp);
		onlinePlayer.SetPrestige(mPrestige);
		OnlineCustomizeData[] array = new OnlineCustomizeData[mCustomizeDataArray.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new OnlineCustomizeData(mCustomizeDataArray[i]);
		}
		onlinePlayer.SetOnlineCustomizeArray(array);
		GameApp.GetInstance().GetUserState().SetOnlinePlayer(onlinePlayer);
		if (MainMenuScript.mInstance != null)
		{
			MainMenuScript.mInstance.bIsRegist = mIsRegist;
			MainMenuScript.mInstance.bNeedResetFriendTime = mNeedResetFriendTime;
			if ((mUserType == EUserType.USER || mUserType == EUserType.GUEST) && mSendDownloadData)
			{
				MainMenuScript.mInstance.SetRecordFromServer(mRecord);
			}
			MainMenuScript.mInstance.CorrectLogin();
			MainMenuScript.mInstance.UploadDataLogic();
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			MainMenu2DScript.mInstance.bIsRegist = mIsRegist;
			MainMenu2DScript.mInstance.bNeedResetFriendTime = mNeedResetFriendTime;
			MainMenu2DScript.mInstance.bSendDownloadData = mSendDownloadData;
			MainMenu2DScript.mInstance.bCanUploadData = mCanUploadData;
			if ((mUserType == EUserType.USER || mUserType == EUserType.GUEST) && mSendDownloadData)
			{
				MainMenu2DScript.mInstance.SetRecordFromServer(mRecord);
			}
			MainMenu2DScript.mInstance.CorrectLogin();
			MainMenu2DScript.mInstance.UploadDataLogic();
		}
		else if (LobbyScript.mInstance != null)
		{
			LobbyScript.mInstance.OnRegistComplete();
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		if (UIRobot.sShowLog)
		{
			Debug.Log("RoleLoginRespose::ProcessRobotLogic");
		}
		robot.lobby.SetChannelID(mChannelId);
	}
}

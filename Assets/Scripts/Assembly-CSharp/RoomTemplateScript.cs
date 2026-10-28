using UnityEngine;

public class RoomTemplateScript : MonoBehaviour
{
	public UILabel RoomName;

	public UILabel RoomMode;

	public UILabel RoomPopulation;

	public UISprite Background;

	public UISprite MapIcon;

	public UISprite LockIcon;

	public UISprite PingIcon;

	public UISprite DiffcultyIcon;

	public UILabel LevelLimit;

	public UISprite Highlight;

	protected int mRoomID;

	protected VSTarget mTarget;

	protected bool mHasFriendIn;

	protected bool mHasPassword;

	protected short mPassword;

	protected bool mLevelFit;

	protected short mPing;

	public int RoomID
	{
		get
		{
			return mRoomID;
		}
	}

	public void Init(int roomID, VSTarget target, bool hasFriendIn, Mode mode, int maxPlayerCount, int playerCount, int mapID, string roomName, bool hasPassword, short password, short ping, EDifficultMode diff, int minLevel, int maxLevel)
	{
		mLevelFit = true;
		mRoomID = roomID;
		mTarget = target;
		mHasFriendIn = hasFriendIn;
		if (mTarget == VSTarget.PVE)
		{
			Background.spriteName = "room_bk";
			switch (diff)
			{
			case EDifficultMode.NORMAL:
				DiffcultyIcon.spriteName = "normal";
				break;
			case EDifficultMode.HEROIC:
				DiffcultyIcon.spriteName = "hard";
				break;
			}
			DiffcultyIcon.gameObject.SetActive(true);
			LevelLimit.gameObject.SetActive(true);
			LevelLimit.text = "LV." + minLevel + "-" + maxLevel;
			int rankID = GameApp.GetInstance().GetUserState().GetRank()
				.rankID;
			if (rankID < minLevel || rankID > maxLevel)
			{
				LevelLimit.text = "[FF0000]" + LevelLimit.text + "[-]";
				mLevelFit = false;
			}
		}
		else
		{
			Background.spriteName = "room1_bk";
			DiffcultyIcon.gameObject.SetActive(false);
			LevelLimit.gameObject.SetActive(true);
			LevelLimit.text = "LV." + minLevel + "-" + maxLevel;
			int rankID2 = GameApp.GetInstance().GetUserState().GetRank()
				.rankID;
			if (rankID2 < minLevel || rankID2 > maxLevel)
			{
				LevelLimit.text = "[FF0000]" + LevelLimit.text + "[-]";
				mLevelFit = false;
			}
		}
		if (mHasFriendIn)
		{
		}
		RoomName.text = roomName;
		RoomPopulation.text = playerCount + "/" + maxPlayerCount;
		switch (mode)
		{
		case Mode.Boss:
			if (mapID == 0)
			{
				MapIcon.spriteName = "LOC_LEVEL_MAP_0_BOSS";
			}
			else
			{
				MapIcon.spriteName = "LOC_LEVEL_MAP_" + ((mapID - 1) % Global.TOTAL_ACTIVATED_STAGE + 1) + "_BOSS";
			}
			RoomMode.text = LocalizationManager.GetInstance().GetString("LOC_MISSION_STAGE_BOSS");
			break;
		case Mode.Survival:
			MapIcon.spriteName = "LOC_LEVEL_MAP_" + ((mapID - 1) % Global.TOTAL_ACTIVATED_STAGE + 1);
			RoomMode.text = LocalizationManager.GetInstance().GetString("LOC_MISSION_STAGE_EXPLORE");
			break;
		case Mode.VS_TDM:
			RoomMode.text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_2");
			MapIcon.spriteName = "LOC_VS_MAP_" + ((mapID - 1) % Global.TOTAL_ACTIVATED_STAGE + 1);
			break;
		case Mode.VS_FFA:
			RoomMode.text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_3");
			MapIcon.spriteName = "LOC_VS_MAP_" + ((mapID - 1) % Global.TOTAL_ACTIVATED_STAGE + 1);
			break;
		case Mode.VS_CTF_TDM:
			RoomMode.text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_4");
			MapIcon.spriteName = "LOC_VS_MAP_" + ((mapID - 1) % Global.TOTAL_ACTIVATED_STAGE + 1);
			break;
		case Mode.VS_KOTH:
			RoomMode.text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_5");
			MapIcon.spriteName = "LOC_VS_MAP_" + ((mapID - 1) % Global.TOTAL_ACTIVATED_STAGE + 1);
			break;
		case Mode.Domination:
			RoomMode.text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_6");
			MapIcon.spriteName = "LOC_VS_MAP_" + ((mapID - 1) % Global.TOTAL_ACTIVATED_STAGE + 1);
			break;
		case Mode.GIANT_BETRAYAL:
			RoomMode.text = LocalizationManager.GetInstance().GetString("LOC_PVP_ALLMODE_7");
			MapIcon.spriteName = "LOC_VS_BOSSS_0";
			break;
		}
		Highlight.gameObject.SetActive(false);
		mHasPassword = hasPassword;
		mPassword = password;
		LockIcon.gameObject.SetActive(mHasPassword);
		mPing = ping;
		if (mPing <= 100)
		{
			PingIcon.spriteName = "net_5";
		}
		else if (mPing <= 200)
		{
			PingIcon.spriteName = "net_4";
		}
		else if (mPing <= 400)
		{
			PingIcon.spriteName = "net_3";
		}
		else if (mPing <= 700)
		{
			PingIcon.spriteName = "net_2";
		}
		else
		{
			PingIcon.spriteName = "net_1";
		}
	}

	public void SetHighlight(bool isHighlight)
	{
		Highlight.gameObject.SetActive(isHighlight);
	}

	public bool HasPassword()
	{
		return mHasPassword;
	}

	public short GetPassword()
	{
		return mPassword;
	}

	public bool IsLevelFit()
	{
		return mLevelFit;
	}
}

using System.Collections.Generic;

public class GlobalData
{
	public static CreateRoomData CreateRoomData;

	private static GlobalData mInstance;

	public LocalPlayer mPlayer { get; set; }

	public List<RemotePlayer> RemotePlayerList { get; set; }

	public string BossLevelName { get; set; }

	public OnlineFriend Partner { get; set; }

	public int PartnerHp { get; set; }

	public int PartnerWeaponIndex { get; set; }

	public short[] ItemArray { get; set; }

	public static GlobalData GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new GlobalData();
		}
		return mInstance;
	}
}

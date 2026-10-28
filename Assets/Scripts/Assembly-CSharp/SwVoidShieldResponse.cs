using System.Collections.Generic;

public class SwVoidShieldResponse : Response
{
	private byte isSettingMaxHP;

	private int HP;

	private int channelID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		isSettingMaxHP = bytesBuffer.ReadByte();
		HP = bytesBuffer.ReadInt();
		channelID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (GameApp.GetInstance().GetGameWorld() == null)
		{
			return;
		}
		List<ShieldScript> currentShields = GameApp.GetInstance().GetGameWorld().GetCurrentShields();
		foreach (ShieldScript item in currentShields)
		{
			if (item != null && item.ownedPlayer != null && item.ownedPlayer.GetChannelID() == channelID)
			{
				if (isSettingMaxHP == 1)
				{
					item.SetShieldHP(HP, true);
				}
				else
				{
					item.SetShieldHP(HP, false);
				}
			}
		}
	}
}

using UnityEngine;

public class PlayerPlayVFXResponse : Response
{
	protected int playerID;

	protected short skillID;

	protected bool playOrDestroy;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		skillID = bytesBuffer.ReadShort();
		playOrDestroy = bytesBuffer.ReadByte() == 1;
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		Player playerByUserID = gameWorld.GetPlayerByUserID(playerID);
		if (playerByUserID != null)
		{
			AdvancedSkill advancedSkill = playerByUserID.GetSkills().GetAdvancedSkill(skillID);
			if (advancedSkill != null)
			{
				if (advancedSkill.GetOnTriggerType() == AdvancedSkill.EOnTriggerType.OPEN_SUPER_SHIELD)
				{
					if (playOrDestroy)
					{
						advancedSkill.Active();
					}
					else
					{
						advancedSkill.Inactive();
					}
				}
			}
			else
			{
				Debug.Log("Skill == null");
			}
			if (playOrDestroy)
			{
				gameWorld.PlayVFXOnPlayer(playerByUserID, skillID);
			}
			else
			{
				gameWorld.StopVFXOnPlayer(playerByUserID, skillID);
			}
		}
		else
		{
			Debug.Log("Can't find player " + playerID);
		}
	}
}

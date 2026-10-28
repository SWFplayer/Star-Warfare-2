public class PlayerReadyToMutantResponse : Response
{
	protected int score;

	protected byte type;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		score = bytesBuffer.ReadInt();
		type = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
		if (gameWorld != null && localPlayer != null)
		{
			if (HUDBase.instance != null)
			{
				((HUDOnline)HUDBase.instance).OpenScorePage(false);
			}
			UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
			if (infoBox != null)
			{
				infoBox.SetMutantInfo(LocalizationManager.GetInstance().GetString("LOC_MUTANT_HUD6"));
			}
			localPlayer.RefreshAvatar((EnemyType)type);
			localPlayer.SetState(Player.BOSS_CHOOSE_SKILL_STATE);
			localPlayer.BossSkillScore = score;
		}
	}
}

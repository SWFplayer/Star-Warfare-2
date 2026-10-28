public class MissionActionDialog : MissionAction
{
	private string mLocId;

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num == base.ID)
			{
				mLocId = dataTable.GetString(i, 1);
				break;
			}
		}
	}

	public override void StartAction()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		UserState userState = GameApp.GetInstance().GetUserState();
		bool firstComplete = gameWorld.GetFirstComplete();
		bool skipDialog = userState.GetSkipDialog();
		if ((firstComplete || !skipDialog) && userState.GetSelectedDifficultMode() == EDifficultMode.NORMAL && GameApp.GetInstance().GetGameMode().IsSingle())
		{
			UserStateUI.GetInstance().FightDialogManager.PushMissionDialog(mLocId);
		}
	}
}

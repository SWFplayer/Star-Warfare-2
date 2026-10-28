public class MissionActionVehicle : MissionAction
{
	private int parameter1;

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num == base.ID)
			{
				parameter1 = dataTable.GetInt(i, 1);
				break;
			}
		}
	}

	public override void StartAction()
	{
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			GameApp.GetInstance().GetGameWorld().SpawnVehicle(GameApp.GetInstance().GetGameWorld().GetLocalPlayer(), (EVehicleType)parameter1);
		}
	}
}

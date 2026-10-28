public class MissionActionRadar : MissionAction
{
	public override void LoadConfig()
	{
	}

	public override void StartAction()
	{
		if (UIRadar.mInstance != null)
		{
			UIRadar.mInstance.DisturbRadarByStory();
		}
	}
}

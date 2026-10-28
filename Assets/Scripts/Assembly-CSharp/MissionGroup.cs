public class MissionGroup
{
	public Mission[] MissionArray;

	public Mission this[int id]
	{
		get
		{
			return MissionArray[id];
		}
		set
		{
			MissionArray[id] = value;
		}
	}

	public int Length
	{
		get
		{
			return MissionArray.Length;
		}
	}
}

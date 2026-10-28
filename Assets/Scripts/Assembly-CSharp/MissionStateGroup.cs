public class MissionStateGroup
{
	public bool[] StatusArray;

	public string[] VersionStatusArray;

	public bool this[int id]
	{
		get
		{
			return StatusArray[id];
		}
		set
		{
			StatusArray[id] = value;
		}
	}

	public int Length
	{
		get
		{
			return StatusArray.Length;
		}
	}

	public string GetVersion(int id)
	{
		return VersionStatusArray[id];
	}

	public void SetVersion(int id, string version)
	{
		VersionStatusArray[id] = version;
	}
}

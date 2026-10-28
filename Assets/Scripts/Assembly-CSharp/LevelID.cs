public struct LevelID
{
	public EDifficultMode Difficult;

	public byte StageID;

	public byte SubStageID;

	public bool IsBefore(LevelID levelID)
	{
		if (Difficult < levelID.Difficult)
		{
			return true;
		}
		if (Difficult == levelID.Difficult)
		{
			if (StageID < levelID.StageID)
			{
				return true;
			}
			if (StageID == levelID.StageID)
			{
				if (SubStageID <= levelID.SubStageID)
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}
}

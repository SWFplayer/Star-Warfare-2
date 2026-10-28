public class RecordFactory
{
	public static IRecordset CreateRecord(string version, UserState userState)
	{
		IRecordset result = null;
		if (version.Equals(Record100.mVersion))
		{
			result = new Record100(userState);
		}
		else if (version.Equals(Record101.mVersion))
		{
			result = new Record101(userState);
		}
		else if (version.Equals(Record102.mVersion))
		{
			result = new Record102(userState);
		}
		else if (version.Equals(Record103.mVersion))
		{
			result = new Record103(userState);
		}
		else if (version.Equals(Record104.mVersion))
		{
			result = new Record104(userState);
		}
		else if (version.Equals(Record110.mVersion))
		{
			result = new Record110(userState);
		}
		else if (version.Equals(Record111.mVersion))
		{
			result = new Record111(userState);
		}
		else if (version.Equals(Record112.mVersion))
		{
			result = new Record112(userState);
		}
		else if (version.Equals(Record113.mVersion))
		{
			result = new Record113(userState);
		}
		else if (version.Equals(Record120.mVersion))
		{
			result = new Record120(userState);
		}
		return result;
	}

	public static IRecordPart CreateAchievementRecord(string version, AchievementState state)
	{
		IRecordPart result = null;
		if (version.Equals(Record100.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record101.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record102.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record103.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record104.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record110.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record111.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record112.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record113.mVersion))
		{
			result = new AchievementRec100(state);
		}
		else if (version.Equals(Record120.mVersion))
		{
			result = new AchievementRec100(state);
		}
		return result;
	}
}

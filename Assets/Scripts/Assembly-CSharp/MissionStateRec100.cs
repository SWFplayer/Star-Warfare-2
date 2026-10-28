public class MissionStateRec100
{
	private MissionState mState;

	public MissionStateRec100(MissionState state)
	{
		mState = state;
	}

	public void SaveData(BytesBuffer bb)
	{
		MissionStateGroup[][] missionStatusArray = mState.GetMissionStatusArray();
		bb.AddByte((byte)missionStatusArray.Length);
		if (missionStatusArray.Length > 0)
		{
			bb.AddByte((byte)missionStatusArray[0].Length);
			if (missionStatusArray[0].Length > 0)
			{
				bb.AddByte((byte)missionStatusArray[0][0].Length);
			}
			else
			{
				bb.AddByte(0);
			}
		}
		else
		{
			bb.AddByte(0);
			bb.AddByte(0);
		}
		for (int i = 0; i < missionStatusArray.Length; i++)
		{
			for (int j = 0; j < missionStatusArray[0].Length; j++)
			{
				for (int k = 0; k < missionStatusArray[0][0].Length; k++)
				{
					bb.AddBool(missionStatusArray[i][j][k]);
				}
			}
		}
		bb.AddByte(mState.GetUnlockedStageId());
		bb.AddByte(mState.GetUnlockedSubStageId());
	}

	public void LoadData(BytesBuffer bb)
	{
		MissionStateGroup[][] array = new MissionStateGroup[Global.TOTAL_STAGE][];
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			array[i] = new MissionStateGroup[Global.TOTAL_SUB_STAGE];
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				array[i][j] = new MissionStateGroup();
				array[i][j].StatusArray = new bool[Global.MISSION_SIZE];
			}
		}
		byte b = bb.ReadByte();
		byte b2 = bb.ReadByte();
		byte b3 = bb.ReadByte();
		for (int k = 0; k < b; k++)
		{
			for (int l = 0; l < b2; l++)
			{
				for (int m = 0; m < b3; m++)
				{
					array[k][l][m] = bb.ReadBool();
				}
			}
		}
		mState.SetUnlockedStageId(bb.ReadByte());
		mState.SetUnlockedSubStageId(bb.ReadByte());
		mState.SetMissionStatusArray(array);
	}
}

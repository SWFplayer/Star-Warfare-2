using UnityEngine;

public abstract class MissionAction
{
	private float mStartTime;

	public int ID { get; set; }

	public float Delay { get; set; }

	public MissionActionType Type { get; set; }

	public EDifficultMode difficultMode { get; set; }

	public virtual void LoadConfig()
	{
	}

	public virtual void LevelStart()
	{
	}

	public virtual void StartAction()
	{
	}

	public void OnTrigger()
	{
		mStartTime = Time.time;
		MissionManager.GetInstance().AddAction(this);
	}

	public bool CanStart()
	{
		return Time.time - mStartTime > Delay;
	}

	protected GameDataTable GetDataTable()
	{
		string fileName = DataFileName.ACTION_LIST;
		if (difficultMode == EDifficultMode.HEROIC)
		{
			fileName = DataFileName.ACTION_LIST_HEROIC;
		}
		return GameDataManager.GetInstance().GetGameDataTable(fileName, Type.ToString());
	}
}

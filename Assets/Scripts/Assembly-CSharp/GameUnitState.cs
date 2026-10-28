public class GameUnitState
{
	public virtual void Start(GameUnit gameUnit)
	{
	}

	public virtual void Update(GameUnit gameUnit)
	{
	}

	public virtual void End(GameUnit gameUnit)
	{
	}

	public virtual bool IsFinish(GameUnit gameUnit)
	{
		return false;
	}

	public virtual void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
	}

	protected T GetData<T>(GameUnit gameUnit) where T : GameUnitStateData, new()
	{
		if (!gameUnit.StateData.ContainsKey(this))
		{
			gameUnit.StateData.Add(this, new T());
		}
		return (T)gameUnit.StateData[this];
	}
}

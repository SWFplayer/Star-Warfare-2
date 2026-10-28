public abstract class PlayerState : GameUnitState
{
	public abstract void NextState(Player player, float deltaTime);
}

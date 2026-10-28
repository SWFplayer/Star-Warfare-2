public class BossParameter
{
	public static float BOSS_HARD_MODE_HP_SCALE = 5f;

	public static float BOSS_HARD_MODE_ATTACK_SCALE = 15f;

	public int[] DAMAGES;

	public PlayerDamageType[] DAMAGE_TYPES;

	public int GetDamage(int index)
	{
		if (GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() == EDifficultMode.HEROIC)
		{
			return (int)((float)DAMAGES[index] * BOSS_HARD_MODE_ATTACK_SCALE);
		}
		return DAMAGES[index];
	}

	public PlayerDamageType GetDamageType(int index)
	{
		return DAMAGE_TYPES[index];
	}
}

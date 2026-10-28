public class PlayerSkillBorrowedTime : PlayerSpecialSkill
{
	public static bool isActive;

	private Player player;

	public void Init()
	{
		isActive = true;
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		player.ResetAbsorbDamage();
	}

	public override void OnInactive()
	{
		player.AddShield(player.GetAbsorbDamage(), 0f);
		isActive = false;
	}
}

public class MonsterConfig
{
	public class Attack
	{
		public int damage;

		public float attackRate;

		public float range;

		public float bombRange;

		public float splashDamage;

		public float splashDuration;

		public float splashRange;

		public float moveSpeed;
	}

	public float walkSpeed;

	public bool canAccelerate;

	public int hp;

	public int exp;

	public int lootCash;

	public int[] AttackDamages = new int[3];

	public float AttackFrequency;

	public float pickupRate;

	public int pickupLower;

	public int pickupUpper;
}

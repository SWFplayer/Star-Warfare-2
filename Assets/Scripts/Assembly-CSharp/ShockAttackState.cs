using UnityEngine;

public class ShockAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

	private bool vfxPlayed;

	public override void Start(Enemy enemy)
	{
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		enemy.PlayAnimation("attack_sp", WrapMode.ClampForever);
		enemy.StartEnemyAttack();
		vfxPlayed = false;
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.CheckHit2(((Tank)enemy).ShockAttackSkill);
		if (enemy.AnimationPlayed("attack_sp", 0.42f) && !vfxPlayed)
		{
			Object original = Resources.Load(EffectName.E7_Earthquake);
			Object.Instantiate(original, enemy.GetPosition() + Vector3.up * 0.1f, enemy.GetTransform().rotation);
			vfxPlayed = true;
		}
	}

	public override void End(Enemy enemy)
	{
	}
}

using UnityEngine;

public class TacorlionRangeAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

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
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Tacorlion tacorlion = enemy as Tacorlion;
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.InnerState != 0 || !(tacorlion.CurrentAnimTime >= 0.34146342f))
		{
			return;
		}
		myData.InnerState++;
		if (enemy.IsMasterPlayer)
		{
			float groundHeight = tacorlion.GetGroundHeight();
			Vector3 position = tacorlion.GetPosition();
			float num = -2f;
			Vector3 vector = tacorlion.GetTargetPosition() - position;
			vector.y += 1.5f;
			float magnitude = vector.magnitude;
			float num2 = 12f;
			float num3 = magnitude / num2;
			float num4 = (num - 0.5f * Physics.gravity.y * num3 * num3) / num3;
			Vector3 vector2 = Vector3.up * num4 + vector.normalized * num2;
			GameObject original = Resources.Load(EffectName.E3_Stick) as GameObject;
			GameObject gameObject = Object.Instantiate(original, position + Vector3.up * (0f - num), Quaternion.LookRotation(-vector2)) as GameObject;
			EnemyShotScript component = gameObject.GetComponent<EnemyShotScript>();
			component.trType = TrajectoryType.Parabola;
			component.damageType = EnemyDamageType.Normal;
			component.speed = vector2;
			component.attackDamage = tacorlion.attackDamage;
			component.enemyType = tacorlion.EnemyType;
			component.attacker = enemy;
			component.PlayerDamageType = enemy.GetPlayerDamageType();
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				EnemyShotRequest request = new EnemyShotRequest(2, position + Vector3.up * (0f - num), vector2, enemy.EnemyID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}
}

using UnityEngine;

public class DeadEyeDashMoveState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public GameObject Vfx;
	}

	public override void Start(Enemy enemy)
	{
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		DeadEye deadEye = (DeadEye)enemy;
		sDashAttack dashAttackParameter = deadEye.DashAttackParameter;
		enemy.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		enemy.MoveToward(enemy.GetTargetPosition(), dashAttackParameter.DASH_SPEED[(int)deadEye.GetMindState()]);
		deadEye.ResetAttack(deadEye.mSkill.DashAttackSkill);
		GameObject original = Resources.Load("SW2_Effect/B2_Dash02_smoke") as GameObject;
		myData.Vfx = Object.Instantiate(original, enemy.GetPosition(), Quaternion.identity) as GameObject;
		myData.Vfx.transform.parent = enemy.GetTransform();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.CheckHit2(((DeadEye)enemy).mSkill.DashAttackSkill);
		if (enemy.IsAnimationTriggered(0f) || enemy.IsAnimationTriggered(0.5f))
		{
			enemy.PlaySound(AudioName.DEADEYE_DASH);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		MyData myData = enemy.StateData[this] as MyData;
		Object.Destroy(myData.Vfx);
	}
}

using UnityEngine;

public class CerberliskBigFireBallState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Transform FireTransform;
	}

	public bool IsInitState;

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		data.FireTransform = cerberlisk.mSkill.FirePosition;
		enemy.PlayAnimation("bigfireball", WrapMode.ClampForever);
		if (!IsInitState)
		{
			enemy.LookAtTargetHorizontal();
		}
		cerberlisk.mSkill.PreFireVfx.SetActive(true);
		cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PRE_FIRE);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		MyData data = GetData<MyData>(enemy);
		if (enemy.IsAnimationTriggered(0.1f))
		{
			cerberlisk.PlaySound(AudioName.CERBERLISK_BIG_FIREBALL);
		}
		if (enemy.CurrentAnimTime < CerberliskParameter.BIG_FIRE_BALL_TRIGGER_TIME)
		{
			LookAtTargetPosition(enemy, CerberliskParameter.BIG_FIRE_BALL_ROTATE_SPEED);
		}
		if (enemy.IsAnimationTriggered(CerberliskParameter.BIG_FIRE_BALL_TRIGGER_TIME))
		{
			if (cerberlisk.mSkill.PreFireVfx.activeSelf)
			{
				cerberlisk.mSkill.PreFireVfx.SetActive(false);
				cerberlisk.StopSound(AudioName.CERBERLISK_PRE_FIRE);
			}
			float num = 0f;
			num = ((!IsInitState) ? Vector3.Magnitude(enemy.GetTargetPosition() - enemy.GetPosition()) : (Vector3.Magnitude(GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetPosition() - enemy.GetPosition()) - 4f));
			Vector3 vector = enemy.GetPosition() + enemy.GetTransform().forward * num + Vector3.up;
			Vector3 forward = vector - data.FireTransform.position;
			GameObject go = Pool.sAllocate("SW2_Effect/B4_bigfireball", data.FireTransform.position, forward);
			CerberliskBigFireBallScript component = GameWorld.GetComponent<CerberliskBigFireBallScript>(go);
			component.Speed = forward.normalized * CerberliskParameter.BIG_FIRE_BALL_SPEED;
			component.SetDamageInfo(CerberliskParameter.GetInstance(), 5);
			component.ExplodeRadius = CerberliskParameter.BIG_FIRE_BALL_EXPLODE_RADIUS;
			component.Vfx = "SW2_Effect/B4_bigfireballhit";
			component.VfxSound = AudioName.CERBERLISK_BIG_FIREBALL_HIT;
			component.VfxDuration = CerberliskParameter.BIG_FIRE_BALL_DOT_DURATION;
			component.Attacker = enemy;
			component.Reset();
		}
	}

	public override void End(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.mSkill.PreFireVfx.SetActive(false);
		cerberlisk.StopSound(AudioName.CERBERLISK_PRE_FIRE);
	}
}

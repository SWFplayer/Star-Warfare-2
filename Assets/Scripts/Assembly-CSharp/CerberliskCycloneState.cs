using UnityEngine;

public class CerberliskCycloneState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Transform FireTransform;
	}

	public override void Start(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		data.FireTransform = cerberlisk.mSkill.FirePosition;
		enemy.PlayAnimation("cyclone", WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		cerberlisk.mSkill.PreFireVfx.SetActive(true);
		cerberlisk.mSkill.PreWindVfx.SetActive(true);
		cerberlisk.mSkill.BreatheOutVfx.SetActive(false);
		cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PRE_FIRE);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData data = GetData<MyData>(enemy);
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		LookAtTargetPosition(enemy, CerberliskParameter.CYCLONE_ROTATE_TIME);
		if (enemy.IsAnimationTriggered(0.08f))
		{
			cerberlisk.PlaySound(AudioName.CERBERLISK_BIG_FIREBALL);
		}
		if (enemy.IsAnimationTriggered(CerberliskParameter.CYCLONE_FIRE_TIME))
		{
			cerberlisk.mSkill.PreFireVfx.SetActive(false);
			cerberlisk.StopSound(AudioName.CERBERLISK_PRE_FIRE);
			Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
			vector += vector.normalized * CerberliskParameter.CYCLONE_OFFSET;
			Vector3 forward = enemy.GetPosition() + vector - data.FireTransform.position;
			GameObject go = Pool.sAllocate("SW2_Effect/B4_Cyclone_fire_ball", data.FireTransform.position, forward);
			CerberliskCycloneFireBallScript component = GameWorld.GetComponent<CerberliskCycloneFireBallScript>(go);
			component.Speed = forward.normalized * CerberliskParameter.CYCLONE_FIRE_BALL_SPEED;
			component.SetDamageInfo(CerberliskParameter.GetInstance(), 5);
			component.ExplodeRadius = CerberliskParameter.BIG_FIRE_BALL_EXPLODE_RADIUS;
			component.Vfx = "SW2_Effect/B4_Cyclone_show";
			component.VfxSound = AudioName.CERBERLISK_BIG_FIREBALL_HIT;
			component.Target = enemy.GetCurrentTarget().GetObject();
			component.Attacker = enemy;
			component.Reset();
		}
		if (enemy.IsAnimationTriggered(CerberliskParameter.CYCLONE_START_BREATHE_OUT_TIME))
		{
			cerberlisk.mSkill.PreWindVfx.SetActive(false);
			cerberlisk.mSkill.BreatheOutVfx.SetActive(true);
			cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PUSH);
		}
	}

	public override void End(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.mSkill.PreFireVfx.SetActive(false);
		cerberlisk.mSkill.PreWindVfx.SetActive(false);
		cerberlisk.StopSound(AudioName.CERBERLISK_PRE_FIRE);
		cerberlisk.StopSound(AudioName.CERBERLISK_PUSH);
	}
}

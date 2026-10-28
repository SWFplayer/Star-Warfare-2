using UnityEngine;

public class Cerberlisk3FireBallState : EnemyState
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
		enemy.PlayAnimation("3fireball", WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		cerberlisk.mSkill.PreFireVfx.SetActive(true);
		cerberlisk.PlaySound(AudioName.CERBERLISK_3FIREBALL);
		cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PRE_FIRE);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		MyData data = GetData<MyData>(enemy);
		if (enemy.CurrentAnimTime < CerberliskParameter.THREE_FIRE_BALL_TIME[0])
		{
			LookAtTargetPosition(enemy, CerberliskParameter.THREE_FIRE_BALL_ROTATE_SPEED);
		}
		if (data.InnerState < CerberliskParameter.THREE_FIRE_BALL_TIMES && enemy.IsAnimationTriggered(CerberliskParameter.THREE_FIRE_BALL_TIME[data.InnerState]))
		{
			if (cerberlisk.mSkill.PreFireVfx.activeSelf)
			{
				cerberlisk.mSkill.PreFireVfx.SetActive(false);
				cerberlisk.StopSound(AudioName.CERBERLISK_PRE_FIRE);
			}
			Vector3 forward = enemy.GetTransform().forward;
			int num;
			if (data.InnerState % 2 == 0)
			{
				num = CerberliskParameter.THREE_FIRE_BALL_NUM;
				forward = Quaternion.Euler(0f, (float)(-((CerberliskParameter.THREE_FIRE_BALL_NUM - 1) / 2)) * CerberliskParameter.THREE_FIRE_BALL_ANGLE, 0f) * forward;
			}
			else
			{
				num = CerberliskParameter.THREE_FIRE_BALL_NUM + 1;
				forward = Quaternion.Euler(0f, (0f - ((float)(num / 2) - 0.5f)) * CerberliskParameter.THREE_FIRE_BALL_ANGLE, 0f) * forward;
			}
			forward.y = 0f;
			Vector3[] array = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = (array[i] = Quaternion.Euler(0f, (float)i * CerberliskParameter.THREE_FIRE_BALL_ANGLE, 0f) * forward);
				GameObject go = Pool.sAllocate("SW2_Effect/B4_fireball", data.FireTransform.position, vector);
				ParaHitCheckScript component = GameWorld.GetComponent<ParaHitCheckScript>(go);
				component.Speed = CerberliskParameter.THREE_FIRE_BALL_SPEED * vector;
				component.SetDamageInfo(CerberliskParameter.GetInstance(), 4);
				component.Attacker = enemy;
				component.Reset();
			}
			data.InnerState++;
		}
	}

	public override void End(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.mSkill.PreFireVfx.SetActive(false);
		cerberlisk.StopSound(AudioName.CERBERLISK_PRE_FIRE);
	}
}

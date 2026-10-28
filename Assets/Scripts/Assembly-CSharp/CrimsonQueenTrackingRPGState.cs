using UnityEngine;

public class CrimsonQueenTrackingRPGState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public float StartTime;

		public bool HasSpawnedSecondWaveRPG;

		public float State2Duration;
	}

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		data.HasSpawnedSecondWaveRPG = false;
		enemy.PlayAnimation("bee01", WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		crimsonQueen.LastRPGTime = Time.time;
		if (crimsonQueen.GetMindStateInt() > 0)
		{
			data.State2Duration = CrimsonQueenParameter.TRACKING_RPG_STATE2_DURATION + CrimsonQueenParameter.TRACKING_RPG_SECOND_WAVE_TIME;
		}
		else
		{
			data.State2Duration = CrimsonQueenParameter.TRACKING_RPG_STATE2_DURATION;
		}
		crimsonQueen.ClearTrackingRPGList();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData data = GetData<MyData>(enemy);
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		LookAtTargetPosition(enemy, CrimsonQueenParameter.TRACKING_RPG_BOSS_ROTATE_SPEED);
		if (data.InnerState == 0)
		{
			if (enemy.IsAnimationTriggered(CrimsonQueenParameter.BEE_OPEN_SOUND_TIME))
			{
				enemy.PlaySound(AudioName.CRIMSONQUEEN_BEE_OPEN);
			}
			if (enemy.IsAnimationFinish())
			{
				enemy.PlayAnimation("bee02", WrapMode.Loop);
				data.StartTime = Time.time;
				data.InnerState++;
				if (enemy.IsMasterPlayer)
				{
					crimsonQueen.SpawnRPG(CrimsonQueenParameter.TRACKING_RPG_NUM[crimsonQueen.GetMindStateInt()]);
				}
			}
		}
		else if (data.InnerState == 1)
		{
			if (crimsonQueen.GetMindStateInt() > 0 && !data.HasSpawnedSecondWaveRPG && Time.time - data.StartTime > CrimsonQueenParameter.TRACKING_RPG_SECOND_WAVE_TIME)
			{
				data.HasSpawnedSecondWaveRPG = true;
				if (enemy.IsMasterPlayer)
				{
					crimsonQueen.SpawnRPG(CrimsonQueenParameter.TRACKING_RPG_NUM[crimsonQueen.GetMindStateInt()]);
				}
			}
			if (Time.time - data.StartTime > data.State2Duration)
			{
				enemy.PlayAnimation("bee03", WrapMode.ClampForever);
				enemy.PlaySound(AudioName.CRIMSONQUEEN_BEE_CLOSE);
				data.InnerState++;
			}
		}
		else if (data.InnerState != 2)
		{
		}
	}

	public override void End(Enemy enemy)
	{
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		return data.InnerState == 2 && gameUnit.IsAnimationFinish();
	}
}

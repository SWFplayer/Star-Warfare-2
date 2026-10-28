using UnityEngine;

public class PlayerCerberliskDashState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 DashTarget;

		public GameObject DashSmoke2;

		public float MoveDistance;
	}

	public const float Distance = 40f;

	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData data = GetData<MyData>(gameUnit);
		data.InnerState = 0;
		data.DashSmoke2 = null;
		data.MoveDistance = 0f;
		player.PlayAnimation(AnimationString.BOSS_DASH01, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_DASH01].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[4];
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData data = GetData<MyData>(gameUnit);
		sDashAttack dashAttackParameter = player.DashAttackParameter;
		RestrictRotationSpeed(player, CerberliskParameter.DASH_ROTATE_SPEED);
		if (data.InnerState == 0)
		{
			if (player.IsAnimationFinish())
			{
				data.InnerState = 1;
				player.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
				player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_DASH02].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[4];
				player.ResetAttack(player.CerberliskSkill.DashAttackSkill);
				player.PlaySoundLoop(AudioName.CERBERLISK_DASH02);
			}
		}
		else if (data.InnerState == 1)
		{
			player.CheckHit2(player.CerberliskSkill.DashAttackSkill);
			if (!player.IsLocal())
			{
				return;
			}
			player.SimpleMoveWithGravity(player.GetTransform().forward * dashAttackParameter.DASH_SPEED[0]);
			data.MoveDistance += dashAttackParameter.DASH_SPEED[0] * Time.deltaTime;
			if (data.MoveDistance >= 40f)
			{
				data.InnerState++;
				player.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
				player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_DASH03].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[4];
				player.PlaySound(AudioName.CERBERLISK_3HIT);
				if (player.IsLocal() && GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
				{
					PlayerBossStateInnerStateRequest request = new PlayerBossStateInnerStateRequest(this, data.InnerState);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
				player.StopSound(AudioName.CERBERLISK_DASH02);
			}
		}
		else if (data.InnerState == 2)
		{
			if (player.IsLocal() && player.CurrentAnimTime <= 0.6f)
			{
				player.SimpleMoveWithGravity(player.GetTransform().forward * dashAttackParameter.DASH_SPEED1[0]);
			}
			StartIdleWhenAnimFinish(player);
		}
	}

	public override void End(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		if (data.DashSmoke2 != null)
		{
			Pool.sGetInstance().Deallocate(data.DashSmoke2);
		}
		gameUnit.StopSound(AudioName.CERBERLISK_DASH02);
	}

	public override void ChangeInnerState(Player player, int innerState)
	{
		MyData data = GetData<MyData>(player);
		if (innerState == 2)
		{
			data.InnerState = 2;
			if (data.DashSmoke2 != null)
			{
				Pool.sGetInstance().Deallocate(data.DashSmoke2);
			}
			player.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
			player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_DASH03].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[4];
			player.StopSound(AudioName.CERBERLISK_DASH02);
			player.PlaySound(AudioName.CERBERLISK_3HIT);
		}
		else
		{
			Debug.LogError("ChangeInnerState " + innerState);
		}
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
	}
}

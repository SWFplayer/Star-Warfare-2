using UnityEngine;

public class PlayerDeadEyeRushAttackState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public GameObject Vfx;

		public float MoveDistance;
	}

	public override void Start(GameUnit gameUnit)
	{
		if (!gameUnit.StateData.ContainsKey(this))
		{
			gameUnit.StateData.Add(this, new MyData());
		}
		MyData myData = gameUnit.StateData[this] as MyData;
		myData.InnerState = 0;
		myData.MoveDistance = 0f;
		Player player = gameUnit as Player;
		player.PlayAnimation(AnimationString.BOSS_DASH01, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_DASH01].speed = DeadEyeParameter.DEAD_EYE_PLAYER_BOSS_ANIMATION_SPEED[4];
		Pool.sAllocate("SW2_Effect/B2_Dash01_smoke", player.GetPosition());
		player.SetTargetPosition(player.GetPosition() + player.GetTransform().forward * 20f);
		if (player.IsLocal() && GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			PlayerBossChangeTargetPositionRequest request = new PlayerBossChangeTargetPositionRequest(player.GetChannelID(), player.GetTargetPosition(), player.GetPosition());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public override void Update(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		Player player = gameUnit as Player;
		sDashAttack dashAttackParameter = player.DashAttackParameter;
		if (myData.InnerState == 0)
		{
			if (player.CurrentAnimTime >= 1f)
			{
				myData.InnerState++;
				player.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
				player.ResetAttack(player.DeadEyeSkill.DashAttackSkill);
				myData.Vfx = Pool.sGetInstance().Allocate("SW2_Effect/B2_Dash02_smoke");
				myData.Vfx.transform.parent = player.GetTransform();
				myData.Vfx.transform.localPosition = Vector3.zero;
			}
		}
		else if (myData.InnerState == 1)
		{
			player.CheckHit2(player.DeadEyeSkill.DashAttackSkill);
			if (player.IsLocal())
			{
				player.SimpleMoveWithGravity(player.GetTransform().forward * dashAttackParameter.DASH_SPEED[0]);
				myData.MoveDistance += dashAttackParameter.DASH_SPEED[0] * Time.deltaTime;
			}
			if (myData.MoveDistance >= 20f - dashAttackParameter.DASH_LOCK_DISTANCE)
			{
				myData.InnerState++;
			}
		}
		else if (myData.InnerState == 2)
		{
			player.CheckHit2(player.DeadEyeSkill.DashAttackSkill);
			if (!player.IsLocal())
			{
				return;
			}
			player.SimpleMoveWithGravity(player.GetTransform().forward * dashAttackParameter.DASH_SPEED1[0]);
			myData.MoveDistance += dashAttackParameter.DASH_SPEED[0] * Time.deltaTime;
			if (myData.MoveDistance >= 20f)
			{
				myData.InnerState++;
				Pool.sGetInstance().Deallocate(myData.Vfx);
				player.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
				if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
				{
					PlayerBossStateInnerStateRequest request = new PlayerBossStateInnerStateRequest(this, myData.InnerState);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		else if (myData.InnerState == 3)
		{
			player.CheckHit2(player.DeadEyeSkill.DashAttackSkill);
			if (player.IsLocal() && player.CurrentAnimTime <= 0.6f)
			{
				player.SimpleMoveWithGravity(player.GetTransform().forward * dashAttackParameter.DASH_SPEED1[0]);
			}
			StartIdleWhenAnimFinish(player);
		}
	}

	public override void End(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		Pool.sGetInstance().Deallocate(myData.Vfx);
	}

	public override void ChangeInnerState(Player player, int innerState)
	{
		if (player.StateData.ContainsKey(this))
		{
			MyData myData = player.StateData[this] as MyData;
			if (innerState == 3)
			{
				Pool.sGetInstance().Deallocate(myData.Vfx);
				player.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
				myData.InnerState = 3;
			}
		}
	}
}

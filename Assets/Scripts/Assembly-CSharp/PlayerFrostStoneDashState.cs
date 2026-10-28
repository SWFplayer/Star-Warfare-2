using UnityEngine;

public class PlayerFrostStoneDashState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 DashTarget;

		public GameObject DashSmoke2;

		public Vector3 LastPosition;

		public float MoveDistance;
	}

	public const float Distance = 40f;

	public override void Start(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		data.InnerState = 0;
		data.MoveDistance = 0f;
		Player player = gameUnit as Player;
		player.PlayAnimation(AnimationString.BOSS_DASH01, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_DASH01].speed = FrostStoneParameter.FROST_STONE_PLAYER_BOSS_ANIMATION_SPEED[5];
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
				player.ResetAttack(player.FrostStoneSkill.DashAttackSkill);
				Pool.sAllocate("SW2_Effect/B3_Dash01_smoke", player.GetPosition(), 4f);
				myData.DashSmoke2 = Pool.sGetInstance().Allocate("SW2_Effect/B3_Dash02_smoke");
				myData.DashSmoke2.transform.parent = player.GetTransform();
				myData.DashSmoke2.transform.localPosition = Vector3.zero;
				player.PlaySoundLoop(AudioName.FROSTSTONE_DASH);
			}
		}
		else if (myData.InnerState == 1)
		{
			player.CheckHit2(player.FrostStoneSkill.DashAttackSkill);
			if (!player.IsLocal())
			{
				return;
			}
			player.SimpleMoveWithGravity(player.GetTransform().forward * dashAttackParameter.DASH_SPEED[0]);
			myData.MoveDistance += dashAttackParameter.DASH_SPEED[0] * Time.deltaTime;
			if (myData.MoveDistance >= 40f)
			{
				myData.InnerState++;
				Pool.sGetInstance().Deallocate(myData.DashSmoke2);
				player.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
				if (player.IsLocal() && GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
				{
					PlayerBossStateInnerStateRequest request = new PlayerBossStateInnerStateRequest(this, myData.InnerState);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		else if (myData.InnerState == 2)
		{
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
		gameUnit.StopSound(AudioName.FROSTSTONE_DASH);
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

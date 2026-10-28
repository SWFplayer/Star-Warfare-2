using UnityEngine;

public class PlayerSlasherWeaponDashState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

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
		player.SlasherSkill.PlayEyeEffect();
		player.PlayAnimation(AnimationString.SLASHER_WEAPON_DASH01, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.SLASHER_WEAPON_DASH01].speed = SlasherParameter.SLASHER_PLAYER_BOSS_ANIMATION_SPEED[3];
		player.ResetAttack(player.SlasherSkill.WeaponDashSkill);
	}

	public override void Update(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		Player player = gameUnit as Player;
		if (myData.InnerState == 0)
		{
			if (player.CurrentAnimTime >= 0.65f)
			{
				player.SimpleMoveWithGravity(player.GetTransform().forward * SlasherParameter.DASH_SPEED[0]);
				myData.MoveDistance += SlasherParameter.DASH_SPEED[0] * Time.deltaTime;
			}
			if (player.CurrentAnimTime >= 1f)
			{
				myData.InnerState++;
				player.PlayAnimation(AnimationString.SLASHER_WEAPON_DASH02, WrapMode.Loop);
				player.PlaySoundLoop(AudioName.SLASHER_WEAPON_DASH02);
			}
		}
		else if (myData.InnerState == 1)
		{
			player.CheckHit2(player.SlasherSkill.WeaponDashSkill);
			if (player.IsLocal())
			{
				player.SimpleMoveWithGravity(player.GetTransform().forward * SlasherParameter.DASH_SPEED[0]);
				myData.MoveDistance += SlasherParameter.DASH_SPEED[0] * Time.deltaTime;
			}
			if (myData.MoveDistance >= 20f - SlasherParameter.DASH_LOCK_DISTANCE)
			{
				myData.InnerState++;
			}
		}
		else if (myData.InnerState == 2)
		{
			player.CheckHit2(player.SlasherSkill.WeaponDashSkill);
			if (!player.IsLocal())
			{
				return;
			}
			player.SimpleMoveWithGravity(player.GetTransform().forward * SlasherParameter.DASH_SPEED[0]);
			myData.MoveDistance += SlasherParameter.DASH_SPEED[0] * Time.deltaTime;
			if (myData.MoveDistance >= 20f)
			{
				myData.InnerState++;
				player.StopSound(AudioName.SLASHER_WEAPON_DASH02);
				player.PlayAnimation(AnimationString.SLASHER_WEAPON_DASH03, WrapMode.ClampForever);
				if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
				{
					PlayerBossStateInnerStateRequest request = new PlayerBossStateInnerStateRequest(this, myData.InnerState);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		else if (myData.InnerState == 3)
		{
			player.CheckHit2(player.SlasherSkill.WeaponDashSkill);
			if (player.IsLocal() && player.CurrentAnimTime <= 0.6f)
			{
				player.SimpleMoveWithGravity(player.GetTransform().forward * SlasherParameter.DASH_SPEED[0]);
			}
			StartIdleWhenAnimFinish(player);
		}
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.StopSound(AudioName.SLASHER_WEAPON_DASH02);
	}

	public override void ChangeInnerState(Player player, int innerState)
	{
		if (player.StateData.ContainsKey(this))
		{
			MyData myData = player.StateData[this] as MyData;
			if (innerState == 3)
			{
				player.StopSound(AudioName.SLASHER_WEAPON_DASH02);
				player.PlayAnimation(AnimationString.SLASHER_WEAPON_DASH03, WrapMode.ClampForever);
				myData.InnerState = 3;
			}
		}
	}
}

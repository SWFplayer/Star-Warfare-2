using UnityEngine;

public class PlayerFrostStoneJumpAttack : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 Speed;

		public Vector3 Position;

		public float Gravity;

		public float startJumpingTime;
	}

	public const float Distance = 40f;

	public const float JumpingLastTime = 3f;

	private GameUnitStateComponent component = new GameUnitStateComponent();

	public override void Start(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		data.InnerState = 0;
		Player player = gameUnit as Player;
		player.PlayAnimation(AnimationString.ENEMY_JUMPSTART, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.ENEMY_JUMPSTART].speed = FrostStoneParameter.FROST_STONE_PLAYER_BOSS_ANIMATION_SPEED[4];
		player.PlaySound(AudioName.FROSTSTONE_JUMP01);
		component.UpdateTargetPositionOnGround(player, 40f, true);
	}

	public override void Update(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		Player player = gameUnit as Player;
		if (data.InnerState == 0)
		{
			RestrictRotationSpeed(player, FrostStoneParameter.JUMP_ATTACK_ROTATE_SPEED);
			component.UpdateTargetPositionOnGround(player, 40f, false);
			if (player.IsAnimationFinish())
			{
				data.InnerState = 1;
				data.startJumpingTime = Time.time;
				player.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
				Vector3 vector = player.GetTargetPosition() - player.GetPosition();
				vector.y = 0f;
				data.Speed = vector / FrostStoneParameter.JUMP_ATTACK_TIME;
				data.Speed.y = FrostStoneParameter.JUMP_ATTACK_HEIGHT / FrostStoneParameter.JUMP_ATTACK_TIME;
				data.Gravity = (0f - data.Speed.y) * 2f / FrostStoneParameter.JUMP_ATTACK_TIME;
				data.Position = player.GetTargetPosition();
				player.PlaySoundLoop(AudioName.FROSTSTONE_JUMP_ATTACK);
				player.ResetAttack(player.FrostStoneSkill.JumpAttackSkill);
			}
		}
		else if (data.InnerState == 1)
		{
			Vector3 speed = data.Speed;
			data.Speed += new Vector3(0f, Time.deltaTime * data.Gravity, 0f);
			Vector3 vector2 = (speed + data.Speed) / 2f;
			player.MoveDelta(vector2 * Time.deltaTime);
			if ((player.GetPosition().y < data.Position.y + 0.5f && player.GetCharacterController().isGrounded) || Time.time - data.startJumpingTime > 3f)
			{
				data.InnerState = 2;
				player.CheckHit2(player.FrostStoneSkill.JumpAttackSkill);
				player.StopSound(AudioName.FROSTSTONE_JUMP_ATTACK);
				player.PlayAnimation(AnimationString.ENEMY_JUMPING, WrapMode.ClampForever);
				GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Jump02_smoke", player.GetPosition(), 2f);
				player.PlaySound(AudioName.FROSTSTONE_JUMP02);
				if (player.IsLocal() && GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
				{
					PlayerBossStateInnerStateRequest request = new PlayerBossStateInnerStateRequest(this, data.InnerState);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		else if (data.InnerState == 2)
		{
			player.MoveDelta(Vector3.down * Time.deltaTime);
			StartIdleWhenAnimFinish(player);
		}
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.StopSound(AudioName.FROSTSTONE_JUMP_ATTACK);
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
	}

	public override void ChangeInnerState(Player player, int innerState)
	{
		if (innerState == 2)
		{
			MyData data = GetData<MyData>(player);
			data.InnerState = 2;
			player.PlayAnimation(AnimationString.ENEMY_JUMPING, WrapMode.ClampForever);
			GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Jump02_smoke", player.GetPosition(), 2f);
			player.PlaySound(AudioName.FROSTSTONE_JUMP02);
		}
		else
		{
			Debug.LogError("innerState 不对");
		}
	}
}

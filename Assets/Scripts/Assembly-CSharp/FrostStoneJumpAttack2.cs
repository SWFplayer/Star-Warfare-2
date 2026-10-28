using UnityEngine;

public class FrostStoneJumpAttack2 : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 Speed;

		public Vector3 Position;

		public float Gravity;

		public Timer CheckTimer = new Timer();
	}

	public override void Start(Enemy enemy)
	{
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		myData.CheckTimer.SetTimer(0.1f, false);
		FrostStone frostStone = enemy as FrostStone;
		frostStone.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
		vector.y = 0f;
		myData.Speed = vector / FrostStoneParameter.JUMP_ATTACK_TIME;
		myData.Speed.y = FrostStoneParameter.JUMP_ATTACK_HEIGHT / FrostStoneParameter.JUMP_ATTACK_TIME;
		myData.Gravity = (0f - myData.Speed.y) * 2f / FrostStoneParameter.JUMP_ATTACK_TIME;
		myData.Position = enemy.GetTargetPosition();
		enemy.SetCanTurn(false);
		frostStone.SetShadow(false);
		frostStone.PlaySoundLoop(AudioName.FROSTSTONE_JUMP_ATTACK);
		frostStone.ResetAttack(frostStone.mSkill.JumpAttackSkill);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		FrostStone frostStone = enemy as FrostStone;
		if (myData.InnerState != 0)
		{
			return;
		}
		Vector3 speed = myData.Speed;
		myData.Speed += new Vector3(0f, Time.deltaTime * myData.Gravity, 0f);
		Vector3 vector = (speed + myData.Speed) / 2f;
		enemy.MoveDelta(vector * Time.deltaTime);
		bool flag = false;
		if (myData.CheckTimer.Ready2())
		{
			Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), Vector3.down);
			int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
			if (Physics.Raycast(ray, 0.7f, layerMask))
			{
				flag = true;
			}
		}
		if ((enemy.GetPosition().y < myData.Position.y + 0.5f && enemy.GetCharacterController().isGrounded) || flag)
		{
			myData.InnerState = 1;
			enemy.CheckHit2(((FrostStone)enemy).mSkill.JumpAttackSkill);
			if (frostStone.HasIceArmor)
			{
				CreateBall(enemy);
			}
		}
	}

	public override void End(Enemy enemy)
	{
		FrostStone frostStone = enemy as FrostStone;
		frostStone.SetShadow(true);
		frostStone.StopSound(AudioName.FROSTSTONE_JUMP_ATTACK);
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return myData.InnerState == 1;
	}

	private void CreateBall(GameUnit gameUnit)
	{
		Vector3 normalized = new Vector3(Random.value, 0f, Random.value).normalized;
		for (int i = 0; i < FrostStoneParameter.JUMP_ATTACK_BALL_NUM; i++)
		{
			Vector3 position = gameUnit.GetPosition();
			Vector3 vector = Quaternion.Euler(0f, (float)i * 360f / (float)FrostStoneParameter.JUMP_ATTACK_BALL_NUM, 0f) * normalized * Random.Range(FrostStoneParameter.JUMP_ATTACK_BALL_RADIUS.x, FrostStoneParameter.JUMP_ATTACK_BALL_RADIUS.y);
			vector.y = 0f;
			float num = vector.magnitude / FrostStoneParameter.SMALL_SNOW_BALL_HORIZONTAL_SPEED;
			Vector3 speed = vector.normalized * FrostStoneParameter.SMALL_SNOW_BALL_HORIZONTAL_SPEED;
			speed.y += FrostStoneParameter.SMALL_SNOW_BALL_HEIGHT / num;
			float num2 = 2f * speed.y / num;
			GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Snowball01_ball");
			gameObject.transform.position = position;
			ParaHitCheckScript paraHitCheckScript = gameObject.GetComponent<ParaHitCheckScript>();
			if (paraHitCheckScript == null)
			{
				paraHitCheckScript = gameObject.AddComponent<ParaHitCheckScript>();
			}
			paraHitCheckScript.Speed = speed;
			paraHitCheckScript.Gravity = 0f - num2;
			paraHitCheckScript.SetDamageInfo(FrostStoneParameter.GetInstance(), 3);
			paraHitCheckScript.Vfx = "SW2_Effect/B3_Snowball01_hit";
			paraHitCheckScript.VfxDuration = 2f;
			paraHitCheckScript.Attacker = gameUnit;
			paraHitCheckScript.Reset();
		}
	}
}

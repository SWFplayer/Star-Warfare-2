using UnityEngine;

public class PlayerSlasherWeaponRangeState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public Timer mRainTimer = new Timer(SlasherParameter.RAIN_INTERVAL);

		public Transform mMouthTransform;
	}

	public GameUnitStateComponent component = new GameUnitStateComponent();

	public override void Start(GameUnit gameUnit)
	{
		if (!gameUnit.StateData.ContainsKey(this))
		{
			gameUnit.StateData.Add(this, new MyData());
		}
		MyData myData = gameUnit.StateData[this] as MyData;
		if (myData.mMouthTransform == null)
		{
			myData.mMouthTransform = gameUnit.GetTransform().Find(BoneName.SLASHER_MOUTH);
		}
		myData.mRainTimer.SetTimer(SlasherParameter.RAIN_INTERVAL / SlasherParameter.SLASHER_PLAYER_BOSS_ANIMATION_SPEED[2], true);
		Player player = gameUnit as Player;
		player.PlaySound(AudioName.SLASHER_RANGE01);
		player.PlayAnimation(AnimationString.SLASHER_WEAPON_RANGE01, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.SLASHER_WEAPON_RANGE01].speed = SlasherParameter.SLASHER_PLAYER_BOSS_ANIMATION_SPEED[2];
		component.UpdateTargetPositionOnGround(player, 20f, true);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData myData = gameUnit.StateData[this] as MyData;
		component.UpdateTargetPositionOnGround(player, 20f, false);
		if (myData.mRainTimer.Ready() && player.CurrentAnimTime > 0.45f && player.CurrentAnimTime < 0.66f)
		{
			myData.mRainTimer.Do();
			float angleH = (float)Random.Range(-100, 100) * SlasherParameter.RAIN_HORIZONTAL_RANGE / 100f;
			float angleV = (float)Random.Range(-100, 100) * SlasherParameter.RAIN_VERTICAL_UP_RANGE / 100f;
			ShotThornRain(player, myData.mMouthTransform, angleH, angleV);
		}
		StartIdleWhenAnimFinish(player);
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.StopSound(AudioName.SLASHER_RANGE01);
	}

	public void ShotThornRain(Player player, Transform mMouthTransform, float angleH, float angleV)
	{
		GameObject original = Resources.Load(EffectName.SLASHER_ARROW) as GameObject;
		GameObject gameObject = Object.Instantiate(original, mMouthTransform.position, Quaternion.identity) as GameObject;
		Vector3 vector = player.GetTargetPosition() - mMouthTransform.position;
		vector.y = 0f;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = magnitude / SlasherParameter.RAIN_SPEED;
		float num2 = mMouthTransform.position.y - player.GetPosition().y;
		float num3 = num2 / num + 0.5f * Physics.gravity.y * num;
		Vector3 vector2 = num3 * Vector3.down;
		Vector3 vector3 = vector2 + SlasherParameter.RAIN_SPEED * normalized;
		gameObject.transform.LookAt(mMouthTransform.position + vector3 * 10f);
		gameObject.transform.RotateAround(mMouthTransform.position, gameObject.transform.up, angleH);
		gameObject.transform.RotateAround(mMouthTransform.position, gameObject.transform.right, 0f - angleV);
		SlasherThornScript slasherThornScript = gameObject.GetComponent<SlasherThornScript>();
		slasherThornScript.Speed = gameObject.transform.forward * vector3.magnitude;
		slasherThornScript.SetDamageInfo(SlasherParameter.GetInstance(), 7);
		slasherThornScript.ExplodeRadius = SlasherParameter.RAIN_EXPLODE_RADIUS;
		slasherThornScript.ExplosionTime = SlasherParameter.RAIN_EXPLODE_TIME;
		slasherThornScript.SetSlasher(player);
	}
}

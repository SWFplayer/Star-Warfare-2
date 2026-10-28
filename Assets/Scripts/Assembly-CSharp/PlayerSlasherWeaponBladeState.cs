using UnityEngine;

public class PlayerSlasherWeaponBladeState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public int BladeWindNumber;
	}

	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.PlayAnimation(AnimationString.SLASHER_WEAPON_ATTACK02, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.SLASHER_WEAPON_ATTACK02].speed = SlasherParameter.SLASHER_PLAYER_BOSS_ANIMATION_SPEED[5];
		if (!gameUnit.StateData.ContainsKey(this))
		{
			gameUnit.StateData.Add(this, new MyData());
		}
		MyData myData = gameUnit.StateData[this] as MyData;
		myData.InnerState = 0;
		if (player.IsLocal())
		{
			myData.BladeWindNumber = ((Random.Range(0, 100) >= 50) ? SlasherParameter.WEAPON_BLADE_WIND_NUMBER2 : SlasherParameter.WEAPON_BLADE_WIND_NUMBER1);
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				SlasherBladeWindRequest request = new SlasherBladeWindRequest(player.GetChannelID(), (byte)myData.BladeWindNumber);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public override void Update(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		Player player = gameUnit as Player;
		if (myData.InnerState == 0 && player.CurrentAnimTime >= 0.5f)
		{
			myData.InnerState++;
			GameObject original = Resources.Load(EffectName.SLASHER_SWORD_WIND) as GameObject;
			GameObject[] array = new GameObject[myData.BladeWindNumber];
			for (int i = 0; i < myData.BladeWindNumber; i++)
			{
				array[i] = Object.Instantiate(original, player.GetPosition() + player.GetTransform().forward * 3f + Vector3.up * 0.5f, Quaternion.LookRotation(player.GetTransform().forward)) as GameObject;
				array[i].transform.RotateAround(array[i].transform.position, Vector3.up, ((float)myData.BladeWindNumber / 2f - 0.5f - (float)i) * SlasherParameter.WEAPON_BLADE_ANGLE);
				SlasherBladeScript component = array[i].GetComponent<SlasherBladeScript>();
				component.SpeedValue = SlasherParameter.WEAPON_BLADE_SPEED;
				component.Speed = array[i].transform.forward * SlasherParameter.WEAPON_BLADE_SPEED;
				component.SetDamageInfo(SlasherParameter.GetInstance(), 12);
				component.Attacker = gameUnit;
			}
		}
		StartIdleWhenAnimFinish(player);
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.StopSound(AudioName.SLASHER_WEAPON_ATTACK02);
	}
}

using UnityEngine;

public class PlayerSlasherWeaponWaveState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		if (!gameUnit.StateData.ContainsKey(this))
		{
			gameUnit.StateData.Add(this, new MyData());
		}
		MyData myData = gameUnit.StateData[this] as MyData;
		myData.InnerState = 0;
		player.SlasherSkill.PlayEyeEffect();
		player.PlayAnimation(AnimationString.SLASHER_WEAPON_ATTACK01, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.SLASHER_WEAPON_ATTACK01].speed = SlasherParameter.SLASHER_PLAYER_BOSS_ANIMATION_SPEED[4];
		player.PlaySound(AudioName.SLASHER_ATTACK03);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData myData = gameUnit.StateData[this] as MyData;
		if (myData.InnerState == 0 && player.CurrentAnimTime >= 0.42f)
		{
			myData.InnerState++;
			GameObject original = Resources.Load(EffectName.SLASHER_WAVE_DIRECT) as GameObject;
			GameObject gameObject = Object.Instantiate(original, player.GetPosition() + player.GetTransform().forward * SlasherParameter.WAVE_OFFSET, Quaternion.LookRotation(player.GetTransform().forward)) as GameObject;
			SlasherWaveRootScript component = gameObject.GetComponent<SlasherWaveRootScript>();
			if (null != component)
			{
				component.SetDamageInfo(SlasherParameter.GetInstance(), 10);
				component.KnockSpeed = SlasherParameter.WAVE_LIGHT_KNOCK_SPEED;
				component.Attacker = gameUnit;
			}
		}
		StartIdleWhenAnimFinish(player);
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.StopSound(AudioName.SLASHER_ATTACK03);
	}
}

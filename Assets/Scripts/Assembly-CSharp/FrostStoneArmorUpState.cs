using UnityEngine;

public class FrostStoneArmorUpState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		FrostStone frostStone = enemy as FrostStone;
		frostStone.PlayAnimation("armorup", WrapMode.ClampForever);
		GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_show", enemy.GetPosition(), 5f);
		frostStone.PlaySound(AudioName.FROSTSTONE_ARMOR_UP);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (enemy.IsAnimationTriggered(FrostStoneParameter.ICE_ARMOR_SHOW_TIME))
		{
			FrostStone frostStone = enemy as FrostStone;
			frostStone.SetArmor(true);
			frostStone.HasIceArmor = true;
			frostStone.StartArmorHp = frostStone.Hp;
		}
	}

	public override void End(Enemy enemy)
	{
	}
}

using System.Collections.Generic;
using UnityEngine;

public class Baneling : Enemy
{
	private float mBombRangeSqr;

	public Baneling()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.BANELING;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		SetShoutTimer("_Old/_Audio/enemy/zibaochong", 3f);
		attackRange = 2.5f;
		mBombRangeSqr = 100f;
		bloodColor = BloodColor.Green;
		ConditionStateManager.GetInstance().InitConditionState(this);
	}

	public void Explode()
	{
		List<GameUnit> targetList = GetTargetList();
		foreach (GameUnit item in targetList)
		{
			if ((item.GetTransform().position - entityTransform.position).sqrMagnitude < mBombRangeSqr)
			{
				item.OnHit(CreateDamageProperty(attackDamage));
			}
		}
	}

	public override void StartEnemyAttack()
	{
		base.StartEnemyAttack();
		PlayAnimation(AnimationString.ENEMY_ATTACK, WrapMode.ClampForever);
		if (base.IsMasterPlayer)
		{
			Object original = Resources.Load(EffectName.E8_Boom);
			GameObject gameObject = Object.Instantiate(original, entityTransform.position, entityTransform.rotation) as GameObject;
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				EnemyOnHitRequest request = new EnemyOnHitRequest(base.EnemyID, Hp, 0, true, 0);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				DamageProperty damageProperty = CreateDamageProperty(Hp);
				damageProperty.damage = Hp;
				damageProperty.isLocal = true;
				damageProperty.criticalAttack = true;
				damageProperty.wType = WeaponType.NoGun;
				OnHit(damageProperty);
			}
		}
		Explode();
	}

	public override void CheckHit()
	{
	}
}

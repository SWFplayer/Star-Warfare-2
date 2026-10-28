using UnityEngine;

public class MachineGun : AssaultRifle
{
	private int stackNum;

	private GameUnit lastHitTarget;

	protected int mCurrentGunPoint;

	protected Transform gunfire2;

	protected Transform gunfire1;

	public override void Init(Player player)
	{
		base.Init(player);
		gunfire1 = gunfire;
		gunfire2 = gun.transform.Find("Point02");
		mCurrentGunPoint = 0;
	}

	public override void GunOn()
	{
		base.GunOn();
		stackNum = 0;
		lastHitTarget = null;
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		base.UIInit(weaponObj, weaponObjL);
		gunfire1 = gunfire;
		gunfire2 = weaponObj.transform.Find("Point02");
		mCurrentGunPoint = 0;
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(AudioName.MACHINE_GUN, avatar, avatar.transform.position);
		gunfire = ((mCurrentGunPoint % 2 != 1) ? gunfire2 : gunfire1);
		gunfireObj.transform.parent = gunfire;
		gunfireObj.transform.localPosition = Vector3.zero;
		mCurrentGunPoint++;
		Vector3 normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(GetBulletEffectPoolType()).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
		if (gameObject != null)
		{
			FireLineScript component = gameObject.GetComponent<FireLineScript>();
			component.beginPos = gunfire.position;
			component.endPos = gunfire.position + normalized * 100f;
			component.speed = 75f;
		}
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.MachineGun;
	}

	public override string GetAnimationSuffix()
	{
		return "jiatelin";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "jiatelin";
	}

	public override void UpdateAttackState(float deltaTime)
	{
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		InputController inputController = player.inputController;
		WrapMode wrapMode = WrapMode.Loop;
		gun.GetComponent<Animation>()["attack"].wrapMode = wrapMode;
		gun.GetComponent<Animation>().CrossFade("attack", 0.1f);
		if (CoolDown())
		{
			gunfire = ((mCurrentGunPoint % 2 != 1) ? gunfire2 : gunfire1);
			gunfireObj.transform.parent = gunfire;
			gunfireObj.transform.localPosition = Vector3.zero;
			mCurrentGunPoint++;
			if (player.IsLocal())
			{
				player.Attack();
			}
			else
			{
				CreateTrajectory();
			}
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, wrapMode);
				}
				else
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackDown, WrapMode.Loop);
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackUp, wrapMode);
				}
			}
			else if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, wrapMode);
			}
			else
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, wrapMode);
			}
		}
		if (!inputController.inputInfo.fire && (!GetWaitForAttackAnimationStop() || (!player.IsPlayingAnimationNotFinished(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && !player.IsPlayingAnimationNotFinished(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackUp) && !player.IsPlayingAnimationNotFinished(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack) && !player.IsPlayingAnimationNotFinished(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack))))
		{
			player.ChangeToIdleState();
			gun.GetComponent<Animation>().Stop("attack");
			if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
			else
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
		}
	}

	public void SetHitTarget(GameUnit target)
	{
		if (target != lastHitTarget)
		{
			stackNum /= 2;
		}
		if (stackNum < Global.MACHINEGUN_MAX_STACK_NUM)
		{
			stackNum++;
		}
		lastHitTarget = target;
	}

	public int GetStackNum()
	{
		return stackNum;
	}

	public override DamageProperty CreateDamageProperty()
	{
		DamageProperty damageProperty = new DamageProperty(player, (int)damage);
		damageProperty.criticalAttack = false;
		if (player.IsLocal())
		{
			damageProperty.isLocal = true;
		}
		else
		{
			damageProperty.isLocal = false;
		}
		damageProperty.wType = GetWeaponType();
		damageProperty.damageType = GetDamageType();
		damageProperty.buff += player.GetSkills().GetSkill(SkillsType.ALL_DAMAGE);
		if (damageType != PlayerDamageType.NORMAL)
		{
			SkillsType sType = (SkillsType)(damageType - 1 + 53);
			damageProperty.buff += player.GetSkills().GetSkill(sType);
		}
		if (BoothType != SkillsType.SKILL_NONE)
		{
			damageProperty.buff += player.GetSkills().GetSkill(BoothType);
		}
		damageProperty.damage += (int)((float)damageProperty.damage * ((float)stackNum * 0.01f));
		damageProperty.damageFactor = (byte)stackNum;
		damageProperty.weaponid = gunID;
		return damageProperty;
	}

	protected override EffectPoolType GetBulletEffectPoolType()
	{
		return EffectPoolType.W30_TRAJECTORY;
	}

	public override void PlaySound()
	{
		AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.MACHINE_GUN, player.GetObject(), player.GetTransform());
	}

	public override void StopSound()
	{
		AudioManager.GetInstance().StopSound(AudioName.MACHINE_GUN, player.GetObject());
	}

	public override void StopSoundForUI(GameObject avatar)
	{
		AudioManager.GetInstance().StopSound(AudioName.MACHINE_GUN, avatar);
	}
}

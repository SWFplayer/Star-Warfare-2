using System;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Weapon
{
	protected float startAimTime;

	private bool SniperAttack;

	public override WeaponType GetWeaponType()
	{
		return WeaponType.Sniper;
	}

	public override string GetAnimationSuffix()
	{
		return "Sniper";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "Sniper";
	}

	public override bool IsTypeOfLoopShootingWeapon()
	{
		return false;
	}

	public override EAimType GetIdleAimType()
	{
		return EAimType.ANGLE;
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.ANGLE;
	}

	public void Aim(float deltaTime)
	{
		if (startAimTime == 0f)
		{
			startAimTime = Time.time;
		}
		if (Time.time - startAimTime > 0.1f && startAimTime != 0f)
		{
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ZoomToSniper(deltaTime);
		}
	}

	public bool isAiming()
	{
		return startAimTime != 0f;
	}

	public bool AttackDoneReadyZoomOut()
	{
		return Time.time - lastAttackTime > 0.5f;
	}

	public override void Init(Player player)
	{
		base.Init(player);
		playSoundTimer.SetTimer(attackFrenquency / 2f, true);
		StopFire();
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		gunfire = GetGunFire(weaponObj);
		playSoundTimer.SetTimer(attackFrenquency / 2f, true);
		StopFire();
	}

	public override void IdleZoom(float deltaTime)
	{
		if (AttackDoneReadyZoomOut())
		{
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ZoomOut(deltaTime);
		}
	}

	public override void AttackZoom(float deltaTime)
	{
		InputController inputController = player.inputController;
		if (!inputController.inputInfo.fire && AttackDoneReadyZoomOut())
		{
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ZoomOut(deltaTime);
		}
	}

	public override void Loop(float deltaTime)
	{
	}

	public override void AutoDestructEffect()
	{
	}

	public override void GunOn()
	{
		base.GunOn();
		if (gunfireObj != null)
		{
			gunfireObj.GetComponent<Renderer>().enabled = true;
		}
		PlayIdleAnimGunOn();
	}

	public override void GunOff()
	{
		base.GunOff();
		if (gunfireObj != null)
		{
			gunfireObj.GetComponent<Renderer>().enabled = false;
		}
	}

	public override void StopFire()
	{
	}

	public override void PlaySound()
	{
		if (playSoundTimer.Ready())
		{
			playSoundTimer.Do();
		}
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		base.CreateTrajectoryUI(avatar, gunObj, gunObjL);
		Vector3 normalized = avatar.transform.forward.normalized;
		AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/sniper/r100_railgun", avatar.transform.position);
		gunfire.transform.rotation = avatar.transform.rotation;
		normalized = gunfire.transform.forward.normalized;
		if (GetSpark() != EffectPoolType.NONE)
		{
			GameObject gameObject = EffectPool.GetInstance().GetEffectPool(GetSpark()).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
		}
	}

	public override void CreateTrajectory()
	{
		base.CreateTrajectory();
		Vector3 normalized = player.GetTransform().forward.normalized;
		AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/sniper/r100_railgun", player.GetTransform().position);
		gunfire.transform.rotation = player.GetTransform().rotation;
		gunfire.transform.rotation = Quaternion.AngleAxis(player.AngleV, -gunfire.transform.right) * gunfire.transform.rotation;
		normalized = gunfire.transform.forward.normalized;
		Vector3 endPos = gunfire.position + normalized * 80f;
		gameWorld.GetEffectPool(GetFireLine()).SetLife(GetFireLineLife());
		GameObject gameObject = gameWorld.GetEffectPool(GetFireLine()).CreateObject(gunfire.transform.position - normalized * 1f, normalized, Quaternion.identity);
		SniperFirelineScript component = gameObject.GetComponent<SniperFirelineScript>();
		component.beginPos = gunfire.position - normalized * 1f;
		component.endPos = endPos;
		component.gun = this;
		component.damageInterval = GetDamageInterval();
		component.Init();
		if (GetSpark() != EffectPoolType.NONE)
		{
			GameObject gameObject2 = gameWorld.GetEffectPool(GetSpark()).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
		}
	}

	public override void Attack(float deltaTime)
	{
		HashSet<Enemy> hashSet = new HashSet<Enemy>();
		startAimTime = 0f;
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/sniper/r100_railgun", player.GetTransform().position);
		Camera main = Camera.main;
		Transform transform = main.transform;
		ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(gunfire, out direction);
		if (GetSpark() != EffectPoolType.NONE)
		{
			GameObject gameObject = gameWorld.GetEffectPool(GetSpark()).CreateObject(gunfire.transform.position, direction, Quaternion.identity);
		}
		List<Ray> list = new List<Ray>();
		list.Add(weaponRay);
		BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, player) as BulletRay;
		if (bulletRay != null)
		{
			DamageProperty damageProperty = CreateDamageProperty();
			damageProperty.hitForce = weaponRay.direction * 2f;
			damageProperty.hitpoint = aimTarget;
			bulletRay.ShootDetect(damageProperty, list, 40, GetShootMark(), range, 0f, 1f, true);
		}
		gameWorld.GetEffectPool(GetFireLine()).SetLife(GetFireLineLife());
		GameObject gameObject2 = gameWorld.GetEffectPool(GetFireLine()).CreateObject(gunfire.transform.position - direction * 1f, direction, Quaternion.identity);
		SniperFirelineScript component2 = gameObject2.GetComponent<SniperFirelineScript>();
		component2.beginPos = gunfire.position - direction * 1f;
		component2.endPos = aimTarget;
		component2.gun = this;
		component2.damageInterval = GetDamageInterval();
		component2.Init();
		attacked = false;
		lastAttackTime = Time.time;
	}

	public virtual void ShootDetect(Vector3 beginPos, Vector3 EndPos)
	{
		Ray item = new Ray(beginPos, EndPos - beginPos);
		List<Ray> list = new List<Ray>();
		list.Add(item);
		BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, player) as BulletRay;
		if (bulletRay != null)
		{
			DamageProperty damageProperty = CreateDamageProperty();
			damageProperty.hitForce = item.direction * 2f;
			damageProperty.hitpoint = aimTarget;
			bulletRay.ShootDetect(damageProperty, list, 40, GetShootMark(), range, 0f, 1f, true);
		}
	}

	public override void StartAttackState()
	{
		SniperAttack = false;
	}

	public override void UpdateAttackState(float deltaTime)
	{
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		InputController inputController = player.inputController;
		WrapMode mode = WrapMode.ClampForever;
		if (player.GameUnitType == EGameUnitType.LOCAL_PLAYER)
		{
			if (!inputController.inputInfo.fire)
			{
				if (AttackDoneReadyZoomOut())
				{
					GameApp.GetInstance().GetGameWorld().GetCamera()
						.ZoomOut(deltaTime);
				}
			}
			else
			{
				SniperAttack = false;
			}
		}
		if (player.GetWeapon().CoolDown())
		{
			if (player.IsLocal())
			{
				if ((player.GameUnitType == EGameUnitType.LOCAL_PLAYER || player.GameUnitType == EGameUnitType.REMOTE_PLAYER) && !inputController.inputInfo.fire && !SniperAttack)
				{
					SniperAttack = true;
					player.Attack();
				}
				else
				{
					if (player.GameUnitType != EGameUnitType.ROBOT)
					{
						Aim(deltaTime);
						if (!inputController.inputInfo.IsMoving())
						{
							if (flag)
							{
								player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
								player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
							}
							else
							{
								player.PlayAnimationAllLayers(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
							}
						}
						else if (flag)
						{
							player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
							player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
						}
						else
						{
							player.PlayAnimationAllLayers(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
						}
						return;
					}
					player.Attack();
				}
			}
			else
			{
				if (inputController.inputInfo.fire)
				{
					if (!inputController.inputInfo.IsMoving())
					{
						if (flag)
						{
							player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
							player.PlayAnimationWithoutBlend(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
						}
						else
						{
							player.PlayAnimationAllLayers(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
						}
					}
					else if (flag)
					{
						player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
						player.PlayAnimationWithoutBlend(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					}
					else
					{
						player.PlayAnimationAllLayers(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					}
					return;
				}
				float num = 0f;
				if (flag)
				{
					num += (float)Math.PI / 15f;
				}
				player.GetTransform().Rotate(-Vector3.up, num);
				CreateTrajectory();
				player.GetTransform().Rotate(Vector3.up, num);
			}
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, mode);
				}
				else
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, mode);
				}
			}
			else if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, mode);
			}
			else
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, mode);
			}
		}
		else if (inputController.inputInfo.IsMoving())
		{
			if (flag)
			{
				if (player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack))
				{
					player.ChangeToIdleState();
				}
				else
				{
					player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
				}
				if (player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack) && player.AnimationPlayed(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, 1f))
				{
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				}
			}
			else
			{
				player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				if (player.IsPlayingAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack) && player.AnimationPlayed(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, 1f))
				{
					player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				}
			}
		}
		else if (flag)
		{
			if (player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && player.AnimationPlayed(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, 1f))
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
		}
		else if (player.IsPlayingAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && player.AnimationPlayed(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, 1f))
		{
			player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
		}
		if (!inputController.inputInfo.fire && (!GetWaitForAttackAnimationStop() || (!player.IsPlayingAnimationNotFinished(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && !player.IsPlayingAnimationNotFinished(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack) && !player.IsPlayingAnimationNotFinished(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && !player.IsPlayingAnimationNotFinished(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack))))
		{
			player.ChangeToIdleState();
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

	public override void UpdateIdleState(float deltaTime)
	{
		InputController inputController = player.inputController;
		player.GetWeapon().StopFire();
		player.GetWeapon().AutoDestructEffect();
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		WeaponType weaponType = player.GetWeapon().GetWeaponType();
		if (player.IsLocal())
		{
			if (flag)
			{
				GameApp.GetInstance().GetGameWorld().GetCamera()
					.SetPivotOffset(Vector3.up * 0.25f);
			}
			if (AttackDoneReadyZoomOut())
			{
				GameApp.GetInstance().GetGameWorld().GetCamera()
					.ZoomOut(deltaTime);
			}
			player.Move(inputController.inputInfo.moveDirection);
		}
		if (inputController.inputInfo.IsMoving())
		{
			if (flag)
			{
				player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				AudioManager.GetInstance().StopSound("float_idle");
				AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_move", player.GetTransform().position);
			}
			else
			{
				player.PlayAnimation(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.PLAYER_FOOTSTEP, player.GetObject(), player.GetTransform());
			}
		}
		else if (flag)
		{
			player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
			player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			AudioManager.GetInstance().StopSound("float_move");
			AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_idle", player.GetTransform().position);
		}
		else
		{
			player.PlayAnimation(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, player.GetObject());
		}
		if (inputController.inputInfo.fire)
		{
			player.SetState(Player.ATTACK_STATE);
		}
	}

	public EffectPoolType GetSpark()
	{
		if (gunID == 13)
		{
			return EffectPoolType.W24_SPARK;
		}
		if (gunID == 24)
		{
			return EffectPoolType.W9_SPARK;
		}
		if (gunID == 27)
		{
			return EffectPoolType.W9_SPARK;
		}
		return EffectPoolType.NONE;
	}

	public EffectPoolType GetFireLine()
	{
		if (gunID == 13)
		{
			return EffectPoolType.W24;
		}
		if (gunID == 24)
		{
			return EffectPoolType.W9_BULLET;
		}
		if (gunID == 27)
		{
			return EffectPoolType.W27_BULLET;
		}
		return EffectPoolType.W9_BULLET;
	}

	public ObjectPool GetShootMark()
	{
		if (gunID == 13)
		{
			return EffectPool.GetInstance().GetEffectPool(EffectPoolType.W24_SHOOTMARK);
		}
		if (gunID == 24)
		{
			return null;
		}
		if (gunID == 27)
		{
			return null;
		}
		return null;
	}

	public float GetFireLineLife()
	{
		if (gunID == 27)
		{
			return 2f;
		}
		return 2f;
	}

	public float GetDamageInterval()
	{
		if (gunID == 27)
		{
			return 1.1f;
		}
		return 0f;
	}

	public float GetFireLineLength()
	{
		if (gunID == 27)
		{
			return 100f;
		}
		return 49f;
	}
}

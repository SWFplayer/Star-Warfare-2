using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
	public enum EAttackType
	{
		Sword_Face_Attack1 = 0,
		Sword_Face_Attack2 = 1,
		Sword_Ass_Attack1 = 2,
		Sword_Ass_Attack2 = 3
	}

	public const float MeleeAttackDamageFactor1 = 1.5f;

	public const float MeleeAttackDamageFactor2 = 1.8f;

	public const float MeleeAttackRadius1 = 4f;

	public const float MeleeHitRadius2 = 4f;

	public const float SpecialSwordAttackChance = 0.3f;

	public float[] MeleeAttackTriggerTime1 = new float[2] { 0.25f, 0.5f };

	public float[] MeleeAttackTriggerTime2 = new float[2] { 0.19148937f, 0.5531915f };

	public float[] MeleeAttackSoundTimeNormal = new float[2] { 0.25f, 0.5f };

	public float[] MeleeAttackSoundTimeRotate = new float[2] { 0.04255319f, 0.38297874f };

	private string mCurrentAttackAnimation = string.Empty;

	private bool mLastAnimIsNormalAttack;

	private List<GameUnit> mTargetList = new List<GameUnit>();

	private CreatureMeleeAttackSkill mSkill1 = new CreatureMeleeAttackSkill();

	private CreatureMeleeAttackSkill mSkill2 = new CreatureMeleeAttackSkill();

	protected Animation mAnimation;

	private float mLastAttackAnimationTime;

	private MeleeWeaponTrail mTrail;

	public override WeaponType GetWeaponType()
	{
		return WeaponType.Sword;
	}

	public override string GetAnimationSuffix()
	{
		return "LaserSword";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "LaserSword";
	}

	public override void AttackZoom(float deltaTime)
	{
	}

	public override bool IsTypeOfLoopShootingWeapon()
	{
		return false;
	}

	public override EAimType GetIdleAimType()
	{
		return EAimType.NONE;
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.NONE;
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		base.UIInit(weaponObj, weaponObjL);
		mAnimation = weaponObj.GetComponent<Animation>();
		PlayIdleAnim();
		if (GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility == DeviceCaps.Capbility.High)
		{
			MeleeWeaponTrail[] componentsInChildren = weaponObj.GetComponentsInChildren<MeleeWeaponTrail>(true);
			if (componentsInChildren.Length > 0)
			{
				mTrail = componentsInChildren[0];
			}
		}
		else
		{
			Transform transform = weaponObj.transform.Find("W26_slash");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	public override void Init(Player player)
	{
		gameWorld = GameApp.GetInstance().GetGameWorld();
		gameCamera = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		cameraComponent = gameCamera.GetComponent<Camera>();
		cameraTransform = gameCamera.CameraTransform;
		base.player = player;
		hitForce = 0f;
		weaponBoneTrans = player.GetTransform().Find(BoneName.Weapon);
		CreateGun();
		gun.transform.parent = weaponBoneTrans;
		gun.transform.localPosition = Vector3.zero;
		gun.transform.localRotation = Quaternion.identity;
		mAnimation = gun.GetComponent<Animation>();
		GunOff();
		SetWeaponShootSpeed();
		mSkill1.Damage = (int)(damage * 1.5f);
		mSkill1.Radius = 4f + player.GetSkills().GetSkill(SkillsType.SWORD_RADIUS_INCREASE);
		mSkill1.Angle = 180f;
		mSkill2.Damage = (int)(damage * 1.8f);
		mSkill2.HitRadius = 4f + player.GetSkills().GetSkill(SkillsType.SWORD_RADIUS_INCREASE);
		if (GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility == DeviceCaps.Capbility.High)
		{
			MeleeWeaponTrail[] componentsInChildren = gun.GetComponentsInChildren<MeleeWeaponTrail>(true);
			if (componentsInChildren.Length > 0)
			{
				mTrail = componentsInChildren[0];
				mTrail.Emit = false;
			}
		}
		else
		{
			Transform transform = gun.transform.Find("W26_slash");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	public override void GunOn()
	{
		Renderer[] componentsInChildren = gun.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = true;
		}
		SetWeaponShootSpeed();
		PlayIdleAnim();
	}

	public override void GunOff()
	{
		if (gun != null)
		{
			Renderer[] componentsInChildren = gun.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
		}
		StopFire();
		PlayIdleAnim();
		if ((bool)mTrail)
		{
			mTrail.Emit = false;
		}
	}

	public override void Loop(float deltaTime)
	{
		if (!player.IsLocal() || player.GameUnitType != EGameUnitType.LOCAL_PLAYER)
		{
			return;
		}
		if (player.IsPlayingAnimation(mCurrentAttackAnimation))
		{
			float legacyAnimationTime = player.GetLegacyAnimationTime(mCurrentAttackAnimation);
			if (mLastAnimIsNormalAttack)
			{
				float[] meleeAttackTriggerTime = MeleeAttackTriggerTime1;
				foreach (float num in meleeAttackTriggerTime)
				{
					if (mLastAttackAnimationTime <= num && num <= legacyAnimationTime)
					{
						CheckHit(mSkill1, 0);
					}
				}
				if (mLastAttackAnimationTime <= MeleeAttackSoundTimeNormal[0] && MeleeAttackSoundTimeNormal[0] <= legacyAnimationTime)
				{
					AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_LEFT, player.GetTransform().position);
				}
				if (mLastAttackAnimationTime <= MeleeAttackSoundTimeNormal[1] && MeleeAttackSoundTimeNormal[1] <= legacyAnimationTime)
				{
					AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_RIGHT, player.GetTransform().position);
				}
			}
			else
			{
				if (mLastAttackAnimationTime <= MeleeAttackTriggerTime2[0] && MeleeAttackTriggerTime2[0] <= legacyAnimationTime)
				{
					CheckHit(mSkill2, 0);
					AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_SWING, player.GetTransform().position);
				}
				else if (mLastAttackAnimationTime <= MeleeAttackTriggerTime2[1] && MeleeAttackTriggerTime2[1] <= legacyAnimationTime)
				{
					CheckHit(mSkill2, 1);
					AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_SWING, player.GetTransform().position);
				}
				if ((mLastAttackAnimationTime <= MeleeAttackSoundTimeRotate[0] || mLastAttackAnimationTime > legacyAnimationTime) && MeleeAttackSoundTimeRotate[0] <= legacyAnimationTime)
				{
					AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_ROTATE, player.GetTransform().position);
				}
				if (mLastAttackAnimationTime <= MeleeAttackSoundTimeRotate[1] && MeleeAttackSoundTimeRotate[1] <= legacyAnimationTime)
				{
					AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_ROTATE, player.GetTransform().position);
				}
			}
			mLastAttackAnimationTime = legacyAnimationTime;
		}
		else if ((bool)mTrail)
		{
			mTrail.Emit = false;
		}
	}

	public override void Attack(float deltaTime)
	{
		lastAttackTime = Time.time;
		attacked = false;
		if ((bool)mTrail)
		{
			mTrail.SendMessage("Start");
			mTrail.Emit = true;
		}
	}

	public override void UpdateAttackState(float deltaTime)
	{
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		InputController inputController = player.inputController;
		WrapMode mode = WrapMode.ClampForever;
		if (CoolDown())
		{
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
					player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), mode);
				}
				else
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), mode);
				}
			}
			else if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), mode);
			}
			else
			{
				player.PlayAnimationAllLayers(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), mode);
			}
		}
		else if (inputController.inputInfo.IsMoving())
		{
			if (flag)
			{
				if (player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) || player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack + "01"))
				{
					player.ChangeToIdleState();
				}
				else
				{
					player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
				}
				if (player.IsPlayingAnimation(mCurrentAttackAnimation) && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
				{
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				}
			}
			else
			{
				player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				if (player.IsPlayingAnimation(mCurrentAttackAnimation) && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
				{
					player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				}
			}
		}
		else if (flag)
		{
			if (player.IsPlayingAnimation(mCurrentAttackAnimation) && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
		}
		else if (player.IsPlayingAnimation(mCurrentAttackAnimation) && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
		{
			player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
		}
		if (!inputController.inputInfo.fire && GetWaitForAttackAnimationStop() && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
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

	public override void StartAttackState()
	{
		base.StartAttackState();
		mLastAnimIsNormalAttack = false;
	}

	private string GetAttackAnim(bool enableFly, bool isMoving)
	{
		string text = ((!enableFly) ? AnimationString.Stand : AnimationString.Fly);
		string text2 = ((!isMoving) ? AnimationString.Attack : AnimationString.RunAttack);
		if (Random.value < 0.3f)
		{
			mCurrentAttackAnimation = text + "_" + GetAnimationSuffix() + "_" + text2 + "01";
			mLastAnimIsNormalAttack = false;
		}
		else
		{
			mCurrentAttackAnimation = text + "_" + GetAnimationSuffix() + "_" + text2;
			mLastAnimIsNormalAttack = true;
		}
		return mCurrentAttackAnimation;
	}

	private void CheckHit(CreatureMeleeAttackSkill skill, byte factor)
	{
		foreach (Enemy value in GameApp.GetInstance().GetGameWorld().GetEnemies()
			.Values)
		{
			if (value != null && value.InPlayingState())
			{
				Vector3 vector = player.GetTransform().InverseTransformDirection(value.GetTransform().position - player.GetPosition());
				float num = vector.magnitude - value.GetHitColliderRadius();
				if (num <= skill.HitRadius || (num <= skill.Radius && vector.z > 0f && Mathf.Abs(vector.z / vector.x) > Mathf.Tan(skill.AngleRad)))
				{
					DamageProperty damageProperty = CreateDamageProperty();
					damageProperty.damage = skill.Damage;
					value.HitEnemy(player, damageProperty);
					((LocalPlayer)player).SetLastHitTarget(value);
				}
			}
		}
		foreach (DestructibleObjectScript value2 in GameApp.GetInstance().GetGameWorld().GetDestructibleDic()
			.Values)
		{
			if (null != value2)
			{
				Vector3 vector2 = player.GetTransform().InverseTransformDirection(value2.transform.position - player.GetPosition());
				vector2.y = 0f;
				float magnitude = vector2.magnitude;
				if (magnitude <= skill.HitRadius || (magnitude <= skill.Radius && vector2.z > 0f && Mathf.Abs(vector2.z / vector2.x) > Mathf.Tan(skill.AngleRad)))
				{
					value2.OnHit(skill.Damage);
					AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_HIT, value2.transform.position);
				}
			}
		}
		if (!GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item == null || item.IsSameTeam(player) || !item.InPlayingState())
			{
				continue;
			}
			Vector3 vector3 = player.GetTransform().InverseTransformDirection(item.GetTransform().position - player.GetPosition());
			float num2 = vector3.magnitude - item.GetHitColliderRadius();
			if (!(num2 <= skill.HitRadius) && (!(num2 <= skill.Radius) || !(vector3.z > 0f) || !(Mathf.Abs(vector3.z / vector3.x) > Mathf.Tan(skill.AngleRad))))
			{
				continue;
			}
			DamageProperty damageProperty2 = CreateDamageProperty();
			float num3 = Vector3.Angle(-item.GetTransform().forward, player.GetPosition() - item.GetPosition());
			if (num3 >= 60f)
			{
				damageProperty2.damageFactor = factor;
			}
			else
			{
				damageProperty2.damageFactor = (byte)(factor + 2);
			}
			if (!item.IsInAnyShields(damageProperty2))
			{
				player.HitTargetShowCrossHit();
				PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty2, item.GetChannelID());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				if (item.InPlayingState())
				{
					player.GetSkills().PlayerHitTheTargetByDamageType(damageProperty2);
				}
				((LocalPlayer)player).SetLastHitTarget(item);
			}
		}
		List<ShieldScript> currentShields = gameWorld.GetCurrentShields();
		foreach (ShieldScript item2 in currentShields)
		{
			if (item2 != null && item2.shieldStarted && (!GameApp.GetInstance().GetGameMode().IsTeamMode() || !item2.ownedPlayer.IsSameTeam(player)))
			{
				Vector3 vector4 = player.GetTransform().InverseTransformDirection(item2.transform.position - player.GetPosition());
				vector4.y -= item2.shieldRadius;
				float num4 = vector4.magnitude - item2.shieldRadius;
				if (num4 <= skill.HitRadius || (num4 <= skill.Radius && vector4.z > 0f && Mathf.Abs(vector4.z / vector4.x) > Mathf.Tan(skill.AngleRad)))
				{
					DamageProperty damageProperty3 = CreateDamageProperty();
					damageProperty3.damageFactor = factor;
					item2.PlayerHitShield(damageProperty3);
				}
			}
		}
	}

	private string GetAttackAnim()
	{
		if (gunID == 26)
		{
			return "open";
		}
		return string.Empty;
	}

	public override void PlayFireAnim()
	{
		base.PlayFireAnim();
	}

	public override void PlayIdleAnim()
	{
		base.PlayIdleAnim();
	}

	public virtual void PlayAnimation(string name, WrapMode mode)
	{
		if (!(mAnimation[name] == null) && (!mAnimation.IsPlaying(name) || mode != WrapMode.ClampForever || AnimationPlayed(name, 1f)))
		{
			mAnimation[name].speed = 1f;
			if (mode != WrapMode.ClampForever || !AnimationPlayed(name, 1f))
			{
				mAnimation[name].wrapMode = mode;
				mAnimation[name].normalizedTime = 0f;
				mAnimation.Play(name);
			}
		}
	}

	public virtual void PlayRevertAnimition(string name, WrapMode mode)
	{
		if (!(mAnimation[name] == null) && (!mAnimation.IsPlaying(name) || mode != WrapMode.ClampForever || AnimationPlayed(name, 1f)))
		{
			mAnimation[name].speed = -1f;
			if (mode != WrapMode.ClampForever || !AnimationPlayed(name, 1f))
			{
				mAnimation[name].wrapMode = mode;
				mAnimation[name].normalizedTime = 1f;
				mAnimation.Play(name);
			}
		}
	}

	public bool AnimationPlayed(string name, float percent)
	{
		if (mAnimation[name] == null)
		{
			return false;
		}
		if (mAnimation[name].speed >= 0f)
		{
			if (mAnimation[name].time >= mAnimation[name].clip.length * percent)
			{
				return true;
			}
			return false;
		}
		if (mAnimation[name].time <= mAnimation[name].clip.length * (1f - percent))
		{
			return true;
		}
		return false;
	}

	public override void StartIdleState()
	{
		base.StartIdleState();
		if ((bool)mTrail)
		{
			mTrail.Emit = false;
		}
	}
}

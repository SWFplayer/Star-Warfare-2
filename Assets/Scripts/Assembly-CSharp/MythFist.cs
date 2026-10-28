using System;
using UnityEngine;

public class MythFist : Fist
{
	public override WeaponType GetWeaponType()
	{
		return WeaponType.Myth370Fist;
	}

	public override void Attack(float deltaTime)
	{
		lastAttackTime = Time.time;
		AudioManager.GetInstance().PlaySound(base.FireSoundName);
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		if (aas != null)
		{
			aas.StartAnimation();
		}
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(gunfire, out direction);
		direction = (direction + Vector3.up * Global.MYTH_FIST_ANGLEV_FACTOR).normalized;
		Vector3 moveDirection = player.InputController.inputInfo.moveDirection;
		moveDirection.y = 0f;
		Vector3 forward = player.GetTransform().forward;
		forward.y = 0f;
		double value = Mathf.Cos(Vector3.Angle(moveDirection, forward) * ((float)Math.PI / 180f)) * Global.MYTH_FIST_ACC_BY_MOVE;
		value = Math.Round(value, 1);
		GameObject gameObject = gameWorld.GetEffectPool(GetFistVFX()).CreateObject(gunfire.position, direction, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<AutoDestroyScript>().enabled = false;
		gameObject.GetComponent<FireLineScript>().enabled = false;
		ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
		component.enabled = true;
		component.isLocal = true;
		component.isUseGravity = true;
		component.dir = direction;
		component.flySpeed = bulletSpeed + (float)value;
		component.explodeRadius = GetBombRange();
		component.hitForce = hitForce;
		component.life = 10f;
		component.damage = (int)damage;
		component.GunType = WeaponType.Myth370Fist;
		component.targetPos = aimTarget;
		component.weapon = this;
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerFireRocketRequest request = new PlayerFireRocketRequest(gunID, gunfire.position, direction, component.flySpeed);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		if ((bool)mAnimation)
		{
			mAnimation.CrossFade("attack", 0.1f);
		}
	}

	public override EffectPoolType GetFistVFX()
	{
		return EffectPoolType.W31_ROCKET;
	}
}

using System.Collections.Generic;
using UnityEngine;

public class SlasherThornScript : BaseLaserScript
{
	public Vector3 Speed;

	public float ExplodeRadius;

	public float ExplosionTime;

	private GameUnit mSlasher;

	private float mStartTime;

	private bool mIsStop;

	private void Start()
	{
		mStartTime = Time.time;
	}

	private void Update()
	{
		if (mIsStop)
		{
			if (Time.time - mStartTime > ExplosionTime)
			{
				Explode();
			}
		}
		else
		{
			Speed += Physics.gravity.y * Vector3.up * Time.deltaTime;
			base.transform.Translate(Speed * Time.deltaTime, Space.World);
			base.transform.LookAt(base.transform.position + Speed * 10f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.FLOOR || other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET)
		{
			mIsStop = true;
		}
		else if (other.gameObject.layer == PhysicsLayer.PLAYER || other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
		{
			Explode();
		}
	}

	private void Explode()
	{
		GameObject original = Resources.Load("_Old/_Effect/Mutalisk/bug_04_bow") as GameObject;
		Object.Instantiate(original, base.transform.position, Quaternion.identity);
		if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.IsBoss)
			{
				List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
				foreach (RemotePlayer item in remotePlayers)
				{
					if (!((item.GetTransform().position - base.transform.position).sqrMagnitude < ExplodeRadius * ExplodeRadius))
					{
						continue;
					}
					Ray ray = new Ray(base.transform.position, item.GetTransform().position + new Vector3(0f, 1f, 0f) - base.transform.position);
					RaycastHit hitInfo;
					Physics.Raycast(ray, out hitInfo, ExplodeRadius, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.VOID_SHIELD) | (1 << PhysicsLayer.REMOTE_PLAYER));
					if (hitInfo.collider != null && hitInfo.collider.gameObject != null)
					{
						if (hitInfo.collider.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
						{
							DamageProperty damageProperty = new DamageProperty(mSlasher, Damage, PlayerDamageType);
							damageProperty.weaponid = 7;
							damageProperty.mAttackerType = AttackerType.PLAYER_BOSS;
							PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty, item.GetChannelID());
							GameApp.GetInstance().GetNetworkManager().SendRequest(request);
						}
						else if (hitInfo.collider.gameObject.layer == PhysicsLayer.VOID_SHIELD)
						{
							DamageProperty damageProperty2 = new DamageProperty(mSlasher, Damage, PlayerDamageType);
							damageProperty2.weaponid = 7;
							ShieldScript component = hitInfo.collider.transform.root.GetComponent<ShieldScript>();
							component.PlayerHitShield(damageProperty2);
						}
					}
				}
			}
		}
		else
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if ((localPlayer.GetTransform().position - base.transform.position).sqrMagnitude < ExplodeRadius * ExplodeRadius)
			{
				Ray ray2 = new Ray(base.transform.position, localPlayer.GetTransform().position - base.transform.position);
				float magnitude = (base.transform.position - localPlayer.GetTransform().position).magnitude;
				RaycastHit hitInfo2;
				if (!Physics.Raycast(ray2, out hitInfo2, magnitude, 1 << PhysicsLayer.WALL_AGAINST_BULLET))
				{
					localPlayer.OnHit(new DamageProperty(mSlasher, Damage, PlayerDamageType));
				}
			}
		}
		AudioManager.GetInstance().PlaySoundAt(AudioName.SLASHER_SPIKE_EXPLODE, base.transform.position);
		Object.DestroyObject(base.gameObject);
	}

	public void SetSlasher(GameUnit attacker)
	{
		mSlasher = attacker;
	}
}

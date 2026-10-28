using System;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonQueenBomb : BaseLaserScript
{
	[NonSerialized]
	public Vector3 EndPosition;

	private static List<GameUnit> mTempList = new List<GameUnit>();

	private bool mIsMoving;

	private float mStartTime;

	public void Reset()
	{
		mIsMoving = true;
		mStartTime = Time.time;
		Ray ray = new Ray(base.transform.position, Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_HEIGHT * 2f, 1 << PhysicsLayer.FLOOR))
		{
			EndPosition = hitInfo.point;
			return;
		}
		EndPosition = base.transform.position - new Vector3(0f, CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_HEIGHT, 0f);
		Debug.LogError("No floor for CrimsonQueenBomb");
	}

	public void Update()
	{
		if (mIsMoving)
		{
			base.transform.Translate(0f, (0f - CrimsonQueenParameter.EXPLOSION_SPAWN_BOMB_SPEED) * Time.deltaTime, 0f, Space.World);
			if (base.transform.position.y < EndPosition.y)
			{
				Vector3 position = base.transform.position;
				position.y = EndPosition.y;
				base.transform.position = position;
				mIsMoving = false;
				AudioManager.GetInstance().PlaySoundAt(AudioName.CRIMSONQUEEN_BOMB_LAND, base.transform.position);
			}
		}
		if (Time.time - mStartTime > CrimsonQueenParameter.EXPLOSION_SPAWN_BOMB_DURATION)
		{
			Explode();
			DestructibleObjectScript component = base.gameObject.GetComponent<DestructibleObjectScript>();
			component.Destroy();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		int layer = other.gameObject.layer;
		if (layer == PhysicsLayer.PLAYER || layer == PhysicsLayer.REMOTE_PLAYER || layer == PhysicsLayer.ROBOT || layer == PhysicsLayer.VOID_SHIELD)
		{
			Explode();
		}
	}

	public void Explode()
	{
		base.gameObject.SetActive(false);
		GameObject go = Pool.sGetInstance().Allocate("SW2_Effect/B5_explode", base.transform.position);
		AutoDestroyScript component = GameWorld.GetComponent<AutoDestroyScript>(go);
		component.type = AutoDestroyType.UNABLE;
		component.life = 4f;
		AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/rpg/rpg-21_boom", base.transform.position);
		mTempList.Clear();
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (localPlayer != null && localPlayer.InPlayingState())
		{
			mTempList.Add(localPlayer);
		}
		foreach (Robot robot in GameApp.GetInstance().GetGameWorld().GetRobotList())
		{
			if (robot.InPlayingState())
			{
				mTempList.Add(robot);
			}
		}
		foreach (GameUnit mTemp in mTempList)
		{
			if (!((mTemp.GetPosition() - base.transform.position).sqrMagnitude < CrimsonQueenParameter.EXPLOSION_SPAWN_BOMB_RADIUS * CrimsonQueenParameter.EXPLOSION_SPAWN_BOMB_RADIUS))
			{
				continue;
			}
			mTemp.OnHit(new DamageProperty(Attacker, Damage, PlayerDamageType));
			if (mTemp.GameUnitType == EGameUnitType.LOCAL_PLAYER && KnockSpeed > 0f)
			{
				Vector3 vector = new Vector3(localPlayer.GetTransform().forward.x, 0f, localPlayer.GetTransform().forward.z);
				Vector3 to = localPlayer.GetPosition() - base.transform.position;
				float f = Vector3.Angle(vector, to);
				float num = KnockSpeed;
				if (Mathf.Abs(f) > 90f)
				{
					num = 0f - num;
				}
				localPlayer.OnKnocked(num);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					PlayerOnKnockedRequest request = new PlayerOnKnockedRequest(num, localPlayer.GetChannelID());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
	}
}

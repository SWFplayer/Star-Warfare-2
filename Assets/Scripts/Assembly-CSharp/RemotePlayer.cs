using UnityEngine;

public class RemotePlayer : Player
{
	protected NetworkTransformInterpolation interpolation;

	protected bool hasGotCoopSwVoidMaterialInfo;

	protected short coopVoidMaterialCount;

	protected Timer beBossTouchTimer = new Timer(1f);

	public RemotePlayer()
	{
		interpolation = new NetworkTransformInterpolation(this);
		userState = new UserState();
		base.GameUnitType = EGameUnitType.REMOTE_PLAYER;
	}

	public NetworkTransformInterpolation GetInterpolation()
	{
		return interpolation;
	}

	public void UpdateTransform(Vector3 pos, float eulerAnglesY, int timeStamp, Transform transform)
	{
		bool run = true;
		if (base.inputController.inputInfo.moveDirection == Vector3.zero)
		{
			run = false;
		}
		NetworkTransform nTrans = new NetworkTransform(pos, eulerAnglesY, timeStamp, run);
		interpolation.SetTransform(transform);
		if (mVehicleState == EPlayerVehicleState.CANNON && mVehicle != null && mVehicle.VehicleType != EVehicleType.LONG_SWORD_CANNON)
		{
			interpolation.SetPositionTransform(mVehicle.GetTransform());
		}
		interpolation.ReceiveTransform(nTrans);
	}

	public override void Loop(float deltaTime)
	{
		if (!(entityObject != null) || entityObject.transform.childCount != 0)
		{
			base.Loop(deltaTime);
			if (IsBoss)
			{
				mBossState.Update(this);
			}
			else
			{
				base.State.NextState(this, deltaTime);
			}
			if (mVehicleState != EPlayerVehicleState.CANNON)
			{
				interpolation.Loop();
			}
			if (!base.inputController.inputInfo.fire)
			{
				base.TargetAngleV = 0f;
			}
			base.AngleV = Mathf.LerpAngle(base.AngleV, base.TargetAngleV, deltaTime * 10f);
		}
	}

	public override void Init()
	{
		InitSkills();
		base.Init();
		entityObject.name = channelID.ToString();
		entityObject.layer = PhysicsLayer.REMOTE_PLAYER;
		entityObject.tag = TagName.PLAYER_ROOT;
		Transform transform = entityTransform.Find(BoneName.Bag).Find("Bag");
		if (transform != null)
		{
			transform.localPosition = Vector3.zero;
		}
		ReduceRemotePlayerQuality();
	}

	public override void ChangeWeaponInBag(int bagIndex)
	{
		Weapon weaponInBag = userState.GetWeaponInBag(bagIndex);
		if (weaponInBag != null)
		{
			ChangeWeapon(weaponInBag);
		}
	}

	public override void RefreshAvatar(EnemyType type)
	{
		base.RefreshAvatar(type);
		entityObject.layer = PhysicsLayer.BOSS;
		interpolation.SetTransform(entityTransform);
		GameApp.GetInstance().GetGameMode().ModePlay = Mode.GIANT_BETRAYAL;
		entityObject.name = channelID.ToString();
		entityObject.tag = TagName.PLAYER_ROOT;
		ReduceRemotePlayerQuality();
	}

	public override float GetHitColliderRadius()
	{
		return cc.radius;
	}

	public bool HasLeavedGame()
	{
		return mHasLeavedGame;
	}

	public void ReduceRemotePlayerQuality()
	{
		if (!GameApp.GetInstance().GetGameMode().IsVSMode() || (GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility != DeviceCaps.Capbility.General && GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility != DeviceCaps.Capbility.Middle))
		{
			return;
		}
		Renderer[] componentsInChildren = entityObject.GetComponentsInChildren<Renderer>(true);
		Shader shader = null;
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				if (material.shader.name.StartsWith("StarWarfare/SNDM_new") && !material.shader.name.Equals("StarWarfare/Middle/SNDM_new_LOD100"))
				{
					if (null == shader)
					{
						shader = Shader.Find("StarWarfare/Middle/SNDM_new_LOD100");
					}
					if (null != shader)
					{
						material.shader = shader;
					}
				}
			}
		}
	}

	public void SetSwVoidMaterialCount(short count)
	{
		hasGotCoopSwVoidMaterialInfo = true;
		coopVoidMaterialCount = count;
	}

	public bool GetHasGotMaterialInfo()
	{
		return hasGotCoopSwVoidMaterialInfo;
	}

	public short GetCoopVoidMaterialCount()
	{
		return coopVoidMaterialCount;
	}

	public override void OnPickUp(LootType lootType, float param1, float param2)
	{
		base.OnPickUp(lootType, param1, param2);
		int num = -1;
		switch (lootType)
		{
		case LootType.MONEY:
		{
			int num2 = Random.Range(0, 100);
			if (num2 < 50)
			{
				AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/pickup/pickup_money01", entityTransform.position);
			}
			else
			{
				AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/pickup/pickup_money02", entityTransform.position);
			}
			num = GameApp.GetInstance().GetGameWorld().GetLootEffectIndex(EffectName.CASH_LOOT);
			break;
		}
		case LootType.VEHICLE:
			AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/pickup/pickup_energy", entityTransform.position);
			num = GameApp.GetInstance().GetGameWorld().GetLootEffectIndex(EffectName.ENERGY_LOOT);
			break;
		case LootType.PICKUP:
			if (param1 <= 2f)
			{
				num = GameApp.GetInstance().GetGameWorld().GetLootEffectIndex(EffectName.HP_LOOT);
			}
			break;
		}
		if (num != -1)
		{
			GameApp.GetInstance().GetGameWorld().SpawnLootEffect(num, entityTransform.position, entityTransform);
		}
	}

	public bool canBePlayerBossTouch()
	{
		if (beBossTouchTimer.Ready())
		{
			beBossTouchTimer.Do();
			return true;
		}
		return false;
	}
}

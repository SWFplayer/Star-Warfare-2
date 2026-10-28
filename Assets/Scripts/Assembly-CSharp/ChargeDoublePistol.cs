using System;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDoublePistol : DoublePistol
{
	protected const float POWER_TIME = 15f;

	protected const byte CHARGE_TIME = 100;

	protected const float rotateSpeed = 720f;

	protected const float powerEffectDis = 15f;

	protected const float subFireLineDis = 20f;

	protected byte[] chargeTrigger = new byte[7] { 16, 32, 48, 60, 72, 84, 96 };

	protected Vector3 startPos = new Vector3(0f, 1f, 0f);

	protected Vector3 endPos = new Vector3(0.8f, 2.5f, 0f);

	protected Vector3 endPosL = new Vector3(-0.8f, 2.5f, 0f);

	protected byte charge;

	protected float powerStartTime;

	protected bool isPower;

	protected List<GameObject>[] chargeEffectObjList;

	protected GameObject powerFireLine;

	protected GameObject powerFireLineL;

	protected GameObject subPowerFireLine;

	protected GameObject subPowerFireLineL;

	protected GameObject powerEffectFireLine;

	protected GameObject powerEffectFireLineL;

	protected GameUnit powerEffectTarget;

	protected GameUnit powerEffectTargetL;

	protected bool isInUi;

	protected bool isLocalWaitResponse;

	protected GameObject[] powerAttackEffect;

	protected Transform[] roots;

	protected float lastEffectAttackTime;

	protected bool noFoundEnemy;

	protected bool IsCharged
	{
		get
		{
			return charge >= 100;
		}
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.ChargeDoublePistol;
	}

	public override void Init(Player player)
	{
		base.Init(player);
		InitEffectList();
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
		if (player != null)
		{
			charge = player.GetChargeWeaponStoreChargeValue();
		}
		isPower = false;
		isInUi = false;
		InitPowerEffect();
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		gun = weaponObj;
		gunL = weaponObjL;
		base.UIInit(weaponObj, weaponObjL);
		InitEffectList();
		isInUi = true;
		InitPowerEffect();
	}

	public void AddStack()
	{
		if (!IsCharged && !isPower)
		{
			bool flag = false;
			byte[] array = chargeTrigger;
			foreach (byte b in array)
			{
				if (charge < b && charge + 2 >= b)
				{
					flag = true;
				}
			}
			charge += 2;
			updateEffect();
			if (flag && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerChargeWeaponInfoRequest request = new PlayerChargeWeaponInfoRequest(GetCharge(), IsPower(), 0);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else if (!isPower)
		{
			PowerOn();
		}
	}

	public void AttackTarget(GameUnit enemy, Vector3 pos)
	{
		if (!isPower || player == null || !player.IsLocal())
		{
			return;
		}
		int beginID = 0;
		if (enemy != null)
		{
			if (enemy.GameUnitType == EGameUnitType.ENEMY)
			{
				beginID = (enemy as Enemy).EnemyID;
			}
			else if (enemy.GameUnitType == EGameUnitType.REMOTE_PLAYER)
			{
				beginID = (enemy as RemotePlayer).GetChannelID();
			}
		}
		GameUnit[] array = new GameUnit[2];
		float[] array2 = new float[2] { 20f, 20f };
		if (gameWorld == null)
		{
			gameWorld = GameApp.GetInstance().GetGameWorld();
		}
		if (gameWorld == null)
		{
			return;
		}
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			foreach (RemotePlayer remotePlayer in gameWorld.GetRemotePlayers())
			{
				if (!remotePlayer.IsSameTeam(player) && remotePlayer != enemy)
				{
					float num = Vector3.Distance(pos, remotePlayer.GetPosition());
					if (num < array2[0])
					{
						array[1] = array[0];
						array2[1] = array2[0];
						array[0] = remotePlayer;
						array2[0] = num;
					}
					else if (num < array2[1])
					{
						array[1] = remotePlayer;
						array2[1] = num;
					}
				}
			}
		}
		else
		{
			foreach (Enemy value in gameWorld.GetEnemies().Values)
			{
				if (value != enemy)
				{
					float num2 = Vector3.Distance(pos, value.GetPosition());
					if (num2 < array2[0])
					{
						array[1] = array[0];
						array2[1] = array2[0];
						array[0] = value;
						array2[0] = num2;
					}
					else if (num2 < array2[1])
					{
						array[1] = value;
						array2[1] = num2;
					}
				}
			}
		}
		byte b = 0;
		GameUnit[] array3 = array;
		foreach (GameUnit gameUnit in array3)
		{
			if (gameUnit == null)
			{
				continue;
			}
			Vector3 normalized = (gameUnit.GetPosition() - pos).normalized;
			if (!gameUnit.UnitCanBeHitByRay(pos, 100f))
			{
				continue;
			}
			GameObject gameObject = ((b != 0) ? subPowerFireLineL : subPowerFireLine);
			if (gameObject == null || !gameObject.activeSelf)
			{
				gameObject = gameWorld.GetEffectPool(EffectPoolType.W32_TRAJECTORY_02).CreateObject(pos, normalized, Quaternion.identity);
				if (gameObject != null)
				{
					NightmareLaserScript component = gameObject.GetComponent<NightmareLaserScript>();
					component.mIsLeftFire = b == 1;
					component.weapon = this;
					component.gunfire = enemy.GetTransform();
					component.length = 80f;
					component.Init();
					component.m_Type = NightmareLaserScript.EFireLineType.SUB;
					component.targetTrans = gameUnit.GetTransform();
					component.targetGameUnit = gameUnit;
					if (b == 0)
					{
						if (powerFireLine != null)
						{
							component.nightmareLaserScript = powerFireLine.GetComponent<NightmareLaserScript>();
						}
					}
					else if (powerFireLineL != null)
					{
						component.nightmareLaserScript = powerFireLineL.GetComponent<NightmareLaserScript>();
					}
				}
				if (mIsLeftFire)
				{
					subPowerFireLineL = gameObject;
				}
				else
				{
					subPowerFireLine = gameObject;
				}
			}
			else
			{
				NightmareLaserScript component2 = gameObject.GetComponent<NightmareLaserScript>();
				component2.targetTrans = gameUnit.GetTransform();
				component2.targetGameUnit = gameUnit;
				component2.gunfire = enemy.GetTransform();
				component2.resetTime();
			}
			int num3 = 0;
			if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
			{
				num3 = (gameUnit as Enemy).EnemyID;
				(gameUnit as Enemy).HitEnemy(player, CreateDamageProperty());
			}
			else if (gameUnit.GameUnitType == EGameUnitType.REMOTE_PLAYER)
			{
				num3 = (gameUnit as RemotePlayer).GetChannelID();
				PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(CreateDamageProperty(), num3);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerNightmarePowerAttackRequest request2 = new PlayerNightmarePowerAttackRequest(1, b == 0, beginID, num3);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
			b = 1;
		}
		if (IsEffectAttackCoolDown() && noFoundEnemy)
		{
			EffectAttack(enemy);
		}
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		if (isPower)
		{
			AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_06_DoublePistol, avatar.transform.position);
			Transform transform = gunfire;
			Vector3 normalized = transform.transform.forward.normalized;
			GameObject gameObject = powerFireLine;
			if (gameObject == null || !gameObject.activeSelf)
			{
				gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W32_TRAJECTORY_02).CreateObject(transform.transform.position, normalized, Quaternion.identity);
				if (gameObject != null)
				{
					NightmareLaserScript component = gameObject.GetComponent<NightmareLaserScript>();
					Vector3 vector = gunfire.position + normalized * 80f;
					component.mIsLeftFire = mIsLeftFire;
					component.weapon = this;
					component.gunfire = transform;
					component.length = 10f;
					component.Init();
				}
				powerFireLine = gameObject;
			}
			else
			{
				NightmareLaserScript component2 = gameObject.GetComponent<NightmareLaserScript>();
				component2.resetTime();
			}
			transform = gunfireL;
			normalized = transform.transform.forward.normalized;
			GameObject gameObject2 = powerFireLineL;
			if (gameObject2 == null || !gameObject2.activeSelf)
			{
				gameObject2 = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W32_TRAJECTORY_02).CreateObject(transform.transform.position, normalized, Quaternion.identity);
				if (gameObject2 != null)
				{
					NightmareLaserScript component3 = gameObject2.GetComponent<NightmareLaserScript>();
					Vector3 vector2 = gunfire.position + normalized * 80f;
					component3.mIsLeftFire = mIsLeftFire;
					component3.weapon = this;
					component3.gunfire = transform;
					component3.length = 10f;
					component3.Init();
				}
				powerFireLineL = gameObject2;
			}
			else
			{
				NightmareLaserScript component4 = gameObject2.GetComponent<NightmareLaserScript>();
				component4.resetTime();
			}
		}
		else
		{
			AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_06_DoublePistol, avatar.transform.position);
			gunfire = GetGunFire(gunObj);
			Vector3 normalized2 = gunfire.transform.forward.normalized;
			GameObject gameObject3 = EffectPool.GetInstance().GetEffectPool(GetTrajectoryEffect()).CreateObject(gunfire.transform.position, normalized2, Quaternion.identity);
			if (gameObject3 != null)
			{
				FireLineScript component5 = gameObject3.GetComponent<FireLineScript>();
				component5.beginPos = gunfire.position;
				component5.endPos = gunfire.position + normalized2 * 100f;
			}
			if (gunfireObj != null)
			{
				gunfireObj.SetActive(true);
			}
			gunfireL = GetGunFire(gunObjL);
			Vector3 normalized3 = gunfireL.transform.forward.normalized;
			GameObject gameObject4 = EffectPool.GetInstance().GetEffectPool(GetTrajectoryEffect()).CreateObject(gunfireL.transform.position, normalized3, Quaternion.identity);
			if (gameObject4 != null)
			{
				FireLineScript component6 = gameObject4.GetComponent<FireLineScript>();
				component6.beginPos = gunfireL.position;
				component6.endPos = gunfireL.position + normalized2 * 100f;
			}
			if (gunfireObjL != null)
			{
				gunfireObjL.SetActive(true);
			}
		}
		if (isPower)
		{
			charge -= 4;
			if (charge < 0)
			{
				PowerOff();
			}
			updateEffect();
		}
		else
		{
			AddStack();
		}
	}

	protected void updateEffect()
	{
		for (int i = 0; i < chargeTrigger.Length; i++)
		{
			if (charge >= chargeTrigger[i])
			{
				chargeEffectObjList[0][i].SetActive(true);
				chargeEffectObjList[1][i].SetActive(true);
			}
			else
			{
				chargeEffectObjList[0][i].SetActive(false);
				chargeEffectObjList[1][i].SetActive(false);
			}
		}
	}

	protected override EffectPoolType GetTrajectoryEffect()
	{
		if (isPower)
		{
			return EffectPoolType.W32_TRAJECTORY_02;
		}
		return EffectPoolType.W32_TRAJECTORY_01;
	}

	protected override ObjectPool GetShootMark()
	{
		if (isPower)
		{
			return gameWorld.GetEffectPool(EffectPoolType.W32_SHOOT_MARK02);
		}
		return gameWorld.GetEffectPool(EffectPoolType.W32_SHOOT_MARK);
	}

	protected void InitEffectList()
	{
		if (chargeEffectObjList == null)
		{
			chargeEffectObjList = new List<GameObject>[2];
		}
		chargeEffectObjList[0] = new List<GameObject>();
		chargeEffectObjList[1] = new List<GameObject>();
		for (int i = 0; i < chargeTrigger.Length; i++)
		{
			string name = EffectName.W32_CHARGE_PRE + (i + 1) + EffectName.W32_CHARGE_END;
			Transform transform = gun.transform.Find(name);
			if (transform != null)
			{
				chargeEffectObjList[0].Add(transform.gameObject);
			}
			transform = gunL.transform.Find(name);
			if (transform != null)
			{
				chargeEffectObjList[1].Add(transform.gameObject);
			}
		}
	}

	protected void InitPowerEffect()
	{
		if (powerAttackEffect == null)
		{
			powerAttackEffect = new GameObject[2];
			roots = new Transform[2];
		}
		for (int i = 0; i < powerAttackEffect.Length; i++)
		{
			string name = "powerEffect";
			Transform transform = ((i != 0) ? gunL.transform : gun.transform);
			Transform transform2 = transform.Find(name);
			if (transform2 != null)
			{
				powerAttackEffect[i] = transform2.gameObject;
				roots[i] = powerAttackEffect[i].transform.Find("root");
				transform2.gameObject.SetActive(false);
			}
		}
		if (isInUi)
		{
			powerAttackEffect = null;
		}
	}

	public byte GetCharge()
	{
		return charge;
	}

	public bool IsPower()
	{
		return isPower;
	}

	public override void GunOn()
	{
		base.GunOn();
		PowerOff();
		updateEffect();
		isLocalWaitResponse = false;
	}

	public override void GunOff()
	{
		base.GunOff();
		if (isPower)
		{
			charge = 0;
			isLocalWaitResponse = false;
			PowerOff();
		}
		else
		{
			if (chargeEffectObjList == null)
			{
				return;
			}
			List<GameObject>[] array = chargeEffectObjList;
			foreach (List<GameObject> list in array)
			{
				if (list == null)
				{
					continue;
				}
				foreach (GameObject item in list)
				{
					if (item != null)
					{
						item.SetActive(false);
					}
				}
			}
		}
	}

	public void PowerOn()
	{
		if (player != null && player.IsLocal())
		{
			if (isLocalWaitResponse)
			{
				return;
			}
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerChargeWeaponInfoRequest request = new PlayerChargeWeaponInfoRequest(charge, true, 0);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				isLocalWaitResponse = true;
				return;
			}
			isPower = true;
			powerStartTime = Time.time;
			if (player != null)
			{
				PowerAttack();
			}
		}
		else if (isInUi)
		{
			isPower = true;
			powerStartTime = Time.time;
			if (player != null)
			{
				PowerAttack();
			}
		}
	}

	public void PowerOnFromSever(byte charge)
	{
		isPower = true;
		if (charge >= 100)
		{
			powerStartTime = Time.time;
		}
		else
		{
			powerStartTime = Time.time - (float)(100 - charge) * 15f;
		}
		if (player == null)
		{
		}
		isLocalWaitResponse = false;
		PowerAttack();
	}

	public void PowerOff()
	{
		isPower = false;
		if (player == null)
		{
		}
		if (powerAttackEffect != null)
		{
			for (int i = 0; i < powerAttackEffect.Length; i++)
			{
				if (powerAttackEffect[i] != null)
				{
					powerAttackEffect[i].SetActive(false);
				}
			}
		}
		if (chargeEffectObjList == null)
		{
			return;
		}
		List<GameObject>[] array = chargeEffectObjList;
		foreach (List<GameObject> list in array)
		{
			if (list == null)
			{
				continue;
			}
			foreach (GameObject item in list)
			{
				if (item != null)
				{
					item.SetActive(false);
				}
			}
		}
	}

	public override void DoLogic()
	{
		base.DoLogic();
		if (!isPower)
		{
			return;
		}
		charge = (byte)(100f * (1f - (Time.time - powerStartTime) * 1f / 15f));
		updateEffect();
		if (Time.time - powerStartTime > 15f)
		{
			PowerOff();
			return;
		}
		if (IsEffectAttackCoolDown())
		{
			EffectAttack();
		}
		float num = Time.time - powerStartTime;
		if (num > 1f)
		{
			num = 1f;
		}
		if (powerAttackEffect == null)
		{
			return;
		}
		for (int i = 0; i < powerAttackEffect.Length; i++)
		{
			if (!(powerAttackEffect[i] != null))
			{
				continue;
			}
			if (num == 1f)
			{
				if (i == 0)
				{
					powerAttackEffect[i].transform.position = getEndPos(player.GetTransform(), endPos);
					if (powerEffectTarget != null && powerEffectTarget.GetTransform() != null && powerEffectTarget.GetTransform().gameObject.activeSelf)
					{
						powerAttackEffect[i].transform.LookAt(powerEffectTarget.GetTransform());
					}
					else
					{
						powerAttackEffect[i].transform.LookAt(player.GetTransform().forward * 40f + player.GetPosition());
					}
					roots[i].localRotation = Quaternion.Euler((Time.time - powerStartTime) * 720f, 90f, 0f);
					roots[i].localScale = Vector3.one / 2f;
				}
				else
				{
					powerAttackEffect[i].transform.position = getEndPos(player.GetTransform(), endPosL);
					if (powerEffectTargetL != null && powerEffectTargetL.GetTransform() != null && powerEffectTargetL.GetTransform().gameObject.activeSelf)
					{
						powerAttackEffect[i].transform.LookAt(powerEffectTargetL.GetTransform());
					}
					else
					{
						powerAttackEffect[i].transform.LookAt(player.GetTransform().forward * 40f + player.GetPosition());
					}
					roots[i].localRotation = Quaternion.Euler((Time.time - powerStartTime) * 720f, -90f, 0f);
					roots[i].localScale = Vector3.one / 2f;
				}
			}
			else if (i == 0)
			{
				powerAttackEffect[i].transform.position = Vector3.Lerp(gun.transform.position, getEndPos(player.GetTransform(), endPos), num);
				if (powerEffectTarget != null && powerEffectTarget.GetTransform() != null && powerEffectTarget.GetTransform().gameObject.activeSelf)
				{
					powerAttackEffect[i].transform.LookAt(powerEffectTarget.GetTransform());
				}
				else
				{
					powerAttackEffect[i].transform.LookAt(player.GetTransform().forward * 40f + player.GetPosition());
				}
				roots[i].localRotation = Quaternion.Euler((Time.time - powerStartTime) * 720f, 90f, 0f);
				roots[i].localScale = Vector3.Lerp(Vector3.one, Vector3.one / 2f, num);
			}
			else
			{
				powerAttackEffect[i].transform.position = Vector3.Lerp(gunL.transform.position, getEndPos(player.GetTransform(), endPosL), num);
				if (powerEffectTargetL != null && powerEffectTargetL.GetTransform() != null && powerEffectTargetL.GetTransform().gameObject.activeSelf)
				{
					powerAttackEffect[i].transform.LookAt(powerEffectTargetL.GetTransform());
				}
				else
				{
					powerAttackEffect[i].transform.LookAt(player.GetTransform().forward * 40f + player.GetPosition());
				}
				roots[i].localRotation = Quaternion.Euler((Time.time - powerStartTime) * 720f, -90f, 0f);
				roots[i].localScale = Vector3.Lerp(Vector3.one, Vector3.one / 2f, num);
			}
		}
	}

	protected Vector3 getEndPos(Transform playerTrans, Vector3 localEndPos)
	{
		return playerTrans.position + localEndPos.x * playerTrans.right + localEndPos.y * playerTrans.up + localEndPos.z * playerTrans.forward;
	}

	public void PowerAttack()
	{
		if (isInUi || powerAttackEffect == null)
		{
			return;
		}
		for (int i = 0; i < powerAttackEffect.Length; i++)
		{
			if (powerAttackEffect[i] != null)
			{
				powerAttackEffect[i].SetActive(true);
				if (i == 0)
				{
					powerAttackEffect[i].transform.position = gun.transform.position;
					powerAttackEffect[i].transform.parent = player.GetTransform();
				}
				else
				{
					powerAttackEffect[i].transform.position = gunL.transform.position;
					powerAttackEffect[i].transform.parent = player.GetTransform();
				}
			}
		}
	}

	public void PowerAttackOld(bool isLeft)
	{
		Transform transform = gunfire;
		if (isLeft)
		{
			transform = gunfireL;
		}
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(transform, out direction);
		direction = (direction + Vector3.up * 0.4f).normalized;
		direction = ((!isLeft) ? (direction + transform.transform.right * 0.4f).normalized : (direction + -transform.transform.right * 0.4f).normalized);
		Vector3 moveDirection = player.InputController.inputInfo.moveDirection;
		moveDirection.y = 0f;
		Vector3 forward = player.GetTransform().forward;
		forward.y = 0f;
		double value = Mathf.Cos(Vector3.Angle(moveDirection, forward) * ((float)Math.PI / 180f)) * Global.MYTH_FIST_ACC_BY_MOVE;
		value = Math.Round(value, 1);
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.W31_ROCKET).CreateObject(transform.position, direction, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<AutoDestroyScript>().enabled = false;
		gameObject.GetComponent<FireLineScript>().enabled = false;
		ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
		component.enabled = true;
		component.isLocal = true;
		component.dir = direction;
		component.flySpeed = 20f;
		component.explodeRadius = 7f;
		component.hitForce = hitForce;
		component.life = 10f;
		component.damage = (int)damage * 10;
		component.GunType = WeaponType.ChargeDoublePistol;
		component.targetPos = aimTarget;
		component.weapon = this;
		component.isTrackingPos = true;
	}

	public override void CreateTrajectory()
	{
		lastAttackTime = Time.time;
		Transform transform = gunfire;
		if (mIsLeftFire)
		{
			transform = gunfireL;
		}
		transform.transform.rotation = player.GetTransform().rotation;
		transform.transform.rotation = Quaternion.AngleAxis(player.AngleV, -transform.transform.right) * transform.transform.rotation;
		Vector3 normalized = transform.transform.forward.normalized;
		if (isPower)
		{
			GameObject gameObject = ((!mIsLeftFire) ? powerFireLine : powerFireLineL);
			if (gameObject == null || !gameObject.activeSelf)
			{
				gameObject = gameWorld.GetEffectPool(EffectPoolType.W32_TRAJECTORY_02).CreateObject(transform.transform.position, normalized, Quaternion.identity);
				if (gameObject != null)
				{
					NightmareLaserScript component = gameObject.GetComponent<NightmareLaserScript>();
					Vector3 vector = gunfire.position + normalized * 80f;
					component.mIsLeftFire = mIsLeftFire;
					component.weapon = this;
					component.gunfire = transform;
					component.length = 80f;
					component.Init();
				}
				if (mIsLeftFire)
				{
					powerFireLineL = gameObject;
				}
				else
				{
					powerFireLine = gameObject;
				}
			}
			else
			{
				NightmareLaserScript component2 = gameObject.GetComponent<NightmareLaserScript>();
				component2.resetTime();
			}
		}
		else
		{
			GameObject gameObject2 = gameWorld.GetEffectPool(GetTrajectoryEffect()).CreateObject(transform.transform.position, normalized, Quaternion.identity);
			if (gameObject2 != null)
			{
				FireLineScript component3 = gameObject2.GetComponent<FireLineScript>();
				component3.beginPos = transform.position;
				component3.endPos = transform.position + normalized * 100f;
			}
			if (gunfireObj != null)
			{
				gunfireObj.SetActive(true);
			}
			if (gunfireObjL != null)
			{
				gunfireObjL.SetActive(true);
			}
		}
		mIsLeftFire = !mIsLeftFire;
	}

	public override void Attack(float deltaTime)
	{
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
		if (gunfireObjL != null)
		{
			gunfireObjL.SetActive(true);
		}
		Transform transform = gunfire;
		if (mIsLeftFire)
		{
			transform = gunfireL;
		}
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(transform, out direction);
		List<Ray> list = new List<Ray>();
		list.Add(weaponRay);
		BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, player) as BulletRay;
		if (bulletRay != null)
		{
			DamageProperty damageProperty = CreateDamageProperty();
			damageProperty.hitForce = weaponRay.direction * 2f;
			damageProperty.hitpoint = aimTarget;
			bulletRay.ShootDetect(damageProperty, list, 40, GetShootMark(), range, 0f, 1f, false);
		}
		if (isPower)
		{
			GameObject gameObject = ((!mIsLeftFire) ? powerFireLine : powerFireLineL);
			if (gameObject == null || !gameObject.activeSelf)
			{
				gameObject = gameWorld.GetEffectPool(EffectPoolType.W32_TRAJECTORY_02).CreateObject(transform.transform.position, direction, Quaternion.identity);
				if (gameObject != null)
				{
					NightmareLaserScript component = gameObject.GetComponent<NightmareLaserScript>();
					component.mIsLeftFire = mIsLeftFire;
					component.weapon = this;
					component.gunfire = transform;
					component.length = 80f;
					component.Init();
				}
				if (mIsLeftFire)
				{
					powerFireLineL = gameObject;
				}
				else
				{
					powerFireLine = gameObject;
				}
			}
			else
			{
				NightmareLaserScript component2 = gameObject.GetComponent<NightmareLaserScript>();
				component2.resetTime();
			}
		}
		else
		{
			GameObject gameObject2 = gameWorld.GetEffectPool(GetTrajectoryEffect()).CreateObject(transform.transform.position, direction, Quaternion.identity);
			if (gameObject2 != null)
			{
				FireLineScript component3 = gameObject2.GetComponent<FireLineScript>();
				component3.beginPos = transform.position;
				component3.endPos = transform.position + direction * 100f;
			}
		}
		attacked = false;
		lastAttackTime = Time.time;
		mIsLeftFire = !mIsLeftFire;
	}

	protected bool IsEffectAttackCoolDown()
	{
		if (player == null || !player.IsLocal())
		{
			return false;
		}
		if (Time.time - lastEffectAttackTime > base.AttackFrequency * 2f)
		{
			return true;
		}
		return false;
	}

	protected void EffectAttack(GameUnit weaponHitEnemy = null)
	{
		for (int i = 0; i < powerAttackEffect.Length; i++)
		{
			if (!(powerAttackEffect[i] != null))
			{
				continue;
			}
			GameUnit gameUnit = null;
			float distance = 15f;
			if (weaponHitEnemy == null)
			{
				gameUnit = GameWorld.FindEnemyTarget(powerAttackEffect[i], 15f);
			}
			else
			{
				gameUnit = weaponHitEnemy;
				distance = 100f;
			}
			if (gameUnit != null && (!gameUnit.UnitCanBeHitByRay(powerAttackEffect[i].transform.position, distance) || !gameUnit.InPlayingState()))
			{
				gameUnit = null;
			}
			if (gameUnit != null)
			{
				Vector3 normalized = (gameUnit.GetTransform().position - powerAttackEffect[i].transform.position).normalized;
				GameObject gameObject = ((i != 0) ? powerEffectFireLineL : powerEffectFireLine);
				if (gameObject == null || !gameObject.activeSelf)
				{
					gameObject = gameWorld.GetEffectPool(EffectPoolType.W32_TRAJECTORY_02).CreateObject(powerAttackEffect[i].transform.position, normalized, Quaternion.identity);
					if (gameObject != null)
					{
						NightmareLaserScript component = gameObject.GetComponent<NightmareLaserScript>();
						component.mIsLeftFire = i == 1;
						component.weapon = this;
						component.gunfire = powerAttackEffect[i].transform;
						component.length = 80f;
						component.Init();
						component.growing = false;
						component.m_Type = NightmareLaserScript.EFireLineType.POWER_EFFECT;
						component.targetTrans = gameUnit.GetTransform();
						component.targetGameUnit = gameUnit;
					}
				}
				else
				{
					NightmareLaserScript component2 = gameObject.GetComponent<NightmareLaserScript>();
					component2.resetTime();
					component2.targetTrans = gameUnit.GetTransform();
					component2.targetGameUnit = gameUnit;
				}
				if (i == 0)
				{
					powerEffectFireLine = gameObject;
					powerEffectTarget = gameUnit;
				}
				else
				{
					powerEffectFireLineL = gameObject;
					powerEffectTargetL = gameUnit;
				}
				lastEffectAttackTime = Time.time;
				noFoundEnemy = false;
				if (player == null || !player.IsLocal())
				{
					continue;
				}
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					int num = 0;
					if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
					{
						num = (gameUnit as Enemy).EnemyID;
						(gameUnit as Enemy).HitEnemy(player, CreateDamageProperty());
					}
					else if (gameUnit.GameUnitType == EGameUnitType.REMOTE_PLAYER)
					{
						num = (gameUnit as RemotePlayer).GetChannelID();
						PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(CreateDamageProperty(), num);
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
					PlayerNightmarePowerAttackRequest request2 = new PlayerNightmarePowerAttackRequest(2, i == 0, 0, num);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
				else if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
				{
					(gameUnit as Enemy).HitEnemy(player, CreateDamageProperty());
				}
			}
			else
			{
				if (i == 0)
				{
					powerEffectTarget = null;
				}
				else
				{
					powerEffectTargetL = null;
				}
				noFoundEnemy = true;
			}
		}
	}

	public void disconnectLaser(bool isLeft, NightmareLaserScript.EFireLineType type)
	{
		switch (type)
		{
		case NightmareLaserScript.EFireLineType.SUB:
			if (isLeft)
			{
				subPowerFireLineL = null;
			}
			else
			{
				subPowerFireLine = null;
			}
			break;
		case NightmareLaserScript.EFireLineType.POWER_EFFECT:
			if (isLeft)
			{
				powerEffectFireLineL = null;
			}
			else
			{
				powerEffectFireLine = null;
			}
			break;
		case NightmareLaserScript.EFireLineType.WEAPON:
			if (isLeft)
			{
				powerFireLineL = null;
			}
			else
			{
				powerFireLine = null;
			}
			break;
		}
	}

	public bool isAttacking()
	{
		if (isInUi)
		{
			return true;
		}
		if (player != null)
		{
			if (player.GetWeapon() != this)
			{
				return false;
			}
			return player.isAttacking();
		}
		return false;
	}

	public void setInfo(byte _charge, bool _isPower)
	{
		charge = _charge;
		isPower = _isPower;
		if (isPower)
		{
			PowerOnFromSever(charge);
		}
		updateEffect();
	}

	public void PlayVfxFromServer(byte type, bool isLeft, int beginID, int targetID)
	{
		if (!isPower)
		{
			isPower = true;
		}
		Transform transform = null;
		Transform transform2 = null;
		GameUnit targetGameUnit = null;
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			Enemy enemyByID = gameWorld.GetEnemyByID("E_" + beginID);
			if (enemyByID != null)
			{
				transform = enemyByID.GetTransform();
			}
			Enemy enemyByID2 = gameWorld.GetEnemyByID("E_" + targetID);
			if (enemyByID2 != null)
			{
				transform2 = enemyByID2.GetTransform();
			}
			targetGameUnit = enemyByID2;
		}
		else if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			Player player = gameWorld.GetRemotePlayerByUserID(beginID);
			if (player == null && gameWorld.GetLocalPlayer().GetChannelID() == beginID)
			{
				player = gameWorld.GetLocalPlayer();
			}
			if (player != null)
			{
				transform = player.GetTransform();
			}
			Player player2 = gameWorld.GetRemotePlayerByUserID(targetID);
			if (player2 == null && gameWorld.GetLocalPlayer().GetChannelID() == targetID)
			{
				player2 = gameWorld.GetLocalPlayer();
			}
			if (player2 != null)
			{
				transform2 = player2.GetTransform();
			}
			targetGameUnit = player2;
		}
		GameObject gameObject;
		switch (type)
		{
		case 1:
			gameObject = ((!isLeft) ? subPowerFireLine : subPowerFireLineL);
			break;
		case 2:
			if (powerAttackEffect == null)
			{
				return;
			}
			if (isLeft)
			{
				gameObject = powerEffectFireLineL;
				if (powerAttackEffect[1] == null)
				{
					return;
				}
				transform = powerAttackEffect[1].transform;
			}
			else
			{
				gameObject = powerEffectFireLine;
				if (powerAttackEffect[0] == null)
				{
					return;
				}
				transform = powerAttackEffect[0].transform;
			}
			break;
		default:
			return;
		}
		if (transform == null || transform2 == null)
		{
			return;
		}
		Vector3 normalized = (transform2.position - transform.position).normalized;
		if (gameObject == null || !gameObject.activeSelf)
		{
			gameObject = gameWorld.GetEffectPool(EffectPoolType.W32_TRAJECTORY_02).CreateObject(transform.position, normalized, Quaternion.identity);
			if (gameObject != null)
			{
				NightmareLaserScript component = gameObject.GetComponent<NightmareLaserScript>();
				component.mIsLeftFire = isLeft;
				component.weapon = this;
				component.gunfire = transform.transform;
				component.length = 80f;
				component.Init();
				component.growing = false;
				component.m_Type = (NightmareLaserScript.EFireLineType)type;
				component.targetTrans = transform2;
				component.targetGameUnit = targetGameUnit;
			}
		}
		else
		{
			NightmareLaserScript component2 = gameObject.GetComponent<NightmareLaserScript>();
			component2.resetTime();
			component2.targetTrans = transform2;
			component2.targetGameUnit = targetGameUnit;
		}
		switch (type)
		{
		case 1:
			if (isLeft)
			{
				subPowerFireLineL = gameObject;
			}
			else
			{
				subPowerFireLine = gameObject;
			}
			break;
		case 2:
			if (isLeft)
			{
				powerEffectFireLineL = gameObject;
			}
			else
			{
				powerEffectFireLine = gameObject;
			}
			break;
		}
	}

	public override void ClearEffect()
	{
		if (isPower)
		{
			charge = 0;
			PowerOff();
		}
		if (!GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		if (powerAttackEffect != null)
		{
			for (int i = 0; i < powerAttackEffect.Length; i++)
			{
				if (powerAttackEffect[i] != null)
				{
					UnityEngine.Object.Destroy(powerAttackEffect[i]);
				}
			}
		}
		powerAttackEffect = null;
	}

	public override void PlaySound()
	{
		if (!isPower && playSoundTimer.Ready())
		{
			AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_06_DoublePistol, player.GetTransform().position);
			playSoundTimer.Do();
		}
	}

	public override void StopSound()
	{
	}
}

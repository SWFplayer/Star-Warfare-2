using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyBaseScript : MonoBehaviour
{
	public int Distance = 5;

	public float Time = 3f;

	public List<GameObject> BombList;

	public Transform NormalExplostion;

	public Transform FinalExplostion;

	public GameObject BombFlash;

	public int ExplodeDamageForPlayer = 200;

	public int ExplodeDamageForEnemy = 10000;

	public float ExplodeRadiusForPlayer = 5f;

	public float ExplodeRadiusForEnemy = 20f;

	public float ExplodeDelay = 3f;

	private Timer mCheckTimer = new Timer();

	private bool mStay;

	private float mStayTime;

	private MissionBaseDestruction mMissionBaseDestruction;

	private bool mReadyToExplode;

	private float mStartExplodeTime;

	public bool Destroyed { get; set; }

	private void Start()
	{
		Debug.Log("MissionEnemyBaseScript Start");
		mCheckTimer.SetTimer(0.5f, false);
		mStay = false;
		mStayTime = 0f;
		Destroyed = false;
		mReadyToExplode = false;
	}

	private void Update()
	{
		if (mReadyToExplode)
		{
			AudioManager.GetInstance().PlaySoundSingle("_Old/_Audio/demo/TMX_grenade03");
			if (!(UnityEngine.Time.time - mStartExplodeTime > ExplodeDelay))
			{
				return;
			}
			mReadyToExplode = false;
			if (null != BombFlash)
			{
				BombFlash.SetActive(false);
			}
			Debug.Log("mCurrentBombId = " + mMissionBaseDestruction.GetBombNumber());
			if (mMissionBaseDestruction.GetBombNumber() >= BombList.Count)
			{
				Destroyed = true;
				GameObject original = Resources.Load("_Old/_Effect/Demo/Base_Final_Fire") as GameObject;
				Object.Instantiate(original, FinalExplostion.position, Quaternion.identity);
				GameObject original2 = Resources.Load("_Old/_Effect/Demo/Base_Final_Smoke") as GameObject;
				Object.Instantiate(original2, FinalExplostion.position, Quaternion.identity);
				AudioManager.GetInstance().PlaySound("_Old/_Audio/rpg/rpg-21_boom");
				GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
				if (gameWorld != null)
				{
					Player localPlayer = gameWorld.GetLocalPlayer();
					if (localPlayer != null && localPlayer.InPlayingState() && localPlayer.GetTransform() != null && (localPlayer.GetTransform().position - base.transform.position).sqrMagnitude < ExplodeRadiusForPlayer * ExplodeRadiusForPlayer)
					{
						int damage = Mathf.Clamp(ExplodeDamageForPlayer, 0, localPlayer.Hp - 1);
						localPlayer.OnHit(new DamageProperty(null, damage));
					}
				}
				DamageProperty damageProperty = new DamageProperty(null, ExplodeDamageForEnemy);
				damageProperty.criticalAttack = true;
				damageProperty.isLocal = true;
				damageProperty.wType = WeaponType.NoGun;
				damageProperty.damageType = PlayerDamageType.EXPLOSION;
				HashSet<Enemy> hashSet = new HashSet<Enemy>();
				Collider[] array = Physics.OverlapSphere(base.transform.position, ExplodeRadiusForEnemy, 1 << PhysicsLayer.ENEMY);
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					if (collider.gameObject.layer != PhysicsLayer.ENEMY)
					{
						continue;
					}
					GameObject rootObjectByCollider = Enemy.GetRootObjectByCollider(collider);
					if (rootObjectByCollider.tag == TagName.ENEMY_ROOT)
					{
						Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID(rootObjectByCollider.name);
						if (enemyByID != null && !hashSet.Contains(enemyByID))
						{
							damageProperty.hitpoint = enemyByID.GetTransform().position + Vector3.up * 2f;
							enemyByID.HitEnemy(gameWorld.GetLocalPlayer(), damageProperty);
							hashSet.Add(enemyByID);
						}
					}
				}
			}
			else
			{
				if (null != BombList[mMissionBaseDestruction.GetBombNumber()])
				{
					MissionManager.SetMissionGameObjectState(BombList[mMissionBaseDestruction.GetBombNumber()], true);
					mMissionBaseDestruction.AddMissionBombPoint(BombList[mMissionBaseDestruction.GetBombNumber()]);
					mMissionBaseDestruction.mEffectPosition = BombList[mMissionBaseDestruction.GetBombNumber()].transform.position;
				}
				GameObject original3 = Resources.Load("_Old/_Effect/Demo/Base_Normal_Exp") as GameObject;
				Object.Instantiate(original3, NormalExplostion.position, Quaternion.identity);
				GameObject original4 = Resources.Load("_Old/_Effect/Demo/Base_Final_Smoke") as GameObject;
				Object.Instantiate(original4, FinalExplostion.position, Quaternion.identity);
				AudioManager.GetInstance().PlaySound("_Old/_Audio/rpg/rpg-21_boom");
				GameWorld gameWorld2 = GameApp.GetInstance().GetGameWorld();
				if (gameWorld2 != null)
				{
					Player localPlayer2 = gameWorld2.GetLocalPlayer();
					if (localPlayer2 != null && localPlayer2.InPlayingState() && localPlayer2.GetTransform() != null && (localPlayer2.GetTransform().position - base.transform.position).sqrMagnitude < ExplodeRadiusForPlayer * ExplodeRadiusForPlayer)
					{
						localPlayer2.OnHit(new DamageProperty(null, ExplodeDamageForPlayer));
					}
				}
				DamageProperty damageProperty2 = new DamageProperty(null, ExplodeDamageForEnemy);
				damageProperty2.criticalAttack = true;
				damageProperty2.isLocal = true;
				damageProperty2.wType = WeaponType.NoGun;
				damageProperty2.damageType = PlayerDamageType.EXPLOSION;
				HashSet<Enemy> hashSet2 = new HashSet<Enemy>();
				Collider[] array3 = Physics.OverlapSphere(base.transform.position, ExplodeRadiusForEnemy, 1 << PhysicsLayer.ENEMY);
				Collider[] array4 = array3;
				foreach (Collider collider2 in array4)
				{
					if (collider2.gameObject.layer != PhysicsLayer.ENEMY)
					{
						continue;
					}
					GameObject rootObjectByCollider2 = Enemy.GetRootObjectByCollider(collider2);
					if (rootObjectByCollider2.tag == TagName.ENEMY_ROOT)
					{
						Enemy enemyByID2 = GameApp.GetInstance().GetGameWorld().GetEnemyByID(rootObjectByCollider2.name);
						if (enemyByID2 != null && !hashSet2.Contains(enemyByID2))
						{
							damageProperty2.hitpoint = enemyByID2.GetTransform().position + Vector3.up * 2f;
							enemyByID2.HitEnemy(gameWorld2.GetLocalPlayer(), damageProperty2);
							hashSet2.Add(enemyByID2);
						}
					}
				}
			}
			mMissionBaseDestruction.IncreaseBaseNumber();
			mMissionBaseDestruction.TriggerAction();
			MissionManager.GetInstance().SetLastRefreshTime();
			MissionManager.SetMissionGameObjectState(base.gameObject, false);
			GameApp.GetInstance().GetGameWorld().RemoveRadarPointByTransform(base.gameObject.transform);
		}
		else
		{
			if (!mCheckTimer.Ready())
			{
				return;
			}
			mCheckTimer.Do();
			GameWorld gameWorld3 = GameApp.GetInstance().GetGameWorld();
			if (gameWorld3 == null)
			{
				return;
			}
			Player localPlayer3 = gameWorld3.GetLocalPlayer();
			if (localPlayer3 != null && localPlayer3.InPlayingState() && null != localPlayer3.GetTransform() && (localPlayer3.GetTransform().position - base.transform.position).sqrMagnitude < (float)(Distance * Distance))
			{
				if (mStay)
				{
					mStayTime += mCheckTimer.GetInterval();
					MissionDetailsHUD.m_Instance.SetProgressBar(mStayTime / Time);
				}
				else
				{
					mStay = true;
					MissionDetailsHUD.m_Instance.SetProgressBarActive(true);
					MissionDetailsHUD.m_Instance.SetProgressBar(0f);
				}
				if (mStayTime > Time)
				{
					mReadyToExplode = true;
					mStartExplodeTime = UnityEngine.Time.time;
					MissionDetailsHUD.m_Instance.SetProgressBarActive(false);
					if (null != BombFlash)
					{
						BombFlash.SetActive(true);
					}
				}
			}
			else
			{
				mStay = false;
				mStayTime = 0f;
				MissionDetailsHUD.m_Instance.SetProgressBarActive(false);
			}
		}
	}

	public void SetMissionBaseDestruction(MissionBaseDestruction missionBaseDestruction)
	{
		mMissionBaseDestruction = missionBaseDestruction;
	}
}

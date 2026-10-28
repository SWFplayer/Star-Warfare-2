using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAim : MonoBehaviour
{
	public static StateAim mInstance;

	public UISprite m_Cross;

	public UISprite m_CrossHit;

	public UISprite m_CrossHitSwVoid;

	public UISprite ChargeBarSprite1;

	public UISprite LockonMark;

	public GameObject CrossObj;

	public Camera HUDCamera;

	public float[] AssistAimDistance;

	public float[] AssistAimToleranceX;

	public float AssistAimToleranceY = 0.15f;

	public float mCheckTargetCD = 0.2f;

	public Vector2 CrossAutoRecenterSpeed = new Vector2(0.1f, 0.05f);

	public TweenRotation CrossRotationTween;

	public TweenScale CrossScaleTween;

	public UISprite CrossNightmareL;

	public UISprite CrossNightmareR;

	public UISprite CrossNightmareLCharge;

	public UISprite CrossNightmareRCharge;

	private Vector3 mBaseScale = Vector3.one;

	private Vector3 mFireScale = Vector3.one;

	private Player player;

	private Timer mAimFireTimer = new Timer();

	private float mLastUpdateAimColorTime;

	private string crossSpriteName = "front star1";

	private string chargeSpriteName = "front star8_2";

	private string chargeInAimSpriteName = "front star8_3";

	private string nightmareChargeSpriteName = "front star15";

	private bool mNeedAimAssist = true;

	private bool mAimAssistToggle = true;

	private bool mHasAimAssisted;

	private Vector3 mAssistedAimPositionInViewport = default(Vector3);

	private GameUnit mAimAssistedTarget;

	private float mLastCheckTargetTime;

	private bool mPrepareToReplayCrossTween = true;

	private bool mNeedReplayCrossTween = true;

	public bool HasAimAssisted
	{
		get
		{
			return mHasAimAssisted;
		}
	}

	public Vector3 AssistedAimPositionInViewport
	{
		get
		{
			return mAssistedAimPositionInViewport;
		}
	}

	private void Awake()
	{
		mInstance = this;
		mBaseScale = m_Cross.transform.localScale;
		mFireScale = mBaseScale * 1.2f;
		mNeedAimAssist = true;
		mHasAimAssisted = false;
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (player != null)
		{
			ChangeWeapon(player.GetWeapon().AimSpriteString);
		}
		mAimFireTimer.SetTimer(0.3f, false);
		mLastUpdateAimColorTime = Time.time;
		LockonMark.gameObject.SetActive(true);
		SetAcitve(LockonMark, false);
		mNeedReplayCrossTween = true;
		mPrepareToReplayCrossTween = true;
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			mAimAssistToggle = player.GetUserState().GetPVPAutoAim();
		}
		else
		{
			mAimAssistToggle = player.GetUserState().GetPVEAutoAim();
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void RefreshToggle()
	{
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			mAimAssistToggle = player.GetUserState().GetPVPAutoAim();
		}
		else
		{
			mAimAssistToggle = player.GetUserState().GetPVEAutoAim();
		}
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (player != null)
			{
				ChangeWeapon(player.GetWeapon().AimSpriteString);
			}
		}
		if (player != null)
		{
			bool flag = false;
			if (player.GetWeapon() != null && player.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol)
			{
				if (player.inputController.inputInfo.fire)
				{
					CrossNightmareL.transform.localScale = mFireScale;
					CrossNightmareR.transform.localScale = mFireScale;
					CrossNightmareLCharge.transform.localScale = mFireScale;
					CrossNightmareRCharge.transform.localScale = mFireScale;
				}
				else
				{
					CrossNightmareL.transform.localScale = mBaseScale;
					CrossNightmareR.transform.localScale = mBaseScale;
					CrossNightmareLCharge.transform.localScale = mBaseScale;
					CrossNightmareRCharge.transform.localScale = mBaseScale;
				}
				flag = true;
			}
			if (player.GetWeapon().GetWeaponType() != WeaponType.Sniper && player.GetWeapon().GetWeaponType() != WeaponType.UltraSniper)
			{
				if (player.inputController.inputInfo.fire)
				{
					if (player.GetWeapon().AttackFrequency <= 0.4f)
					{
						m_Cross.transform.localScale = mFireScale;
					}
					else if (player.JustMakeAShoot)
					{
						player.JustMakeAShoot = false;
						mAimFireTimer.Do();
						m_Cross.transform.localScale = mFireScale;
					}
					else if (!mAimFireTimer.Ready())
					{
						m_Cross.transform.localScale = mFireScale;
					}
					else
					{
						m_Cross.transform.localScale = mBaseScale;
					}
				}
				else
				{
					m_Cross.transform.localScale = mBaseScale;
				}
			}
			else if (player.inputController.inputInfo.fire)
			{
				m_Cross.transform.localScale = mBaseScale;
			}
			else if (player.JustMakeAShoot)
			{
				player.JustMakeAShoot = false;
				mAimFireTimer.Do();
				m_Cross.transform.localScale = mFireScale;
			}
			else if (!mAimFireTimer.Ready())
			{
				m_Cross.transform.localScale = mFireScale;
			}
			else
			{
				m_Cross.transform.localScale = mBaseScale;
			}
			bool flag2 = false;
			mAimAssistedTarget = AssistAim(out mAssistedAimPositionInViewport);
			if (mAimAssistedTarget != null)
			{
				flag2 = true;
				Vector3 position = new Vector3(mAssistedAimPositionInViewport.x, mAssistedAimPositionInViewport.y, 4.1f);
				Vector3 position2 = HUDCamera.ViewportToWorldPoint(position);
				CrossObj.transform.position = position2;
				mHasAimAssisted = true;
				float num = Mathf.Max(Vector3.Distance(player.GetPosition(), mAimAssistedTarget.GetPosition()), 1f);
				float num2 = Mathf.Lerp(0.7f, 0.1f, (num - 5f) / 115f);
				LockonMark.transform.localScale = Vector3.one * num2;
			}
			else
			{
				mHasAimAssisted = false;
			}
			bool flag3 = false;
			flag3 = !IsActive(LockonMark) && mHasAimAssisted;
			if (IsActive(LockonMark) != mHasAimAssisted)
			{
				SetAcitve(LockonMark, mHasAimAssisted);
			}
			if (flag3)
			{
				LockonMark.GetComponent<TweenHeight>().Reset();
				LockonMark.GetComponent<TweenHeight>().PlayForward();
				LockonMark.GetComponent<TweenWidth>().Reset();
				LockonMark.GetComponent<TweenWidth>().PlayForward();
			}
			if (Time.time - mLastUpdateAimColorTime > 0.2f)
			{
				mLastUpdateAimColorTime = Time.time;
				ClearPlayerSign();
				if (mAimAssistedTarget != null)
				{
					if (mAimAssistedTarget is RemotePlayer)
					{
						(mAimAssistedTarget as RemotePlayer).ShowPlayerSignObject();
					}
				}
				else
				{
					CrossObj.transform.localPosition = Vector3.zero;
					Camera main = Camera.main;
					Transform transform = main.transform;
					ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
					Vector3 vector = main.ScreenToWorldPoint(new Vector3(component.ReticlePosition.x, (float)Screen.height - component.ReticlePosition.y, 0.1f));
					Vector3 normalized = (vector - transform.position).normalized;
					Ray ray = new Ray(transform.position + 1.8f * normalized, normalized);
					RaycastHit hitInfo;
					if (Physics.Raycast(ray, out hitInfo, 1000f, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.REMOTE_PLAYER) | (1 << PhysicsLayer.DESTRUCTIBLE)))
					{
						if ((!player.IsBoss && hitInfo.collider.gameObject.layer == PhysicsLayer.ENEMY) || (hitInfo.collider.gameObject.layer == PhysicsLayer.DESTRUCTIBLE && hitInfo.collider.transform.root.tag == TagName.FUNNEL) || (hitInfo.collider.gameObject.layer == PhysicsLayer.DESTRUCTIBLE && (hitInfo.collider.transform.parent.tag == TagName.DESTRUCTIBLE || hitInfo.collider.transform.root.tag == TagName.DESTRUCTIBLE)))
						{
							flag2 = true;
							if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer() && hitInfo.collider.gameObject.layer == PhysicsLayer.ENEMY)
							{
								foreach (RemotePlayer remotePlayer in GameApp.GetInstance().GetGameWorld().GetRemotePlayers())
								{
									if (remotePlayer.IsBoss)
									{
										remotePlayer.ShowPlayerSignObject();
										break;
									}
								}
							}
						}
						else if (hitInfo.collider.gameObject.layer == PhysicsLayer.REMOTE_PLAYER && GameApp.GetInstance().GetGameMode().IsVSMode())
						{
							int userID = 0;
							if (hitInfo.collider.gameObject.tag == TagName.PLAYER_ROOT)
							{
								userID = int.Parse(hitInfo.collider.gameObject.name);
							}
							else if (hitInfo.collider.gameObject.tag == TagName.VEHICLE_ROOT)
							{
								userID = 0;
								for (int i = 0; i < hitInfo.collider.gameObject.transform.childCount; i++)
								{
									if (hitInfo.collider.gameObject.transform.GetChild(i).tag == TagName.PLAYER_ROOT)
									{
										userID = int.Parse(hitInfo.collider.gameObject.transform.GetChild(i).name);
										break;
									}
								}
							}
							Player remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(userID);
							if (remotePlayerByUserID != null && !remotePlayerByUserID.IsSameTeam(player))
							{
								remotePlayerByUserID.ShowPlayerSignObject();
								flag2 = true;
							}
						}
					}
				}
				if (flag2)
				{
					if (flag)
					{
						if (CrossNightmareL.spriteName != crossSpriteName + "_1")
						{
							CrossNightmareL.spriteName = crossSpriteName + "_1";
							CrossNightmareR.spriteName = crossSpriteName + "_1";
							CrossNightmareLCharge.spriteName = nightmareChargeSpriteName + "_1";
							CrossNightmareRCharge.spriteName = nightmareChargeSpriteName + "_1";
							if (mPrepareToReplayCrossTween)
							{
								CrossNightmareL.MakePixelPerfect();
								CrossNightmareR.MakePixelPerfect();
								mNeedReplayCrossTween = true;
								mPrepareToReplayCrossTween = false;
							}
							if (mNeedReplayCrossTween)
							{
								mNeedReplayCrossTween = false;
							}
						}
					}
					else if (m_Cross.spriteName != crossSpriteName + "_1")
					{
						m_Cross.spriteName = crossSpriteName + "_1";
						ChargeBarSprite1.spriteName = chargeInAimSpriteName;
						if (mPrepareToReplayCrossTween)
						{
							m_Cross.MakePixelPerfect();
							mNeedReplayCrossTween = true;
							mPrepareToReplayCrossTween = false;
						}
						if (mNeedReplayCrossTween)
						{
							mNeedReplayCrossTween = false;
						}
					}
				}
				else if (flag)
				{
					if (CrossNightmareL.spriteName != crossSpriteName)
					{
						CrossNightmareL.spriteName = crossSpriteName;
						CrossNightmareR.spriteName = crossSpriteName;
						CrossNightmareLCharge.spriteName = nightmareChargeSpriteName;
						CrossNightmareRCharge.spriteName = nightmareChargeSpriteName;
						CrossNightmareL.MakePixelPerfect();
						CrossNightmareR.MakePixelPerfect();
						mPrepareToReplayCrossTween = true;
					}
				}
				else if (m_Cross.spriteName != crossSpriteName)
				{
					m_Cross.spriteName = crossSpriteName;
					ChargeBarSprite1.spriteName = chargeSpriteName;
					m_Cross.MakePixelPerfect();
					m_Cross.transform.localRotation = Quaternion.identity;
					mPrepareToReplayCrossTween = true;
				}
			}
		}
		if (player != null && player.GetWeapon().GetWeaponType() == WeaponType.RailGun)
		{
			RailGun railGun = player.GetWeapon() as RailGun;
			float energyPercentage = railGun.GetEnergyPercentage();
			ChargeBarSprite1.fillAmount = energyPercentage;
		}
		if (player != null && player.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol)
		{
			ChargeDoublePistol chargeDoublePistol = player.GetWeapon() as ChargeDoublePistol;
			float num3 = (float)(int)chargeDoublePistol.GetCharge() / 100f;
			CrossNightmareLCharge.fillAmount = 0.06f + num3 * 0.64f;
			CrossNightmareRCharge.fillAmount = 0.06f + num3 * 0.64f;
		}
	}

	public void Hide()
	{
		m_Cross.gameObject.SetActive(false);
		ChargeBarSprite1.fillAmount = 0f;
		ChargeBarSprite1.gameObject.SetActive(false);
	}

	public void Show()
	{
		m_Cross.gameObject.SetActive(true);
		ChargeBarSprite1.gameObject.SetActive(true);
	}

	private void ClearPlayerSign()
	{
		if (player == null || !GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		foreach (RemotePlayer remotePlayer in GameApp.GetInstance().GetGameWorld().GetRemotePlayers())
		{
			if (!remotePlayer.IsSameTeam(player))
			{
				remotePlayer.HidePlayerSignObject();
			}
		}
	}

	public void ChangeWeapon(string spriteName)
	{
		if (player != null && player.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol && !player.IsBoss && player.mVehicleState == EPlayerVehicleState.NONE)
		{
			CrossNightmareLCharge.gameObject.SetActive(true);
			CrossNightmareRCharge.gameObject.SetActive(true);
			CrossNightmareL.gameObject.SetActive(true);
			CrossNightmareR.gameObject.SetActive(true);
			m_Cross.gameObject.SetActive(false);
			crossSpriteName = spriteName;
			mNeedAimAssist = true;
			return;
		}
		CrossNightmareLCharge.gameObject.SetActive(false);
		CrossNightmareRCharge.gameObject.SetActive(false);
		CrossNightmareL.gameObject.SetActive(false);
		CrossNightmareR.gameObject.SetActive(false);
		m_Cross.gameObject.SetActive(true);
		if (player != null && player.GetWeapon().GetWeaponType() == WeaponType.RailGun)
		{
			ChargeBarSprite1.gameObject.SetActive(true);
		}
		else
		{
			ChargeBarSprite1.gameObject.SetActive(false);
		}
		if (player != null && player.mVehicleState == EPlayerVehicleState.NONE && (player.GetWeapon().GetWeaponType() == WeaponType.AssaultRifle || player.GetWeapon().GetWeaponType() == WeaponType.MachineGun || player.GetWeapon().GetWeaponType() == WeaponType.LaserRifle || player.GetWeapon().GetWeaponType() == WeaponType.DoublePistol || player.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol || player.GetWeapon().GetWeaponType() == WeaponType.RailGun || player.GetWeapon().GetWeaponType() == WeaponType.ShotGun || player.GetWeapon().GetWeaponType() == WeaponType.LaserShotGun))
		{
			mNeedAimAssist = true;
		}
		else
		{
			mNeedAimAssist = false;
		}
		crossSpriteName = spriteName;
		m_Cross.spriteName = crossSpriteName;
		m_Cross.MakePixelPerfect();
	}

	private GameUnit AssistAim(out Vector3 adsorbViewportPoint)
	{
		GameUnit result = null;
		adsorbViewportPoint = Vector3.zero;
		if (mNeedAimAssist && mAimAssistToggle)
		{
			float num = 999f;
			Camera main = Camera.main;
			if (Time.time - mLastCheckTargetTime < mCheckTargetCD && mAimAssistedTarget != null)
			{
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					RemotePlayer remotePlayer = mAimAssistedTarget as RemotePlayer;
					if (remotePlayer != null && remotePlayer.InPlayingState())
					{
						result = remotePlayer;
						adsorbViewportPoint = main.WorldToViewportPoint(remotePlayer.GetPosition() + Vector3.up * 1.2f);
						return result;
					}
				}
				else
				{
					Enemy enemy = mAimAssistedTarget as Enemy;
					if (enemy != null && enemy.InPlayingState())
					{
						result = enemy;
						adsorbViewportPoint = main.WorldToViewportPoint(enemy.GetColliderCenterPosition());
						return result;
					}
					Funnel funnel = mAimAssistedTarget as Funnel;
					if (funnel != null && funnel.InPlayingState())
					{
						result = funnel;
						adsorbViewportPoint = main.WorldToViewportPoint(funnel.GetPosition());
						return result;
					}
				}
			}
			if (GameApp.GetInstance().GetGameMode().IsVSMode() && !GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
				foreach (RemotePlayer item in remotePlayers)
				{
					if (!item.InPlayingState() || item.IsSameTeam(player))
					{
						continue;
					}
					float num2 = Mathf.Max(Vector3.Distance(player.GetPosition(), item.GetPosition()), 1f);
					bool flag = false;
					for (int i = 0; i < AssistAimDistance.Length; i++)
					{
						float num3 = AssistAimDistance[i];
						if (player.GetWeapon().GetWeaponType() == WeaponType.ShotGun || player.GetWeapon().GetWeaponType() == WeaponType.LaserShotGun)
						{
							num3 *= 0.5f;
						}
						if (num2 <= num3)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
					Vector3 vector = main.WorldToViewportPoint(item.GetPosition() + Vector3.up * 1.2f) - new Vector3(0.5f, 0.5f, 0f);
					if (!(vector.z > 0f))
					{
						continue;
					}
					for (int j = 0; j < AssistAimToleranceX.Length; j++)
					{
						if (!(Mathf.Abs(vector.x) <= AssistAimToleranceX[j]) || !(Mathf.Abs(vector.y) <= AssistAimToleranceY))
						{
							continue;
						}
						if (Mathf.Abs(vector.x) <= num)
						{
							Vector3 normalized = (item.GetPosition() + Vector3.up * 1.2f - main.transform.position).normalized;
							Ray ray = new Ray(main.transform.position - 1.8f * normalized, normalized);
							RaycastHit hitInfo;
							if (!Physics.Raycast(ray, out hitInfo, num2, (1 << PhysicsLayer.DEFAULT) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR)))
							{
								num = Mathf.Abs(vector.x);
								result = item;
								adsorbViewportPoint = main.WorldToViewportPoint(item.GetPosition() + Vector3.up * 1.2f);
								mLastCheckTargetTime = Time.time;
							}
						}
						break;
					}
				}
			}
			else
			{
				Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
				foreach (Enemy value in enemies.Values)
				{
					if (Enemy.IsBoss(value.EnemyType) || !value.InPlayingState())
					{
						continue;
					}
					float num4 = Mathf.Max(Vector3.Distance(player.GetPosition(), value.GetPosition()), 1f);
					bool flag2 = false;
					for (int k = 0; k < AssistAimDistance.Length; k++)
					{
						float num5 = AssistAimDistance[k];
						if (player.GetWeapon().GetWeaponType() == WeaponType.ShotGun || player.GetWeapon().GetWeaponType() == WeaponType.LaserShotGun)
						{
							num5 *= 0.5f;
						}
						if (num4 <= num5)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						continue;
					}
					Vector3 vector2 = main.WorldToViewportPoint(value.GetColliderCenterPosition()) - new Vector3(0.5f, 0.5f, 0f);
					if (!(vector2.z > 0f))
					{
						continue;
					}
					for (int l = 0; l < AssistAimToleranceX.Length; l++)
					{
						if (!(Mathf.Abs(vector2.x) <= AssistAimToleranceX[l]) || !(Mathf.Abs(vector2.y) <= AssistAimToleranceY))
						{
							continue;
						}
						if (Mathf.Abs(vector2.x) <= num)
						{
							Vector3 normalized2 = (value.GetColliderCenterPosition() - main.transform.position).normalized;
							Ray ray2 = new Ray(main.transform.position - 1.8f * normalized2, normalized2);
							RaycastHit hitInfo2;
							if (!Physics.Raycast(ray2, out hitInfo2, num4, (1 << PhysicsLayer.DEFAULT) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR)))
							{
								num = Mathf.Abs(vector2.x);
								result = value;
								adsorbViewportPoint = main.WorldToViewportPoint(value.GetColliderCenterPosition());
								mLastCheckTargetTime = Time.time;
							}
						}
						break;
					}
				}
				CrimsonQueen instance = CrimsonQueen.Instance;
				List<Funnel> list = new List<Funnel>();
				if (instance != null)
				{
					list = instance.GetRpgList();
				}
				foreach (Funnel item2 in list)
				{
					if (!item2.InPlayingState())
					{
						continue;
					}
					float num6 = Mathf.Max(Vector3.Distance(player.GetPosition(), item2.GetPosition()), 1f);
					bool flag3 = false;
					for (int m = 0; m < AssistAimDistance.Length; m++)
					{
						float num7 = AssistAimDistance[m];
						if (player.GetWeapon().GetWeaponType() == WeaponType.ShotGun || player.GetWeapon().GetWeaponType() == WeaponType.LaserShotGun)
						{
							num7 *= 0.5f;
						}
						if (num6 <= num7)
						{
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						continue;
					}
					Vector3 vector3 = main.WorldToViewportPoint(item2.GetPosition()) - new Vector3(0.5f, 0.5f, 0f);
					if (!(vector3.z > 0f))
					{
						continue;
					}
					for (int n = 0; n < AssistAimToleranceX.Length; n++)
					{
						if (!(Mathf.Abs(vector3.x) <= AssistAimToleranceX[n]) || !(Mathf.Abs(vector3.y) <= AssistAimToleranceY))
						{
							continue;
						}
						if (Mathf.Abs(vector3.x) <= num)
						{
							Vector3 normalized3 = (item2.GetPosition() - main.transform.position).normalized;
							Ray ray3 = new Ray(main.transform.position - 1.8f * normalized3, normalized3);
							RaycastHit hitInfo3;
							if (!Physics.Raycast(ray3, out hitInfo3, num6, (1 << PhysicsLayer.DEFAULT) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR)))
							{
								num = Mathf.Abs(vector3.x);
								result = item2;
								adsorbViewportPoint = main.WorldToViewportPoint(item2.GetPosition());
								mLastCheckTargetTime = Time.time;
							}
						}
						break;
					}
				}
			}
		}
		return result;
	}

	private void AutoRecenter()
	{
		if (mHasAimAssisted)
		{
			float num = 0f;
			float y = 0f;
			Vector2 vector = new Vector2(mAssistedAimPositionInViewport.x - 0.5f, mAssistedAimPositionInViewport.y - 0.5f);
			if (Mathf.Abs(vector.x) > 0.001f)
			{
				num = CrossAutoRecenterSpeed.x * Time.deltaTime * vector.x / Mathf.Abs(vector.x);
				y = Mathf.Abs(num) * vector.y / Mathf.Abs(vector.x);
			}
			else if (Mathf.Abs(vector.y) > 0.001f)
			{
				y = CrossAutoRecenterSpeed.y * Time.deltaTime * vector.y / Mathf.Abs(vector.y);
			}
			if (Mathf.Abs(player.InputController.CameraRotation.x) < 0.01f && Mathf.Abs(player.InputController.CameraRotation.y) < 0.01f)
			{
				player.InputController.CameraRotation = new Vector2(num, y);
			}
		}
	}

	private void SetAcitve(UISprite sprite, bool active)
	{
		if (!active)
		{
			sprite.transform.localPosition = new Vector3(-10000f, -10000f, 0f);
		}
		else
		{
			sprite.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	private bool IsActive(UISprite sprite)
	{
		return sprite.transform.localPosition.x > -9999f;
	}

	public void HitTarget(bool isSwVoidHit = false)
	{
		if (m_CrossHit != null)
		{
			m_CrossHit.gameObject.SetActive(true);
			m_CrossHit.GetComponent<TweenAlpha>().Reset();
			m_CrossHit.GetComponent<TweenAlpha>().PlayForward();
			if (isSwVoidHit)
			{
				m_CrossHitSwVoid.gameObject.SetActive(true);
				m_CrossHitSwVoid.GetComponent<TweenAlpha>().Reset();
				m_CrossHitSwVoid.GetComponent<TweenAlpha>().PlayForward();
			}
		}
	}

	public void LateUpdate()
	{
		if (player != null && player.GetWeapon() != null && player.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol)
		{
			m_Cross.gameObject.SetActive(false);
		}
	}

	public GameUnit GetAssistedTarget()
	{
		return mAimAssistedTarget;
	}
}

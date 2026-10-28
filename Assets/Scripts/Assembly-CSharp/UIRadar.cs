using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRadar : GameUI
{
	public static UIRadar mInstance;

	[SerializeField]
	public UIMiniMap m_UIMiniMap;

	[SerializeField]
	private UIMiniMapViewClipping m_UIMiniMapViewClipping;

	[SerializeField]
	private Vector2 m_PlayerViewField = new Vector2(200f, 200f);

	[SerializeField]
	private Vector3 m_WorldTopLeft = Vector3.zero;

	[SerializeField]
	private Vector3 m_WorldBottomRight = Vector3.zero;

	[SerializeField]
	private GameObject m_UIMiniMapPanel;

	[SerializeField]
	private UIScale m_MapScale;

	[SerializeField]
	private TweenPosition m_ScanningLine;

	[SerializeField]
	private GameObject m_MapContainer;

	[SerializeField]
	private bool m_IsForPauseMenu;

	public GameObject TargetEffects;

	public GameObject TargetEffectTemplate;

	public GameObject TargetIndication;

	public List<string> mTargetEffectKeys = new List<string>();

	private Transform mLocalPlayer;

	private string mLocalPlayerKey;

	private List<string> mEnemyKeyList = new List<string>();

	private bool mIsRefreshEnemy = true;

	private GameObject m_MapImage;

	public bool IsForPVP;

	private List<string> mRemotePlayerKeyList = new List<string>();

	private List<string> mFiredRemotePlayerList = new List<string>();

	private Dictionary<int, bool> mIsRemotePlayerOutofRadar = new Dictionary<int, bool>();

	public bool IsForBombStreak;

	public GameObject BombIcon;

	public GameObject BombButton;

	private float BombRadius = 1f;

	public short SkillId;

	public AdvancedSkill.EMissleType BombType;

	private bool mPreCalcFinish;

	private float mPreCalcZ;

	public bool IsForRebirth;

	public GameObject RebirthSelect;

	public UILabel mRebirthMithril;

	public UILabel m_Number;

	public GameObject RebirthButton;

	public GameObject UnabledButton;

	public GameObject ExitButton;

	private float rebirthTimer;

	private float rebirthAnimaStartTimer;

	private Vector3 mBaseScale;

	private Color mBaseColor;

	private bool bRebirthInit;

	private List<GameObject> respawnPoints = new List<GameObject>();

	private List<string> respawnPointsKeyList = new List<string>();

	public GameObject MutantTimeObj;

	public UILabel TimeLabel;

	private PointScript KOHPoint;

	private PointScript[] DominationPoints = new PointScript[3];

	private void Awake()
	{
		if (!m_IsForPauseMenu)
		{
			mInstance = this;
		}
		AddButtonListener(m_UIMiniMapPanel);
		if (IsForBombStreak)
		{
			UIEventListener uIEventListener = UIEventListener.Get(BombButton);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickThumb));
			BombButton.SetActive(false);
			BombIcon.SetActive(false);
			mPreCalcFinish = false;
		}
		if (IsForRebirth)
		{
			mBaseScale = m_Number.transform.localScale;
			mBaseColor = m_Number.color;
			mPreCalcFinish = false;
			RebirthSelect.SetActive(false);
			UIEventListener uIEventListener2 = UIEventListener.Get(ExitButton);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickThumb));
			UIEventListener uIEventListener3 = UIEventListener.Get(RebirthButton);
			uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClickThumb));
		}
	}

	private void OnDestroy()
	{
		if (m_MapImage != null)
		{
			MemoryManager.FreeNGUI(m_MapImage);
		}
		if (!m_IsForPauseMenu)
		{
			mInstance = null;
		}
	}

	private void OnDisable()
	{
		if (IsForBombStreak)
		{
			BombIcon.transform.localScale /= BombRadius;
			BombRadius = 1f;
		}
	}

	public void Reset()
	{
		mLocalPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetTransform();
	}

	private void Start()
	{
		if (m_ScanningLine != null)
		{
			m_ScanningLine.from = new Vector3(0f, m_PlayerViewField.y / 2f * 0.9f, 0f);
			m_ScanningLine.to = new Vector3(0f, (0f - m_PlayerViewField.y) / 2f * 0.9f, 0f);
		}
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			mLocalPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetTransform();
			if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetSkills()
				.GetSkill(SkillsType.RADAR_RANGE_ENLARGE) > 0f)
			{
				SetPowerUpRadar(true);
			}
			else
			{
				SetPowerUpRadar(false);
			}
		}
		else
		{
			mLocalPlayer = null;
		}
		mLocalPlayerKey = "Player";
		m_UIMiniMap.ActivateSpriteIfNot(mLocalPlayerKey, "Player");
		LevelConfig levelConfig = (GameApp.GetInstance().GetGameMode().IsCoopMode() ? ((GameApp.GetInstance().GetGameMode().ModePlay != Mode.BOSS_FINAL) ? GameApp.GetInstance().GetUserState().GetLevelConfig(GameApp.GetInstance().GetUserState().GetSelectedNetIndex(), 0) : GameApp.GetInstance().GetUserState().GetLevelConfig(GameApp.GetInstance().GetUserState().GetSelectedNetIndex(), Global.BOSS_FINAL_SUB_STAGE_ID)) : ((!GameApp.GetInstance().GetGameMode().IsVSMode()) ? GameApp.GetInstance().GetUserState().GetCurrentLevelConfig() : GameApp.GetInstance().GetUserState().GetVSLevelConfig(0, GameApp.GetInstance().GetUserState().GetSelectedNetIndex())));
		string text = "map11";
		GameObject gameObject = Resources.Load("UI/Common/MiniMaps/" + levelConfig.mapName) as GameObject;
		if (gameObject == null)
		{
			gameObject = Resources.Load("UI/Common/MiniMaps/" + text) as GameObject;
		}
		m_MapImage = UnityEngine.Object.Instantiate(gameObject) as GameObject;
		m_MapImage.SetActive(true);
		m_MapImage.transform.parent = m_MapContainer.transform;
		m_MapImage.transform.localPosition = Vector3.zero;
		m_MapImage.transform.localRotation = Quaternion.identity;
		m_MapImage.transform.localScale = Vector3.one;
		m_MapImage.GetComponent<UITexture>().height = (int)m_UIMiniMap.GetMapSize().y;
		m_MapImage.GetComponent<UITexture>().width = (int)m_UIMiniMap.GetMapSize().x;
		m_MapImage.GetComponent<UITexture>().alpha = 0.6f;
		m_WorldTopLeft = levelConfig.topLeft;
		m_WorldBottomRight = levelConfig.bottomRight;
		if (MutantTimeObj != null)
		{
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				MutantTimeObj.SetActive(true);
			}
			else
			{
				MutantTimeObj.SetActive(false);
			}
		}
	}

	protected override void OnUIUpdate()
	{
		base.OnUIUpdate();
		List<string> list = new List<string>();
		list.Add(mLocalPlayerKey);
		if (mLocalPlayer == null)
		{
			if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
			{
				mLocalPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.GetTransform();
				if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.GetSkills()
					.GetSkill(SkillsType.RADAR_RANGE_ENLARGE) > 0f)
				{
					SetPowerUpRadar(true);
				}
				else
				{
					SetPowerUpRadar(false);
				}
			}
			else
			{
				mLocalPlayer = null;
			}
		}
		if (mLocalPlayer != null)
		{
			MovePlayerTo(mLocalPlayer.position);
			RotatePlayerTo(mLocalPlayer.eulerAngles.y);
			if (GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				foreach (string mRemotePlayerKey in mRemotePlayerKeyList)
				{
					list.Add(mRemotePlayerKey);
				}
				mRemotePlayerKeyList.Clear();
				List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
				foreach (RemotePlayer item in remotePlayers)
				{
					if (item != null && item.State != Player.DEAD_STATE && !item.HasLeavedGame())
					{
						m_UIMiniMap.ActivateSpriteIfNot(item.GetChannelID().ToString(), "Partner");
						MoveSpriteTo(item.GetChannelID().ToString(), item.GetTransform().position, !m_IsForPauseMenu);
						m_UIMiniMap.UpdateSpriteAngle(item.GetChannelID().ToString(), item.GetTransform().eulerAngles.y);
						mRemotePlayerKeyList.Add(item.GetChannelID().ToString());
						if (!list.Contains(item.GetChannelID().ToString()))
						{
							list.Add(item.GetChannelID().ToString());
						}
					}
				}
			}
			List<RadarPoint> radarPointList = GameApp.GetInstance().GetGameWorld().GetRadarPointList();
			foreach (RadarPoint item2 in radarPointList)
			{
				string text = string.Empty;
				if (item2.Tag.Equals(TagName.MISSION_DESTINATION))
				{
					text = "QuestTarget";
				}
				else if (item2.Tag.Equals(TagName.MISSION_ENEMY_AREA))
				{
					text = "QuestEnemyArea";
				}
				else if (item2.Tag.Equals(TagName.MISSION_PROTECTED_ITEM))
				{
					text = "QuestProtect";
				}
				else if (item2.Tag.Equals(TagName.MISSION_BOMB))
				{
					text = "QuestBomb";
				}
				else if (item2.Tag.Equals(TagName.MISSION_ENEMY_BASE))
				{
					text = "QuestTarget";
				}
				else if (item2.Tag.Equals(TagName.PICKUP_VEHICLE))
				{
					text = "PickupVehicle";
				}
				else if (item2.Tag.Equals(TagName.PVP_DOMINATION_POINT))
				{
					text = "Point";
				}
				else if (item2.Tag.Equals(TagName.MISSION_ACTION_BOSS))
				{
					text = "QuestBoss";
				}
				if (text == "QuestEnemyArea")
				{
					MissionEnemiesAreaScript component = item2.Transform.gameObject.GetComponent<MissionEnemiesAreaScript>();
					if (null != component)
					{
						float scale = 2f * (float)component.Distance * m_UIMiniMap.GetMapSize().x / (m_WorldBottomRight.x - m_WorldTopLeft.x);
						m_UIMiniMap.ActivateSpriteIfNot(item2.Key, text, scale);
					}
				}
				else
				{
					m_UIMiniMap.ActivateSpriteIfNot(item2.Key, text);
				}
				if (!mTargetEffectKeys.Contains(item2.Key))
				{
					AddTargetEffect(item2.Key, item2.Transform.position, item2.NeedArrow);
				}
				MoveSpriteTo(item2.Key, item2.Transform.position, !m_IsForPauseMenu);
				list.Add(item2.Key);
			}
			if (RandomPickUpManagerScript.instance != null)
			{
				List<DynamicPickUpScript> availablePickupPoints = RandomPickUpManagerScript.instance.GetAvailablePickupPoints();
				foreach (DynamicPickUpScript item3 in availablePickupPoints)
				{
					string text2 = item3.name + item3.GetItemScript().name;
					m_UIMiniMap.ActivateSpriteIfNot(text2, "PickupPoint");
					MoveSpriteTo(text2, item3.GetItemScript().transform.position, false);
					list.Add(text2);
				}
			}
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_CTF_TDM)
			{
				bool flag = false;
				Vector3 point = default(Vector3);
				string spriteName = "14";
				if (GameApp.GetInstance().GetGameWorld().FlagObject != null)
				{
					flag = true;
					point = GameApp.GetInstance().GetGameWorld().FlagObject.transform.position;
					if (GameApp.GetInstance().GetGameWorld().LastFlagInPlayerID == GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
						.GetChannelID())
					{
						spriteName = ((GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
							.Team != TeamName.Blue) ? "12" : "13");
					}
					else
					{
						RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(GameApp.GetInstance().GetGameWorld().LastFlagInPlayerID);
						if (remotePlayerByUserID != null)
						{
							spriteName = ((remotePlayerByUserID.Team != TeamName.Blue) ? "12" : "13");
						}
					}
				}
				else if (GameApp.GetInstance().GetGameWorld().FlagInPlayerID == GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.GetChannelID())
				{
					flag = true;
					point = mLocalPlayer.position;
					spriteName = ((GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
						.Team != TeamName.Blue) ? "12" : "13");
				}
				else
				{
					RemotePlayer remotePlayerByUserID2 = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(GameApp.GetInstance().GetGameWorld().FlagInPlayerID);
					if (remotePlayerByUserID2 != null)
					{
						flag = true;
						point = remotePlayerByUserID2.GetPosition();
						spriteName = ((remotePlayerByUserID2.Team != TeamName.Blue) ? "12" : "13");
					}
				}
				if (flag)
				{
					m_UIMiniMap.ActivateSpriteIfNot("TheFlag", "Flag");
					MoveSpriteTo("TheFlag", point, !IsForBombStreak && !IsForRebirth);
					list.Add("TheFlag");
					m_UIMiniMap.ActivateSpriteIfNot("TheFlagEffect", "FlagEffect");
					MoveSpriteTo("TheFlagEffect", point, !IsForBombStreak && !IsForRebirth);
					list.Add("TheFlagEffect");
					m_UIMiniMap.GetSprite("TheFlag").spriteName = spriteName;
				}
			}
			if (IsForRebirth)
			{
				respawnPointsKeyList = new List<string>();
				Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
				if (!bRebirthInit)
				{
					rebirthAnimaStartTimer = 0f;
					rebirthTimer = 1f;
					bRebirthInit = true;
					GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.RESPAWN);
					GameObject[] array2 = array;
					foreach (GameObject gameObject in array2)
					{
						if (gameObject.name.StartsWith(ObjectNamePrefix.PLAYER_SPAWN_POINT))
						{
							respawnPoints.Add(gameObject);
						}
					}
				}
				if (localPlayer.State != Player.WAIT_VS_REBIRTH_STATE)
				{
					rebirthAnimaStartTimer = 0f;
					rebirthTimer = 0f;
					bRebirthInit = false;
					((HUDOnline)HUDBase.instance).HideRebirthPage();
					return;
				}
				for (int j = 0; j < respawnPoints.Count; j++)
				{
					string text3 = "RespawnPoint" + j;
					m_UIMiniMap.ActivateSpriteIfNot(text3, "RespawnPoint");
					MoveSpriteTo(text3, respawnPoints[j].transform.position, false);
					list.Add(text3);
					respawnPointsKeyList.Add(text3);
				}
				mRebirthMithril.text = StateRebirth.GetCurrentRebirthPrice().ToString();
				if (localPlayer.GetUserState().GetMithril() < StateRebirth.GetCurrentRebirthPrice())
				{
					RebirthButton.SetActive(false);
					UnabledButton.SetActive(true);
				}
				else
				{
					RebirthButton.SetActive(true);
					UnabledButton.SetActive(false);
				}
				Timer timer = localPlayer.GetRebirthTimer();
				float num = 1f;
				float timeSpan = timer.GetTimeSpan();
				if (timeSpan >= rebirthTimer)
				{
					rebirthAnimaStartTimer = timeSpan;
					rebirthTimer = timeSpan + 1f;
				}
				else
				{
					int num2 = (int)(6f - timeSpan);
					if (m_Number != null)
					{
						if (GameApp.GetInstance().GetGameMode().IsVSMode())
						{
							m_Number.text = localPlayer.GetVSRebirthRemainingTime().ToString();
						}
						else
						{
							m_Number.text = num2.ToString();
						}
					}
					num = 1f + (timeSpan - rebirthAnimaStartTimer);
					m_Number.gameObject.SetActive(true);
					m_Number.transform.localScale = new Vector3(mBaseScale.x * num, mBaseScale.y * num, 1f);
					m_Number.color = new Color(mBaseColor.r, mBaseColor.g, mBaseColor.b, mBaseColor.a + 1f - num);
				}
				if (!mPreCalcFinish)
				{
					mPreCalcZ = UICamera.currentCamera.WorldToScreenPoint(RebirthButton.transform.position).z;
					mPreCalcFinish = true;
				}
				Vector2 vector = UICamera.lastTouchPosition - new Vector2(Screen.width / 2, Screen.height / 2);
				if (-256f < vector.x && vector.x < 256f && -256f < vector.y && vector.y < 256f)
				{
					vector.x = Mathf.Clamp(vector.x, -256f, 256f);
					vector.y = Mathf.Clamp(vector.y, -256f, 256f);
					Vector3 vector2 = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(UICamera.lastTouchPosition.x, UICamera.lastTouchPosition.y, mPreCalcZ));
					string text4 = string.Empty;
					float num3 = float.MaxValue;
					for (int k = 0; k < respawnPointsKeyList.Count; k++)
					{
						if (m_UIMiniMap.GetSpriteWorldPosition(respawnPointsKeyList[k]) != null)
						{
							float sqrMagnitude = (m_UIMiniMap.GetSpriteWorldPosition(respawnPointsKeyList[k]).position - vector2).sqrMagnitude;
							if (sqrMagnitude < num3)
							{
								num3 = sqrMagnitude;
								text4 = respawnPointsKeyList[k];
							}
						}
					}
					if (text4 != string.Empty)
					{
						vector2 = m_UIMiniMap.GetSpriteWorldPosition(text4).position;
						RebirthSelect.transform.position = vector2;
					}
				}
				if (Input.GetButtonUp("Bomb"))
				{
					RebirthButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				}
			}
			if (IsForPVP)
			{
				List<RemotePlayer> remotePlayers2 = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
				foreach (RemotePlayer item4 in remotePlayers2)
				{
					if (item4 != null)
					{
						int channelID = item4.GetChannelID();
						if (mIsRemotePlayerOutofRadar.ContainsKey(channelID) && mIsRemotePlayerOutofRadar[channelID])
						{
							bool value = MoveSpriteTo(channelID.ToString(), item4.GetTransform().position, true);
							mIsRemotePlayerOutofRadar[channelID] = value;
						}
					}
				}
				if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.GetSkills()
					.GetSkill(SkillsType.RADAR_DISTURB) > 0f)
				{
					if (m_MapScale.gameObject.activeSelf)
					{
						m_MapScale.gameObject.SetActive(false);
					}
				}
				else
				{
					if (!m_MapScale.gameObject.activeSelf)
					{
						m_MapScale.gameObject.SetActive(true);
					}
					foreach (string mRemotePlayerKey2 in mRemotePlayerKeyList)
					{
						list.Add(mRemotePlayerKey2);
					}
					CheckPlayerFired();
				}
			}
			else
			{
				foreach (string mEnemyKey in mEnemyKeyList)
				{
					list.Add(mEnemyKey);
				}
			}
			if (m_ScanningLine.tweenFactor > 0.75f && mIsRefreshEnemy)
			{
				mIsRefreshEnemy = false;
				if (IsForPVP)
				{
					mRemotePlayerKeyList.Clear();
					mIsRemotePlayerOutofRadar.Clear();
					List<RemotePlayer> remotePlayers3 = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
					foreach (RemotePlayer item5 in remotePlayers3)
					{
						if (item5 == null || item5.State == Player.DEAD_STATE)
						{
							continue;
						}
						if (item5.IsSameTeam(GameApp.GetInstance().GetGameWorld().GetLocalPlayer()))
						{
							if (m_UIMiniMap.HasKeyAndType(item5.GetChannelID().ToString(), "EnemyPlayer") || m_UIMiniMap.HasKeyAndType(item5.GetChannelID().ToString(), "QuestBoss"))
							{
								m_UIMiniMap.RemoveSprite(item5.GetChannelID().ToString());
							}
							m_UIMiniMap.ActivateSpriteIfNot(item5.GetChannelID().ToString(), "Partner");
							bool value2 = MoveSpriteTo(item5.GetChannelID().ToString(), item5.GetTransform().position, !m_IsForPauseMenu);
							m_UIMiniMap.UpdateSpriteAngle(item5.GetChannelID().ToString(), item5.GetTransform().eulerAngles.y);
							mRemotePlayerKeyList.Add(item5.GetChannelID().ToString());
							if (!mIsRemotePlayerOutofRadar.ContainsKey(item5.GetChannelID()))
							{
								mIsRemotePlayerOutofRadar.Add(item5.GetChannelID(), value2);
							}
							else
							{
								mIsRemotePlayerOutofRadar[item5.GetChannelID()] = value2;
							}
							if (!list.Contains(item5.GetChannelID().ToString()))
							{
								list.Add(item5.GetChannelID().ToString());
							}
						}
						else if (IsForBombStreak || IsForRebirth || GameApp.GetInstance().GetGameMode().IsBossVSPlayer() || GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
							.GetSkills()
							.GetSkill(SkillsType.RADAR_ENHANCE) > 0f || item5.GetSkills().GetSkill(SkillsType.RADAR_FOCUS) > 0f || GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
							.GetRevengePlayerId() == item5.GetChannelID() || (item5.GetSkills().GetSkill(SkillsType.RADAR_INVISIBLE) <= 0f && mFiredRemotePlayerList.Contains(item5.GetChannelID().ToString())))
						{
							if (m_UIMiniMap.HasKeyAndType(item5.GetChannelID().ToString(), "Partner"))
							{
								m_UIMiniMap.RemoveSprite(item5.GetChannelID().ToString());
							}
							if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer() && item5.IsBoss)
							{
								if (m_UIMiniMap.HasKeyAndType(item5.GetChannelID().ToString(), "EnemyPlayer"))
								{
									m_UIMiniMap.RemoveSprite(item5.GetChannelID().ToString());
								}
								m_UIMiniMap.ActivateSpriteIfNot(item5.GetChannelID().ToString(), "QuestBoss");
							}
							else
							{
								if (m_UIMiniMap.HasKeyAndType(item5.GetChannelID().ToString(), "QuestBoss"))
								{
									m_UIMiniMap.RemoveSprite(item5.GetChannelID().ToString());
								}
								m_UIMiniMap.ActivateSpriteIfNot(item5.GetChannelID().ToString(), "EnemyPlayer");
							}
							bool value3 = MoveSpriteTo(item5.GetChannelID().ToString(), item5.GetTransform().position, !IsForBombStreak && !IsForRebirth);
							mRemotePlayerKeyList.Add(item5.GetChannelID().ToString());
							if (!mIsRemotePlayerOutofRadar.ContainsKey(item5.GetChannelID()))
							{
								mIsRemotePlayerOutofRadar.Add(item5.GetChannelID(), value3);
							}
							if (!list.Contains(item5.GetChannelID().ToString()))
							{
								list.Add(item5.GetChannelID().ToString());
							}
							else
							{
								mIsRemotePlayerOutofRadar[item5.GetChannelID()] = value3;
							}
						}
						else
						{
							m_UIMiniMap.RemoveSprite(item5.GetChannelID().ToString());
						}
					}
					mFiredRemotePlayerList.Clear();
				}
				else
				{
					mEnemyKeyList.Clear();
					Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
					foreach (Enemy value4 in enemies.Values)
					{
						if (value4 != null && (value4.InPlayingState() || value4.GetState() == Enemy.GRAVEBORN_STATE))
						{
							if (Enemy.IsBoss(value4.EnemyType))
							{
								m_UIMiniMap.ActivateSpriteIfNot(value4.Name, "Enemy", 80f);
							}
							else
							{
								m_UIMiniMap.ActivateSpriteIfNot(value4.Name, "Enemy");
							}
							MoveSpriteTo(value4.Name, value4.GetTransform().position, false);
							mEnemyKeyList.Add(value4.Name);
						}
					}
				}
			}
			else if (m_ScanningLine.tweenFactor < 0.75f && !mIsRefreshEnemy)
			{
				mIsRefreshEnemy = true;
			}
			if (IsForBombStreak)
			{
				if (BombIcon.activeSelf)
				{
					if (!mPreCalcFinish)
					{
						mPreCalcZ = UICamera.currentCamera.WorldToScreenPoint(BombButton.transform.position).z;
						mPreCalcFinish = true;
					}
					Vector2 vector3 = UICamera.lastTouchPosition - new Vector2(Screen.width / 2, Screen.height / 2);
					if (-256f < vector3.x && vector3.x < 256f && -256f < vector3.y && vector3.y < 256f)
					{
						vector3.x = Mathf.Clamp(vector3.x, -256f, 256f);
						vector3.y = Mathf.Clamp(vector3.y, -256f, 256f);
						Vector3 position = new Vector3(UICamera.lastTouchPosition.x, UICamera.lastTouchPosition.y, mPreCalcZ);
						BombIcon.transform.position = UICamera.currentCamera.ScreenToWorldPoint(position);
					}
				}
				if (Input.GetButtonUp("Bomb"))
				{
					Bomb();
				}
			}
		}
		SpecialForKOH_Domination(list);
		m_UIMiniMap.RemoveSpriteWithout(list);
		list.Clear();
		if (MutantTimeObj != null && GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			GiantBetrayalModeLogic giantBetrayalModeLogic = GameApp.GetInstance().GetGameWorld().VsBattleLogic as GiantBetrayalModeLogic;
			TimeLabel.text = GetMuntantMapTime(giantBetrayalModeLogic.GetMatchRemainingTime());
		}
	}

	private bool MoveSpriteTo(string key, Vector3 point, bool limited)
	{
		Vector2 vector = TransformWorldPointToLocalPoint(point);
		Vector2 vector2 = vector;
		if (limited)
		{
			vector2 = m_UIMiniMapViewClipping.LimitSpritePosInClipping(vector, m_MapScale.CurrScale());
		}
		m_UIMiniMap.UpdateSpritePos(key, vector2);
		if (vector2 != vector)
		{
			return true;
		}
		return false;
	}

	private void MovePlayerTo(Vector3 point)
	{
		Vector2 pos = TransformWorldPointToLocalPoint(point);
		m_UIMiniMap.UpdateSpritePos(mLocalPlayerKey, pos);
		if (!m_IsForPauseMenu)
		{
			m_UIMiniMapViewClipping.MoveClippingTo(pos);
		}
	}

	private void RotatePlayerTo(float angle)
	{
		m_UIMiniMap.UpdateSpriteAngle(mLocalPlayerKey, angle);
		if (!m_IsForPauseMenu)
		{
			m_UIMiniMapViewClipping.RotateClippingTo(angle);
		}
	}

	private Vector2 TransformWorldPointToLocalPoint(Vector3 worldPoint)
	{
		float num = (worldPoint.x - m_WorldTopLeft.x) / (m_WorldBottomRight.x - m_WorldTopLeft.x);
		float num2 = (worldPoint.z - m_WorldTopLeft.z) / (m_WorldBottomRight.z - m_WorldTopLeft.z);
		float x = m_UIMiniMap.GetMapSize().x * num + m_UIMiniMap.GetMapTopLeftPoint().x;
		float y = m_UIMiniMap.GetMapTopLeftPoint().y - m_UIMiniMap.GetMapSize().y * num2;
		return new Vector2(x, y);
	}

	private Vector3 TransformLocalPointToWorldPoint(Vector2 localPoint)
	{
		float num = (localPoint.x - m_UIMiniMap.GetMapTopLeftPoint().x) / m_UIMiniMap.GetMapSize().x;
		float num2 = (m_UIMiniMap.GetMapTopLeftPoint().y - localPoint.y) / m_UIMiniMap.GetMapSize().y;
		float x = num * (m_WorldBottomRight.x - m_WorldTopLeft.x) + m_WorldTopLeft.x;
		float z = num2 * (m_WorldBottomRight.z - m_WorldTopLeft.z) + m_WorldTopLeft.z;
		float y = Global.FLOORHEIGHT;
		Vector3 up = Vector3.up;
		Ray ray = new Ray(new Vector3(x, 500f, z), Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 1000f, 1 << PhysicsLayer.FLOOR))
		{
			y = hitInfo.point.y;
		}
		return new Vector3(x, y, z);
	}

	protected override void OnClickThumb(GameObject go)
	{
		base.OnClickThumb(go);
		if (go.Equals(m_UIMiniMapPanel.gameObject))
		{
			m_MapScale.NextScale();
			if (IsForBombStreak)
			{
				BombIcon.SetActive(true);
				BombButton.SetActive(true);
			}
			if (IsForRebirth)
			{
				RebirthSelect.SetActive(true);
				SetRebirthPositionIndex();
			}
		}
		else if (go == BombButton)
		{
			Bomb();
		}
		else if (go == RebirthButton)
		{
			SetRebirthPositionIndex();
		}
		else if (go == ExitButton && HUDBase.instance != null)
		{
			((HUDOnline)HUDBase.instance).HideRebirthPage();
			StateRebirth.needShowRebithObj = true;
		}
	}

	private void SetRebirthPositionIndex()
	{
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		localPlayer.SetNextRebirthIndex(GetNearestRespawnPointIndex());
	}

	private int GetNearestRespawnPointIndex()
	{
		Vector3 vector = TransformLocalPointToWorldPoint(new Vector2(RebirthSelect.transform.localPosition.x, RebirthSelect.transform.localPosition.y));
		float num = float.MaxValue;
		int index = 0;
		for (int i = 0; i < respawnPoints.Count; i++)
		{
			float sqrMagnitude = (respawnPoints[i].transform.position - vector).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				index = i;
				num = sqrMagnitude;
			}
		}
		List<GameObject> list = new List<GameObject>(GameObject.FindGameObjectsWithTag(TagName.RESPAWN));
		return list.IndexOf(respawnPoints[index]);
	}

	private void Bomb()
	{
		Vector3 vector = TransformLocalPointToWorldPoint(new Vector2(BombIcon.transform.localPosition.x, BombIcon.transform.localPosition.y));
		switch (BombType)
		{
		case AdvancedSkill.EMissleType.MISSLE:
		{
			SpawnObjectRequest request = new SpawnObjectRequest(2, vector);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			break;
		}
		case AdvancedSkill.EMissleType.SLOW_MISSLE:
		{
			SpawnObjectRequest request = new SpawnObjectRequest(4, vector, SkillId);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			break;
		}
		case AdvancedSkill.EMissleType.RANDOM_MISSLE:
		{
			List<Vector2> list = new List<Vector2>();
			list.Add(new Vector2(UnityEngine.Random.Range(0f, BombRadius), UnityEngine.Random.Range(0f, 360f)));
			for (int i = 1; i < 3; i++)
			{
				bool flag = false;
				Vector2 item = default(Vector2);
				while (!flag)
				{
					flag = true;
					item = new Vector2(UnityEngine.Random.Range(0f, BombRadius), UnityEngine.Random.Range(0f, 360f));
					for (int j = 0; j < i; j++)
					{
						float num = Mathf.Sqrt(Mathf.Pow(list[j].x, 2f) + Mathf.Pow(item.x, 2f) - 2f * list[j].x * item.x * Mathf.Cos(list[j].y - item.y));
						if (num < 20f)
						{
							flag = false;
						}
					}
				}
				list.Add(item);
			}
			foreach (Vector2 item2 in list)
			{
				Vector3 position = vector + new Vector3(item2.x * Mathf.Cos(item2.y), 0f, item2.x * Mathf.Sin(item2.y));
				SpawnObjectRequest request = new SpawnObjectRequest(2, position, SkillId);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			break;
		}
		case AdvancedSkill.EMissleType.TRIPLE_MISSLE:
		{
			SpawnObjectRequest request = new SpawnObjectRequest(5, vector, SkillId);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			break;
		}
		}
		base.gameObject.SetActive(false);
	}

	public void AddTargetEffect(string key, Vector3 position, bool NeedArrow)
	{
		if (!m_IsForPauseMenu)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(TargetEffectTemplate) as GameObject;
			gameObject.transform.parent = TargetEffects.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponent<RadarTargetEffect>().Init(key, position);
			if (NeedArrow)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(TargetIndication) as GameObject;
				gameObject2.transform.parent = TargetEffects.transform;
				gameObject2.GetComponent<IndicationEffect>().Init(key, position);
				GameApp.GetInstance().GetGameWorld().AddIndication(key, gameObject2.GetComponent<IndicationEffect>());
			}
			GameApp.GetInstance().GetGameWorld().AddRadarEffect(key, gameObject.GetComponent<RadarTargetEffect>());
			mTargetEffectKeys.Add(key);
		}
	}

	public void RefreshTargetEffect(string key, Vector3 position)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(TargetEffectTemplate) as GameObject;
		gameObject.transform.parent = TargetEffects.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<RadarTargetEffect>().Init(key, position);
	}

	private void CheckPlayerFired()
	{
		if (!IsForPVP)
		{
			return;
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (!mFiredRemotePlayerList.Contains(item.GetChannelID().ToString()) && item.isAttacking())
			{
				mFiredRemotePlayerList.Add(item.GetChannelID().ToString());
			}
		}
	}

	private void SetPowerUpRadar(bool isPowerUp)
	{
		if (IsForPVP)
		{
			if (isPowerUp)
			{
				m_ScanningLine.duration = 0.75f;
			}
			else
			{
				m_ScanningLine.duration = 1.5f;
			}
			if (!IsForBombStreak && !IsForRebirth)
			{
				m_MapScale.SetPowerUpScale(isPowerUp);
			}
		}
	}

	public void SetBombIconRadius(float radius)
	{
		BombRadius = radius;
		BombIcon.transform.localScale *= BombRadius;
	}

	public void SpecialForKOH_Domination(List<string> keyList)
	{
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_KOTH)
		{
			KOHPoint = (GameApp.GetInstance().GetGameWorld().VsBattleLogic as KOTHModeLogic).GetPoint();
			if (!(KOHPoint != null))
			{
				return;
			}
			TeamName capturingTeam = KOHPoint.GetCapturingTeam();
			if (capturingTeam != TeamName.TeamCount)
			{
				m_UIMiniMap.ActivateSpriteIfNot("PointEffect0", "PointEffect");
				MoveSpriteTo("PointEffect0", KOHPoint.transform.position, !IsForBombStreak && !IsForRebirth);
				keyList.Add("PointEffect0");
				m_UIMiniMap.ActivateSpriteIfNot("PointEffect0d", "PointEffectDelay");
				MoveSpriteTo("PointEffect0d", KOHPoint.transform.position, !IsForBombStreak && !IsForRebirth);
				keyList.Add("PointEffect0d");
				switch (capturingTeam)
				{
				case TeamName.Blue:
					m_UIMiniMap.GetSprite("PointEffect0").spriteName = "round";
					m_UIMiniMap.GetSprite("PointEffect0d").spriteName = "round1";
					break;
				case TeamName.Red:
					m_UIMiniMap.GetSprite("PointEffect0").spriteName = "round2";
					m_UIMiniMap.GetSprite("PointEffect0d").spriteName = "round3";
					break;
				}
			}
			if (KOHPoint.isNeutral)
			{
				m_UIMiniMap.GetSprite("Point0").spriteName = "9";
			}
			else if (KOHPoint.GetController() == TeamName.Blue)
			{
				m_UIMiniMap.GetSprite("Point0").spriteName = "7";
			}
			else
			{
				m_UIMiniMap.GetSprite("Point0").spriteName = "8";
			}
		}
		else
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay != Mode.Domination)
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				DominationPoints[i] = (GameApp.GetInstance().GetGameWorld().VsBattleLogic as DominationModeLogic).GetPoint(i);
				if (!(DominationPoints[i] != null))
				{
					continue;
				}
				TeamName capturingTeam2 = DominationPoints[i].GetCapturingTeam();
				if (capturingTeam2 != TeamName.TeamCount)
				{
					m_UIMiniMap.ActivateSpriteIfNot("PointEffect" + i, "PointEffect");
					MoveSpriteTo("PointEffect" + i, DominationPoints[i].transform.position, !IsForBombStreak && !IsForRebirth);
					keyList.Add("PointEffect" + i);
					m_UIMiniMap.ActivateSpriteIfNot("PointEffect" + i + "d", "PointEffectDelay");
					MoveSpriteTo("PointEffect" + i + "d", DominationPoints[i].transform.position, !IsForBombStreak && !IsForRebirth);
					keyList.Add("PointEffect" + i + "d");
					switch (capturingTeam2)
					{
					case TeamName.Blue:
						m_UIMiniMap.GetSprite("PointEffect" + i).spriteName = "round";
						m_UIMiniMap.GetSprite("PointEffect" + i + "d").spriteName = "round1";
						break;
					case TeamName.Red:
						m_UIMiniMap.GetSprite("PointEffect" + i).spriteName = "round2";
						m_UIMiniMap.GetSprite("PointEffect" + i + "d").spriteName = "round3";
						break;
					}
				}
				if (DominationPoints[i].isNeutral)
				{
					m_UIMiniMap.GetSprite("Point" + i).spriteName = "9";
				}
				else if (DominationPoints[i].GetController() == TeamName.Blue)
				{
					m_UIMiniMap.GetSprite("Point" + i).spriteName = "7";
				}
				else
				{
					m_UIMiniMap.GetSprite("Point" + i).spriteName = "8";
				}
			}
		}
	}

	public void DisturbRadarByStory()
	{
		if (m_MapContainer.activeSelf)
		{
			m_MapContainer.SetActive(false);
		}
	}

	public string GetMuntantMapTime(float restSeconds)
	{
		string empty = string.Empty;
		int num = Mathf.CeilToInt(restSeconds);
		if (num < 0)
		{
			return "OT";
		}
		int num2 = num / 60;
		empty = ((num2 < 10) ? ("0" + num2) : num2.ToString()) + ":";
		int num3 = num % 60;
		return empty + ((num3 < 10) ? ("0" + num3) : num3.ToString());
	}
}

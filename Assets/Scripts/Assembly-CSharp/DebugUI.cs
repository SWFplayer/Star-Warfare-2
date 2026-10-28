using UnityEngine;

public class DebugUI : MonoBehaviour
{
	public static DebugUI mInstance;

	private float mLastUpdateTime;

	public UILabel m_FPS;

	protected float frames;

	protected float updateInterval = 2f;

	protected float timeLeft;

	protected string fpsStr;

	protected float accum;

	public UILabel m_Memory;

	public UILabel m_Protocol;

	protected float protocolDisappearTime = 10f;

	protected float lastProtocolTime;

	public UILabel m_NetworkTime;

	public UILabel m_ActionCounter;

	public UILabel m_EnemyKilledCounter;

	public UILabel m_RunDistance;

	public UILabel m_Precision;

	public UILabel m_Difficulty;

	public UILabel m_Damage;

	public static string selectResolution = Screen.width + "X" + Screen.height + " ";

	private void Awake()
	{
		mInstance = this;
		m_FPS = base.gameObject.transform.Find("Camera/Anchor/Panel/Global/Label - FPS").gameObject.GetComponent<UILabel>();
		m_Memory = base.gameObject.transform.Find("Camera/Anchor/Panel/Global/Label - Memory").gameObject.GetComponent<UILabel>();
		m_Protocol = base.gameObject.transform.Find("Camera/Anchor/Panel/Global/Label - Protocol").gameObject.GetComponent<UILabel>();
		m_NetworkTime = base.gameObject.transform.Find("Camera/Anchor/Panel/Global/Label - NetworkTime").gameObject.GetComponent<UILabel>();
		m_Damage = base.gameObject.transform.Find("Camera/Anchor/Panel/Global/Label - Damage").gameObject.GetComponent<UILabel>();
		m_Difficulty = base.gameObject.transform.Find("Camera/Anchor/Panel/Mission/Label - Difficulty").gameObject.GetComponent<UILabel>();
		m_EnemyKilledCounter = base.gameObject.transform.Find("Camera/Anchor/Panel/Mission/Label - EnemyKilled").gameObject.GetComponent<UILabel>();
		m_ActionCounter = base.gameObject.transform.Find("Camera/Anchor/Panel/Mission/Label - NextAction").gameObject.GetComponent<UILabel>();
		m_Precision = base.gameObject.transform.Find("Camera/Anchor/Panel/Mission/Label - Precision").gameObject.GetComponent<UILabel>();
		m_RunDistance = base.gameObject.transform.Find("Camera/Anchor/Panel/Mission/Label - RunDistance").gameObject.GetComponent<UILabel>();
	}

	private void Update()
	{
		if (!Global.ENABLE_DEBUG_UI)
		{
			return;
		}
		if (m_FPS != null && m_FPS.gameObject.activeSelf)
		{
			timeLeft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			frames++;
			if (timeLeft <= 0f)
			{
				fpsStr = "FPS: " + accum / frames;
				frames = 0f;
				accum = 0f;
				timeLeft = updateInterval;
			}
		}
		if (!(Time.time - mLastUpdateTime > 0.2f))
		{
			return;
		}
		mLastUpdateTime = Time.time;
		if (m_FPS != null && m_FPS.gameObject.activeSelf)
		{
			m_FPS.text = fpsStr;
		}
		if (!(m_Memory != null) || m_Memory.gameObject.activeSelf)
		{
		}
		if (m_Protocol != null && m_Protocol.gameObject.activeSelf && Time.time - lastProtocolTime > protocolDisappearTime)
		{
			m_Protocol.text = string.Empty;
		}
		if (m_NetworkTime != null && m_NetworkTime.gameObject.activeSelf && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			m_NetworkTime.text = "NetWorkTime: " + TimeManager.GetInstance().NetworkTime;
		}
		if (m_Difficulty != null && m_Difficulty.gameObject.activeSelf)
		{
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			if (gameWorld != null)
			{
				m_Difficulty.text = "Difficulty: " + gameWorld.DifficultyLevel;
			}
		}
		if (m_ActionCounter != null && m_ActionCounter.gameObject.activeSelf && MissionManager.GetInstance().IsInited())
		{
			Mission currentStageMission = MissionManager.GetInstance().GetCurrentStageMission(0);
			if (currentStageMission != null)
			{
				m_ActionCounter.text = "NextAction: " + currentStageMission.ActionCounter;
			}
		}
		if ((m_EnemyKilledCounter != null && m_EnemyKilledCounter.gameObject.activeSelf) || m_RunDistance.gameObject.activeSelf || m_Precision.gameObject.activeSelf)
		{
			GameWorld gameWorld2 = GameApp.GetInstance().GetGameWorld();
			if (gameWorld2 != null)
			{
				LocalPlayer localPlayer = gameWorld2.GetLocalPlayer();
				if (localPlayer != null)
				{
					if (m_EnemyKilledCounter.gameObject.activeSelf)
					{
						m_EnemyKilledCounter.text = "EnemyKilled: " + localPlayer.Kills;
					}
					if (m_RunDistance.gameObject.activeSelf)
					{
						m_RunDistance.text = "RunDistance: " + string.Format("{0:f2}", localPlayer.GetTotalMoveDistance());
					}
					if (m_Precision.gameObject.activeSelf)
					{
						m_Precision.text = "Precision: " + string.Format("{0:f2}%", (localPlayer.GetAttackCount() <= 0) ? 0f : ((float)localPlayer.GetHitEnemyCount() * 100f / (float)localPlayer.GetAttackCount()));
					}
				}
			}
		}
		if (!(m_Damage != null) || !m_Damage.gameObject.activeSelf)
		{
			return;
		}
		GameWorld gameWorld3 = GameApp.GetInstance().GetGameWorld();
		if (gameWorld3 == null)
		{
			return;
		}
		LocalPlayer localPlayer2 = gameWorld3.GetLocalPlayer();
		if (localPlayer2 != null)
		{
			Weapon weapon = localPlayer2.GetWeapon();
			if (weapon != null)
			{
				m_Damage.text = weapon.Name + ": " + weapon.Damage;
			}
		}
	}

	public void SetProtocol(string url)
	{
		if (Global.ENABLE_DEBUG_UI)
		{
			lastProtocolTime = Time.time;
			if (m_Protocol != null && m_Protocol.gameObject.activeSelf)
			{
				m_Protocol.text = url;
			}
		}
	}

	private void OnClickTestButton1(GameObject go)
	{
	}

	private void OnGUI()
	{
		if (!Global.ENABLE_DEBUG_UI)
		{
		}
	}
}

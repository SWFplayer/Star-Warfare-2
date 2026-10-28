using System.Collections.Generic;
using UnityEngine;

public class PointScript : MonoBehaviour
{
	public enum EPlayerCacthPointState
	{
		NOT_YET_IN = 0,
		HAD_IN = 1,
		NOW_IN = 2
	}

	public int SqrDistance = 25;

	public byte Id;

	public float CheckTimeInterval = 0.1f;

	private Timer mCheckTimer = new Timer();

	public float mBalance;

	private byte[] playerCount;

	private TeamName mController;

	public bool isNeutral;

	private byte attackNeutralTeam;

	private static float PointControllerBaseSpeed = 1f;

	public static float PointControlNeedValue = 10f;

	private VsModeLogic mModeLogic;

	private bool inlocalPlayerIn;

	private Dictionary<int, EPlayerCacthPointState> catchPointMenber = new Dictionary<int, EPlayerCacthPointState>();

	private bool timeRun;

	private bool isDanger;

	private GameObject mEffect;

	private void Awake()
	{
		mBalance = 0f;
		playerCount = new byte[2];
		isNeutral = true;
		attackNeutralTeam = 2;
	}

	private void Start()
	{
		mCheckTimer.SetTimer(CheckTimeInterval, false);
		catchPointMenber = new Dictionary<int, EPlayerCacthPointState>();
		timeRun = true;
		Object original = Resources.Load(EffectName.CTF_HIGH_TOWER);
		mEffect = Object.Instantiate(original, base.transform.position, Quaternion.identity) as GameObject;
		ResetEffectColor();
	}

	private void Update()
	{
		if (mModeLogic == null || !mModeLogic.isTimeRun)
		{
			return;
		}
		if (mCheckTimer.Ready())
		{
			mCheckTimer.Do();
			playerCount[0] = 0;
			playerCount[1] = 0;
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			if (gameWorld != null)
			{
				Player localPlayer = gameWorld.GetLocalPlayer();
				if (localPlayer != null && null != localPlayer.GetTransform() && localPlayer.State != null && localPlayer.InPlayingState() && (localPlayer.GetTransform().position - base.transform.position).sqrMagnitude < (float)SqrDistance)
				{
					playerCount[(int)localPlayer.Team]++;
					MenberCatchPoint(localPlayer.GetChannelID());
					CheckLocalPlayerInOut(true);
				}
				else
				{
					MenberNotCatchPoint(localPlayer.GetChannelID());
					CheckLocalPlayerInOut(false);
				}
				foreach (RemotePlayer remotePlayer in gameWorld.GetRemotePlayers())
				{
					if (remotePlayer != null && null != remotePlayer.GetTransform() && remotePlayer.State != null && remotePlayer.InPlayingState() && (remotePlayer.GetTransform().position - base.transform.position).sqrMagnitude < (float)SqrDistance)
					{
						playerCount[(int)remotePlayer.Team]++;
						MenberCatchPoint(remotePlayer.GetChannelID());
					}
					else
					{
						MenberNotCatchPoint(remotePlayer.GetChannelID());
					}
				}
			}
		}
		UpdateBalance();
	}

	private void UpdateBalance()
	{
		if (playerCount[(int)GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.Team] >= 6)
		{
			GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_6InPoint();
		}
		if (isNeutral)
		{
			if (playerCount[0] == 0)
			{
				if (playerCount[1] == 0)
				{
					BalanceTo(0f, PointControllerBaseSpeed * 0.5f);
					return;
				}
				if (attackNeutralTeam == 0)
				{
					mBalance = 0f - mBalance;
				}
				BalanceTo(PointControlNeedValue, PointControllerBaseSpeed * ((float)(int)playerCount[1] * 0.5f + 0.5f));
				attackNeutralTeam = 1;
				CheckChangeController(TeamName.Red);
				SetEffectColor(TeamName.Red, mBalance / 10f);
			}
			else if (playerCount[1] == 0)
			{
				if (attackNeutralTeam == 1)
				{
					mBalance = 0f - mBalance;
				}
				BalanceTo(PointControlNeedValue, PointControllerBaseSpeed * ((float)(int)playerCount[0] * 0.5f + 0.5f));
				attackNeutralTeam = 0;
				CheckChangeController(TeamName.Blue);
				SetEffectColor(TeamName.Blue, mBalance / 10f);
			}
			return;
		}
		if (playerCount[(int)(1 - mController)] == 0)
		{
			BalanceTo(0f, PointControllerBaseSpeed * 0.5f);
			if (isDanger && mBalance == 0f && mController == GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.Team)
			{
				GameApp.GetInstance().GetUserState().Achievement.AddDefenceNum();
				isDanger = false;
			}
		}
		else if (playerCount[(int)mController] == 0)
		{
			BalanceTo(PointControlNeedValue, PointControllerBaseSpeed * ((float)(int)playerCount[(int)(1 - mController)] * 0.5f + 0.5f));
			CheckChangeController(1 - mController);
			if (mController == GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.Team)
			{
				isDanger = true;
			}
		}
		if (mController == TeamName.Blue)
		{
			SetEffectColor(1f - mBalance / 10f);
		}
		else
		{
			SetEffectColor(mBalance / 10f);
		}
	}

	private void BalanceTo(float targetNumber, float speed)
	{
		speed *= Time.deltaTime;
		if (mBalance > targetNumber)
		{
			mBalance -= speed;
			if (mBalance < targetNumber)
			{
				mBalance = targetNumber;
			}
		}
		else if (mBalance < targetNumber)
		{
			mBalance += speed;
			if (mBalance > targetNumber)
			{
				mBalance = targetNumber;
			}
		}
		if (targetNumber == 0f && mBalance == 0f)
		{
			ClearCatchPointMenber();
		}
	}

	private void CheckChangeController(TeamName attackTeam)
	{
		if (mBalance.Equals(PointControlNeedValue) && timeRun && Lobby.GetInstance().IsMasterPlayer)
		{
			ChangePointControllerRequest request = new ChangePointControllerRequest(Id, (byte)attackTeam, catchPointMenber);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			timeRun = false;
		}
	}

	public void SetModeLogic(VsModeLogic modeLogic)
	{
		mModeLogic = modeLogic;
	}

	private void MenberCatchPoint(int playerId)
	{
		if (!catchPointMenber.ContainsKey(playerId))
		{
			catchPointMenber.Add(playerId, EPlayerCacthPointState.NOW_IN);
		}
		else
		{
			catchPointMenber[playerId] = EPlayerCacthPointState.NOW_IN;
		}
	}

	private void MenberNotCatchPoint(int playerId)
	{
		if (!catchPointMenber.ContainsKey(playerId))
		{
			catchPointMenber.Add(playerId, EPlayerCacthPointState.NOT_YET_IN);
		}
		else if (catchPointMenber[playerId] == EPlayerCacthPointState.NOW_IN)
		{
			catchPointMenber[playerId] = EPlayerCacthPointState.HAD_IN;
		}
	}

	public void ClearCatchPointMenber()
	{
		catchPointMenber.Clear();
	}

	private void CheckLocalPlayerInOut(bool isIn)
	{
		if (inlocalPlayerIn != isIn)
		{
			SendPlayerInOutPointRequest request = new SendPlayerInOutPointRequest(isIn, Id);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			if (isIn)
			{
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.GetSkills()
					.PlayerInPoint();
			}
			else
			{
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.GetSkills()
					.PlayerOutPoint();
			}
			if (isIn && mController != GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.Team)
			{
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.InEnemyPoint = true;
			}
			else
			{
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.InEnemyPoint = false;
			}
			inlocalPlayerIn = isIn;
		}
	}

	public void SetController(TeamName team)
	{
		mController = team;
		ResetEffectColor(team);
	}

	public byte GetAttackNetualTeam()
	{
		return attackNeutralTeam;
	}

	public TeamName GetController()
	{
		return mController;
	}

	public void SetTimeRun()
	{
		timeRun = true;
	}

	public void SetEffectColor(float blueRate)
	{
		if (mEffect != null)
		{
			mEffect.transform.Find("ctf_hightower_fx").GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(Color.red, Color.blue, blueRate));
		}
	}

	public void SetEffectColor(TeamName team, float rate)
	{
		if (mEffect != null)
		{
			switch (team)
			{
			case TeamName.Blue:
				mEffect.transform.Find("ctf_hightower_fx").GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(Color.white, Color.blue, rate));
				break;
			case TeamName.Red:
				mEffect.transform.Find("ctf_hightower_fx").GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(Color.white, Color.red, rate));
				break;
			}
		}
	}

	public void ResetEffectColor()
	{
		if (mEffect != null)
		{
			mEffect.transform.Find("ctf_hightower_fx").GetComponent<Renderer>().material.SetColor("_Color", Color.white);
		}
	}

	public void ResetEffectColor(TeamName team)
	{
		if (!isNeutral)
		{
			if (mEffect != null)
			{
				switch (team)
				{
				case TeamName.Blue:
					mEffect.transform.Find("ctf_hightower_fx").GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
					break;
				case TeamName.Red:
					mEffect.transform.Find("ctf_hightower_fx").GetComponent<Renderer>().material.SetColor("_Color", Color.red);
					break;
				}
			}
		}
		else if (mEffect != null)
		{
			mEffect.transform.Find("ctf_hightower_fx").GetComponent<Renderer>().material.SetColor("_Color", Color.white);
		}
	}

	public bool IsCapturing()
	{
		if (isNeutral)
		{
			if (playerCount[0] > 0 || playerCount[1] > 0)
			{
				return true;
			}
		}
		else if (playerCount[(int)(1 - mController)] != 0)
		{
			return true;
		}
		return false;
	}

	public TeamName GetCapturingTeam()
	{
		if (IsCapturing())
		{
			if (isNeutral)
			{
				return (TeamName)attackNeutralTeam;
			}
			return 1 - mController;
		}
		return TeamName.TeamCount;
	}
}

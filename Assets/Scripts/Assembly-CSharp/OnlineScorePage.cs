using System;
using System.Collections.Generic;
using UnityEngine;

public class OnlineScorePage : MonoBehaviour
{
	public UIGrid ScoreList;

	public PlayerScoreScript ScoreTemplate;

	public UILabel ModeDiffTitle;

	public bool IsForResultPage;

	private float mRefreshTime = 0.3f;

	private float mLastRefreshTime;

	private Dictionary<int, PlayerScoreScript> mPlayerScores = new Dictionary<int, PlayerScoreScript>();

	private List<Player> mPlayers = new List<Player>();

	private PlayerScoreScript mSelectPlayerScore;

	private void OnEnable()
	{
		mLastRefreshTime = Time.time;
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			Player localPlayer = gameWorld.GetLocalPlayer();
			if (!mPlayerScores.ContainsKey(localPlayer.GetChannelID()))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(ScoreTemplate.gameObject) as GameObject;
				gameObject.transform.parent = ScoreList.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				PlayerScoreScript component = gameObject.GetComponent<PlayerScoreScript>();
				component.Init(localPlayer);
				mPlayerScores.Add(localPlayer.GetChannelID(), component);
			}
			foreach (RemotePlayer remotePlayer in gameWorld.GetRemotePlayers())
			{
				if (!mPlayerScores.ContainsKey(remotePlayer.GetChannelID()) && remotePlayer != null && (remotePlayer as RemotePlayer).GetUserState() != null)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(ScoreTemplate.gameObject) as GameObject;
					gameObject2.transform.parent = ScoreList.transform;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.identity;
					gameObject2.transform.localScale = Vector3.one;
					PlayerScoreScript component2 = gameObject2.GetComponent<PlayerScoreScript>();
					component2.Init(remotePlayer);
					mPlayerScores.Add(remotePlayer.GetChannelID(), component2);
				}
			}
			ScoreList.Reposition();
		}
		if (IsForResultPage)
		{
			OnUpdate();
			foreach (PlayerScoreScript value in mPlayerScores.Values)
			{
				UIEventListener uIEventListener = UIEventListener.Get(value.gameObject);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			}
		}
		if (ModeDiffTitle != null)
		{
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				ModeDiffTitle.text = "DAMAGE";
			}
			else
			{
				ModeDiffTitle.text = "OCCUPY";
			}
		}
	}

	private void OnDisable()
	{
		if (!IsForResultPage)
		{
			return;
		}
		OnUpdate();
		foreach (PlayerScoreScript value in mPlayerScores.Values)
		{
			UIEventListener uIEventListener = UIEventListener.Get(value.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		}
	}

	private void Update()
	{
		if (!IsForResultPage && Time.time - mLastRefreshTime > mRefreshTime)
		{
			mLastRefreshTime = Time.time;
			OnUpdate();
		}
	}

	protected void OnUpdate()
	{
		mPlayers.Clear();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Sort(gameWorld.GetLocalPlayer());
		List<int> list = new List<int>();
		foreach (int key in mPlayerScores.Keys)
		{
			if (key != gameWorld.GetLocalPlayer().GetChannelID() && !gameWorld.RemotePlayerExists(key))
			{
				list.Add(key);
				mPlayerScores[key].SelfDestrution();
			}
		}
		foreach (int item in list)
		{
			mPlayerScores.Remove(item);
		}
		foreach (PlayerScoreScript value in mPlayerScores.Values)
		{
			value.OnUpdate();
		}
		foreach (RemotePlayer remotePlayer in gameWorld.GetRemotePlayers())
		{
			Sort(remotePlayer);
			if (!mPlayerScores.ContainsKey(remotePlayer.GetChannelID()) && remotePlayer != null && (remotePlayer as RemotePlayer).GetUserState() != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(ScoreTemplate.gameObject) as GameObject;
				gameObject.transform.parent = ScoreList.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				PlayerScoreScript component = gameObject.GetComponent<PlayerScoreScript>();
				component.Init(remotePlayer);
				mPlayerScores.Add(remotePlayer.GetChannelID(), component);
			}
		}
		for (int i = 0; i < mPlayers.Count; i++)
		{
			mPlayerScores[mPlayers[i].GetChannelID()].UpdateName(i);
		}
		ScoreList.Reposition();
	}

	public bool Sort(Player newPlayer)
	{
		if (mPlayers.Count == 0)
		{
			mPlayers.Add(newPlayer);
		}
		else
		{
			int num = 0;
			int num2 = mPlayers.Count - 1;
			int num3 = (num + num2) / 2;
			if (newPlayer.VSStatistics.Score >= mPlayers[num].VSStatistics.Score)
			{
				mPlayers.Insert(num, newPlayer);
			}
			else if (newPlayer.VSStatistics.Score <= mPlayers[num2].VSStatistics.Score)
			{
				mPlayers.Insert(num2 + 1, newPlayer);
			}
			else
			{
				while (num2 - num > 1)
				{
					float num4 = mPlayers[num3].VSStatistics.Score;
					if ((float)newPlayer.VSStatistics.Score == num4)
					{
						num = num3;
						break;
					}
					if ((float)newPlayer.VSStatistics.Score < num4)
					{
						num = num3;
					}
					else
					{
						num2 = num3;
					}
					num3 = (num + num2) / 2;
				}
				mPlayers.Insert(num + 1, newPlayer);
			}
		}
		return true;
	}

	public void OnButtonClick(GameObject go)
	{
		foreach (PlayerScoreScript value in mPlayerScores.Values)
		{
			if (!(value.gameObject == go))
			{
				continue;
			}
			if (mSelectPlayerScore == value)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
				value.SetHighlight(false);
				mSelectPlayerScore = null;
				break;
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mSelectPlayerScore != null)
			{
				mSelectPlayerScore.SetHighlight(false);
			}
			mSelectPlayerScore = value;
			mSelectPlayerScore.SetHighlight(true);
			break;
		}
	}
}

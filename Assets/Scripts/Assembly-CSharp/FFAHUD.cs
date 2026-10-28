using UnityEngine;

public class FFAHUD : MonoBehaviour
{
	public static FFAHUD mInstance;

	public UILabel TopScore;

	public UILabel TopName;

	public UITexture TopHead;

	public UISprite TopHeadFrame;

	public UILabel SelfScore;

	public UILabel SelfName;

	public UITexture SelfHead;

	public UISprite SelfHeadFrame;

	private bool mResetComplete;

	private Texture mTopHeadDefault;

	private Player mLastTopPlayer;

	private void Start()
	{
		mInstance = this;
		mResetComplete = false;
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			mResetComplete = true;
			Reset();
		}
		mTopHeadDefault = TopHead.mainTexture;
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			SocialManager.UpdateHead(GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.SocialPlayer, SelfHead);
				GameWorld.RefreshHeadFrame(SelfHeadFrame, GameApp.GetInstance().GetGameWorld().GetLocalPlayer());
			}
		}

		private void OnDestroy()
		{
			mInstance = null;
		}

		private void Update()
		{
			if (!mResetComplete)
			{
				if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() == null)
				{
					return;
				}
				mResetComplete = true;
				Reset();
			}
			TopScore topScore = GameApp.GetInstance().GetGameWorld().BattleInfo.TopScore;
			SelfScore.text = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.VSStatistics.FfaScore + " / " + Lobby.GetInstance().WinValue;
			TopScore.text = topScore.score + " / " + Lobby.GetInstance().WinValue;
			Player playerBySeatID = GameApp.GetInstance().GetGameWorld().GetPlayerBySeatID(topScore.seatID);
			if (playerBySeatID != null && mLastTopPlayer != playerBySeatID)
			{
				mLastTopPlayer = playerBySeatID;
				TopName.text = playerBySeatID.Name;
				TopName.color = UIConstant.FFA_SEAT_COLOR[playerBySeatID.GetSeatID()];
				if (playerBySeatID.SocialPlayer == null || (!playerBySeatID.SocialPlayer.HasPhoto() && TopHead.mainTexture != mTopHeadDefault))
				{
					TopHead.mainTexture = mTopHeadDefault;
				}
				else
				{
					SocialManager.UpdateHead(playerBySeatID.SocialPlayer, TopHead);
				}
				GameWorld.RefreshHeadFrame(TopHeadFrame, playerBySeatID);
			}
		}

		public void Reset()
		{
			SelfName.text = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetName();
			SelfName.color = UIConstant.FFA_SEAT_COLOR[GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetSeatID()];
			SelfScore.text = "0 / " + Lobby.GetInstance().WinValue;
			TopName.text = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetName();
			TopScore.text = "0 / " + Lobby.GetInstance().WinValue;
			GameWorld.RefreshHeadFrame(SelfHeadFrame, GameApp.GetInstance().GetGameWorld().GetLocalPlayer());
		}
	}

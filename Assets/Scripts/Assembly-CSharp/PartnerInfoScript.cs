using System;
using UnityEngine;

public class PartnerInfoScript : MonoBehaviour, UIMsgListener
{
	public GameObject PlayerInfo;

	public UILabel PlayeName;

	public UISprite RankIcon;

	public UILabel RankLevel;

	public UITexture HeadPortrait;

	public UISprite HeadFrame;

	public UISprite FriendIcon;

	public UISprite MasterIcon;

	public UISprite SelfFrame;

	public GameObject KickPlayerBtn;

	private RoomPlayer mRoomPlayer;

	private SocialPlayer mSocialPlayer;

	private Texture2D defaultTex;

	private void Awake()
	{
		PlayeName.text = string.Empty;
		RankIcon.gameObject.SetActive(false);
		RankLevel.gameObject.SetActive(false);
		HeadPortrait.gameObject.SetActive(false);
		FriendIcon.gameObject.SetActive(false);
		MasterIcon.gameObject.SetActive(false);
		SelfFrame.gameObject.SetActive(false);
		KickPlayerBtn.SetActive(false);
		PlayerInfo.SetActive(false);
		defaultTex = Resources.Load<Texture2D>("UI/PcPhoto/head_0");
		UIEventListener uIEventListener = UIEventListener.Get(KickPlayerBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	public void Init(RoomPlayer player)
	{
		mRoomPlayer = player;
		PlayeName.text = string.Empty;
		RankIcon.gameObject.SetActive(false);
		RankLevel.gameObject.SetActive(false);
		HeadPortrait.gameObject.SetActive(false);
		FriendIcon.gameObject.SetActive(false);
		MasterIcon.gameObject.SetActive(false);
		SelfFrame.gameObject.SetActive(false);
		KickPlayerBtn.SetActive(false);
		PlayerInfo.SetActive(false);
		if (player != null)
		{
			if (player.ChannelId == Lobby.GetInstance().MasterChannelID)
			{
				MasterIcon.gameObject.SetActive(true);
			}
			else if (Lobby.GetInstance().IsMasterPlayer)
			{
				KickPlayerBtn.SetActive(true);
			}
			if (player.ChannelId == Lobby.GetInstance().GetChannelID())
			{
				SelfFrame.gameObject.SetActive(true);
			}
			PlayeName.text = player.getPlayerName();
			SetExp(player.EXP);
			RankIcon.gameObject.SetActive(true);
			RankLevel.gameObject.SetActive(true);
			PlayerInfo.SetActive(true);
			HeadPortrait.gameObject.SetActive(true);
			mSocialPlayer = player.SocialPlayer;
			GameWorld.RefreshHeadFrame(HeadFrame, player.GetFrameType(), player.GetPrestige());
		}
	}

	private void Update()
	{
		if (mSocialPlayer != null && mSocialPlayer.HasPhoto())
		{
			if (HeadPortrait.mainTexture != mSocialPlayer.GetPhoto() || !HeadPortrait.gameObject.activeSelf)
			{
				HeadPortrait.gameObject.SetActive(true);
				HeadPortrait.mainTexture = mSocialPlayer.GetPhoto();
			}
		}
		else
		{
			HeadPortrait.mainTexture = defaultTex;
		}
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == KickPlayerBtn)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_COOP_BOSS_KICK"), 2, 95);
		}
	}

	private void OnClick()
	{
		if (mRoomPlayer != null)
		{
			if (mRoomPlayer.ChannelId == Lobby.GetInstance().GetChannelID())
			{
				GameObject original = Resources.Load("UI/Menu/PlayerInfo/Prefab/PlayerInfoUI") as GameObject;
				GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
				PlayerInfoUI component = gameObject.GetComponent<PlayerInfoUI>();
				component.InitByUserState(GameApp.GetInstance().GetUserState(), true);
			}
			else
			{
				GameObject original2 = Resources.Load("UI/Menu/PlayerInfo/Prefab/PlayerInfoUI") as GameObject;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original2) as GameObject;
				PlayerInfoUI component2 = gameObject2.GetComponent<PlayerInfoUI>();
				component2.InitByRoomPlayer(mRoomPlayer);
			}
		}
	}

	private void SetExp(int exp)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		int rankID = userState.GetRank(exp).rankID;
		RankIcon.spriteName = Rank.GetRankIconName(rankID);
		RankLevel.text = Rank.GetRankLevelString(rankID);
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 95)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				LeaveRoomRequest request = new LeaveRoomRequest(mRoomPlayer.ChannelId);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			UIMsgBox.instance.CloseMessage();
		}
	}
}

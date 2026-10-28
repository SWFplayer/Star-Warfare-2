using System;
using UnityEngine;

public class PlayerScoreScript : MonoBehaviour
{
	public UISprite RankIcon;

	public UILabel RankLevel;

	public UILabel PlayerName;

	public UISprite WeaponIcon;

	public UISprite MuntantBossIcon;

	public UILabel ModeSpecialInfo;

	public UILabel KillCount;

	public UILabel DeathCount;

	public UILabel AssistCount;

	public UILabel Score;

	public UISprite SELF_Background;

	public GameObject HighlightObjs;

	public GameObject AddFriend;

	public GameObject SeeInfo;

	public UITexture Head;

	public UISprite Frame;

	private Player mPlayer;

	private UserState mUserState;

	private Color mNameColor = default(Color);

	private Color mScoreColor = UIConstant.COLOR_SCORE;

	private static string mGameObjectInitName = "_PlayerScore_";

	private SocialPlayer mSocialPlayer;

	private void Awake()
	{
		if (AddFriend != null && SeeInfo != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(AddFriend);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			UIEventListener uIEventListener2 = UIEventListener.Get(SeeInfo);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		}
	}

	public void Init(Player player)
	{
		mPlayer = player;
		if (player.GameUnitType == EGameUnitType.LOCAL_PLAYER)
		{
			mUserState = GameApp.GetInstance().GetUserState();
			SELF_Background.gameObject.SetActive(true);
			PlayerName.text = Lobby.GetInstance().GetUserName();
		}
		else
		{
			mUserState = (player as RemotePlayer).GetUserState();
			mScoreColor = UIConstant.COLOR_SCORE;
			PlayerName.text = mPlayer.GetName();
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
		{
			mNameColor = UIConstant.FFA_SEAT_COLOR[player.GetSeatID()];
		}
		else if (player.Team == TeamName.Blue)
		{
			mNameColor = UIConstant.COLOR_BLUE_TEAM;
		}
		else if (player.Team == TeamName.Red)
		{
			mNameColor = UIConstant.COLOR_RED_TEAM;
		}
		RankIcon.spriteName = Rank.GetRankIconName(mUserState.GetRank().rankID);
		RankLevel.text = Rank.GetRankLevelString(mUserState.GetRank().rankID);
		PlayerName.color = mNameColor;
		if (player.IsBoss && MuntantBossIcon != null)
		{
			WeaponIcon.gameObject.SetActive(false);
			MuntantBossIcon.gameObject.SetActive(true);
			int num = player.BossIndex + 1;
			MuntantBossIcon.spriteName = "head_boss_0" + num;
		}
		else
		{
			WeaponIcon.gameObject.SetActive(false);
			if (MuntantBossIcon != null)
			{
				MuntantBossIcon.gameObject.SetActive(true);
			}
			WeaponIcon.spriteName = "weapons_" + mPlayer.GetWeapon().GetGunID();
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
		{
			ModeSpecialInfo.text = mPlayer.VSStatistics.GetTotleDamageToPlayerBoss().ToString();
			ModeSpecialInfo.color = mScoreColor;
			KillCount.text = mPlayer.VSStatistics.PlayerBossKills.ToString();
			KillCount.color = mScoreColor;
		}
		else
		{
			ModeSpecialInfo.text = "x" + mPlayer.VSStatistics.Occupy;
			ModeSpecialInfo.color = mScoreColor;
			KillCount.text = mPlayer.VSStatistics.Kills.ToString();
			KillCount.color = mScoreColor;
		}
		KillCount.text = mPlayer.VSStatistics.Kills.ToString();
		DeathCount.text = mPlayer.VSStatistics.Death.ToString();
		DeathCount.color = mScoreColor;
		AssistCount.text = (mPlayer.VSStatistics.Assist + mPlayer.VSStatistics.GoodAssist).ToString();
		AssistCount.color = mScoreColor;
		int score = mPlayer.VSStatistics.Score;
		Score.text = score.ToString();
		Score.color = mScoreColor;
		UserState userState = GameApp.GetInstance().GetUserState();
		if (AddFriend != null && (mPlayer.IsLocal() || !mPlayer.GetIsRegisted() || userState.GetFriendDictionary().ContainsKey(mPlayer.GetAccountID())))
		{
			AddFriend.SetActive(false);
		}
		mSocialPlayer = player.SocialPlayer;
		GameWorld.RefreshHeadFrame(Frame, player);
	}

	public void OnUpdate()
	{
		if (mPlayer == null || mUserState == null)
		{
			return;
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
		{
			mNameColor = UIConstant.FFA_SEAT_COLOR[mPlayer.GetSeatID()];
		}
		else if (mPlayer.Team == TeamName.Blue)
		{
			mNameColor = UIConstant.COLOR_BLUE_TEAM;
		}
		else if (mPlayer.Team == TeamName.Red)
		{
			mNameColor = UIConstant.COLOR_RED_TEAM;
		}
		PlayerName.color = mNameColor;
		if (mPlayer.IsBoss && MuntantBossIcon != null)
		{
			WeaponIcon.gameObject.SetActive(false);
			MuntantBossIcon.gameObject.SetActive(true);
			int num = mPlayer.BossIndex + 1;
			MuntantBossIcon.spriteName = "head_boss_0" + num;
		}
		else
		{
			WeaponIcon.gameObject.SetActive(true);
			if (MuntantBossIcon != null)
			{
				MuntantBossIcon.gameObject.SetActive(false);
			}
			WeaponIcon.spriteName = "weapons_" + mPlayer.GetWeapon().GetGunID();
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
		{
			ModeSpecialInfo.text = mPlayer.VSStatistics.GetTotleDamageToPlayerBoss().ToString();
			KillCount.text = mPlayer.VSStatistics.PlayerBossKills.ToString();
		}
		else
		{
			ModeSpecialInfo.text = "x" + mPlayer.VSStatistics.Occupy;
			KillCount.text = mPlayer.VSStatistics.Kills.ToString();
		}
		DeathCount.text = mPlayer.VSStatistics.Death.ToString();
		AssistCount.text = (mPlayer.VSStatistics.Assist + mPlayer.VSStatistics.GoodAssist).ToString();
		int score = mPlayer.VSStatistics.Score;
		Score.text = score.ToString();
	}

	private void Update()
	{
		SocialManager.UpdateHead(mSocialPlayer, Head);
	}

	public void UpdateName(int index)
	{
		string empty = string.Empty;
		empty = ((!mPlayer.IsLocal() && mPlayer.Team != GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.Team) ? "1" : "0");
		base.gameObject.name = empty + mGameObjectInitName + index;
	}

	public void SelfDestrution()
	{
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	public void SetHighlight(bool highlight)
	{
		HighlightObjs.SetActive(highlight);
		PlayerName.gameObject.SetActive(!highlight);
	}

	public void OnButtonClick(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (go == AddFriend)
		{
			if (!mPlayer.IsLocal())
			{
				if (GameApp.GetInstance().GetUserState().DontMindIfRegisted())
				{
					SwFriendAddRequest request = new SwFriendAddRequest(mPlayer.GetAccountID());
					GameApp.GetInstance().GetWWWManager().SendRequest(request);
				}
				else
				{
					UIMsgBox.instance.ShowMessage(PVPResultPageUI.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_3"), 8, 23);
				}
			}
		}
		else if (go == SeeInfo)
		{

            //Annoying Freeze Bug on the build, Workaround for now.
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return;
            }

            GameObject original = Resources.Load("UI/Menu/PlayerInfo/Prefab/PlayerInfoUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, new Vector3(0f, 2000f, 0f), Quaternion.identity) as GameObject;
			PlayerInfoUI component = gameObject.GetComponent<PlayerInfoUI>();
			component.InitByUserState(mUserState, mPlayer.IsLocal());
			component.SetAccountID(mPlayer.GetAccountID());
			PVPResultPageUI.mInstance.SelfStatiticPage.AvatarFrame.SetActive(false);
		}
	}
}

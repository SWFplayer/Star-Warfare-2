using UnityEngine;

public class CoopPlayerResultScript : MonoBehaviour
{
	public UILabel PlayerName;

	public UILabel PlayerNumber;

	public UITexture HeadPortrait;

	public UISprite HeadFrame;

	public UISprite FriendIcon;

	public UILabel BossCash;

	public UILabel EliminateCash;

	public UILabel Mithril;

	public UILabel VoidParts;

	public UISprite SelfFrame;

	private SocialPlayer mSocialPlayer;

	private Player mPlayer;

	private bool voidPartsSetted;

	public void Init(int number, string name, int bossCash, int eliminateCash, int mithril, int voidParts, bool isFriend, Player player)
	{
		PlayerNumber.text = number.ToString();
		PlayerName.text = name;
		Mode modePlay = GameApp.GetInstance().GetGameMode().ModePlay;
		if (bossCash == 0 && modePlay != Mode.Survival && (player.IsLocal() || !((RemotePlayer)player).HasLeavedGame()) && player.State != Player.DEAD_STATE && player.State != Player.LOSE_STATE)
		{
			BossCash.text = string.Empty;
			Mithril.text = string.Empty;
		}
		else
		{
			BossCash.text = bossCash.ToString();
			Mithril.text = mithril.ToString();
		}
		EliminateCash.text = eliminateCash.ToString();
		if (voidParts == -1)
		{
			VoidParts.text = string.Empty;
		}
		else
		{
			VoidParts.text = voidParts.ToString();
		}
		if (isFriend)
		{
			FriendIcon.gameObject.SetActive(true);
		}
		else
		{
			FriendIcon.gameObject.SetActive(false);
		}
		mSocialPlayer = player.SocialPlayer;
		mPlayer = player;
		GameWorld.RefreshHeadFrame(HeadFrame, player);
	}

	public void Refresh()
	{
		if (mPlayer != null)
		{
			int bossCash = mPlayer.GetBossCash();
			int num = mPlayer.GetMonsterCash() + mPlayer.GetPickupCash() + mPlayer.GetBonusCash();
			int bossMithril = mPlayer.GetBossMithril();
			BossCash.text = bossCash.ToString();
			EliminateCash.text = num.ToString();
			Mithril.text = bossMithril.ToString();
		}
	}

	private void Update()
	{
		SocialManager.UpdateHead(mSocialPlayer, HeadPortrait);
		if (voidPartsSetted || mPlayer == null || mPlayer.IsLocal())
		{
			return;
		}
		RemotePlayer remotePlayer = mPlayer as RemotePlayer;
		if (remotePlayer != null)
		{
			voidPartsSetted = remotePlayer.GetHasGotMaterialInfo();
			if (voidPartsSetted)
			{
				VoidParts.text = remotePlayer.GetCoopVoidMaterialCount().ToString();
			}
		}
	}

	public void SetVoidParts(int count)
	{
		voidPartsSetted = true;
		VoidParts.text = count.ToString();
	}

	public int GetPlayerChannelID()
	{
		if (mPlayer != null)
		{
			return mPlayer.GetChannelID();
		}
		return 0;
	}
}

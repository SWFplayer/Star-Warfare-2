using UnityEngine;

public class StateKillStreak : MonoBehaviour
{
	public UISprite m_Icon;

	public GameObject m_Background;

	private int mKillStreakId;

	private AdvancedSkill mSkill;

	private Player mLocalPlayer;

	private UserState mUserState;

	public void SetAdvancedSkill(AdvancedSkill skill)
	{
		mSkill = skill;
		m_Icon.spriteName = mSkill.GetIconName();
	}

	public AdvancedSkill GetAdvancedSkill()
	{
		return mSkill;
	}

	public void SetKillStreakId(int id)
	{
		mKillStreakId = id;
	}

	public int GetKillStreakId()
	{
		return mKillStreakId;
	}

	private void Awake()
	{
		mLocalPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		mUserState = GameApp.GetInstance().GetUserState();
	}

	public void OnClick()
	{
		if (mSkill.CanActive() && mSkill != null)
		{
			mSkill.Active();
			KillStreakUsedRequest request = new KillStreakUsedRequest(mKillStreakId);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			mLocalPlayer.GetSkills().PlayerUseActiveSkill();
			HUDBase.instance.m_KillStreakHUDManager.RemoveSkill(this);
		}
	}
}

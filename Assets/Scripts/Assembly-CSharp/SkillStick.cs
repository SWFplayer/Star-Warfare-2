using UnityEngine;

public class SkillStick : MonoBehaviour
{
	public int ID;

	public bool isActiveBuff;

	public int skillId;

	public UISprite innerFront;

	private Joystick mSelfJoystick;

	private Player localPlayer;

	private int bossSkillIndex;

	public float cd { get; set; }

	public float startTime { get; set; }

	public float mEffectTime { get; set; }

	private void Awake()
	{
		mSelfJoystick = GetComponent<Joystick>();
	}

	public void SetSkill(int id, int index)
	{
		ID = id;
		mSelfJoystick.inner.GetComponent<UISprite>().spriteName = GameApp.GetInstance().GetUserState().GetPvpSkillManager()
			.GetMutantSkillById(id)
			.GetIconName();
		isActiveBuff = MutantSkill.isActiveBuff(ID);
		skillId = MutantSkill.GetSkillId(ID);
		bossSkillIndex = index;
	}

	private void OnPress(bool isPressed)
	{
		if (!isPressed || ID <= 0)
		{
			return;
		}
		if (ID == 99)
		{
			if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.IsBoss && GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.CanJump())
			{
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.SetState(Player.BOSS_JUMP_STATE);
				if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
				{
					PlayerBossStateRequest request = new PlayerBossStateRequest(Player.BOSS_JUMP_STATE);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		else
		{
			OnCastBossSkill();
		}
	}

	private void OnCastBossSkill()
	{
		if (isActiveBuff)
		{
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.TryCastBossActiveBuffer(ID);
		}
		else
		{
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.OnCastBossSkill(skillId);
		}
	}

	public void SetTimer(byte bossIndex)
	{
		cd = MutantSkill.GetCdOfActiveBuff(ID);
		startTime = Time.time;
		innerFront.gameObject.SetActive(true);
		innerFront.spriteName = mSelfJoystick.inner.GetComponent<UISprite>().spriteName;
		mSelfJoystick.inner.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.5f);
	}

	public void SetTimerNull()
	{
		cd = 0f;
		innerFront.gameObject.SetActive(false);
		mSelfJoystick.inner.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 1f);
	}

	private void Update()
	{
		if (ID <= 0 || cd == 0f)
		{
			return;
		}
		if (localPlayer == null)
		{
			localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		}
		if (localPlayer == null)
		{
			return;
		}
		if (Time.time - startTime > cd)
		{
			base.GetComponent<Collider>().enabled = true;
			return;
		}
		base.GetComponent<Collider>().enabled = false;
		if (Time.time - startTime < mEffectTime)
		{
			innerFront.invert = false;
			innerFront.fillAmount = 1f - (Time.time - startTime) / mEffectTime;
		}
		else
		{
			innerFront.invert = true;
			innerFront.fillAmount = (Time.time - startTime - mEffectTime) / (cd - mEffectTime);
		}
	}
}

using UnityEngine;

public class StateSkill : MonoBehaviour
{
	public enum EStateSkillType
	{
		SW_VOID = 0,
		ADVANCED_SKILL = 1
	}

	public UISprite m_Button;

	public UISprite m_Effect;

	public UISprite m_SkillBackground;

	public UISprite m_Background;

	public int SkillId;

	private EStateSkillType mSkillType;

	private AdvancedSkill mSkill;

	private SwVoid mSwVoid;

	private Player player;

	private UserState userState;

	private float mStartTime = -9999f;

	private float mStopTime = -9999f;

	private float mEffectTime;

	private float mOriginalCoolDownTime;

	private float mCoolDownTime;

	private bool mEffectPlayed = true;

	private string iconName;

	public void SetSkillType(EStateSkillType type)
	{
		mSkillType = type;
	}

	public void SetAdvancedSkill(AdvancedSkill skill)
	{
		mSkill = skill;
		iconName = mSkill.GetIconName();
		m_Button.spriteName = mSkill.GetIconName();
		m_SkillBackground.spriteName = mSkill.GetIconName();
	}

	public void SetSwVoid(SwVoid swVoid)
	{
		mSwVoid = swVoid;
		iconName = swVoid.GetIconName();
		m_Button.spriteName = iconName;
		m_SkillBackground.spriteName = iconName;
	}

	public void SetEffectTime(float time)
	{
		mEffectTime = time;
	}

	public void SetCoolDownTime(float time)
	{
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		}
		if (player.GetSkills() != null)
		{
			mOriginalCoolDownTime = time;
		}
	}

	private void Awake()
	{
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		userState = GameApp.GetInstance().GetUserState();
	}

	public void OnClick()
	{
		if (!(Time.time - mStartTime > mCoolDownTime))
		{
			return;
		}
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
		if (mSkillType == EStateSkillType.ADVANCED_SKILL && mSkill != null && mSkill.GetExternalId() == 2014 && (player == null || player.GetSkills().GetLastActiveSkillType() == PlayerSkill.ELastSkillType.NONE || (player.GetSkills().GetLastActiveSkillType() == PlayerSkill.ELastSkillType.SWVOID && (player.mSwVoid == null || !player.mSwVoid.IsReadyForActiveSkill())) || (player.GetSkills().GetLastActiveSkillType() == PlayerSkill.ELastSkillType.ACTIVE_SKILL && !player.GetSkills().GetLastSkillCanActive())))
		{
			return;
		}
		mStartTime = Time.time;
		mEffectPlayed = false;
		if (mSkillType == EStateSkillType.SW_VOID)
		{
			if (mSwVoid != null && player != null)
			{
				mSwVoid.ActiveSkill();
				mSwVoid.InitSkillButton(this);
				player.GetSkills().PlayerUseActiveSkill();
			}
		}
		else if (mSkillType == EStateSkillType.ADVANCED_SKILL && mSkill != null)
		{
			mSkill.Active();
			SetCoolDownTime(mSkill.GetOnTriggerValues()[0]);
			if (player != null)
			{
				player.TrySendPlayerSkillCheck(true, 1, mSkill.GetExternalId());
				player.GetSkills().PlayerUseActiveSkill();
			}
		}
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		}
		if (player == null)
		{
			return;
		}
		mCoolDownTime = mOriginalCoolDownTime / (1f + player.GetSkills().GetSkill(SkillsType.COOL_DOWN_SPEEDUP));
		if (player.GetSkills().HasSkill(SkillsType.SILENCE))
		{
			base.GetComponent<Collider>().enabled = false;
			m_Button.spriteName = "skill_disable";
			m_Button.fillAmount = 1f;
			return;
		}
		if (Time.time - mStartTime > mCoolDownTime && mCoolDownTime > mEffectTime)
		{
			base.GetComponent<Collider>().enabled = true;
			m_Button.spriteName = iconName;
			m_Button.fillAmount = 1f;
			m_Background.fillAmount = 1f;
			if (mEffectPlayed)
			{
				return;
			}
			mEffectPlayed = true;
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
			if (m_Effect.gameObject != null)
			{
				TweenScale component = m_Effect.gameObject.GetComponent<TweenScale>();
				if (null != component)
				{
					component.gameObject.SetActive(true);
					component.Reset();
					component.PlayForward();
				}
				TweenAlpha component2 = m_Effect.gameObject.GetComponent<TweenAlpha>();
				if (null != component2)
				{
					component2.gameObject.SetActive(true);
					component2.Reset();
					component2.PlayForward();
				}
			}
			return;
		}
		base.GetComponent<Collider>().enabled = false;
		m_Button.spriteName = iconName;
		if (Time.time - mStartTime < mEffectTime)
		{
			m_Button.invert = false;
			m_Button.fillAmount = 1f - (Time.time - mStartTime) / mEffectTime;
			m_Background.invert = false;
			m_Background.fillAmount = m_Button.fillAmount;
			return;
		}
		m_Button.invert = true;
		m_Background.invert = true;
		if (mStopTime < mStartTime)
		{
			mStopTime = Time.time;
		}
		if (mCoolDownTime > mEffectTime)
		{
			m_Button.fillAmount = (Time.time - mStopTime) / (mCoolDownTime - mEffectTime);
			m_Background.fillAmount = m_Button.fillAmount;
			return;
		}
		m_Button.fillAmount = 1f;
		m_Background.fillAmount = m_Button.fillAmount;
		base.GetComponent<Collider>().enabled = true;
		if (mEffectPlayed)
		{
			return;
		}
		mEffectPlayed = true;
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
		if (m_Effect.gameObject != null)
		{
			TweenScale component3 = m_Effect.gameObject.GetComponent<TweenScale>();
			if (null != component3)
			{
				component3.gameObject.SetActive(true);
				component3.Reset();
				component3.PlayForward();
			}
			TweenAlpha component4 = m_Effect.gameObject.GetComponent<TweenAlpha>();
			if (null != component4)
			{
				component4.gameObject.SetActive(true);
				component4.Reset();
				component4.PlayForward();
			}
		}
	}

	public void Hide()
	{
		base.GetComponent<Collider>().enabled = false;
		m_Background.gameObject.SetActive(false);
		m_Button.gameObject.SetActive(false);
		m_SkillBackground.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.GetComponent<Collider>().enabled = true;
		m_Background.gameObject.SetActive(true);
		m_Button.gameObject.SetActive(true);
		m_SkillBackground.gameObject.SetActive(true);
	}

	public void ReduceCoolDown(float reduceTime)
	{
		mStopTime -= reduceTime;
		mStartTime -= reduceTime;
	}

	public bool IsSkillCoolDown()
	{
		return Time.time - mStartTime > mCoolDownTime;
	}
}

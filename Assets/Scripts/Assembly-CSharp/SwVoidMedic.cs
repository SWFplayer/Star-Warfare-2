using System.Collections.Generic;
using UnityEngine;

public class SwVoidMedic : SwVoid
{
	public static SwVoidState HEAL_STATE = new SwVoidHealState();

	public static SwVoidState AUTO_HEAL_STATE = new SwVoidAutoHealState();

	private float beginHealTime;

	protected int restoreAmount;

	protected int skillRestoreAmount;

	private Timer skillTimer;

	private Timer autoHealCheckTimer;

	private Timer healStateTimer;

	private float secondTimer;

	private bool hasHealed;

	private Player healedPlayer;

	private GameObject AutohealVFX;

	private GameObject HealVFX;

	public override void init(GameObject gObject, Player player)
	{
		base.init(gObject, player);
		mType = ESwVoidType.MEDIC;
		LoadMedicInfo();
		Object original = Resources.Load(EffectName.VD4_HEAL);
		HealVFX = Object.Instantiate(original) as GameObject;
		HealVFX.transform.parent = entityTransform.Find("VoidEntity");
		HealVFX.transform.localPosition = Vector3.zero;
		HealVFX.SetActive(false);
		skillRestoreAmount = SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE3);
		skillTimer = new Timer();
		float swVoidParameterFloat = SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.CD);
		skillTimer.SetTimer(swVoidParameterFloat, true);
		healStateTimer = new Timer();
		healStateTimer.SetTimer(0.5f, true);
		if (IsHighLevel())
		{
			HighLevelInit();
		}
	}

	public override void DoIdleFloating(float deltaTime)
	{
		base.DoIdleFloating(deltaTime);
		if (autoHealCheckTimer.Ready())
		{
			autoHealCheckTimer.Do();
			TryAutoHeal(deltaTime);
		}
	}

	public override void Loop(float deltaTime)
	{
		base.Loop(deltaTime);
	}

	private void TryAutoHeal(float deltaTime)
	{
		if (isLocal && IsHighLevel() && mPlayer.InPlayingState() && mPlayer.Hp < mPlayer.MaxHp)
		{
			SetState(AUTO_HEAL_STATE);
		}
	}

	public void StartAutoHeal()
	{
		secondTimer = 0f;
		PlayAnimation(AnimationString.SWVOID_ATTACK, WrapMode.Loop);
		AutohealVFX.SetActive(true);
	}

	public void DoAutoHeal(float deltaTime)
	{
		if (!isLocal)
		{
			return;
		}
		catchPlayerWithoutAnimation();
		secondTimer += deltaTime;
		if (secondTimer > 1f)
		{
			if (mPlayer.Hp < mPlayer.MaxHp && mPlayer.InPlayingState())
			{
				int num = restoreAmount;
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					num = (int)VSMath.GetAutoRecoverInVS(num);
				}
				mPlayer.RecoverHP(num);
			}
			secondTimer--;
		}
		if (mPlayer.Hp == mPlayer.MaxHp)
		{
			SetState(SwVoid.FLOATING_STATE);
		}
	}

	public void EndAutoHeal()
	{
		AutohealVFX.SetActive(false);
	}

	private void catchPlayerWithoutAnimation()
	{
		if (!(mPlayerSwVoidTransform == null))
		{
			Vector3 vector = mPlayerSwVoidTransform.position - GetTransform().position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			if (magnitude < DISTANCE_CLOSE)
			{
				StartNavMesh(mPlayerSwVoidTransform.position, Speed);
			}
			else if (magnitude < DISTANCE_MIDDLE)
			{
				StartNavMesh(mPlayerSwVoidTransform.position, Speed);
				isDash = false;
			}
			else if (magnitude < DISTANCE_FAR && !isDash)
			{
				StartNavMesh(mPlayerSwVoidTransform.position, Speed);
			}
			else
			{
				StartNavMesh(mPlayerSwVoidTransform.position, farSpeed);
				isDash = true;
			}
		}
	}

	private void TryToHeal()
	{
		healedPlayer = mPlayer;
		if (mPlayer.InPlayingState() && GameApp.GetInstance().GetGameMode().IsSingle())
		{
			SetState(HEAL_STATE);
			skillTimer.Do();
		}
		else
		{
			if (!isLocal || !mPlayer.InPlayingState())
			{
				return;
			}
			List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
			List<int> list = new List<int>();
			foreach (RemotePlayer item in remotePlayers)
			{
				if (item != null && item.InPlayingState() && item.IsFriendForce(mPlayer) && Vector3.Distance(item.GetPosition(), entityTransform.position) <= SwVoidParameter.HEAL_RADIUS)
				{
					list.Add(item.GetChannelID());
				}
			}
			NeighborPlayerRequest request = new NeighborPlayerRequest(list);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public void StartHeal()
	{
		hasHealed = false;
		healStateTimer.Do();
	}

	public void DoHeal()
	{
		if (hasHealed && AnimationPlayed(AnimationString.SWVOID_ATTACK_SP, 1f))
		{
			SetState(SwVoid.FLOATING_STATE);
			return;
		}
		if (!hasHealed && Vector3.Distance(healedPlayer.GetPosition(), GetTransform().position) > 3f)
		{
			StartNavMesh(healedPlayer.GetPosition(), farSpeed * 2f);
			PlayAnimation(AnimationString.SWVOID_DASH, WrapMode.Loop);
		}
		if (!hasHealed && Vector3.Distance(healedPlayer.GetPosition(), GetTransform().position) <= 3f && healStateTimer.Ready())
		{
			Heal();
			PlayAnimation(AnimationString.SWVOID_ATTACK_SP, WrapMode.ClampForever);
			AudioManager.GetInstance().PlaySoundAt(AudioName.VOID2_HEAL, entityTransform.position);
			HealVFX.SetActive(true);
			hasHealed = true;
		}
	}

	public void EndHeal()
	{
		hasHealed = false;
		HealVFX.SetActive(false);
	}

	private void Heal()
	{
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			mPlayer.RecoverHP((float)skillRestoreAmount / 100f * (float)mPlayer.MaxHp);
			gameWorld.PlayVFXOnPlayer(mPlayer, 4);
		}
		else if (isLocal)
		{
			SwVoidStateRequest request = new SwVoidStateRequest(ESwVoidStateID.MEDIC_HEAL, 0, healedPlayer.GetChannelID(), entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	private void LoadMedicInfo()
	{
		autoHealCheckTimer = new Timer();
		autoHealCheckTimer.SetTimer(0.5f, true);
		restoreAmount = SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE1);
		restoreAmount += (int)(SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE2) * (float)mPlayer.MaxHp);
		secondTimer = 0f;
	}

	public override void HighLevelInit()
	{
		Object original = Resources.Load(EffectName.VD4_IDLE);
		AutohealVFX = Object.Instantiate(original) as GameObject;
		AutohealVFX.transform.parent = entityTransform.Find("VoidEntity");
		AutohealVFX.transform.localPosition = Vector3.zero;
		AutohealVFX.SetActive(false);
	}

	public override void InitSkillButton(StateSkill stateSkill)
	{
		stateSkill.gameObject.SetActive(true);
		stateSkill.SetSwVoid(this);
		stateSkill.SetSkillType(StateSkill.EStateSkillType.SW_VOID);
		stateSkill.SetCoolDownTime(GetTimer(0).GetInterval());
		stateSkill.SetEffectTime(GetTimer(1).GetInterval());
	}

	public override void ActiveSkill()
	{
		base.ActiveSkill();
		TryToHeal();
		if (isLocal && mPlayer != null)
		{
			mPlayer.GetSkills().SetLastActiveSkill(ESwVoidStateID.MEDIC_HEAL);
		}
	}

	public override Timer GetTimer(int id)
	{
		if (id == 0)
		{
			return skillTimer;
		}
		return healStateTimer;
	}

	public override bool IsCoolDown()
	{
		return base.IsCoolDown() && skillTimer.Ready();
	}

	public void HealOthers(Player player)
	{
		healedPlayer = player;
		SetState(HEAL_STATE);
		skillTimer.Do();
	}

	public override string GetCenter()
	{
		return SwVoidParameter.MedicCenterBone;
	}
}

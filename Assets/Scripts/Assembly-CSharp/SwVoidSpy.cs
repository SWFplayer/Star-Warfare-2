using System.Collections.Generic;
using UnityEngine;

public class SwVoidSpy : SwVoid
{
	public static SwVoidState INVISIBLE_STATE = new SwVoidInvisibleState();

	private List<Material> matList;

	private float alphaValue = 1f;

	private float lastActiveSkillTime;

	private bool beginningState;

	private Timer skillTimer;

	private Timer invisibleTimer;

	private byte invisibleType;

	private float changeSpeed;

	private int changeTime;

	private float alphaRate;

	public override void init(GameObject gObject, Player player)
	{
		base.init(gObject, player);
		mType = ESwVoidType.SPY;
		InitSwVoidSpyParameter();
		InitPlayerMaterial();
	}

	private void InitPlayerMaterial()
	{
		matList = new List<Material>();
		string[] sUB_AVATAR = UIConstant.SUB_AVATAR;
		foreach (string name in sUB_AVATAR)
		{
			if (mPlayerTransform.Find(name).GetComponent<Renderer>() != null)
			{
				matList.Add(mPlayerTransform.Find(name).GetComponent<Renderer>().material);
				continue;
			}
			Renderer componentInChildren = mPlayerTransform.Find(name).GetComponentInChildren<Renderer>();
			if (componentInChildren != null)
			{
				matList.Add(componentInChildren.material);
			}
		}
	}

	public override void DoIdleFloating(float deltaTime)
	{
		base.DoIdleFloating(deltaTime);
	}

	public void TurnInvisible()
	{
		if (isLocal && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			SwVoidStateRequest request = new SwVoidStateRequest(ESwVoidStateID.SPY_INVISIBLE, 0, 0, Vector3.zero);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		SetState(INVISIBLE_STATE);
	}

	public void StartInvisible()
	{
		lastActiveSkillTime = Time.time;
		alphaValue = 1f;
		beginningState = true;
		skillTimer.Do();
		invisibleTimer.Do();
		foreach (Material mat in matList)
		{
			mat.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
		}
	}

	public void DoInvisible()
	{
		if (beginningState)
		{
			if (changeSpeed == 0f)
			{
				beginningState = false;
				invisibleTimer.Do();
				ChangeAlpha(alphaRate);
				return;
			}
			alphaValue -= Time.deltaTime * changeSpeed;
			if (alphaValue > alphaRate)
			{
				ChangeAlpha(alphaValue);
				return;
			}
			beginningState = false;
			invisibleTimer.Do();
			ChangeAlpha(alphaRate);
		}
		else if (invisibleTimer.Ready())
		{
			alphaValue += Time.deltaTime * 1f;
			if (alphaValue < 1f)
			{
				ChangeAlpha(alphaValue);
				return;
			}
			ChangeAlpha(1f);
			SetState(SwVoid.FLOATING_STATE);
		}
	}

	public void EndInvisible()
	{
		foreach (Material mat in matList)
		{
			if (mat.HasProperty("_Alpha"))
			{
				mat.SetFloat("_Alpha", 1f);
				mat.shader = Shader.Find("StarWarfare/SNDM_new");
			}
		}
	}

	public Timer GetTimer()
	{
		return skillTimer;
	}

	public Timer GetInvisibleTimer()
	{
		return invisibleTimer;
	}

	public override void InitSkillButton(StateSkill stateSkill)
	{
		stateSkill.gameObject.SetActive(true);
		stateSkill.SetSwVoid(this);
		stateSkill.SetSkillType(StateSkill.EStateSkillType.SW_VOID);
		stateSkill.SetCoolDownTime(GetTimer(0).GetInterval());
		stateSkill.SetEffectTime(GetTimer(1).GetInterval());
	}

	public override void Destroy()
	{
		EndInvisible();
		base.Destroy();
	}

	public override void ActiveSkill()
	{
		base.ActiveSkill();
		TurnInvisible();
	}

	public override bool IsCoolDown()
	{
		return skillTimer.Ready();
	}

	public override Timer GetTimer(int id)
	{
		if (id == 0)
		{
			return skillTimer;
		}
		return invisibleTimer;
	}

	protected void InitSwVoidSpyParameter()
	{
		skillTimer = new Timer();
		skillTimer.SetTimer(SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.CD), true);
		invisibleTimer = new Timer();
		invisibleTimer.SetTimer(SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE2), false);
		invisibleType = (byte)SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE3);
		changeTime = SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE1);
		if (changeTime == 0)
		{
			changeSpeed = 0f;
		}
		else
		{
			changeSpeed = 1f / (float)changeTime * 1f;
		}
		if (isLocal)
		{
			alphaRate = 0.2f;
		}
		else
		{
			alphaRate = 0f;
		}
	}

	protected void ChangeAlpha(float alpha)
	{
		foreach (Material mat in matList)
		{
			if (mat.HasProperty("_Alpha"))
			{
				mat.SetFloat("_Alpha", alpha);
			}
			else
			{
				Debug.Log(string.Concat(mat, " has no alpha"));
			}
		}
	}
}

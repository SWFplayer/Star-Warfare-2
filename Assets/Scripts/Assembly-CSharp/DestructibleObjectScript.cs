using System;
using UnityEngine;

public class DestructibleObjectScript : MonoBehaviour
{
	public delegate void VoidDelegate();

	public int MaxHp;

	public GameObject BaseObject;

	public float[] HpPencetages;

	public GameObject[] Objects;

	public GameObject[] Effects;

	public GameObject DeadObject;

	public GameObject DeadEffect;

	public GameObject HpPanel;

	public UISprite CurrentHpSprite;

	public bool DoNotDestroy;

	private MissionChargeDestroy mOwnerMission;

	private int mHp;

	private int mLevel;

	private GameObject CurrentObject;

	private int mID;

	[NonSerialized]
	public bool mDestroyed;

	[NonSerialized]
	public int mTwinID;

	public VoidDelegate onDead;

	public int ID
	{
		get
		{
			return mID;
		}
	}

	public void Activate()
	{
		if (BaseObject != null)
		{
			CurrentObject = UnityEngine.Object.Instantiate(BaseObject) as GameObject;
			CurrentObject.transform.parent = base.gameObject.transform;
			CurrentObject.transform.localPosition = Vector3.zero;
			CurrentObject.transform.localRotation = Quaternion.identity;
			CurrentObject.transform.localScale = Vector3.one;
			if (CurrentObject.GetComponent<TwinBlockScript>() != null)
			{
				CurrentObject.GetComponent<TwinBlockScript>().ID = mTwinID;
				CurrentObject.GetComponent<TwinBlockScript>().Init();
			}
		}
		if ((bool)HpPanel)
		{
			HpPanel.SetActive(true);
		}
		mHp = MaxHp;
		mLevel = 0;
		if ((bool)CurrentHpSprite)
		{
			CurrentHpSprite.fillAmount = 1f;
		}
		mDestroyed = false;
	}

	public void OnHit(int damage, bool isPlayerHit = true)
	{
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (!localPlayer.isPowerful)
		{
			return;
		}
		if (localPlayer.IsLocal())
		{
			localPlayer.HitEnemy();
		}
		int num = mHp - damage;
		if (mLevel < HpPencetages.Length && (float)num < (float)MaxHp * HpPencetages[mLevel])
		{
			UnityEngine.Object.Destroy(CurrentObject);
			CurrentObject = UnityEngine.Object.Instantiate(Objects[mLevel]) as GameObject;
			CurrentObject.transform.parent = base.gameObject.transform;
			CurrentObject.transform.localPosition = Vector3.zero;
			CurrentObject.transform.localRotation = Quaternion.identity;
			CurrentObject.transform.localScale = Vector3.one;
			if (mOwnerMission != null)
			{
				mOwnerMission.TriggerAction();
				mOwnerMission.AddMissionChargerPoint();
				mOwnerMission.RemoveMissionDestructiblePoint();
				MissionManager.SetMissionGameObjectState(mOwnerMission.mPowerCharger, true);
				localPlayer.isPowerful = false;
			}
			if (Effects.Length == 1)
			{
				if (null != Effects[0])
				{
					UnityEngine.Object.Instantiate(Effects[0], CurrentObject.transform.position, Quaternion.identity);
				}
			}
			else if (Effects.Length != 0 && null != Effects[mLevel])
			{
				UnityEngine.Object.Instantiate(Effects[mLevel], CurrentObject.transform.position, Quaternion.identity);
			}
			mHp = (int)((float)MaxHp * HpPencetages[mLevel]);
			mLevel++;
			Debug.Log("mLevel = " + mLevel);
		}
		else if (num <= 0)
		{
			mHp = 0;
			if (DoNotDestroy)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				UnityEngine.Object.Destroy(CurrentObject);
			}
			if (mOwnerMission != null)
			{
				mOwnerMission.TriggerAction();
				mOwnerMission.AddMissionChargerPoint();
				mOwnerMission.IncreaseDestroyedNumber();
				mOwnerMission.RemoveMissionDestructiblePoint();
				MissionManager.SetMissionGameObjectState(mOwnerMission.mPowerCharger, true);
				localPlayer.isPowerful = false;
			}
			if (base.gameObject.name.Contains("IceSpike") && isPlayerHit)
			{
				localPlayer.IceSpikeDestroy++;
			}
			if (DeadObject != null)
			{
				CurrentObject = UnityEngine.Object.Instantiate(DeadObject) as GameObject;
				CurrentObject.transform.parent = base.gameObject.transform;
				CurrentObject.transform.localPosition = Vector3.zero;
				CurrentObject.transform.localRotation = Quaternion.identity;
				CurrentObject.transform.localScale = Vector3.one;
			}
			if (DeadEffect != null)
			{
				UnityEngine.Object.Instantiate(DeadEffect, CurrentObject.transform.position, Quaternion.identity);
			}
		}
		else
		{
			mHp = num;
		}
		if (mHp > 0)
		{
			if ((bool)CurrentHpSprite)
			{
				CurrentHpSprite.fillAmount = (float)mHp / (float)MaxHp;
			}
			return;
		}
		if (onDead != null)
		{
			onDead();
		}
		mDestroyed = true;
	}

	private void LateUpdate()
	{
		if (mDestroyed)
		{
			if (HpPanel != null)
			{
				HpPanel.SetActive(false);
			}
			GameApp.GetInstance().GetGameWorld().RemoveDestructible(base.gameObject.name);
		}
	}

	public void SetOwnerMission(MissionChargeDestroy mission)
	{
		mOwnerMission = mission;
	}

	public void Destroy()
	{
		OnHit(mHp, false);
	}
}

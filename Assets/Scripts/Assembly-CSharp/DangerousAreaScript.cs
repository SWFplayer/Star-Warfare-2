using System.Collections.Generic;
using UnityEngine;

public class DangerousAreaScript : MonoBehaviour
{
	public enum EDamageType
	{
		CONSTANT = 0,
		PERCENTAGE = 1
	}

	public int Group = -1;

	public int Damage;

	public EDamageType DamageType;

	public float ExistDuration;

	public float DamageInterval;

	private float startTime;

	private Dictionary<GameUnit, Timer> TimerDictionary;

	private static Dictionary<int, Dictionary<GameUnit, Timer>> sSharedTimerDictionary = new Dictionary<int, Dictionary<GameUnit, Timer>>();

	public void Start()
	{
		startTime = Time.time;
		if (Group == -1)
		{
			TimerDictionary = new Dictionary<GameUnit, Timer>();
			return;
		}
		if (!sSharedTimerDictionary.ContainsKey(Group))
		{
			sSharedTimerDictionary.Add(Group, new Dictionary<GameUnit, Timer>());
		}
		TimerDictionary = sSharedTimerDictionary[Group];
	}

	public void Update()
	{
		if (ExistDuration > 0f && Time.time - startTime >= ExistDuration)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer != PhysicsLayer.PLAYER)
		{
			return;
		}
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (!TimerDictionary.ContainsKey(localPlayer))
		{
			Timer value = new Timer(DamageInterval, true);
			TimerDictionary.Add(localPlayer, value);
		}
		if (!TimerDictionary[localPlayer].Ready())
		{
			return;
		}
		if (!(localPlayer.GetSkills().GetSkill(SkillsType.FLY) > 0f) || 1 == 0)
		{
			if (localPlayer.GetUserState().GetSelectedStageIndex() == 3 && localPlayer.GetUserState().GetSelectedSubStageIndex() == Global.BOSS_FINAL_SUB_STAGE_ID)
			{
				localPlayer.mGetFireDamage = true;
			}
			if (DamageType == EDamageType.CONSTANT)
			{
				localPlayer.OnHit(new DamageProperty(null, Damage));
			}
			else if (DamageType == EDamageType.PERCENTAGE)
			{
				localPlayer.OnHit(new DamageProperty(null, Damage * localPlayer.MaxHp / 100));
			}
		}
		TimerDictionary[localPlayer].Do();
	}
}

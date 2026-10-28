using System.Collections.Generic;
using UnityEngine;

public class PVPDangerousAreaScript : MonoBehaviour
{
	public int Group = -1;

	public int Damage;

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
		if (TimerDictionary[localPlayer].Ready())
		{
			if (!(localPlayer.GetSkills().GetSkill(SkillsType.FLY) > 0f) || 1 == 0)
			{
			}
			TimerDictionary[localPlayer].Do();
		}
	}
}

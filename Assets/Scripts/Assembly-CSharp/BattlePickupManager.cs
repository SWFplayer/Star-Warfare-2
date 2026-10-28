using System.Collections.Generic;

public class BattlePickupManager
{
	private Dictionary<byte, BattlePickup> battlePickUpMap;

	private PlayerSkill mPlayerSkill;

	private BattlePickup lastBattlePickup;

	private bool needSendRequest;

	public void Init(PlayerSkill playerSkill)
	{
		mPlayerSkill = playerSkill;
		if (playerSkill.GetPlayer().IsLocal() && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			needSendRequest = true;
		}
		battlePickUpMap = new Dictionary<byte, BattlePickup>();
		for (byte b = 1; b <= 17; b++)
		{
			BattlePickup battlePickup = new BattlePickup();
			battlePickup.Init(b, mPlayerSkill.GetPlayer());
			battlePickUpMap.Add(b, battlePickup);
		}
	}

	public void PickUp(byte id)
	{
		if (needSendRequest)
		{
			BattlePickUpRequest request = new BattlePickUpRequest(id);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		if (lastBattlePickup != null)
		{
			lastBattlePickup.TakeOff();
		}
		BattlePickup battlePickup;
		if (!battlePickUpMap.ContainsKey(id))
		{
			battlePickup = new BattlePickup();
			battlePickup.Init(id, mPlayerSkill.GetPlayer());
			battlePickUpMap.Add(id, battlePickup);
		}
		else
		{
			battlePickup = battlePickUpMap[id];
		}
		battlePickup.TakeOn(mPlayerSkill);
		lastBattlePickup = battlePickup;
	}

	public void SharePickUp(byte id, float shareRate)
	{
		if (lastBattlePickup != null)
		{
			lastBattlePickup.TakeOff();
		}
		BattlePickup battlePickup = new BattlePickup();
		battlePickup.Init(id, mPlayerSkill.GetPlayer());
		battlePickup.SharePickup(mPlayerSkill, shareRate);
		lastBattlePickup = battlePickup;
	}

	public void Loop()
	{
		if (lastBattlePickup != null)
		{
			lastBattlePickup.CheckTimer();
		}
	}

	private void ActiveEffect()
	{
	}

	public void AddEffectList()
	{
	}

	public void PlayerDeadClear()
	{
		if (lastBattlePickup != null)
		{
			lastBattlePickup.TakeOff();
		}
		lastBattlePickup = null;
	}

	public void ReinitEffect()
	{
		for (byte b = 1; b <= 17; b++)
		{
			battlePickUpMap[b].Init(b, mPlayerSkill.GetPlayer());
		}
	}
}

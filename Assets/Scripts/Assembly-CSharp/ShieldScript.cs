using UnityEngine;

public class ShieldScript : MonoBehaviour
{
	public int shieldCurrentHP;

	public int shieldMaxHP;

	public float shieldRadius;

	public SwVoidHeavy ownedSwVoid;

	public Player ownedPlayer;

	public bool shieldInited;

	public bool shieldStarted;

	private float shieldDuringTime;

	private float OnHitStartTime;

	private void Update()
	{
		if (Time.time - OnHitStartTime <= 0.5f)
		{
			float num = (float)shieldCurrentHP / (float)shieldMaxHP * 2f;
			SetAlpha(Mathf.Lerp(num + 5f, num, (Time.time - OnHitStartTime) / 0.5f));
		}
	}

	public void InitShield(int ShieldHP, float ShieldRadius, float duringTime)
	{
		shieldMaxHP = ShieldHP;
		shieldRadius = ShieldRadius;
		shieldCurrentHP = shieldMaxHP;
		shieldDuringTime = duringTime;
		if (ShieldHP > 0)
		{
			SetAlpha();
			shieldInited = true;
		}
	}

	public void SetShieldHP(int HP, bool isSettingMaxHp)
	{
		if (isSettingMaxHp)
		{
			shieldCurrentHP = HP;
			shieldMaxHP = HP;
			shieldInited = true;
			SetAlpha();
		}
		else
		{
			Debug.Log("Absorb damage " + (shieldCurrentHP - HP));
			shieldCurrentHP = HP;
			OnHitStartTime = Time.time;
		}
	}

	public void StartShield()
	{
		shieldStarted = true;
		if (ownedPlayer.IsLocal() && GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			BlackHole blackHole = new BlackHole();
			blackHole.x = base.transform.position.x;
			blackHole.z = base.transform.position.z;
			blackHole.radius = shieldRadius + 0.8f;
			blackHole.speed = 10f;
			blackHole.owner = ownedPlayer;
			blackHole.BlackHoleDuringTime = shieldDuringTime;
			blackHole.isAttract = false;
			BlackholeRequest request = new BlackholeRequest(blackHole);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public void AbsorbDamage(int damage)
	{
		Debug.Log("Absorb damage " + damage);
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			SwVoidShieldStateRequest request = new SwVoidShieldStateRequest(damage, ownedPlayer.GetChannelID());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		else
		{
			OnHitShield(damage);
		}
	}

	public void PlayerHitShield(DamageProperty dp)
	{
		SwVoidShieldStateRequest request = new SwVoidShieldStateRequest(dp, ownedPlayer.GetChannelID());
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}

	public void OnHitShield(int damage)
	{
		shieldCurrentHP -= damage;
		OnHitStartTime = Time.time;
	}

	public void SetAlpha()
	{
		base.transform.Find("VOID_SHIELD_sphere").GetComponent<Renderer>().material.SetFloat("_MMultiplier", (float)shieldCurrentHP / (float)shieldMaxHP * 2f);
	}

	public void SetAlpha(float value)
	{
		base.transform.Find("VOID_SHIELD_sphere").GetComponent<Renderer>().material.SetFloat("_MMultiplier", value);
	}

	public float GetAlpha()
	{
		return base.transform.Find("VOID_SHIELD_sphere").GetComponent<Renderer>().material.GetFloat("_MMultiplier");
	}
}

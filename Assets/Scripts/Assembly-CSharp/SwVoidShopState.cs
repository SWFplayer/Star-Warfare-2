using UnityEngine;

public class SwVoidShopState : SwVoidState
{
	private Timer disableTimer;

	private bool inPlayed;

	private bool outPlayed;

	public override void Start(SwVoid swVoid)
	{
		swVoid.PlayAnimation(AnimationString.SWVOID_SHOP, WrapMode.Loop);
		inPlayed = false;
		outPlayed = false;
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		if (swVoid.GetObject().activeSelf)
		{
			if (swVoid.AnimationPlayed(AnimationString.SWVOID_SHOP, 0.01f) && !inPlayed)
			{
				Transform transform = swVoid.GetTransform().Find(swVoid.GetCenter());
				GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.VD5_FLASH_SMALL).CreateObject(transform.position, Vector3.zero, transform.rotation);
				inPlayed = true;
			}
			else if (swVoid.AnimationPlayed(AnimationString.SWVOID_SHOP, 0.98f) && !outPlayed)
			{
				Transform transform = swVoid.GetTransform().Find(swVoid.GetCenter());
				GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.VD5_FLASH_SMALL).CreateObject(transform.position, Vector3.zero, transform.rotation);
				outPlayed = true;
			}
			else if (swVoid.AnimationPlayed(AnimationString.SWVOID_SHOP, 0.99f))
			{
				float interval = Random.RandomRange(8f, 20f);
				disableTimer = new Timer(interval, false);
				swVoid.GetObject().SetActive(false);
			}
		}
		else if (disableTimer != null && disableTimer.Ready())
		{
			swVoid.GetObject().SetActive(true);
			swVoid.PlayAnimation(AnimationString.SWVOID_SHOP, WrapMode.Loop);
			inPlayed = false;
			outPlayed = false;
		}
	}

	public override void End(SwVoid swVoid)
	{
	}
}

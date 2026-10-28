using UnityEngine;

public class PostProcessRender
{
	private static PostProcessRender sInstance = new PostProcessRender();

	private GlowEffectParameter mCurrGlowEffectParam = new GlowEffectParameter();

	private GlowEffectParameter mTargetGlowEffectParam;

	private bool mNeedUpdateGE;

	private float factor = 0.5f;

	public static PostProcessRender GetInstance()
	{
		return sInstance;
	}

	public void Update()
	{
		if (mNeedUpdateGE)
		{
			mCurrGlowEffectParam.Lerp(mTargetGlowEffectParam, factor * Time.deltaTime);
			if (mCurrGlowEffectParam.Equal(mTargetGlowEffectParam))
			{
				mNeedUpdateGE = false;
			}
			ApplyGlowEffect(mCurrGlowEffectParam);
		}
	}

	public void SetTargetGlowEffect(GlowEffectParameter target)
	{
		if (mTargetGlowEffectParam == null)
		{
			mTargetGlowEffectParam = target;
			mCurrGlowEffectParam.CopyFrom(mTargetGlowEffectParam);
			mNeedUpdateGE = false;
			ApplyGlowEffect(mCurrGlowEffectParam);
		}
		else
		{
			mTargetGlowEffectParam = target;
			mNeedUpdateGE = true;
		}
		if (target.duration <= 0.1f)
		{
			factor = 1000f;
		}
		else
		{
			factor = 1f / target.duration;
		}
	}

	private void ApplyGlowEffect(GlowEffectParameter geParameter)
	{
		GlowEffect component = Camera.main.GetComponent<GlowEffect>();
		if (component != null && component.enabled)
		{
			component.glowIntensity = geParameter.glowIntensity;
			component.blurIterations = geParameter.blurIterations;
			component.blurSpread = geParameter.blurSpread;
			component.glowTint = geParameter.glowTint;
		}
	}
}

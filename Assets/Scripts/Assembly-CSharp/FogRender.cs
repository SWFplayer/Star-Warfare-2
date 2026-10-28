using UnityEngine;

public class FogRender
{
	private static FogRender sInstance = new FogRender();

	private Color32 mTargetFogColor;

	private float mTargetFogDensity;

	private float mTargetLinearFogStart;

	private float mTargetLinerFogEnd;

	private Color32 mCurrFogColor = Color.white;

	private float mCurrFogDensity = 0.001f;

	private float mCurrLinearFogStart;

	private float mCurrLinearFogEnd = 300f;

	private float factor = 0.5f;

	private FogTrigger mFogTrigger;

	private bool mNeedUpdate;

	public Color targetAmbientColor;

	public bool needChangeAmbient;

	private Color lastAmbientColor;

	private float AmbientPercent;

	public static FogRender GetInstance()
	{
		return sInstance;
	}

	public void Update()
	{
		if (RenderSettings.fog && mNeedUpdate)
		{
			float t = factor * Time.deltaTime;
			mCurrFogColor = Color32.Lerp(mCurrFogColor, mTargetFogColor, t);
			mCurrFogDensity = Mathf.Lerp(mCurrFogDensity, mTargetFogDensity, t);
			mCurrLinearFogStart = Mathf.Lerp(mCurrLinearFogStart, mTargetLinearFogStart, t);
			mCurrLinearFogEnd = Mathf.Lerp(mCurrLinearFogEnd, mTargetLinerFogEnd, t);
			RenderSettings.fogColor = mCurrFogColor;
			RenderSettings.fogDensity = mCurrFogDensity;
			RenderSettings.fogStartDistance = mCurrLinearFogStart;
			RenderSettings.fogEndDistance = mCurrLinearFogEnd;
			if (mCurrFogColor.Equals(mTargetFogColor) && Mathf.Approximately(mTargetFogDensity, mCurrFogDensity) && Mathf.Approximately(mCurrLinearFogStart, mTargetLinearFogStart) && Mathf.Approximately(mCurrLinearFogEnd, mTargetLinerFogEnd))
			{
				mNeedUpdate = false;
			}
		}
		if (needChangeAmbient)
		{
			AmbientPercent += Time.deltaTime;
			Color ambientLight = Color.Lerp(lastAmbientColor, targetAmbientColor, AmbientPercent);
			RenderSettings.ambientLight = ambientLight;
			if (RenderSettings.ambientLight.Equals(targetAmbientColor))
			{
				needChangeAmbient = false;
				AmbientPercent = 0f;
			}
		}
	}

	public void SetTargetFog(FogTrigger fogTrigger)
	{
		if (!(mFogTrigger == fogTrigger))
		{
			if (!RenderSettings.fog)
			{
				mCurrFogColor = mTargetFogColor;
				mCurrFogDensity = mTargetFogDensity;
				mCurrLinearFogStart = mTargetLinearFogStart;
				mCurrLinearFogEnd = mTargetLinerFogEnd;
			}
			RenderSettings.fog = fogTrigger.m_fog;
			RenderSettings.fogMode = fogTrigger.m_fogMode;
			mTargetFogColor = fogTrigger.m_fogColor;
			mTargetFogDensity = fogTrigger.m_fogDensity;
			mTargetLinearFogStart = fogTrigger.m_linearFogStart;
			mTargetLinerFogEnd = fogTrigger.m_linerFogEnd;
			if (fogTrigger.m_fadeDuration <= 0.1f)
			{
				factor = 1000f;
			}
			else
			{
				factor = 1f / fogTrigger.m_fadeDuration;
			}
			mNeedUpdate = true;
			mFogTrigger = fogTrigger;
		}
	}

	public void Disable()
	{
		RenderSettings.fog = false;
		mNeedUpdate = false;
		mFogTrigger = null;
	}

	public void SetAmbientColor(Color newColor)
	{
		lastAmbientColor = RenderSettings.ambientLight;
		targetAmbientColor = newColor;
		AmbientPercent = 0f;
		needChangeAmbient = true;
	}
}

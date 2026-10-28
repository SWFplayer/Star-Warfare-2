using System;
using UnityEngine;

[Serializable]
public class GlowEffectParameter
{
	public float glowIntensity = 1.5f;

	public int blurIterations = 3;

	public float blurSpread = 0.7f;

	public Color glowTint = new Color(1f, 1f, 1f, 0f);

	public float duration = 0.5f;

	public void Lerp(GlowEffectParameter target, float percert)
	{
		glowIntensity = Mathf.Lerp(glowIntensity, target.glowIntensity, percert);
		blurIterations = (int)(Mathf.Lerp(blurIterations, target.blurIterations, percert) + 0.001f);
		blurSpread = Mathf.Lerp(blurSpread, target.blurSpread, percert);
		glowTint = Color.Lerp(glowTint, target.glowTint, percert);
	}

	public bool Equal(GlowEffectParameter target)
	{
		return Mathf.Approximately(glowIntensity, target.glowIntensity) && Mathf.Approximately(blurSpread, target.blurSpread) && blurIterations == target.blurIterations && glowTint.Equals(target.glowTint);
	}

	public void CopyFrom(GlowEffectParameter other)
	{
		glowIntensity = other.glowIntensity;
		blurIterations = other.blurIterations;
		blurSpread = other.blurSpread;
		glowTint = other.glowTint;
	}
}

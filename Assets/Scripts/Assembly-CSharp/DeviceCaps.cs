using UnityEngine;

public class DeviceCaps
{
	public enum Capbility
	{
		Low = 0,
		General = 1,
		Middle = 2,
		High = 3
	}

	public static Capbility GetCapbility()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			switch (AndroidPluginScript.CheckDevice())
			{
			case 0:
				return Capbility.General;
			case 1:
				if (nvsyscaps.GpuFamily >= nvsyscaps.Nv_TegraGpuFamily.TEGRA_GPU_T4T4i)
				{
					return Capbility.High;
				}
				if (nvsyscaps.GpuFamily == nvsyscaps.Nv_TegraGpuFamily.TEGRA_GPU_T2T3)
				{
					return Capbility.Middle;
				}
				return Capbility.General;
			case 2:
				return Capbility.Low;
			default:
				return Capbility.General;
			}
		}
		return Capbility.High;
	}
}

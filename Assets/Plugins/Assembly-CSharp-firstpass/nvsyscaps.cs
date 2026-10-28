using System.Runtime.InteropServices;
using UnityEngine;

public class nvsyscaps : MonoBehaviour
{
	public enum Nv_TegraSocFamily
	{
		TEGRA_SOC_ERROR = -2,
		TEGRA_SOC_NOTNV = -1,
		TEGRA_SOC_UNKNOWN = 0,
		TEGRA_SOC_T2 = 1,
		TEGRA_SOC_T3 = 2,
		TEGRA_SOC_T4i = 3,
		TEGRA_SOC_T4 = 4,
		TEGRA_SOC_FUTURE = 5
	}

	public enum Nv_TegraGpuFamily
	{
		TEGRA_GPU_ERROR = -2,
		TEGRA_GPU_NOTNV = -1,
		TEGRA_GPU_T2T3 = 0,
		TEGRA_GPU_T4T4i = 1,
		TEGRA_GPU_FUTURE = 2
	}

	private static Nv_TegraSocFamily soc_family = Nv_TegraSocFamily.TEGRA_SOC_ERROR;

	private static Nv_TegraGpuFamily gpu_family = Nv_TegraGpuFamily.TEGRA_GPU_ERROR;

	private static bool initialized;

	public static Nv_TegraSocFamily SocFamily
	{
		get
		{
			Init();
			return soc_family;
		}
	}

	public static Nv_TegraGpuFamily GpuFamily
	{
		get
		{
			Init();
			return gpu_family;
		}
	}

	[DllImport("nvsyscaps")]
	private static extern int nv_init_syscaps();

	[DllImport("nvsyscaps")]
	private static extern int nv_get_soc_family();

	[DllImport("nvsyscaps")]
	private static extern int nv_get_gpu_family();

	private static void Init()
	{
		if (!initialized)
		{
			initialized = true;
			int num = nv_init_syscaps();
			if (num != -1)
			{
				soc_family = (Nv_TegraSocFamily)nv_get_soc_family();
				gpu_family = (Nv_TegraGpuFamily)nv_get_gpu_family();
			}
		}
	}
}

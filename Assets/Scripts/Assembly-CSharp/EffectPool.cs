using UnityEngine;

public class EffectPool : MonoBehaviour
{
	private ObjectPool[] mEffectPools;

	public static EffectPool mInstance;

	public static EffectPool GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = GameObject.Find("_EffectRoot").GetComponent<EffectPool>();
		}
		return mInstance;
	}

	public void Start()
	{
		if (mEffectPools == null)
		{
			mEffectPools = new ObjectPool[60];
			for (int i = 0; i < mEffectPools.Length; i++)
			{
				mEffectPools[i] = new ObjectPool();
			}
			GameObject prefab = Resources.Load(EffectName.W1_SHOOT_MARK) as GameObject;
			mEffectPools[0].Init(EffectPoolType.W1_SHOOT_MARK.ToString(), prefab, 1, 0.22f);
			GameObject prefab2 = Resources.Load(EffectName.W1_TRAJECTORY) as GameObject;
			mEffectPools[1].Init(EffectPoolType.W1_TRAJECTORY.ToString(), prefab2, 1, 0.8f);
			GameObject prefab3 = Resources.Load(EffectName.W4_SHOOT_MARK) as GameObject;
			mEffectPools[16].Init(EffectPoolType.W4_SHOOT_MARK.ToString(), prefab3, 1, 0.72f);
			GameObject prefab4 = Resources.Load(EffectName.W4_TRAJECTORY) as GameObject;
			mEffectPools[17].Init(EffectPoolType.W4_TRAJECTORY.ToString(), prefab4, 1, 0.8f);
			GameObject prefab5 = Resources.Load(EffectName.W2_SPARK) as GameObject;
			mEffectPools[2].Init(EffectPoolType.W2_SPARK.ToString(), prefab5, 1, 1.2f);
			GameObject prefab6 = Resources.Load(EffectName.W2_SHOOT_MARK) as GameObject;
			mEffectPools[3].Init(EffectPoolType.W2_SHOOT_MARK.ToString(), prefab6, 1, 0.8f);
			GameObject prefab7 = Resources.Load(EffectName.W2_TRAJECTORY) as GameObject;
			mEffectPools[4].Init(EffectPoolType.W2_TRAJECTORY.ToString(), prefab7, 1, 1.2f);
			GameObject prefab8 = Resources.Load(EffectName.W3_ROCKET) as GameObject;
			mEffectPools[5].Init(EffectPoolType.W3_ROCKET.ToString(), prefab8, 1, 10f);
			mEffectPools[5].Recycle = false;
			GameObject prefab9 = Resources.Load(EffectName.W3_ROCKET_BOOM_5) as GameObject;
			mEffectPools[6].Init(EffectPoolType.W3_ROCKET_BOOM_5.ToString(), prefab9, 1, 5f);
			GameObject prefab10 = Resources.Load(EffectName.W3_ROCKET_BOOM_7) as GameObject;
			mEffectPools[7].Init(EffectPoolType.W3_ROCKET_BOOM_7.ToString(), prefab10, 1, 5f);
			GameObject prefab11 = Resources.Load(EffectName.W3_ROCKET_BOOM_9) as GameObject;
			mEffectPools[8].Init(EffectPoolType.W3_ROCKET_BOOM_9.ToString(), prefab11, 1, 5f);
			GameObject prefab12 = Resources.Load(EffectName.W5_SPARK) as GameObject;
			mEffectPools[12].Init(EffectPoolType.W5_SPARK.ToString(), prefab12, 1, 1.2f);
			GameObject prefab13 = Resources.Load(EffectName.W5_SHOOT_MARK) as GameObject;
			mEffectPools[13].Init(EffectPoolType.W5_SHOOT_MARK.ToString(), prefab13, 1, 0.8f);
			GameObject prefab14 = Resources.Load(EffectName.W5_TRAJECTORY) as GameObject;
			mEffectPools[14].Init(EffectPoolType.W5_TRAJECTORY.ToString(), prefab14, 1, 2f);
			GameObject prefab15 = Resources.Load(EffectName.W29_BULLET) as GameObject;
			mEffectPools[48].Init(EffectPoolType.W29_TRAJECTORY.ToString(), prefab15, 1, 2f);
			GameObject prefab16 = Resources.Load(EffectName.W6_TRAJECTORY) as GameObject;
			mEffectPools[19].Init(EffectPoolType.W6_TRAJECTORY.ToString(), prefab16, 1, 0.8f);
			GameObject prefab17 = Resources.Load(EffectName.W8_FIST) as GameObject;
			mEffectPools[21].Init(EffectPoolType.W8_FIST.ToString(), prefab17, 1, 10f);
			mEffectPools[21].Recycle = false;
			GameObject prefab18 = Resources.Load(EffectName.W8_FIST_EXP) as GameObject;
			mEffectPools[22].Init(EffectPoolType.W8_FIST_EXP.ToString(), prefab18, 1, 10f);
			mEffectPools[22].Recycle = false;
			GameObject prefab19 = Resources.Load(EffectName.W8_FIST_BOOM) as GameObject;
			mEffectPools[23].Init(EffectPoolType.W8_FIST_BOOM.ToString(), prefab19, 1, 2f);
			GameObject prefab20 = Resources.Load(EffectName.W8_FIST_BOOM_EXPLOSION) as GameObject;
			mEffectPools[24].Init(EffectPoolType.W8_FIST_BOOM_EXP.ToString(), prefab20, 1, 2f);
			GameObject prefab21 = Resources.Load(EffectName.V1_TRAJECTORY) as GameObject;
			mEffectPools[9].Init(EffectPoolType.V1_TRAJECTORY.ToString(), prefab21, 1, 0.8f);
			GameObject prefab22 = Resources.Load(EffectName.V2_SHOOT_MARK) as GameObject;
			mEffectPools[10].Init(EffectPoolType.V2_SHOOT_MARK.ToString(), prefab22, 1, 0.22f);
			GameObject prefab23 = Resources.Load(EffectName.V2_TRAJECTORY) as GameObject;
			mEffectPools[11].Init(EffectPoolType.V2_TRAJECTORY.ToString(), prefab23, 1, 0.8f);
			GameObject prefab24 = Resources.Load(EffectName.WB_EXPLODE) as GameObject;
			mEffectPools[18].Init(EffectPoolType.WB_VOIDGRENADE.ToString(), prefab24, 1, 5f);
			GameObject prefab25 = Resources.Load(EffectName.W7_SHOOT_MARK) as GameObject;
			mEffectPools[20].Init(EffectPoolType.W7_SHOOT_MARK.ToString(), prefab25, 1, 1f);
			GameObject prefab26 = Resources.Load(EffectName.VD5_FLASH) as GameObject;
			mEffectPools[25].Init(EffectPoolType.VD5_FLASH.ToString(), prefab26, 1, 2f);
			GameObject prefab27 = Resources.Load(EffectName.VOID_SHOP_FLASH) as GameObject;
			mEffectPools[26].Init(EffectPoolType.VD5_FLASH_SMALL.ToString(), prefab27, 1, 2f);
			GameObject prefab28 = Resources.Load(EffectName.W7_TRAJECTORY) as GameObject;
			mEffectPools[27].Init(EffectPoolType.W7_TRAJECTORY.ToString(), prefab28, 1, 1f);
			GameObject prefab29 = Resources.Load(EffectName.W15_ROCKET) as GameObject;
			mEffectPools[28].Init(EffectPoolType.W15_ROCKET.ToString(), prefab29, 1, 10f);
			mEffectPools[28].Recycle = false;
			GameObject prefab30 = Resources.Load(EffectName.W9_BULLET) as GameObject;
			mEffectPools[29].Init(EffectPoolType.W9_BULLET.ToString(), prefab30, 1, 2f);
			GameObject prefab31 = Resources.Load(EffectName.W9_SPARK) as GameObject;
			mEffectPools[31].Init(EffectPoolType.W9_SPARK.ToString(), prefab31, 1, 1f);
			GameObject prefab32 = Resources.Load(EffectName.W24) as GameObject;
			mEffectPools[32].Init(EffectPoolType.W24.ToString(), prefab32, 1, 2f);
			GameObject prefab33 = Resources.Load(EffectName.W24_SPARK) as GameObject;
			mEffectPools[33].Init(EffectPoolType.W24_SPARK.ToString(), prefab33, 1, 1f);
			GameObject prefab34 = Resources.Load(EffectName.W24_SHOOTMARK) as GameObject;
			mEffectPools[34].Init(EffectPoolType.W24_SHOOTMARK.ToString(), prefab34, 1, 1f);
			GameObject prefab35 = Resources.Load(EffectName.W9_EXPLOSION) as GameObject;
			mEffectPools[30].Init(EffectPoolType.W9_EXPLOSION.ToString(), prefab35, 1, 1f);
			GameObject prefab36 = Resources.Load(EffectName.W17) as GameObject;
			mEffectPools[35].Init(EffectPoolType.W17_TRAJECTORY.ToString(), prefab36, 1, 1f);
			GameObject prefab37 = Resources.Load(EffectName.W17_SHOOTMARK) as GameObject;
			mEffectPools[36].Init(EffectPoolType.W17_SHOOTMARK.ToString(), prefab37, 1, 1f);
			GameObject prefab38 = Resources.Load(EffectName.SHOP_GLOW) as GameObject;
			mEffectPools[37].Init(EffectPoolType.SHOP_GLOW.ToString(), prefab38, 1, 1f);
			GameObject prefab39 = Resources.Load(EffectName.MISSION_FAILED_END) as GameObject;
			mEffectPools[40].Init(EffectPoolType.PORTAL_OFF.ToString(), prefab39, 1, 2f);
			GameObject prefab40 = Resources.Load(EffectName.Respawn1) as GameObject;
			mEffectPools[38].Init(EffectPoolType.RESPAWN_1.ToString(), prefab40, 1, 3f);
			GameObject prefab41 = Resources.Load(EffectName.Respawn2) as GameObject;
			mEffectPools[39].Init(EffectPoolType.RESPAWN_2.ToString(), prefab41, 1, 3f);
			GameObject prefab42 = Resources.Load(EffectName.W25_BULLET) as GameObject;
			mEffectPools[41].Init(EffectPoolType.W25_BULLET.ToString(), prefab42, 1, 0.8f);
			GameObject prefab43 = Resources.Load(EffectName.W25_SHOOTMARK) as GameObject;
			mEffectPools[42].Init(EffectPoolType.W25_SHOOTMARK.ToString(), prefab43, 1, 0.22f);
			GameObject prefab44 = Resources.Load(EffectName.W27_BULLET) as GameObject;
			mEffectPools[43].Init(EffectPoolType.W27_BULLET.ToString(), prefab44, 1, 2f);
			GameObject prefab45 = Resources.Load(EffectName.VD6_SHIELD_EXP) as GameObject;
			mEffectPools[44].Init(EffectPoolType.VOID_SHIELD_EXP.ToString(), prefab45, 1, 3f);
			GameObject prefab46 = Resources.Load(EffectName.BOW_ROCKET) as GameObject;
			mEffectPools[45].Init(EffectPoolType.BOW_ROCKET.ToString(), prefab46, 1, 10f);
			mEffectPools[45].Recycle = false;
			GameObject prefab47 = Resources.Load(EffectName.W28_ROCKET) as GameObject;
			mEffectPools[49].Init(EffectPoolType.W28_ROCKET.ToString(), prefab47, 1, 10f);
			mEffectPools[49].Recycle = false;
			GameObject prefab48 = Resources.Load(EffectName.W28_ROCKET1) as GameObject;
			mEffectPools[50].Init(EffectPoolType.W28_ROCKET1.ToString(), prefab48, 1, 10f);
			mEffectPools[50].Recycle = false;
			GameObject prefab49 = Resources.Load(EffectName.W28_ROCKET2) as GameObject;
			mEffectPools[51].Init(EffectPoolType.W28_ROCKET2.ToString(), prefab49, 1, 10f);
			mEffectPools[51].Recycle = false;
			GameObject prefab50 = Resources.Load(EffectName.MINE) as GameObject;
			mEffectPools[46].Init(EffectPoolType.MINE.ToString(), prefab50, 1, 0f);
			mEffectPools[46].doNotDestruct = true;
			mEffectPools[46].Recycle = false;
			GameObject prefab51 = Resources.Load(EffectName.MINEPROJECTILE) as GameObject;
			mEffectPools[47].Init(EffectPoolType.MINE_PROJECTILE.ToString(), prefab51, 1, 10f);
			mEffectPools[47].Recycle = false;
			GameObject prefab52 = Resources.Load(EffectName.STUN_EFFECT) as GameObject;
			mEffectPools[52].Init(EffectPoolType.STUN_EFFECT.ToString(), prefab52, 1, 10f);
			GameObject prefab53 = Resources.Load(EffectName.W31_ROCKET) as GameObject;
			mEffectPools[53].Init(EffectPoolType.W31_ROCKET.ToString(), prefab53, 1, 10f);
			mEffectPools[53].Recycle = false;
			GameObject prefab54 = Resources.Load(EffectName.W31_ROCKET_BOOM) as GameObject;
			mEffectPools[54].Init(EffectPoolType.W31_ROCKET_BOOM.ToString(), prefab54, 1, 5f);
			GameObject prefab55 = Resources.Load(EffectName.W30_TRAJECTORY) as GameObject;
			mEffectPools[55].Init(EffectPoolType.W30_TRAJECTORY.ToString(), prefab55, 1, 0.8f);
			GameObject prefab56 = Resources.Load(EffectName.W32_TRAJECTORY_01) as GameObject;
			mEffectPools[56].Init(EffectPoolType.W32_TRAJECTORY_01.ToString(), prefab56, 1, 0.8f);
			GameObject prefab57 = Resources.Load(EffectName.W32_TRAJECTORY_02) as GameObject;
			mEffectPools[57].Init(EffectPoolType.W32_TRAJECTORY_02.ToString(), prefab57, 1, 100f);
			mEffectPools[57].doNotDestruct = true;
			mEffectPools[57].CanUseFirstOne = false;
			GameObject prefab58 = Resources.Load(EffectName.W32_SHOOT_MARK) as GameObject;
			mEffectPools[58].Init(EffectPoolType.W32_SHOOT_MARK.ToString(), prefab58, 1, 0.72f);
			GameObject prefab59 = Resources.Load(EffectName.W32_SHOOT_MARK02) as GameObject;
			mEffectPools[59].Init(EffectPoolType.W32_SHOOT_MARK02.ToString(), prefab59, 1, 0.72f);
			for (int j = 0; j < mEffectPools.Length; j++)
			{
				mEffectPools[j].SetRoot(base.gameObject);
			}
		}
	}

	private void Update()
	{
		Loop(Time.deltaTime);
	}

	public ObjectPool GetEffectPool(EffectPoolType type)
	{
		return mEffectPools[(int)type];
	}

	public void Loop(float deltaTime)
	{
		ObjectPool[] array = mEffectPools;
		foreach (ObjectPool objectPool in array)
		{
			if (objectPool != null)
			{
				objectPool.AutoDestruct();
			}
		}
	}
}

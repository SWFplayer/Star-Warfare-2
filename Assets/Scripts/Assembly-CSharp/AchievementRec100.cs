public class AchievementRec100 : IRecordPart
{
	public static string mVersion = "100";

	private AchievementState mState;

	public AchievementRec100(AchievementState state)
	{
		mState = state;
	}

	public void SaveData(BytesBuffer bb)
	{
		bb.AddString("120");
		bb.AddInt(mState.GetScore());
		AchievementInfo[] achievements = mState.GetAchievements();
		bb.AddInt(AchievementState.ACHIEVEMENT_COUNT);
		for (int i = 0; i < achievements.Length; i++)
		{
			bb.AddBool(achievements[i].submitting);
		}
		for (int j = 0; j < achievements.Length; j++)
		{
			bb.AddBool(achievements[j].complete);
		}
		int[] weaponTypeWins = mState.GetWeaponTypeWins(0);
		bb.AddByte(8);
		for (int k = 0; k < 8; k++)
		{
			bb.AddInt(weaponTypeWins[k]);
		}
		weaponTypeWins = mState.GetWeaponTypeWins(1);
		for (int l = 0; l < 8; l++)
		{
			bb.AddInt(weaponTypeWins[l]);
		}
		int[] weaponUsed = mState.GetWeaponUsed(0);
		bb.AddByte(33);
		for (int m = 0; m < 33; m++)
		{
			bb.AddInt(weaponUsed[m]);
		}
		weaponUsed = mState.GetWeaponUsed(1);
		for (int n = 0; n < 33; n++)
		{
			bb.AddInt(weaponUsed[n]);
		}
		int[] missionPlayed = mState.GetMissionPlayed();
		int[] heroMissionPlayed = mState.GetHeroMissionPlayed();
		bb.AddByte((byte)Global.TOTAL_STAGE);
		bb.AddByte((byte)Global.TOTAL_SUB_STAGE);
		for (int num = 0; num < Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE; num++)
		{
			bb.AddInt(missionPlayed[num]);
		}
		for (int num2 = 0; num2 < Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE; num2++)
		{
			bb.AddInt(heroMissionPlayed[num2]);
		}
		int[] missionCompleted = mState.GetMissionCompleted();
		int[] heroMissionCompleted = mState.GetHeroMissionCompleted();
		for (int num3 = 0; num3 < Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE; num3++)
		{
			bb.AddInt(missionCompleted[num3]);
		}
		for (int num4 = 0; num4 < Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE; num4++)
		{
			bb.AddInt(heroMissionCompleted[num4]);
		}
		int[] vSModePlayed = mState.GetVSModePlayed();
		bb.AddByte(6);
		for (int num5 = 0; num5 < 6; num5++)
		{
			bb.AddInt(vSModePlayed[num5]);
		}
		int[] normalEnemyKills = mState.GetNormalEnemyKills();
		bb.AddByte(9);
		for (int num6 = 0; num6 < 9; num6++)
		{
			bb.AddInt(normalEnemyKills[num6]);
		}
		normalEnemyKills = mState.GetEliteEnemyKills();
		for (int num7 = 0; num7 < 9; num7++)
		{
			bb.AddInt(normalEnemyKills[num7]);
		}
		int[] bossKills = mState.GetBossKills();
		for (int num8 = 0; num8 < Global.TOTAL_STAGE; num8++)
		{
			bb.AddInt(bossKills[num8]);
		}
		bb.AddInt(mState.GetVoidCancels());
		bb.AddInt(mState.GetPlayerKills());
		bb.AddInt(mState.GetDoubleKill());
		bb.AddInt(mState.GetTripleKill());
		int[] vSModeWins = mState.GetVSModeWins();
		int[] specialVSModeWins = mState.GetSpecialVSModeWins();
		int[] saturdayWinNum = mState.GetSaturdayWinNum();
		bb.AddByte(6);
		for (int num9 = 0; num9 < 6; num9++)
		{
			bb.AddInt(vSModeWins[num9]);
		}
		for (int num10 = 0; num10 < 6; num10++)
		{
			bb.AddInt(specialVSModeWins[num10]);
		}
		for (int num11 = 0; num11 < 6; num11++)
		{
			bb.AddInt(saturdayWinNum[num11]);
		}
		bool[] pickUp = mState.GetPickUp();
		bb.AddByte(17);
		for (int num12 = 0; num12 < 17; num12++)
		{
			bb.AddBool(pickUp[num12]);
		}
		bb.AddInt(mState.GetTotalCash());
		bb.AddInt(mState.GetVoidMat());
		bb.AddInt(mState.GetMaxDamage());
		bb.AddInt(mState.GetMaxGS());
		bb.AddInt(mState.GetPVEDeath());
		bb.AddInt(mState.GetPVPDeath());
		bb.AddInt(mState.GetOccupationNum());
		bb.AddInt(mState.GetFlagNum());
		bb.AddInt(mState.GetFBNum());
		bb.AddInt(mState.GetComboKillNum());
		bb.AddInt(mState.GetAttackCount());
		bb.AddInt(mState.GetHitEnemyCount());
		bb.AddInt(mState.GetVipCount());
		int[] itemUsed = mState.GetItemUsed();
		bb.AddByte(22);
		for (int num13 = 0; num13 < 22; num13++)
		{
			bb.AddInt(itemUsed[num13]);
		}
		int[] flaskUsed = mState.GetFlaskUsed();
		bb.AddByte(4);
		for (int num14 = 0; num14 < 4; num14++)
		{
			bb.AddInt(flaskUsed[num14]);
		}
		bb.AddInt(mState.GetMaxComboWins());
		bb.AddInt(mState.GetCurrentComboWins());
		bb.AddByte((byte)mState.GetRank());
		bb.AddByte((byte)mState.GetPerkNum());
		bool[] missionAllStar = mState.GetMissionAllStar();
		bool[] heroMissionAllStar = mState.GetHeroMissionAllStar();
		for (int num15 = 0; num15 < Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE; num15++)
		{
			bb.AddBool(missionAllStar[num15]);
		}
		for (int num16 = 0; num16 < Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE; num16++)
		{
			bb.AddBool(heroMissionAllStar[num16]);
		}
		bb.AddInt(mState.GetFlagerKillsNum());
		bb.AddInt(mState.GetKillWithFlagNum());
		bb.AddByte((byte)Global.TOTAL_SET);
		int[] armorSetWins = mState.GetArmorSetWins();
		for (int num17 = 0; num17 < Global.TOTAL_SET; num17++)
		{
			bb.AddInt(armorSetWins[num17]);
		}
		bb.AddInt(mState.GetSawKill());
		int[] infiniteKills = mState.GetInfiniteKills();
		for (int num18 = 0; num18 < Global.TOTAL_STAGE; num18++)
		{
			bb.AddInt(infiniteKills[num18]);
		}
		int[] heroInfiniteKills = mState.GetHeroInfiniteKills();
		for (int num19 = 0; num19 < Global.TOTAL_STAGE; num19++)
		{
			bb.AddInt(heroInfiniteKills[num19]);
		}
		bb.AddInt(mState.GetPVEMaxCombo());
	}

	public void LoadData(BytesBuffer bb)
	{
		mState.SetScore(bb.ReadInt());
		AchievementInfo[] achievements = mState.GetAchievements();
		int num = bb.ReadInt();
		for (int i = 0; i < num; i++)
		{
			achievements[i].submitting = bb.ReadBool();
		}
		for (int j = 0; j < num; j++)
		{
			achievements[j].complete = bb.ReadBool();
			if (achievements[j].complete && achievements[j].TitleId != 0)
			{
				mState.AddTitleList(achievements[j].TitleId);
			}
		}
		int[] weaponTypeWins = mState.GetWeaponTypeWins(0);
		byte b = bb.ReadByte();
		for (int k = 0; k < b; k++)
		{
			weaponTypeWins[k] = bb.ReadInt();
		}
		weaponTypeWins = mState.GetWeaponTypeWins(1);
		for (int l = 0; l < b; l++)
		{
			weaponTypeWins[l] = bb.ReadInt();
		}
		mState.SetWeaponTypeWins();
		int[] weaponUsed = mState.GetWeaponUsed(0);
		byte b2 = bb.ReadByte();
		for (int m = 0; m < b2; m++)
		{
			weaponUsed[m] = bb.ReadInt();
		}
		weaponUsed = mState.GetWeaponUsed(1);
		for (int n = 0; n < b2; n++)
		{
			weaponUsed[n] = bb.ReadInt();
		}
		int[] missionPlayed = mState.GetMissionPlayed();
		int[] heroMissionPlayed = mState.GetHeroMissionPlayed();
		int num2 = bb.ReadByte();
		int num3 = bb.ReadByte();
		for (int num4 = 0; num4 < num2 * num3; num4++)
		{
			missionPlayed[num4] = bb.ReadInt();
		}
		for (int num5 = 0; num5 < num2 * num3; num5++)
		{
			heroMissionPlayed[num5] = bb.ReadInt();
		}
		int[] missionCompleted = mState.GetMissionCompleted();
		int[] heroMissionCompleted = mState.GetHeroMissionCompleted();
		for (int num6 = 0; num6 < num2 * num3; num6++)
		{
			missionCompleted[num6] = bb.ReadInt();
		}
		for (int num7 = 0; num7 < num2 * num3; num7++)
		{
			heroMissionCompleted[num7] = bb.ReadInt();
		}
		int[] vSModePlayed = mState.GetVSModePlayed();
		int num8 = bb.ReadByte();
		for (int num9 = 0; num9 < num8; num9++)
		{
			vSModePlayed[num9] = bb.ReadInt();
		}
		int[] normalEnemyKills = mState.GetNormalEnemyKills();
		byte b3 = bb.ReadByte();
		for (int num10 = 0; num10 < b3; num10++)
		{
			normalEnemyKills[num10] = bb.ReadInt();
		}
		normalEnemyKills = mState.GetEliteEnemyKills();
		for (int num11 = 0; num11 < b3; num11++)
		{
			normalEnemyKills[num11] = bb.ReadInt();
		}
		mState.SetEnemyKills();
		int[] bossKills = mState.GetBossKills();
		for (int num12 = 0; num12 < num2; num12++)
		{
			bossKills[num12] = bb.ReadInt();
		}
		mState.SetVoidCancels(bb.ReadInt());
		mState.SetPlayerKills(bb.ReadInt());
		mState.SetDoubleKill(bb.ReadInt());
		mState.SetTripleKill(bb.ReadInt());
		byte b4 = bb.ReadByte();
		int[] vSModeWins = mState.GetVSModeWins();
		int[] specialVSModeWins = mState.GetSpecialVSModeWins();
		int[] saturdayWinNum = mState.GetSaturdayWinNum();
		for (int num13 = 0; num13 < b4; num13++)
		{
			vSModeWins[num13] = bb.ReadInt();
		}
		for (int num14 = 0; num14 < b4; num14++)
		{
			specialVSModeWins[num14] = bb.ReadInt();
		}
		for (int num15 = 0; num15 < b4; num15++)
		{
			saturdayWinNum[num15] = bb.ReadInt();
		}
		mState.SetVSModeWins();
		mState.SetSpecialVSModeWins();
		byte b5 = bb.ReadByte();
		bool[] pickUp = mState.GetPickUp();
		for (int num16 = 0; num16 < b5; num16++)
		{
			pickUp[num16] = bb.ReadBool();
		}
		mState.SetPickUp();
		mState.SetTotalCash(bb.ReadInt());
		mState.SetVoidMat(bb.ReadInt());
		mState.SetMaxDamage(bb.ReadInt());
		mState.SetMaxGS(bb.ReadInt());
		mState.SetPVEDeath(bb.ReadInt());
		mState.SetPVPDeath(bb.ReadInt());
		mState.SetOccupationNum(bb.ReadInt());
		mState.SetFlagNum(bb.ReadInt());
		mState.SetFBNum(bb.ReadInt());
		mState.SetComboKillNum(bb.ReadInt());
		mState.SetAttackCount(bb.ReadInt());
		mState.SetHitEnemyCount(bb.ReadInt());
		mState.SetVipCount(bb.ReadInt());
		int[] itemUsed = mState.GetItemUsed();
		int num17 = bb.ReadByte();
		for (int num18 = 0; num18 < num17; num18++)
		{
			itemUsed[num18] = bb.ReadInt();
		}
		int[] flaskUsed = mState.GetFlaskUsed();
		int num19 = bb.ReadByte();
		for (int num20 = 0; num20 < num19; num20++)
		{
			flaskUsed[num20] = bb.ReadInt();
		}
		mState.SetMaxComboWins(bb.ReadInt());
		mState.SetCurrentComboWins(bb.ReadInt());
		mState.SetRank(bb.ReadByte());
		mState.SetPerkNum(bb.ReadByte());
		bool[] missionAllStar = mState.GetMissionAllStar();
		bool[] heroMissionAllStar = mState.GetHeroMissionAllStar();
		for (int num21 = 0; num21 < num2 * num3; num21++)
		{
			missionAllStar[num21] = bb.ReadBool();
		}
		for (int num22 = 0; num22 < num2 * num3; num22++)
		{
			heroMissionAllStar[num22] = bb.ReadBool();
		}
		mState.SetFlagerKillsNum(bb.ReadInt());
		mState.SetKillWithFlagNum(bb.ReadInt());
		int num23 = bb.ReadByte();
		int[] armorSetWins = mState.GetArmorSetWins();
		for (int num24 = 0; num24 < num23; num24++)
		{
			armorSetWins[num24] = bb.ReadInt();
		}
		mState.SetSawKill(bb.ReadInt());
		int[] infiniteKills = mState.GetInfiniteKills();
		for (int num25 = 0; num25 < num2; num25++)
		{
			infiniteKills[num25] = bb.ReadInt();
		}
		int[] heroInfiniteKills = mState.GetHeroInfiniteKills();
		for (int num26 = 0; num26 < num2; num26++)
		{
			heroInfiniteKills[num26] = bb.ReadInt();
		}
		mState.SetPVEMaxCombo(bb.ReadInt());
	}
}

public class GameMode
{
	public NetworkType TypeOfNetwork { get; set; }

	public Mode ModePlay { get; set; }

	public GameMode(NetworkType nType, Mode gMode)
	{
		TypeOfNetwork = nType;
		ModePlay = gMode;
	}

	public bool IsSingle()
	{
		if (TypeOfNetwork == NetworkType.Single)
		{
			return true;
		}
		return false;
	}

	public bool IsMultiPlayer()
	{
		return !IsSingle();
	}

	public bool IsVSMode()
	{
		if (IsMultiPlayer() && (ModePlay == Mode.VS_FFA || ModePlay == Mode.VS_TDM || ModePlay == Mode.VS_CTF_TDM || ModePlay == Mode.VS_CTF_FFA || ModePlay == Mode.VS_KOTH || ModePlay == Mode.Domination || ModePlay == Mode.GIANT_BETRAYAL))
		{
			return true;
		}
		return false;
	}

	public bool IsUsePvpCustomizeMode()
	{
		return IsVSMode() && ModePlay != Mode.GIANT_BETRAYAL;
	}

	public bool IsVSMode(Mode mode)
	{
		if (IsMultiPlayer() && (mode == Mode.VS_FFA || mode == Mode.VS_TDM || mode == Mode.VS_CTF_TDM || mode == Mode.VS_KOTH || mode == Mode.Domination || mode == Mode.GIANT_BETRAYAL))
		{
			return true;
		}
		return false;
	}

	public bool IsCatchTheFlagMode()
	{
		if (IsMultiPlayer() && (ModePlay == Mode.VS_CTF_TDM || ModePlay == Mode.VS_CTF_FFA))
		{
			return true;
		}
		return false;
	}

	public bool IsCoopMode()
	{
		if (IsMultiPlayer() && (ModePlay == Mode.Survival || ModePlay == Mode.Boss || ModePlay == Mode.BOSS_FINAL))
		{
			return true;
		}
		return false;
	}

	public bool IsSurvivalMode()
	{
		return ModePlay == Mode.Survival;
	}

	public bool IsTeamMode()
	{
		if (IsMultiPlayer() && (ModePlay == Mode.VS_TDM || ModePlay == Mode.VS_CTF_TDM || ModePlay == Mode.VS_KOTH || ModePlay == Mode.Domination || ModePlay == Mode.GIANT_BETRAYAL))
		{
			return true;
		}
		return false;
	}

	public bool IsCoopMode(Mode gameMode)
	{
		if (IsMultiPlayer() && (gameMode == Mode.Survival || gameMode == Mode.Boss || gameMode == Mode.BOSS_FINAL))
		{
			return true;
		}
		return false;
	}

	public bool IsBossVSPlayer()
	{
		return ModePlay == Mode.GIANT_BETRAYAL;
	}

	public bool IsPlayerVSPlayer()
	{
		return ModePlay == Mode.VS_FFA || ModePlay == Mode.VS_TDM || ModePlay == Mode.VS_CTF_TDM || ModePlay == Mode.VS_CTF_FFA || ModePlay == Mode.VS_KOTH || ModePlay == Mode.Domination;
	}

	public bool IsVsMode(byte mode)
	{
		return mode == 4 || mode == 5 || mode == 6 || mode == 7 || mode == 9 || mode == 12 || mode == 13;
	}
}

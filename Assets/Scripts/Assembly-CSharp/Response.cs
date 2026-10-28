using UnityEngine;

public abstract class Response
{
	public short responseID;

	public static Response CreateResponse(short responseID)
	{
		Response result = null;
		switch (responseID)
		{
		case 1:
			result = new ServerExceptionResponse();
			break;
		case 2:
			result = new PlayerLogoutResponse();
			break;
		case 7:
			result = new GetRoomListResponse();
			break;
		case 4:
			result = new CreateRoomResponse();
			break;
		case 8:
			result = new GetRoomDataResponse();
			break;
		case 5:
			result = new JoinRoomResponse();
			break;
		case 6:
			result = new LeaveRoomResponse();
			break;
		case 9:
			result = new StartGameResponse();
			break;
		case 10:
			result = new SetMasterPlayerResponse();
			break;
		case 132:
			result = new PlayerSpawnResponse();
			break;
		case 12:
			result = new RoleLoginResponse();
			break;
		case 15:
			result = new UploadDataResponse();
			break;
		case 13:
			result = new RoomTimeSynchronizeResponse();
			break;
		case 100:
			result = new TimeSynchronizeResponse();
			break;
		case 14:
			result = new GuestLoginResponse();
			break;
		case 20:
			result = new ChangeSeatResponse();
			break;
		case 22:
			result = new GetBattleStateResponse();
			break;
		case 50:
			result = new TestRespones();
			break;
		case 101:
			result = new GetSceneStateResponse();
			break;
		case 158:
			result = new ChangeToBossSceneResponse();
			break;
		case 102:
			result = new SendTransformStateResponse();
			break;
		case 103:
			result = new SendPlayerInputResponse();
			break;
		case 104:
			result = new EnemySpawnResponse();
			break;
		case 105:
			result = new EnemyChangeTargetResponse();
			break;
		case 106:
			result = new EnemyStateResponse();
			break;
		case 108:
			result = new EnemyOnHitResponse();
			break;
		case 110:
			result = new PlayerOnHitResponse();
			break;
		case 111:
			result = new PlayerChangeWeaponResponse();
			break;
		case 113:
			result = new PlayerLeaveGameResponse();
			break;
		case 114:
			result = new PlayerFireRocketResponse();
			break;
		case 115:
			result = new PlayerHitPlayerResponse();
			break;
		case 116:
			result = new EnemyShotResponse();
			break;
		case 117:
			result = new PlayerUseItemResponse();
			break;
		case 119:
			result = new ItemSpawnResponse();
			break;
		case 150:
			result = new ItemSpawnInRandomResponse();
			break;
		case 151:
			result = new SendPickUpInfoResponse();
			break;
		case 120:
			result = new PickUpItemResponse();
			break;
		case 121:
			result = new PlayerOnKnockedResponse();
			break;
		case 122:
			result = new PlayerUploadStatisticsResponse();
			break;
		case 123:
			result = new PlayerBuffResponse();
			break;
		case 124:
			result = new SendPlayerShootAngleVResponse();
			break;
		case 125:
			result = new PlayerRebirthResponse();
			break;
		case 126:
			result = new PlayerKillPlayerResponse();
			break;
		case 127:
			result = new VSScoreResponse();
			break;
		case 128:
			result = new VSGameEndResponse();
			break;
		case 129:
			result = new VSGameAutoBalanceResponse();
			break;
		case 130:
			result = new VSUploadStatisticsResponse();
			break;
		case 131:
			result = new ReStartGameResponse();
			break;
		case 133:
			result = new SendVSTimeResponse();
			break;
		case 27:
			result = new GetMithrilPriceResponse();
			break;
		case 29:
			result = new UploadLoadoutResponse();
			break;
		case 134:
			result = new DropTheFlagResponse();
			break;
		case 135:
			result = new CatchTheFlagResponse();
			break;
		case 136:
			result = new GetTheFlagScoreResponse();
			break;
		case 137:
			result = new SendFlagTimeResponse();
			break;
		case 138:
			result = new RequestFlagPositionResponse();
			break;
		case 139:
			result = new SetPlayerHealthResponse();
			break;
		case 141:
			result = new SendPointInfoResponse();
			break;
		case 140:
			result = new ChangePointControllerResponse();
			break;
		case 153:
			result = new VehicleGetOffResponse();
			break;
		case 157:
			result = new PlayerGetOnVehicleResponse();
			break;
		case 60:
			result = new SwVoidFirstBossStaticMaterialsResponse();
			break;
		case 61:
			result = new SwVoidCheckServerResponse();
			break;
		case 160:
			result = new SwVoidSpawnResponse();
			break;
		case 62:
			result = new SwVoidCreateResponse();
			break;
		case 63:
			result = new SwVoidGetDataResponse();
			break;
		case 64:
			result = new SwVoidIapChestResponse();
			break;
		case 65:
			result = new SwVoidGetRewardResponse();
			break;
		case 164:
			result = new SwVoidGetRandomDropResponse();
			break;
		case 165:
			result = new SwVoidGetRewardResponse();
			break;
		case 66:
			result = new SwVoidResolveResponse();
			break;
		case 67:
			result = new SwVoidUpgradeResponse();
			break;
		case 68:
			result = new SwVoidUpgradeConfirmResponse();
			break;
		case 69:
			result = new SwVoidAddSlotResponse();
			break;
		case 169:
			result = new SwVoidStateResponse();
			break;
		case 170:
			result = new BlackHoleResponse();
			break;
		case 171:
			result = new NeighborPlayerResponse();
			break;
		case 173:
			result = new SwVoidShieldResponse();
			break;
		case 172:
			result = new SwVoidRemoteGetMaterialCountResponse();
			break;
		case 180:
			result = new SetPlayerMaxHpResponse();
			break;
		case 181:
			result = new SendGameStatistics();
			break;
		case 182:
			result = new SendPlayerVsStatisticsResponse();
			break;
		case 190:
			result = new PlayerSpawnVehicleResponse();
			break;
		case 199:
			result = new EnemyStateDataResponse();
			break;
		case 200:
			result = new SlasherStickResponse();
			break;
		case 201:
			result = new SlasherBladeWindResponse();
			break;
		case 202:
			result = new CreatureTeleportResponse();
			break;
		case 203:
			result = new FrostStoneRemoveIceArmorResponse();
			break;
		case 204:
			result = new BossCoopResponse();
			break;
		case 205:
			result = new FunnelResponse();
			break;
		case 206:
			result = new EnemySyncResponse();
			break;
		case 250:
			result = new PlayerBossChangeTargetPositionResponse();
			break;
		case 251:
			result = new PlayerChangeToBossResponse();
			break;
		case 252:
			result = new PlayerBossStateResponse();
			break;
		case 253:
			result = new PlayerBossStateInnerStateResponse();
			break;
		case 254:
			result = new PlayerBossChangeToPlayerResponse();
			break;
		case 248:
			result = new PlayerReadyToMutantResponse();
			break;
		case 246:
			result = new PlayerBossChooseSkillResponse();
			break;
		case 247:
			result = new MakeEnoughDamageToPlayerBossResponse();
			break;
		case 249:
			result = new PlayerBossDeadResponse();
			break;
		case 255:
			result = new PlayerBossStateDataResponse();
			break;
		case 257:
			result = new PlayerBossActiveBufferResponse();
			break;
		case 259:
			result = new MutantInfoResponse();
			break;
		case 260:
			result = new MutantInfoDetailResponse();
			break;
		case 142:
			result = new BowSwordChangeResponse();
			break;
		case 144:
			result = new KillStreakGetResponse();
			break;
		case 145:
			result = new KillStreakUsedResponse();
			break;
		case 149:
			result = new KillStreakActiveResponse();
			break;
		case 146:
			result = new PlayerWaitVSRebirthResponse();
			break;
		case 147:
			result = new PlayerVSCanRebirthResponse();
			break;
		case 148:
			result = new SpawnObjectResponse();
			break;
		case 71:
			result = new PlayerPrestigeResponse();
			break;
		case 156:
			result = new PlayerKilledByUltraSniperResponse();
			break;
		case 40:
			result = new DailyRewardInfoResponse();
			break;
		case 41:
			result = new TryGetDailyLoginRewardResponse();
			break;
		case 42:
			result = new DailyQuestInfoResponse();
			break;
		case 43:
			result = new TryGetQuestGoldResponse();
			break;
		case 44:
			result = new TryGetQuestFinalResponse();
			break;
		case 48:
			result = new SendAnnouncementResponse();
			break;
		case 258:
			result = new PlayerPlayVFXResponse();
			break;
		case 72:
			result = new QuickJoinNoRoomResponse();
			break;
		case 280:
			result = new CanSpawnEnemyResponse();
			break;
		case 290:
			result = new ChangeExpRateResponse();
			break;
		case 312:
			result = new PlayerFlashedResponse();
			break;
		case 313:
			result = new PlayerCDReduceResponse();
			break;
		case 291:
			result = new ForcePlayerLeaveRoomResponse();
			break;
		case 191:
			result = new PlayerHitVehicleResponse();
			break;
		case 74:
			result = new PvpExpIapResponse();
			break;
		case 76:
			result = new AddCompensateExpResponse();
			break;
		case 77:
			result = new CompensateMaterialResponse();
			break;
		case 152:
			result = new PlayerSetShieldResponse();
			break;
		case 310:
			result = new PlayerChargeWeaponInfoResponse();
			break;
		case 311:
			result = new PlayerNightmarePowerAttackResponse();
			break;
		case 315:
			result = new HookHitPlayerResponse();
			break;
		default:
			Debug.LogWarning("No response type for " + responseID);
			break;
		}
		return result;
	}

	public virtual void ReadData(byte[] data)
	{
	}

	public abstract void ProcessLogic();

	public virtual void ProcessRobotLogic(UIRobot robot)
	{
	}
}

public class ResponseType
{
	public const short ServerExceptionResponse = 1;

	public const short PlayerLogoutResponse = 2;

	public const short PlayerRegisterResponse = 3;

	public const short CreateRoomResponse = 4;

	public const short JoinRoomResponse = 5;

	public const short LeaveRoomResponse = 6;

	public const short GetRoomListResponse = 7;

	public const short GetRoomDataResponse = 8;

	public const short StartGameResponse = 9;

	public const short SetMasterPlayer = 10;

	public const short RoleLoginResponse = 12;

	public const short RoomTimeSynchronize = 13;

	public const short GuestLoginResponse = 14;

	public const short UploadDataResponse = 15;

	public const short ChangeSeatResponse = 20;

	public const short GetBattleStateResponse = 22;

	public const short GetMithrilPriceResponse = 27;

	public const short UploadLoadoutResponse = 29;

	public const short DailyRewardInfoResponse = 40;

	public const short TryGetDailyLoginRewardResponse = 41;

	public const short DailyQuestInfoResponse = 42;

	public const short TryGetQuestGoldResponse = 43;

	public const short TryGetQuestFinalResponse = 44;

	public const short SendAnnouncementResponse = 48;

	public const short Test = 50;

	public const short SwVoidFirstBossStaticMaterialsResponse = 60;

	public const short SwVoidCheckServer = 61;

	public const short SwVoidCreate = 62;

	public const short SwVoidGetData = 63;

	public const short SwVoidIapChest = 64;

	public const short SwVoidGetRewardOutGame = 65;

	public const short SwVoidResolve = 66;

	public const short SwVoidUpgrade = 67;

	public const short SwVoidUpgradeConfirm = 68;

	public const short SwVoidAddSlot = 69;

	public const short SwVoidAddMaterial = 70;

	public const short PlayerPrestigeResponse = 71;

	public const short QuickJoinNoRoomResponse = 72;

	public const short PvpExpIapResponse = 74;

	public const short AddCompensateExpResponse = 76;

	public const short AddCompensateMaterialResponse = 77;

	public const short TimeSynchronizeResponse = 100;

	public const short GetSceneStateResponse = 101;

	public const short SendTransformStateResponse = 102;

	public const short SendPlayerInputResponse = 103;

	public const short EnemySpawnResponse = 104;

	public const short EnemyChangeTargetResponse = 105;

	public const short EnemyStateResponse = 106;

	public const short EnemyHitResponse = 107;

	public const short EnemyOnHitResponse = 108;

	public const short EnemyDeadResponse = 109;

	public const short PlayerOnHitResponse = 110;

	public const short PlayerChangeWeaponResponse = 111;

	public const short PlayerChangeArmorResponse = 112;

	public const short PlayerLeaveGameResponse = 113;

	public const short PlayerFireRocketResponse = 114;

	public const short PlayerHitPlayerResponse = 115;

	public const short EnemyShotResponse = 116;

	public const short PlayerUseItemResponse = 117;

	public const short ItemSpawnResponse = 119;

	public const short PickUpItemResponse = 120;

	public const short PlayerOnKnockedResponse = 121;

	public const short PlayerUploadStatisticsResponse = 122;

	public const short PlayerBuffResponse = 123;

	public const short SendPlayerShootAngleVResponse = 124;

	public const short PlayerRebirthResponse = 125;

	public const short PlayerKillPlayerResponse = 126;

	public const short VSScoreResponse = 127;

	public const short VSGameEndResponse = 128;

	public const short VSGameAutoBalanceResponse = 129;

	public const short VSUploadStatisticsResponse = 130;

	public const short RestartGameResponse = 131;

	public const short PlayerSpawnResponse = 132;

	public const short SendVSTimeResponse = 133;

	public const short DropTheFlagResponse = 134;

	public const short CatchTheFlagResponse = 135;

	public const short GetTheFlagScoreResponse = 136;

	public const short SendFlagTimeResponse = 137;

	public const short RequestFlagPositionResponse = 138;

	public const short SetPlayerHealthResponse = 139;

	public const short ChangePointController = 140;

	public const short SendPointInfo = 141;

	public const short BowSwordChangeResponse = 142;

	public const short SendPlayerInOutPoint = 143;

	public const short KillStreakGetResponse = 144;

	public const short KillStreakUsedResponse = 145;

	public const short PlayerWaitVSRebirthResponse = 146;

	public const short PlayerVSCanRebirthResponse = 147;

	public const short SpawnObjectResponse = 148;

	public const short KillStreakActiveResponse = 149;

	public const short ItemSpawnInRandom = 150;

	public const short SendIPickUpInfo = 151;

	public const short PlayerSetShieldResponse = 152;

	public const short VehicleGetOff = 153;

	public const short PlayerKilledByUltraSniper = 156;

	public const short PlayerGetOnVehicle = 157;

	public const short ChangeToBossScene = 158;

	public const short SwVoidSpawn = 160;

	public const short SwVoidUnspawn = 161;

	public const short SwVoidGetRandomDrop = 164;

	public const short SwVoidGetReward = 165;

	public const short SwVoidState = 169;

	public const short BlackHoleResponse = 170;

	public const short NeighborPlayerResponse = 171;

	public const short SwVoidRemoteGetMaterialCount = 172;

	public const short SwVoidShieldResponse = 173;

	public const short SetPlayerMaxHpResponse = 180;

	public const short SendGameStatistics = 181;

	public const short SendPlayerVsStatistics = 182;

	public const short PlayerSpawnVehicleResponse = 190;

	public const short PlayerHitVehicleResponse = 191;

	public const short EnemyStateDataResponse = 199;

	public const short SlasherStickResponse = 200;

	public const short SlasherBladeWindResponse = 201;

	public const short CreatureTeleportResponse = 202;

	public const short FrostStoneRemoveIceArmor = 203;

	public const short BossCoopResponse = 204;

	public const short FunnelResponse = 205;

	public const short EnemySyncResponse = 206;

	public const short PlayerBossChooseSkillResponse = 246;

	public const short MakeEnoughDamageToPlayerBoss = 247;

	public const short PlayerReadyToMutantResponse = 248;

	public const short PlayerBossDeadResponse = 249;

	public const short PlayerBossChangeTargetPosition = 250;

	public const short PlayerBossChangeToBoss = 251;

	public const short PlayerBossChangeState = 252;

	public const short PlayerBossChangeInnerState = 253;

	public const short PlayerBossChangeToPlayerResponse = 254;

	public const short PlayerBossStateDataResponse = 255;

	public const short PlayerBossWinRequest = 256;

	public const short PlayerBossActiveBufferResponse = 257;

	public const short MutantInfoResponse = 259;

	public const short MutantInfoDetailResponse = 260;

	public const short PlayerPlayVFXResponse = 258;

	public const short CanSpawnEnemyResponse = 280;

	public const short ChangeExpRateResponse = 290;

	public const short ForcePlayerLeaveRoomResponse = 291;

	public const short PlayerChargeWeaponInfoResponse = 310;

	public const short PlayerNightmarePowerAttackResponse = 311;

	public const short PlayerFlashedResponse = 312;

	public const short PlayerCDReduceResponse = 313;

	public const short HookHitPlayerResponse = 315;
}

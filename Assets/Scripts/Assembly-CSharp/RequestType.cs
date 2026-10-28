public class RequestType
{
	public const short PlayerLogin = 1;

	public const short PlayerLogout = 2;

	public const short PlayerRegister = 3;

	public const short CreateRoom = 4;

	public const short JoinRoom = 5;

	public const short LeaveRoom = 6;

	public const short GetRoomList = 7;

	public const short GetRoomData = 8;

	public const short StartGame = 9;

	public const short SearchRoom = 10;

	public const short QuickJoin = 11;

	public const short RoleLogin = 12;

	public const short RoomTimeSynchronize = 13;

	public const short SetRoomPing = 14;

	public const short UploadData = 15;

	public const short UploadMithril = 16;

	public const short UploadArmorAndBag = 17;

	public const short CreateVSRoom = 18;

	public const short PlayerJoinTeamStartGame = 19;

	public const short ChangeSeat = 20;

	public const short SearchRoomAdvanced = 21;

	public const short UploadBattleState = 22;

	public const short UploadOperating = 23;

	public const short UploadKillBossState = 24;

	public const short GuestLogin = 25;

	public const short UploadTwitterAndFacebook = 26;

	public const short GetMithrilPrice = 27;

	public const short QuickJoinAdvanced = 28;

	public const short UploadLoadout = 29;

	public const short UploadGlobalData = 30;

	public const short UploadStoreData = 31;

	public const short DailyRewardInfoRequest = 40;

	public const short TryGetDailyLoginRewardRequest = 41;

	public const short TryGetQuestGoldRequest = 43;

	public const short TryGetQuestFinalRequest = 44;

	public const short Test = 50;

	public const short SwVoidFirstBossStaticMaterials = 60;

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

	public const short PlayerPrestige = 71;

	public const short SyncMithrilRequest = 72;

	public const short SyncCashRequest = 73;

	public const short PvpExpIapRequest = 74;

	public const short PlayerRename = 75;

	public const short AddCompensateExp = 76;

	public const short AddCompensateMaterial = 77;

	public const short TimeSynchronize = 100;

	public const short GetSceneState = 101;

	public const short SendTransformState = 102;

	public const short SendPlayerInput = 103;

	public const short EnemySpawn = 104;

	public const short EnemyChangeTarget = 105;

	public const short EnemyState = 106;

	public const short EnemyHit = 107;

	public const short EnemyOnHit = 108;

	public const short EnemyDead = 109;

	public const short PlayerOnHit = 110;

	public const short PlayerChangeWeapon = 111;

	public const short PlayerChangeArmor = 112;

	public const short PlayerFireRocket = 114;

	public const short PlayerHitPlayer = 115;

	public const short EnemyShot = 116;

	public const short PlayerUseItem = 117;

	public const short PlayerHpRecovery = 118;

	public const short ItemSpawn = 119;

	public const short PickUpItem = 120;

	public const short PlayerOnKnocked = 121;

	public const short PlayerUploadStatistics = 122;

	public const short PlayerBuff = 123;

	public const short SendPlayerShootAngleV = 124;

	public const short PlayerRebirth = 125;

	public const short PlayerHealOthers = 126;

	public const short VSGameEnd = 128;

	public const short VSUploadStatistics = 130;

	public const short RestartGame = 131;

	public const short SendVSTime = 133;

	public const short DropTheFlag = 134;

	public const short CatchTheFlag = 135;

	public const short GetTheFlagScore = 136;

	public const short SendFlagTime = 137;

	public const short BowSwordChangeRequest = 142;

	public const short KillStreakGetRequest = 144;

	public const short KillStreakUsedRequest = 145;

	public const short PlayerWaitVSRebirth = 146;

	public const short SpawnObjectRequest = 148;

	public const short ChangePointController = 140;

	public const short SendPointInfo = 141;

	public const short SendPlayerInOutPoint = 143;

	public const short ItemSpawnInRandom = 150;

	public const short SendIPickUpInfo = 151;

	public const short VehicleGetOff = 153;

	public const short BattlePickUp = 154;

	public const short PlayerSkillCheck = 155;

	public const short PlayerGetOnVehicle = 157;

	public const short ChangeToBossScene = 158;

	public const short SwVoidSpawn = 160;

	public const short SwVoidUnspawn = 161;

	public const short SwVoidGetRandomDrop = 164;

	public const short SwVoidGetReward = 165;

	public const short SwVoidState = 169;

	public const short BlackholeRequest = 170;

	public const short NeighborPlayer = 171;

	public const short SwVoidShieldStateRequest = 172;

	public const short EnemyStateDataRequest = 199;

	public const short SlasherStick = 200;

	public const short SlasherBladeWind = 201;

	public const short CreatureTeleport = 202;

	public const short FrostStoneRemoveIceArmor = 203;

	public const short BoosCoopRequest = 204;

	public const short FunnelRequest = 205;

	public const short DailyQuestBossCheckRequest = 210;

	public const short PlayerBossChooseSkillRequest = 246;

	public const short PlayerBossChangeTargetPosition = 250;

	public const short PlayerBossChangeToBoss = 251;

	public const short PlayerBossChangeState = 252;

	public const short PlayerBossChangeInnerState = 253;

	public const short PlayerBossStateDataRequest = 255;

	public const short PlayerBossWinRequest = 256;

	public const short PlayerBossActiveBuff = 257;

	public const short MutantInfoRequest = 259;

	public const short PlayerPlayVFX = 258;

	public const short PlayerPutNuclear = 270;

	public const short CheatCheck = 300;

	public const short PlayerChargeWeaponInfo = 310;

	public const short PlayerNightmarePowerAttack = 311;

	public const short HookHitPlayer = 315;
}

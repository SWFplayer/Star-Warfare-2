public class FunnelResponse : Response
{
	public BytesBuffer mData;

	public override void ReadData(byte[] data)
	{
		mData = new BytesBuffer(data);
	}

	public override void ProcessLogic()
	{
		byte id = mData.ReadByte();
		EFunnelActionType eFunnelActionType = (EFunnelActionType)mData.ReadByte();
		CrimsonQueen instance = CrimsonQueen.Instance;
		if (instance != null)
		{
			switch (eFunnelActionType)
			{
			case EFunnelActionType.ESpawn:
				instance.SpawnAssistFunnel();
				break;
			case EFunnelActionType.ERespawn:
			{
				Funnel funnel2 = instance.GetFunnel(id);
				funnel2.Respawn();
				break;
			}
			case EFunnelActionType.EFire:
				instance.FunnelAttack();
				break;
			case EFunnelActionType.EOnhit:
			{
				int newHp = mData.ReadInt();
				Funnel funnel = instance.GetFunnel(id);
				funnel.OnHitResponse(newHp);
				break;
			}
			case EFunnelActionType.ESpawnTrackingRPG:
			{
				byte num = mData.ReadByte();
				instance.SpawnRPG(num);
				break;
			}
			}
		}
	}
}

public class SendAnnouncementResponse : Response
{
	protected SwAnnouncement swAnnouncement;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		swAnnouncement = new SwAnnouncement();
		swAnnouncement.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		DailyBonusManager.GetInstance().SetSwAnnouncement(swAnnouncement);
	}
}

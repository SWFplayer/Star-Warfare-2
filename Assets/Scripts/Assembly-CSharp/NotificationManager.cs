public class NotificationManager
{
	private static NotificationManager mInstance;

	public static NotificationManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new NotificationManager();
		}
		return mInstance;
	}

	private void ClearAllScheduledNotification()
	{
	}

	public void ClearScheduledNotificationForMission()
	{
	}

	public void SendNotificationForMission()
	{
	}

	public void SendLocalNotificationForEquip()
	{
	}

	public void SendLocalNotificationForBoss()
	{
	}
}

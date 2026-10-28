public class RoleLoginRequest : Request
{
	protected string userName;

	protected int userID;

	protected string date;

	protected byte userType;

	protected bool isRegist;

	protected int saveNum;

	protected string saveIdentity;

	protected int lastAnnouncementId;

	protected byte language;

	public RoleLoginRequest(string userName, int userID, string date, byte userType, bool isRegist)
	{
		requestID = 12;
		this.userName = userName;
		this.date = date;
		this.userID = userID;
		this.userType = userType;
		this.isRegist = isRegist;
		saveNum = GameApp.GetInstance().GetUserState().GetSaveNum();
		saveIdentity = GameApp.GetInstance().GetUserState().GetSaveIdentity();
		if (saveIdentity == null)
		{
			saveIdentity = string.Empty;
		}
		lastAnnouncementId = GameApp.GetInstance().GetUserState().lastAnnouncementId;
		language = (byte)LocalizationManager.GetInstance().currentLanguage;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer();
		bytesBuffer.AddInt(userID);
		bytesBuffer.AddString(userName);
		bytesBuffer.AddString(date);
		bytesBuffer.AddByte(userType);
		bytesBuffer.AddBool(isRegist);
		bytesBuffer.AddInt(saveNum);
		bytesBuffer.AddString(saveIdentity);
		bytesBuffer.AddInt(lastAnnouncementId);
		bytesBuffer.AddByte(language);
		return bytesBuffer;
	}
}

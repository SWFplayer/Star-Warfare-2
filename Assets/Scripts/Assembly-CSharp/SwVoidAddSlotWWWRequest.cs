public class SwVoidAddSlotWWWRequest : SwVoidWWWRequest
{
	private byte newSlotCount;

	public SwVoidAddSlotWWWRequest(byte newSlotCount)
	{
		this.newSlotCount = newSlotCount;
		mResponseData = new SwVoidAddSlotResponseData();
		mSign = Hash(mSendUserId.ToString() + newSlotCount);
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidAddSlot?userId=" + mSendUserId + "&slotCount=" + newSlotCount + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidAddSlotResponseData swVoidAddSlotResponseData = mResponseData as SwVoidAddSlotResponseData;
		if (!NeedVerify(swVoidAddSlotResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidAddSlotResponse(swVoidAddSlotResponseData);
		}
	}
}

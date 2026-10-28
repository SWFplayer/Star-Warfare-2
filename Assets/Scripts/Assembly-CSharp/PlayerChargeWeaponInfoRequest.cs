public class PlayerChargeWeaponInfoRequest : Request
{
	protected byte m_charge;

	protected bool m_isPower;

	protected int m_id;

	public PlayerChargeWeaponInfoRequest(byte charge, bool isPower, int id)
	{
		requestID = 310;
		m_charge = charge;
		m_isPower = isPower;
		m_id = id;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(6);
		bytesBuffer.AddByte(m_charge);
		bytesBuffer.AddBool(m_isPower);
		bytesBuffer.AddInt(m_id);
		return bytesBuffer;
	}
}

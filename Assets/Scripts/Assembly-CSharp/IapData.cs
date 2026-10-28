public class IapData
{
	public IapScript.IapType Type;

	public int Index;

	public string ProductID;

	public string LocalizedPrice = string.Empty;

	private string _mAmount;

	public int Amount
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mAmount, "shuliang");
		}
		set
		{
			_mAmount = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}
}

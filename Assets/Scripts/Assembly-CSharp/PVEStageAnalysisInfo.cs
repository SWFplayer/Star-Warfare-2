public class PVEStageAnalysisInfo
{
	public int playTime;

	public int passTime;

	public int goldCollect;

	public int mithrilCollect;

	public int normalVoidMatCollect;

	public int uncommonVoidMatCollect;

	public int rareVoidMatCollect;

	public int RebirthTime;

	public int MithrilUsed;

	public void AddSwVoidMat(int id)
	{
		if (id <= 100)
		{
			normalVoidMatCollect++;
		}
		else if (id <= 200)
		{
			uncommonVoidMatCollect++;
		}
		else if (id <= 300)
		{
			normalVoidMatCollect++;
		}
		else
		{
			rareVoidMatCollect++;
		}
	}
}

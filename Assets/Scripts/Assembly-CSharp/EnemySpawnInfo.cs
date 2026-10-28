public class EnemySpawnInfo
{
	public EnemyType EType { get; set; }

	public int Count { get; set; }

	public SpawnFromType From { get; set; }

	public int EliteProbability { get; set; }

	public EnemySpawnInfo()
	{
		EliteProbability = 5;
	}
}

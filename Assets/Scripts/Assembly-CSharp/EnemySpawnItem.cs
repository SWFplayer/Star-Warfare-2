public class EnemySpawnItem
{
	public int Round;

	public float intermission;

	public SpawnMissionType Mission { get; set; }

	public EnemyType EType { get; set; }

	public SpawnFromType From { get; set; }

	public bool CanMerge { get; set; }

	public int EliteProbability { get; set; }
}

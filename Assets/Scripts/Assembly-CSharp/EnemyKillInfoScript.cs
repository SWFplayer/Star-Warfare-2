using UnityEngine;

public class EnemyKillInfoScript : MonoBehaviour
{
	public UISprite EnemyIcon;

	public UILabel KillCount;

	public UILabel EnemyName;

	public GameObject EliteInfoObj;

	public UILabel EliteKillCount;

	public void Init(KillEnemyInfo info)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ENEMY_LIST, DataSheetName.ENEMY_OVERALL);
		EnemyIcon.spriteName = gameDataTable.GetString((int)info.mEnemyType, 17);
		KillCount.text = info.mKillCount.ToString();
		EliteKillCount.text = info.mKillEliteCount.ToString();
		EnemyName.text = LocalizationManager.GetInstance().GetString(gameDataTable.GetString((int)info.mEnemyType, 18));
	}
}

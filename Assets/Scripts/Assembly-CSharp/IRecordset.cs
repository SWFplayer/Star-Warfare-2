using System.IO;

public interface IRecordset
{
	BytesBuffer SaveBytesBuffer();

	void LoadBytesBuffer(BytesBuffer bb);

	void SaveGlobalBuffer(BytesBuffer bb);

	void SaveMissionBuffer(BytesBuffer bb);

	void SaveEquipBuffer(BytesBuffer bb);

	void SaveData(BinaryWriter bw);

	void LoadData(BinaryReader br);

	void ResetUserState();

	RecordData GetRecordData();
}

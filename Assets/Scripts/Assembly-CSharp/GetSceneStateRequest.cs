using UnityEngine;

public class GetSceneStateRequest : Request
{
	public GetSceneStateRequest()
	{
		requestID = 101;
		Debug.Log("GetSceneStateRequest");
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}

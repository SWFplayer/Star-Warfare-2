using UnityEngine;

public class DownloadObbExample : MonoBehaviour
{
	private static string expPath;

	private static bool downloadButton;

	private static string errorMsg = string.Empty;

    private void Start()
    {

        Application.LoadLevel("MainMenu");
    }
}

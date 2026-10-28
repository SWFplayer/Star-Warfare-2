using UnityEngine;
using UnityEngine.Video;

public class PlayStartMovieScript : MonoBehaviour
{
    public VideoPlayer videoPlayer; 

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.PlayFullScreenMovie("movie.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
#else
        
        if (videoPlayer != null)
        {
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "movie.mp4");
            videoPlayer.Play();
        }
#endif
    }

    private void Update()
    {
    }
}

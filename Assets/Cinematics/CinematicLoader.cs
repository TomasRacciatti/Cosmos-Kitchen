using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CinematicLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextScene = "GameScene";

    private AsyncOperation loadOp;

    void Start()
    {
        loadOp = SceneManager.LoadSceneAsync(nextScene);
        loadOp.allowSceneActivation = false;

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        loadOp.allowSceneActivation = true;
    }

    public void SkipCinematic()
    {
        loadOp.allowSceneActivation = true;
    }
}

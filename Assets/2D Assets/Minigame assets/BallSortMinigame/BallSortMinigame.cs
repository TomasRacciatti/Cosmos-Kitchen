using UnityEngine;

public class BallSortMinigame : MonoBehaviour, IMinigame
{
    [SerializeField] private BallSortLogic logic;
    private System.Action onSuccess;
    private System.Action onFailure;
    private bool puzzleStarted = false;
    
    [Header("SFX")]
    [SerializeField] AudioClip EggEnter;
    [SerializeField] AudioClip EggWin;

    public void StartMinigame(System.Action onSuccess, System.Action onFailure)
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(EggEnter);
        this.onSuccess = onSuccess;
        this.onFailure = onFailure;
        puzzleStarted = true;
        gameObject.SetActive(true);
        logic.StartPuzzle(onSuccess, onFailure);

        InputManager._instance.UnlockMouse();
    }

    private void Update()
    {
        if (!puzzleStarted) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            CancelMinigame();
            return;
        }

        if (logic.HasWon())
        {
            AudioManager.instance.StopAllSFX();
            AudioManager.instance.PlaySFX(EggWin);

            
            puzzleStarted = false;
            InputManager._instance.LockMouse();
            onSuccess?.Invoke();
            gameObject.SetActive(false);
        }
    }


    private void CancelMinigame()
    {
        puzzleStarted = false;
        InputManager._instance.LockMouse();
        onFailure?.Invoke();
        gameObject.SetActive(false);
    }

}
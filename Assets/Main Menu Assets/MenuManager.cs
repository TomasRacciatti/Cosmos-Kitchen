using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject bg;
    private bool _isOpen;
    
    [Header("SFX")]
    [SerializeField] AudioClip UIClickSound;
    [SerializeField] AudioClip UICOpenSound;
    [SerializeField] AudioClip UICloseSound;



    private void Start()
    {
        _isOpen = false;
    }

    public void ButtonSound()
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(UIClickSound);
    }

    // Para mostrar solo el menu principal
    public void ShowMainMenu()
    {
        bg.SetActive(true);
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        _isOpen = true;
    }

    public void ToggleMainMenu()
    {
        if (_isOpen)
        {
            AudioManager.instance.StopAllSFX();
            AudioManager.instance.PlaySFX(UICloseSound);

            bg.SetActive(false);
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            _isOpen = false;
            InputManager._instance.ResumeAll();
            InputManager._instance.ResumeInput();
            InputManager._instance.SwitchMouselock();
        }
        else
        {
            AudioManager.instance.StopAllSFX();
            AudioManager.instance.PlaySFX(UICOpenSound);

            bg.SetActive(true);
            mainMenuPanel.SetActive(true);
            settingsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            _isOpen = true;
            InputManager._instance.UnlockMouse();   //algo aca no anda en el main con el boton de exit
            InputManager._instance.StopAll();
            InputManager._instance.StopInput();
        }
    }

    // Play
    public void PlayGame()
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(UIClickSound);
        
        SceneManager.LoadScene("Eorth");
    }
    
    // Back to Main Menu
    public void BackToMainMenu()
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(UIClickSound);
        
        SceneManager.LoadScene("MainMenuScene");
    }

    // Settings
    public void ShowSettings()
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(UIClickSound);
        
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    // Controls
    public void ShowControls()
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(UIClickSound);
        
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    // Exit
    public void ExitGame()
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(UIClickSound);
        
        ToggleMainMenu(); //algo aca no anda
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void ExitGameMainMenu()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif        
    }
    
}
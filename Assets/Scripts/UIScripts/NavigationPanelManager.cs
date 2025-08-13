using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationPanelManager : MonoBehaviour
{
    public static NavigationPanelManager _instance;

    [Header("Prefabs")]
    [SerializeField] GameObject _planetData;
    [SerializeField] GameObject _planetBlocked;

    [SerializeField] GameObject _travelButton;
    [SerializeField] float _clientsCompleted = 0;
    [SerializeField] string _currentPlanet = "Eorth";

    [Header("DisplayPrefabs")]
    [SerializeField] GameObject _ingredientPrefab;
    [SerializeField] GameObject _clientPrefab;
    [SerializeField] GameObject _prefabSpawnpoint;

    [Header("Planet Info")]
    [SerializeField] TextMeshProUGUI _planetInfoName;
    [SerializeField] TextMeshProUGUI _planetInfoDescription;
    [SerializeField] Image _planetInfoImage;

    [Header("Blocked Info")]
    [SerializeField] TextMeshProUGUI _progressBarText;
    [SerializeField] Image _progressBar;

    [Header("Eorth Info")]
    [SerializeField] ScriptableIngredient[] _eorthIngredients;
    [SerializeField] ClientScript[] _eorthClients;

    [Header("Desert Info")]
    [SerializeField] ScriptableIngredient[] _desertIngredients;
    [SerializeField] bool _isDesertUnlocked = false;
    [SerializeField] float _desertClientUnlock;

    [Header("Volcano Info")]
    [SerializeField] ScriptableIngredient[] _volcanoIngredients;
    [SerializeField] bool _isVolcanoUnlocked = false;
    [SerializeField] float _volcanoClientUnlock;

    [Header("Frozen Info")]
    [SerializeField] ScriptableIngredient[] _frozenIngredients;
    [SerializeField] bool _isFrozenUnlocked = false;
    [SerializeField] float _frozenClientUnlock;

    [Header("Ocean Info")]
    [SerializeField] ScriptableIngredient[] _oceanIngredients;
    [SerializeField] bool _isOceanUnlocked = false;
    [SerializeField] float _oceanClientUnlock;

    [Header("Current Prefabs")]
    [SerializeField] GameObject[] _currentPrefabs;
    [SerializeField] List<ClientScript> _clients;
    
    [Header("SFX")]
    [SerializeField] AudioClip TravelSound;
    [SerializeField] AudioClip SelectSound;
    [SerializeField] AudioClip DeniedSound;
    [SerializeField] AudioClip PlanetInfoSound;
    [SerializeField] AudioClip PlanetInfoDeniedSound;
    [SerializeField] AudioClip ClientListSound;
    [SerializeField] AudioClip IngredientListSound;
    


    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else { Destroy(this); }
    }

    public void UpdatePlanetInfo(string name, string info, Sprite icon)
    {
        CleanDisplay();


        _planetInfoName.text = name;
        _planetInfoDescription.text = info;
        _planetInfoImage.sprite = icon;

        switch (name)
        {
            case "Desert":
                if (!_isDesertUnlocked)
                {
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(PlanetInfoDeniedSound); 
                    
                    _planetBlocked.SetActive(true);
                    _planetData.SetActive(false);
                    _progressBar.fillAmount = _clientsCompleted/_desertClientUnlock;
                    _progressBarText.text = _clientsCompleted + "/" + _desertClientUnlock;
                    _travelButton.SetActive(false);
                }
                else
                {
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(PlanetInfoSound);
                    
                    _planetBlocked.SetActive(false);
                    _planetData.SetActive(true);
                    if (_currentPlanet != name)
                    {
                        _travelButton.SetActive(true);
                    }
                }
                break;

            case "Volcano":

                if (!_isVolcanoUnlocked)
                {
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(PlanetInfoDeniedSound); 
                    
                    _planetBlocked.SetActive(true);
                    _planetData.SetActive(false);
                    _progressBar.fillAmount = _clientsCompleted / _volcanoClientUnlock;
                    _progressBarText.text = _clientsCompleted + "/" + _volcanoClientUnlock;
                    _travelButton.SetActive(false);
                }
                else
                {
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(PlanetInfoSound);
                    
                    _planetBlocked.SetActive(false);
                    _planetData.SetActive(true);
                    if (_currentPlanet != name)
                    {
                        _travelButton.SetActive(true);
                    }
                }
                break;

            case "Frozen":
                if (!_isFrozenUnlocked)
                {
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(PlanetInfoDeniedSound); 

                    _planetBlocked.SetActive(true);
                    _planetData.SetActive(false);
                    _progressBar.fillAmount = _clientsCompleted / _frozenClientUnlock;
                    _progressBarText.text = _clientsCompleted + "/" + _frozenClientUnlock;
                    _travelButton.SetActive(false);
                }
                else
                {
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(PlanetInfoSound);
                    
                    _planetBlocked.SetActive(false);
                    _planetData.SetActive(true);
                    if (_currentPlanet != name)
                    {
                        _travelButton.SetActive(true);
                    }
                }
                break;

            case "Ocean":
                if (!_isOceanUnlocked)
                {
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(PlanetInfoDeniedSound); 

                    _planetBlocked.SetActive(true);
                    _planetData.SetActive(false);
                    _progressBar.fillAmount = _clientsCompleted / _oceanClientUnlock;
                    _progressBarText.text = _clientsCompleted + "/" + _oceanClientUnlock;
                    _travelButton.SetActive(false);
                }
                else
                {
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(PlanetInfoSound);
                    
                    _planetBlocked.SetActive(false);
                    _planetData.SetActive(true);
                    if (_currentPlanet != name)
                    {
                        _travelButton.SetActive(true);
                    }
                }
                break;

            default:
                
                AudioManager.instance.StopAllSFX();
                AudioManager.instance.PlaySFX(PlanetInfoSound);
                
                _planetBlocked.SetActive(false);
                _planetData.SetActive(true);
                if (_currentPlanet != name)
                {
                    _travelButton.SetActive(true);
                }
                break;
        }

        if (_currentPlanet == name)
        {
            _travelButton.SetActive(false);
        }
    }

    public void ShowIngredients()
    {
        AudioManager.instance.PlaySFX(IngredientListSound); 

        CleanDisplay();

        ScriptableIngredient[] _spawner;

        switch (_planetInfoName.text)
        {
            case "Eorth":
                _spawner = _eorthIngredients;
                break;
            case "Desert":
                _spawner = _desertIngredients;
                break;
            case "Volcano":
                _spawner = _volcanoIngredients;
                break;
            case "Frozen":
                _spawner = _frozenIngredients;
                break;
            case "Ocean":
                _spawner = _oceanIngredients;
                break;
            default:
                _spawner = _eorthIngredients;
                break;
        }

        _currentPrefabs = new GameObject[_spawner.Length];

        for (int i = 0; i < _spawner.Length; i++)
        {
            IngredientDisplay _ingDisplay;
            _ingDisplay = Instantiate(_ingredientPrefab, _prefabSpawnpoint.transform).GetComponent<IngredientDisplay>();
            _ingDisplay.UpdateIngredientDisplay(_spawner[i]._ingredientName, _spawner[i]._inventoryRaw);
            _currentPrefabs[i] = _ingDisplay.gameObject;
        }

    }
    public void ShowClients()
    {
        if (_clients != null)
        {
            AudioManager.instance.PlaySFX(ClientListSound);

            CleanDisplay();

            _currentPrefabs = new GameObject[_clients.Count];
            int i = 0;

            foreach (var client in _clients)
            {
                ClientDisplay _clientDisplay;
                _clientDisplay = Instantiate(_clientPrefab, _prefabSpawnpoint.transform).GetComponent<ClientDisplay>();
                _clientDisplay.UpdateClientDisplay(client);
                _currentPrefabs[i] = _clientDisplay.gameObject;
                i++;
            }
        }
    }

    private void CleanDisplay()
    {
        if (_currentPrefabs != null)
        {
            for (int i = 0; i < _currentPrefabs.Length; i++)
            {
                Destroy(_currentPrefabs[i]);
            }
        }
    }

    public void ClientAdd(ClientScript client)
    {
        if (!_clients.Contains(client))
        {
            _clients.Add(client);
        }

        if (client._deliveryComplete)
        {
            _clientsCompleted++;

            switch (_clientsCompleted)
            {
                case 3:
                    ClientManager.instance.UnlockCritic("Eorth");
                    DialogueManager._instance.Notify("The Critic has appeared in Eorth!");
                    break;
                case 6:
                    ClientManager.instance.UnlockCritic("Desert");
                    DialogueManager._instance.Notify("The Critic has appeared in Desert!");
                    break;
                case 12:
                    ClientManager.instance.UnlockCritic("Ocean");
                    DialogueManager._instance.Notify("The Critic has appeared in Ocean!");
                    break;
                case 15:
                    ClientManager.instance.UnlockCritic("Frozen");
                    DialogueManager._instance.Notify("The Critic has appeared in Frozen!");
                    break;
                case 30:
                    ClientManager.instance.UnlockCritic("Volcano");
                    DialogueManager._instance.Notify("The Critic has appeared in Volcano!");
                    break;
                default:
                    break;
            }
        }
    }

    public void UnlockPlanet(string planetName)
    {

        switch (planetName)
        {
            case "Desert":
                _isDesertUnlocked = true;
                DialogueManager._instance.Notify("You can travel to Desert!");
                break;
            case "Volcano":
                _isVolcanoUnlocked = true;
                DialogueManager._instance.Notify("You can travel to Volcano!");
                break;
            case "Frozen":
                _isFrozenUnlocked = true;
                DialogueManager._instance.Notify("You can travel to Frozen!");
                break;
            case "Ocean":
                _isOceanUnlocked = true;
                DialogueManager._instance.Notify("You can travel to Ocean!");
                break;
            default:
                break;
        }

    }

    public void Travel()
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(TravelSound); 
        AudioManager.instance.PlaySFX(SelectSound); 

        _currentPlanet = _planetInfoName.text;
        PanelManager._instance.CloseCookingStation(9);
        PanelManager._instance.CloseFirstPerson();
        SceneManager.LoadScene(_planetInfoName.text);
        
    }

    public void ClientRetry()
    {
        _clientsCompleted--;
    }

}

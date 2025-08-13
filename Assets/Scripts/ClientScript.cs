using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientScript : MonoBehaviour
{
    [SerializeField] GameObject _signal;
    [SerializeField] Sprite _questionSprite;
    [SerializeField] Animator _animator;
    [SerializeField] string _startingAnimation;
    [SerializeField] ClientSO _clientData;

    private QualityLevel _plateQuality;
    private ClientOrder _order;

    private bool _hasInteracted = false;
    private bool _canInteract = false;

    private string _askingText;
    private string _deliveredText;
    private string _perfectText;
    private string _repeatingText;
    private string _wrongText;

    public bool _deliveryComplete = false;
    public bool _perfectDelivery = false;

    private string[] _ingredients;
    private string[] _processes;

    private string _clientName;
    private int _clientNumber;
    private Sprite _clientIcon;

    private string _orderNotification = "List of Clients Updated at the Cooking Station!!";

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
    }

    private void Start()
    {
        SetClientDialogue(_clientData._askingDialogue, _clientData._deliveredDialogue, _clientData._perfectDialogue, _clientData._repeatingDialogue, _clientData._wrongDialogue);
        SetClientPlate(_clientData._plateIngredientsScripts);
        SetClientNumber(_clientData._clientNumber, _clientData._clientName, _clientData._clientIcon);
    }

    private void Update()
    {
        if (_canInteract && Input.GetKeyDown(KeyCode.E) && InputManager._instance._canInteract)
        {
            if (!_hasInteracted)
            {
                _hasInteracted = true;
                NavigationPanelManager._instance.ClientAdd(this);
                ChangeSignal();
            }

            if (_perfectDelivery)
            {
                DialogueManager._instance.ChangeDialogue(_repeatingText);
            }

            if (_deliveryComplete && !_perfectDelivery)
            {
                DialogueManager._instance.ChangeDialogue(_deliveredText);
                DialogueManager._instance.SetClient(this);
                DialogueManager._instance.ShowRetryButton();
            }
            
            if (!_perfectDelivery && !_deliveryComplete)
            {
                DialogueManager._instance.ChangeDialogue(_askingText);
                DialogueManager._instance.SetClient(this);
                DialogueManager._instance.ShowDeliveryButton();
            }

            DialogueManager._instance.SwitchDialogue();
        }
    }

    public void GetPlate(PlateScript _plate) //esto revisa que el plato sea lo que quiere el cliente
    {
        if (_plate._plateIngredients.ContainsKey(_ingredients[0]) && _plate._plateIngredients.ContainsKey(_ingredients[1]) && _plate._plateIngredients.ContainsKey(_ingredients[2]))
        {
            if (_plate._plateIngredients[_ingredients[0]] == _processes[0] && _plate._plateIngredients[_ingredients[1]] == _processes[1] && _plate._plateIngredients[_ingredients[2]] == _processes[2])
            {
                DialogueManager._instance.CorrectPlate();
                CheckPlate(_plate);
            }
            else
            {
                DialogueManager._instance.ChangeDialogue(_wrongText);
            }
        }
        else
        {
            DialogueManager._instance.ChangeDialogue(_wrongText);
        }
    }

    private void CheckPlate(PlateScript _plate) //esto revisa la calidad
    {
        _plateQuality = _plate.ReturnQualityLevel();
        if (_plateQuality == QualityLevel.Perfect)
        {
            DialogueManager._instance.ChangeDialogue(_perfectText);
            _perfectDelivery = true;
        }
        else
        {
            string _critiqueText = "Uhmm, it's good but I have some thoughts...";

            for (int i = 0; i < _plate._plateComments.Length; i++)
            {
                if (_plate._plateComments[i] != "")
                {
                    _critiqueText += "\n" + _plate._plateComments[i];
                }
            }

            DialogueManager._instance.ChangeDialogue(_critiqueText);

        }
        _deliveryComplete = true;
        ChangeSignal();
        DialogueManager._instance.ClosePlateReceiver();
        NavigationPanelManager._instance.ClientAdd(this);

        if (_clientData._isCritic)
        {
            NavigationPanelManager._instance.UnlockPlanet("Desert");
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.GetComponent<PlayerMovement>())
        {
            DialogueManager._instance.ShowInteraction();
            _canInteract = true;
            
            FindObjectOfType<AudioManager>().Play("NPC_talk");
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.GetComponent<PlayerMovement>())
        {
            _canInteract = false;
            DialogueManager._instance.HideInteraction();
            
        }
    }

    public void SetClientDialogue(string _newAskingText, string _newDeliveredText, string _newPerfectText, string _newRepeatingText, string _newWrongText)
    {
        _askingText = _newAskingText;
        _deliveredText = _newDeliveredText;
        _perfectText = _newPerfectText;
        _repeatingText = _newRepeatingText;
        _wrongText = _newWrongText;
    }

    public void SetClientNumber(int i, string name, Sprite icon)
    {
        _clientNumber = i;
        _clientName = name;
        _clientIcon = icon;
    }

    public string ReturnName()
    {
        return _clientName;
    }
    public Sprite ReturnPicture()
    {
        return _clientIcon;
    }

    public void SetClientPlate(IngredientScript[] _ingredientsPlate)
    {
        _ingredients = new string[3];
        _processes = new string[3];

        for (int i = 0; i < 3; i++)
        {
            _ingredients[i] = _ingredientsPlate[i].ReturnIngredientName();
            _processes[i] = _ingredientsPlate[i].ReturnProcessName();
        }
    }

    public void ChangeSignal()
    {
        if (_hasInteracted && !_deliveryComplete)
        {
            _signal.GetComponent<Image>().sprite = _questionSprite;
            _order = ClientManager.instance.MakeClientOrder(_clientName, _clientData._orderDescription);
            DialogueManager._instance.Notify(_orderNotification);
        }
        else
        {
            _signal.SetActive(false);
            _order.OrderComplete();
        }
    }

    public void Retry()
    {
        _signal.SetActive(true);
        _signal.GetComponent<Image>().sprite = _questionSprite;
        _order = ClientManager.instance.MakeClientOrder(_clientName, _clientData._orderDescription);
        DialogueManager._instance.Notify(_orderNotification);
        NavigationPanelManager._instance.ClientRetry();
        _deliveryComplete = false;
    }

    public QualityLevel ReturnQualityLevel()
    {
        return _plateQuality;
    }

    public string ReturnClientName()
    {
        return _clientName;
    }

    public Sprite ReturnClientIcon()
    {
        return _clientIcon;
    }
    
    public void SetAnimation(string animationName)
    {
        _animator.Play(animationName);
    }
}

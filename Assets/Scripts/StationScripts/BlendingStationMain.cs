using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlendingStationMain : MonoBehaviour, IDropHandler
{
    public GameObject _itemInSlot;
    public IngredientScript _ingredientInSlot;
    IngredientScript _ingredientCheck;
    int _toolRequired;
    [SerializeField] Slider _toolCheck;

    [Header("SFX")]
    [SerializeField] AudioClip StationSound;
    
    
    public void OnDrop(PointerEventData _eventData)
    {
        _ingredientCheck = _eventData.pointerDrag.GetComponent<IngredientScript>();

        if (transform.childCount == 0)
        {
            if (_ingredientCheck != null)
            {
                if (_ingredientCheck.GetFoodState() == CookingState.Raw)
                {
                    GameObject _dropped = _eventData.pointerDrag;
                    DragableItem _dragableItem = _dropped.GetComponent<DragableItem>();
                    _dragableItem._parentAfterDrag = transform;
                    _itemInSlot = _dropped;
                    _ingredientInSlot = _ingredientCheck;
                    _toolRequired = _ingredientInSlot.BlendRequirement();
                }
            }
        }
    }
    
    
    public void BlendIngredient()
    {
        AudioManager.instance.PlaySFX(StationSound); //sfx

        if (transform.childCount == 1)
        {

            QualityLevel _qualityResult;
            int _qualityValue;

            _qualityValue = Mathf.Abs((int)_toolCheck.value - _toolRequired);
            if (_qualityValue > 3)
            {
                _qualityValue = 3;
            }

            switch (_qualityValue)
            {
                case 0:
                    _qualityResult = QualityLevel.Perfect;
                    Debug.Log("Cooked: Perfect");
                    break;
                case 1:
                    _qualityResult = QualityLevel.Excelent;
                    Debug.Log("Cooked: Excelent");
                    break;
                case 2:
                    _qualityResult = QualityLevel.Good;
                    Debug.Log("Cooked: Good");
                    break;
                case 3:
                    _qualityResult = QualityLevel.Decent;
                    Debug.Log("Cooked: Decent");
                    break;
                case 4:
                    _qualityResult = QualityLevel.Bad;
                    Debug.Log("Cooked: Bad");
                    break;
                default:
                    _qualityResult = QualityLevel.Horrible;
                    Debug.Log("Cooked: Horrible");
                    break;
            }

            _ingredientInSlot.ProcessFood(CookingState.Blended, _qualityResult);
            InventoryManager._instance.Add(_itemInSlot);

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = StationSound;
            source.Play();
            StartCoroutine(FadeOutSFX(source, 1.2f));

            _itemInSlot = null;
            _ingredientInSlot = null;
        }
    }
    
    private IEnumerator FadeOutSFX(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.Stop();
        AudioManager.instance.StopAllSFX();
        source.volume = startVolume;
    }



}

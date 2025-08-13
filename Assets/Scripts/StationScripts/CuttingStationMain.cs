using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingStationMain : MonoBehaviour, IDropHandler
{
    public GameObject _itemInSlot;
    public IngredientScript _ingredientInSlot;
    IngredientScript _ingredientCheck;
    int _toolRequired;
    CuttingStationTool _toolCheck;
    
    [Header("SFX")]
    [SerializeField] AudioClip StationSound;

    public void OnDrop(PointerEventData _eventData)
    {
         _ingredientCheck = _eventData.pointerDrag.GetComponent<IngredientScript>();
         _toolCheck = _eventData.pointerDrag.GetComponent<CuttingStationTool>();

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
                    _toolRequired = _ingredientInSlot.CutRequirement();
                }
            }
        }
        else
        {
            if (_toolCheck && transform.childCount == 1)
            {
                QualityLevel _qualityResult;
                int _result = (int)MathF.Abs(_toolRequired - _toolCheck._toolNumber);

                switch (_result)
                {
                    case 5:
                        _qualityResult = QualityLevel.Horrible;
                        Debug.Log("Cooked: Horrible");
                        break;
                    case 4:
                        _qualityResult = QualityLevel.Bad;
                        Debug.Log("Cooked: Bad");
                        break;
                    case 3:
                        _qualityResult = QualityLevel.Decent;
                        Debug.Log("Cooked: Decent");
                        break;
                    case 2:
                        _qualityResult = QualityLevel.Good;
                        Debug.Log("Cooked: Good");
                        break;
                    case 1:
                        _qualityResult = QualityLevel.Excelent;
                        Debug.Log("Cooked: Excelent");
                        break;
                    case 0:
                        _qualityResult = QualityLevel.Perfect;
                        Debug.Log("Cooked: Perfect");
                        break;
                    default:
                        _qualityResult = QualityLevel.Horrible;
                        Debug.Log("Cooked: Horrible");
                        break;
                }

                _ingredientInSlot.ProcessFood(CookingState.Cut,_qualityResult);
                InventoryManager._instance.Add(_itemInSlot);
                
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = StationSound;
                source.Play();
                StartCoroutine(FadeOutSFX(source, 1.2f));
                
                
                _itemInSlot = null;
                _ingredientInSlot = null;
            }
        }

        _toolCheck = null;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookButtonScript : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<GameObject> pages;
    [SerializeField] GameObject forwardButton;
    [SerializeField] GameObject backButton;
    
    [Header("SFX")]
    [SerializeField] AudioClip PageForwardSound;
    [SerializeField] AudioClip PageBackSound;
    
    int index = -1;
    bool rotate = false;
    private bool _invert;

    private void Start()
    {
        InitialState();
    }

    public void InitialState()
    {
        for (int i = 0; i <pages.Count; i++)
        {
            pages[i].transform.rotation = Quaternion.identity;
        }
        pages[0].transform.SetAsLastSibling();
        backButton.SetActive(false);
    }
    
    public void RotateForward()
    {
        if (rotate == true) { return; }
        InputManager._instance._canToggleBook = false;
        InventoryManager._instance._canToggleBook = false;

        AudioManager.instance.PlaySFX(PageForwardSound); 

        index++;
        float angle = 180;
        ForwardButtonActions();
        _invert = true;
        pages[index].transform.SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));
    }

    public void RotateBack()
    {
        if (rotate == true) { return; }
        InputManager._instance._canToggleBook = false;
        InventoryManager._instance._canToggleBook = false;
        
        AudioManager.instance.PlaySFX(PageBackSound); 
        
        float angle = 0;
        _invert = false;
        pages[index].transform.SetAsLastSibling();
        BackwardButtonActions();
        StartCoroutine(Rotate(angle, false));
    }

    private void ForwardButtonActions()
    {
        if (backButton.activeInHierarchy == false)
        {
            backButton.SetActive(true);
        }
        if (index == pages.Count - 1)
        {
            forwardButton.SetActive(false);
        }
    }
    private void BackwardButtonActions()
    {
        if (forwardButton.activeInHierarchy == false)
        {
            forwardButton.SetActive(true);
        }

        if (index - 1 == -1)
        {
            backButton.SetActive(false);
        }
    }


    IEnumerator Rotate(float angle, bool forward)
    {
        float value = 0f;
        while (true)
        {
            rotate = true;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            value += Time.deltaTime * pageSpeed;
            pages[index].transform.rotation = Quaternion.Slerp(pages[index].transform.rotation, targetRotation, value);
            float angle1 = Quaternion.Angle(pages[index].transform.rotation, targetRotation);

            if (angle1 < 90f)
            {
                if (_invert)
                {
                    pages[index].GetComponent<PageInvert>().ShowInverted();
                }
                else
                {
                    pages[index].GetComponent<PageInvert>().ShowNormal();
                }
            }

            if (angle1 < 0.1f)
            {

                if (forward == false)
                {
                    index--;
                }

                rotate = false;
                break;
            }
            yield return null;
        }
            InputManager._instance._canToggleBook = true;
        InventoryManager._instance._canToggleBook = true;
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using Audio;
// using UnityEngine;
//
// namespace Book
// {
//     public class BookHandler : MonoBehaviour
//     {
//         [Header("Flip")]
//         [SerializeField] float pageSpeed = 0.5f;
//     
//         [Header("Pages & UI")]
//         [SerializeField] List<GameObject> pages;
//         [SerializeField] GameObject forwardButton;
//         [SerializeField] GameObject backButton;
//
//         [Header("SFX")] 
//         [SerializeField] private AudioCue flipCue;
//
//         private int _index = -1;
//         private bool _isFlipping;
//         private float _forwardAngle = 0f;
//         private float _backAngle = 180f;
//
//         private void Start()
//         {
//             InitialState();
//         }
//
//         public void InitialState()
//         {
//             if (_isFlipping || pages == null || pages.Count == 0) return;
//             
//             for (int i = 0; i < pages.Count; i++)
//             {
//                 pages[i].transform.rotation = Quaternion.identity;
//             }
//
//             pages[0].transform.SetAsLastSibling();
//             _index = -1;
//             UpdateButtons();
//         }
//         
//         public void RotateForward() => TryFlip(_forwardAngle, forward: true);
//         public void RotateBack() => TryFlip(_backAngle, forward: false);
//
//         public void TryFlip(float angle, bool forward)
//         {
//             if (forward)
//             {
//                 if (_index >= pages.Count - 1) return;
//
//                 _index++;
//             }
//             else
//             {
//                 if (_index < 0) return;
//             }
//
//             PlayFlipSfx();
//             BringPageOnTop(_index);
//             StartCoroutine(FlipCoroutine(pages[_index].transform, angle, forward, startedIndex: _index));
//         }
//
//
//         private IEnumerator FlipCoroutine(Transform page, float targetAngle, bool forward, int startedIndex)
//         {
//             float value = 0f;
//             while (true)
//             {
//                 rotate = true;
//                 Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
//                 value += Time.deltaTime * pageSpeed;
//                 pages[_index].transform.rotation = Quaternion.Slerp(pages[_index].transform.rotation, targetRotation, value);
//                 float angle1 = Quaternion.Angle(pages[_index].transform.rotation, targetRotation);
//
//                 if (angle1 < 90f)
//                 {
//                     if (_invert)
//                     {
//                         pages[_index].GetComponent<PageInvert>().ShowInverted();
//                     }
//                     else
//                     {
//                         pages[_index].GetComponent<PageInvert>().ShowNormal();
//                     }
//                 }
//
//                 if (angle1 < 0.1f)
//                 {
//                     if (forward == false)
//                     {
//                         _index--;
//                     }
//
//                     rotate = false;
//                     break;
//                 }
//
//                 yield return null;
//             }
//
//             InputManager._instance._canToggleBook = true;
//             InventoryManager._instance._canToggleBook = true;
//         }
//         
//         // Funciones Helper
//     
//         private void PlayFlipSfx()
//         {
//             if (flipCue == null) return;
//
//             var clip = flipCue.GetRandomClip();
//             if (clip != null)
//             {
//                 AudioManager.instance?.PlaySFX(clip); // Cambiar si dejamos de usar el AudioManager
//             }
//         }
//     
//         private void UpdateButtons()
//         {
//             if (backButton)    
//                 backButton.SetActive(_index >= 0);
//             
//             if (forwardButton) 
//                 forwardButton.SetActive(pages != null && _index < pages.Count - 1);
//         }
//         
//         private void BringPageOnTop(int i)
//         {
//             pages[i].transform.SetAsLastSibling();
//         }
//     }
// }
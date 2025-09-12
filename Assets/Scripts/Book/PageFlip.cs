using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace Book
{
    public class PageFlip : MonoBehaviour
    {
        [Header("Flip")] 
        [SerializeField] float pageSpeed = 0.5f;

        [Header("Pages & UI")] 
        [SerializeField] List<GameObject> pages;

        [SerializeField] GameObject forwardButton;
        [SerializeField] GameObject backButton;

        [Header("SFX")] 
        [SerializeField] private AudioCue flipCue;

        private int _index = -1;
        private bool _isFlipping;
        private float _forwardAngle = 180f;
        private float _backAngle = 0f;

        private void Start()
        {
            InitialState();
        }

        public void InitialState()
        {
            if (_isFlipping || pages == null || pages.Count == 0) return;

            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].transform.rotation = Quaternion.identity;
            }

            pages[0].transform.SetAsLastSibling();
            _index = -1;
            UpdateButtons();
        }

        public void RotateForward() => TryFlip(_forwardAngle, forward: true);
        public void RotateBack() => TryFlip(_backAngle, forward: false);

        public void TryFlip(float angle, bool forward)
        {
            if (forward)
            {
                if (_index >= pages.Count - 1) return;

                _index++;
            }
            else
            {
                if (_index < 0) return;
            }

            PlayFlipSfx();
            BringPageOnTop(_index);
            StartCoroutine(FlipCoroutine(pages[_index].transform, angle, forward, startedIndex: _index));
        }


        private IEnumerator FlipCoroutine(Transform page, float targetAngle, bool forward, int startedIndex)
        {
            _isFlipping = true;
            
            var targetRotation = Quaternion.Euler(0, targetAngle, 0);
            float time = 0f;

            PageInvert inverter = null;
            page.TryGetComponent(out inverter);
            
            while (true)
            {
                time += Time.deltaTime * pageSpeed;
                page.rotation = Quaternion.Slerp(page.rotation, targetRotation, time);
                
                float remaining = Quaternion.Angle(page.rotation, targetRotation);

                if (remaining < 90f && inverter != null)
                {
                    if (forward)
                        inverter.ShowInverted();
                    else
                        inverter.ShowNormal();
                }

                if (remaining < 0.1f) break;
                yield return null;
            }

            if (!forward)
                _index = startedIndex - 1;
            
            _isFlipping = false;
            UpdateButtons();
        }

        #region Helpers

        private void PlayFlipSfx()
        {
            if (flipCue == null) return;

            var clip = AudioCue.GetRandomClip(flipCue.Clips);
            if (clip != null)
            {
                AudioManager.instance?.PlaySFX(clip);
            }
        }

        private void UpdateButtons()
        {
            if (backButton)
                backButton.SetActive(_index >= 0);

            if (forwardButton)
                forwardButton.SetActive(pages != null && _index < pages.Count - 1);
        }

        private void BringPageOnTop(int i)
        {
            pages[i].transform.SetAsLastSibling();
        }

        #endregion
    }
}
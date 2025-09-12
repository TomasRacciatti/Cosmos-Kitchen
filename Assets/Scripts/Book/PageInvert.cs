using UnityEngine;

namespace Book
{
    public class PageInvert : MonoBehaviour
    {
        [SerializeField] GameObject _pageNormal;
        [SerializeField] GameObject _pageInverted;

        private void Start()
        {
            ShowNormal();
        }

        public void ShowNormal()
        {
            _pageNormal.SetActive(true);
            _pageInverted.SetActive(false);
        }
        public void ShowInverted()
        {
            _pageNormal.SetActive(false);
            _pageInverted.SetActive(true);
        }
    }
}

using UnityEngine;

namespace Book
{
    public class BookHandler : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AudioClip openSfx;
        [SerializeField] private AudioClip closeSfx;
        private bool _bookOpen = false;
        
        public bool IsOpen => _bookOpen;

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
        }
    
        private void Start()
        {
            animator.Play("CloseBook", 0,1f);
        }
    
        public bool ToggleBook()
        {
            _bookOpen = !_bookOpen;
            animator.speed = 1;
            animator.Play(_bookOpen ? "OpenBook" : "CloseBook", 0, 
                1 - Mathf.Clamp01(animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
            //AudioManager.instance.PlaySFX(_bookOpen ? openSfx : closeSfx);
            return _bookOpen;
        }
    }
}

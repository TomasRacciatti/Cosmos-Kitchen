using UnityEngine;

namespace Regulators
{
    public class ObjectSwitcher : MonoBehaviour
    {
        [SerializeField] private int activeIndex = 0;
        public int CurrentIndex => activeIndex;

        void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(i == activeIndex);
            }
        }

        public void SwitchTo(int index)
        {
            if (index == activeIndex) return;
            if (index < 0 || index >= transform.childCount) return;

            transform.GetChild(activeIndex).gameObject.SetActive(false);
            activeIndex = index;
            transform.GetChild(activeIndex).gameObject.SetActive(true);
        }

        public void Next()
        {
            int childCount = transform.childCount;
            int nextIndex = (activeIndex + 1) % childCount;
            SwitchTo(nextIndex);
        }

        public void Previous()
        {
            int childCount = transform.childCount;
            int prevIndex = (activeIndex - 1 + childCount) % childCount;
            SwitchTo(prevIndex);
        }
        
        public static int GetIndexSwitcher(ObjectSwitcher switcher) //-1 si esta desactivado
        {
            if (!switcher.gameObject.activeInHierarchy) return -1;
            return switcher.CurrentIndex;
        }
    }
}
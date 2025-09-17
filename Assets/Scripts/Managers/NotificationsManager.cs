using HUD;
using Regulators;
using UnityEngine;

namespace Managers
{
    public class NotificationsManager : MonoBehaviour
    {
        private static NotificationsManager _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
        }

        public static void NewNotification(string text, Sprite sprite)
        {
            GameObject notification = ObjectPool.SpawnObject(PrefabsManager.NotificationPrefabUI, Vector3.zero, Quaternion.identity);
            notification.GetComponent<Notification>().SetNotification(text, sprite);
            notification.transform.SetParent(_instance.transform, false);
            notification.transform.localScale = Vector3.one;
        }
    }
}
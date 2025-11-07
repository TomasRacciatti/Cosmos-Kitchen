using System.Collections.Generic;
using Characters.Customers;
using Managers;
using UnityEngine;

namespace Characters.Clients.Plates
{
    public class PlatesOrdered : MonoBehaviour
    {
        [SerializeField] private GameObject orderUIPrefab;
        
        private static PlatesOrdered _instance;
        private readonly Dictionary<Customer, OrderUI> customersOrders = new();
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
                return;
            }
            
            _instance = this;
            Hide();
        }

        public static void AddCustomer(Customer customer)
        {
            if (_instance.customersOrders.ContainsKey(customer)) return;
            
            GameObject spawnedObject = Instantiate(_instance.orderUIPrefab, _instance.transform);
            OrderUI orderUI = spawnedObject.GetComponent<OrderUI>();
            orderUI.SetOrderUI(customer, false);
            
            _instance.customersOrders.Add(customer, orderUI);
            
            NotificationsManager.NewNotification(
                "New Order: " + customer.soClient.clientName,
                customer.soClient.clientIcon
            );
        }
        
        public static void UpdateCustomerOrder(Customer customer)
        {
            if (_instance.customersOrders.TryGetValue(customer, out OrderUI orderUI))
            {
                orderUI.SetOrderUI(customer, true);
            }
        }

        public static void RemoveCustomer(Customer customer)
        {
            if (!_instance.customersOrders.TryGetValue(customer, out OrderUI orderUI)) return;
            
            if (orderUI != null) Destroy(orderUI.gameObject);

            _instance.customersOrders.Remove(customer);
        }

        public static void Show() => _instance.gameObject.SetActive(true);
        public static void Hide() => _instance.gameObject.SetActive(false);
        public static void Toggle() => _instance.gameObject.SetActive(!_instance.gameObject.activeSelf);
    }
}

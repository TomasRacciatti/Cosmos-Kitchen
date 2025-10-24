using System.Collections.Generic;
using System.Linq;
using Characters.Customers;
using Managers;
using UnityEngine;

namespace Characters.Clients.Plates
{
    public class PlatesOrdered : MonoBehaviour //ESTA TODO HARDCODEADO HARD
    {
        [SerializeField] private GameObject orderUIPrefab;
        
        private static PlatesOrdered _instance;

        private readonly List<Customer> customers = new();
        private readonly List<OrderUI> orders = new();
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
            }
            _instance = this;
            Hide();
        }

        public static void AddCustomer(Customer customer)
        {
            if (!_instance.customers.Contains(customer))
            {
                _instance.customers.Add(customer);
                GameObject spawnedObject = Instantiate(_instance.orderUIPrefab, _instance.transform); // falta obj pool pero paja me dio
                spawnedObject.transform.SetParent(_instance.transform);
                OrderUI orderUI = spawnedObject.GetComponent<OrderUI>();
                orderUI.SetOrderUI(customer);
                _instance.orders.Add(orderUI);
                
                NotificationsManager.NewNotification("New Order: " + customer.soClient.clientName, customer.soClient.clientIcon);
            }
        }

        public static void RemoveCustomer(Customer customer)
        {
            if (_instance.customers.Contains(customer))
            {
                foreach (var order in _instance.orders.ToList())
                {
                    if (order.IsCustomer(customer))
                    {
                        _instance.orders.Remove(order);
                        Destroy(order.gameObject);
                    }
                }
                _instance.customers.Remove(customer);
            }
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
        }
        
        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
        }

        public static void Toggle()
        {
            _instance.gameObject.SetActive(!_instance.gameObject.activeSelf);
        }
    }
}
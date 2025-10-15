using Characters.Customers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Clients.Plates
{
    public class OrderUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;

        private Customer _customer;

        public void SetOrderUI(Customer customer)
        {
            _customer = customer;
            image.sprite = customer.soClient.clientIcon;
            text.text = customer.soClient.requestedSoPlate.ItemName;
        }

        public bool IsCustomer(Customer customer)
        {
            return _customer == customer;
        }
    }
}
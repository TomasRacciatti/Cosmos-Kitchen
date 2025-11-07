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

        public void SetOrderUI(Customer customer, bool retry)
        {
            _customer = customer;
            image.sprite = customer.soClient.clientIcon;
            text.text = retry ? customer.soClient.clue2 : customer.soClient.clue1;
        }

        public bool IsCustomer(Customer customer)
        {
            return _customer == customer;
        }
    }
}
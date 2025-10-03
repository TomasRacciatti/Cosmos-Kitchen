using System;
using Items.Inventory;
using UnityEngine;

namespace Stations.Serving
{
    public class ServingUIManager : MonoBehaviour
    {
        [SerializeField] public InvView inputView;
        [SerializeField] public InvView outputView;
    }
}
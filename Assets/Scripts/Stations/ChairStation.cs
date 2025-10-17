using Characters.Player;
using Managers;
using Regulators;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stations
{
    public class ChairStation : Station
    {
        protected override void EnterStation()
        {
            if (GameManager.Player.GetScore() >= 10)
            {
                SceneManager.LoadScene("Desert");
            }
        }
    }
}
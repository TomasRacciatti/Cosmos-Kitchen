using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMinigame
{
    void StartMinigame(System.Action onSuccess, System.Action onFailure);
}


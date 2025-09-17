using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCExpressions : MonoBehaviour
{
    [Header("Eyes")]
    [SerializeField] Texture2D[] eyeTex;
    [SerializeField] Material eyeMat;

    [Header("Mouth")]

    [SerializeField] Texture2D[] mouthTex;
    [SerializeField] Material mouthMat;

    [Header("Icon")]
    [SerializeField] Texture2D[] iconTex;
    [SerializeField] Material iconMat;

    public void ChangeEyes(int index)
    {
        if (eyeMat != null && eyeTex != null && index >= 0 && index < eyeTex.Length)
        {
            eyeMat.SetTexture("_BaseMap", eyeTex[index]);
        }
        else
        {
            Debug.LogWarning("Algo salio mal asignando el material de los ojos :(.");
        }
    }

    public void Shuffle()  //Esta es para testear
    {
        eyeMat.SetTexture("_BaseMap", eyeTex[Random.Range(0, eyeTex.Length)]);
        mouthMat.SetTexture("_BaseMap", mouthTex[Random.Range(0, mouthTex.Length)]);
        iconMat.SetTexture("_BaseMap", iconTex[Random.Range(0, iconTex.Length)]);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    Vector2 StartPos;
    Vector2 StartPercent;

    [SerializeField] int moveModifier;

    private void Start()
    {
        StartPercent = new Vector2 (transform.position.x*100f / Screen.width, transform.position.y*100f / Screen.height);
    }
    private void Update()
    {
        Vector2 pz = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        float posX = Mathf.Lerp(transform.position.x, Screen.width * StartPercent.x/100f + (pz.x * moveModifier), 2f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.position.y, Screen.height * StartPercent.y/100f + (pz.y * moveModifier), 2f * Time.deltaTime);
        transform.position =  new Vector3 (posX,posY,0);
    }
}

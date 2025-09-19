using UnityEngine;

public class Grass1 : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float bendAngle = 30f;

    private Quaternion originalRotation; 
    private Quaternion targetRotation; 
    private Transform intruder;

    private int charactersLayer;

    void Start()
    {
        originalRotation = transform.rotation;
        targetRotation = originalRotation;

        charactersLayer = LayerMask.NameToLayer("Characters");
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != charactersLayer) return; 

        intruder = other.transform;

        Vector3 awayDir = (transform.position - intruder.position).normalized;
        targetRotation = Quaternion.LookRotation(awayDir, Vector3.up) * Quaternion.Euler(bendAngle, 0, 0);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != charactersLayer) return; 

        if (other.transform == intruder)
        {
            targetRotation = originalRotation;
            intruder = null;
        }
    }
}
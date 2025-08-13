/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpacing : MonoBehaviour
{
    public GameObject fencePrefab;
    public int numberOfFences = 70;
    public float spacing = 2.0f;
    public Vector3 direction = Vector3.right;
    public Color gizmoColor = Color.green;

    private void Start()
    {
        for (int i = 0; i < numberOfFences; i++)
        {
            Vector3 position = transform.position + direction.normalized * spacing * i;
            Instantiate(fencePrefab, position, Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        for (int i = 0; i < numberOfFences; i++)
        {
            Vector3 position = transform.position + direction.normalized * spacing * i;
            Gizmos.DrawWireCube(position, new Vector3(1, 1, 0.2f)); // adjust to your fence size
        }
    }
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpacing : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject fencePrefab;
    public int numberOfFences = 70;
    public float spacing = 2.0f;
    public Vector3 direction = Vector3.right;

    [Header("Randomization Settings")]
    public Vector3 positionJitter = Vector3.zero; // e.g., new Vector3(0.2f, 0f, 0.2f)
    public Vector3 randomRotationRange = Vector3.zero; // per instance random rotation (degrees)

    [Header("Global Rotation")]
    public Vector3 baseRotation = Vector3.zero; // applied to all instances

    [Header("Gizmo Settings")]
    public Color gizmoColor = Color.green;

    private void Start()
    {
        for (int i = 0; i < numberOfFences; i++)
        {
            // Base position
            Vector3 basePosition = transform.position + direction.normalized * spacing * i;

            // Add random offset
            Vector3 jitter = new Vector3(
                Random.Range(-positionJitter.x, positionJitter.x),
                Random.Range(-positionJitter.y, positionJitter.y),
                Random.Range(-positionJitter.z, positionJitter.z)
            );
            Vector3 finalPosition = basePosition + jitter;

            // Calculate rotation
            Quaternion rotation = Quaternion.Euler(baseRotation +
                new Vector3(
                    Random.Range(-randomRotationRange.x, randomRotationRange.x),
                    Random.Range(-randomRotationRange.y, randomRotationRange.y),
                    Random.Range(-randomRotationRange.z, randomRotationRange.z)
                )
            );

            // Instantiate prefab
            Instantiate(fencePrefab, finalPosition, rotation, transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        for (int i = 0; i < numberOfFences; i++)
        {
            Vector3 basePosition = transform.position + direction.normalized * spacing * i;
            Vector3 jitter = new Vector3(
                Random.Range(-positionJitter.x, positionJitter.x),
                Random.Range(-positionJitter.y, positionJitter.y),
                Random.Range(-positionJitter.z, positionJitter.z)
            );
            Vector3 finalPosition = basePosition + jitter;

            Gizmos.DrawWireCube(finalPosition, new Vector3(1, 1, 0.2f)); // adjust as needed
        }
    }
}

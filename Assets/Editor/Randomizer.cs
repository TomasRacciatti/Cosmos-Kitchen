using UnityEngine;
using UnityEditor;

public class Randomizer : EditorWindow
{

    bool randomX;
    bool randomY;
    bool randomZ;

    bool randomScale;
    float minScale;
    float maxScale;

    [MenuItem("Custom Tools/Sacle and Rotation Randomizer")]

    static void OpenWindow()
    {
        Randomizer window = (Randomizer) GetWindow(typeof(Randomizer));

        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Randomise selected objects", EditorStyles.boldLabel);

        GUILayout.Label("Rotation");
        randomX = EditorGUILayout.Toggle("Randomise X", randomX);
        randomY = EditorGUILayout.Toggle("Randomise Y", randomY);
        randomZ = EditorGUILayout.Toggle("Randomise Z", randomZ);
        
        GUILayout.Label("Scaling");
        randomScale = EditorGUILayout.Toggle("Randomize Scale", randomScale);
        minScale = EditorGUILayout.FloatField("Min Scale", minScale);
        maxScale = EditorGUILayout.FloatField("Max Scale", maxScale);

        if (GUILayout.Button("Randomise"))
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                go.transform.rotation = Quaternion.Euler(GetRandomRotations(go.transform.rotation.eulerAngles));

                if (randomScale)
                {
                    float scaleValue = Random.Range(minScale, maxScale);
                    go.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
                }
            }
        }
    }

    private Vector3 GetRandomRotations (Vector3 currentRotation)
    {
        float x = randomX ? Random.Range(0f, 360f) : currentRotation.x;
        float y = randomY ? Random.Range(0f, 360f) : currentRotation.y;
        float z = randomZ ? Random.Range(0f, 360f) : currentRotation.z;

        return new Vector3(x, y, z);
    }

}

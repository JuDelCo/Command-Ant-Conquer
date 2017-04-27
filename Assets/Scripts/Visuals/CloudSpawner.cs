using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{

    public GameObject cloudPrefab;

    public int cloudCount = 10;
    public float radius = 5;

    void Start()
    {
        for (int i = 0; i < cloudCount; i++)
        {
            float x = Mathf.Sin(Random.value * 2 * Mathf.PI) * radius * Random.value;
            float z = Mathf.Cos(Random.value * 2 * Mathf.PI) * radius * Random.value;
            var position = transform.position + new Vector3(x, 0, z);
            Instantiate(cloudPrefab, position, Quaternion.identity);
        }
    }

    void Update()
    {

    }
}

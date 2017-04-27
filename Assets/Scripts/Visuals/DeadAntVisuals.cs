using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadAntVisuals : MonoBehaviour
{

    public GameObject ghostPrefab;

    void Start()
    {
        Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        Destroy(this);
    }

}

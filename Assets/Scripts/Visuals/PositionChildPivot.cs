using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionChildPivot : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        var delta = transform.GetChild(0).position - transform.GetComponentInChildren<Collider>().bounds.center;
        transform.GetChild(0).localPosition = delta;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusGizmo : MonoBehaviour
{

    public float radius = 0.1f;
    public bool disabled = false;

    void OnDrawGizmos()
    {
        if (!disabled)
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}

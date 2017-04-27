using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrthoBillboard : MonoBehaviour
{

    public bool destroyAfterStart = false;

    void Start()
    {
        if (Application.isPlaying && destroyAfterStart)
        {
            Destroy(this, 0.1f);
        }
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position - Camera.main.transform.forward);
    }

}

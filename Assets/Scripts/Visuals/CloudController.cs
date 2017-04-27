using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{

    public float maxSpeed;
    public float minSize = 0.1f;
    public float maxSize = 2f;

    float speed = 1;

    void Start()
    {
        speed = Random.Range(-maxSpeed, maxSpeed);
        transform.localScale = Random.Range(minSize, maxSize) * Vector3.one;
    }

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
        if (transform.position.z < -40) { transform.position += 55 * Vector3.forward; }
        if (transform.position.z > 20) { transform.position -= 55 * Vector3.forward; }
    }
}

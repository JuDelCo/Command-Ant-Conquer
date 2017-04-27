using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollectTipText : MonoBehaviour
{

    public float amplitude = 0.5f;
    public float frequency = 2;

    Vector3 startPosition;

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
        Invoke("Vanish", 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
    }

    void Vanish()
    {
        transform.DOScale(0, 1).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using AntWars;

public class FoodDropEffect : MonoBehaviour
{

    public float duration = 1f;
    public float height = 0.5f;

    public void SetEffect(Vector3 origin, Vector3 destination)
    {
        transform.position = origin;
        transform.DOMoveX(destination.x, duration).SetEase(Ease.Linear);
        transform.DOMoveZ(destination.z, duration).SetEase(Ease.Linear);
        transform.DOMoveY(origin.y + height, duration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() => transform.DOMoveY(destination.y, duration * 0.5f).SetEase(Ease.InQuad));
        transform.DOScale(0.5f, duration * 0.5f).SetDelay(duration * 0.5f).SetEase(Ease.OutExpo);
        Destroy(gameObject, duration);
    }

}

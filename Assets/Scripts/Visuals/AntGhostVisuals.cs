using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AntGhostVisuals : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    public float animationTime = 2f;
    public float animationHeight = 1;

    public Ease ease1, ease2;
    public float strenght = 1;
    public int vibrato = 1;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        transform.DOMoveY(animationHeight, animationTime).SetEase(ease1);
        DOTween.To(() => spriteRenderer.color, x => spriteRenderer.color = x, new Color(1, 1, 1, 0), animationTime).SetEase(ease2);
        spriteRenderer.transform.DOPunchPosition(transform.right * strenght, animationTime, Random.Range(0, vibrato), 90);
    }

}

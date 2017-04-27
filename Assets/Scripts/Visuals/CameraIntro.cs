using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CameraIntro : MonoBehaviour
{

    public Ease ease;
    public CanvasGroup canvasGroup;
    public AudioSource audioSource;
    public AudioSource alternateMusic;

    void Start()
    {
        DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, 120f, 5f).From().SetEase(ease);
        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 5f).From().SetEase(ease);
        Invoke("PlayMusic", 4);
    }

    void PlayMusic()
    {
        //if (Random.value > 0.1f)
        {
            audioSource.Play();
        }
        /*else
        {
            alternateMusic.Play();
        }*/
    }

}

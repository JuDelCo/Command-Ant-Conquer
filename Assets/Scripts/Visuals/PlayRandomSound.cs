using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{

    public AudioClip[] clips;

    public bool playAtStart = true;
    public bool destroyAfterPlay = true;

    void Start()
    {
        if (playAtStart)
        {
            PlaySound();
        }
    }

    public void PlaySound()
    {
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        if (destroyAfterPlay)
        {
            Destroy(this, clip.length);
        }
    }


}

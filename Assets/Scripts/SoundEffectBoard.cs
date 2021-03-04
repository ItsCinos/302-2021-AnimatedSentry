using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectBoard : MonoBehaviour
{
    public static SoundEffectBoard main;

    public AudioClip soundPlayerShot;
    public AudioClip soundSentryShot;

    private AudioSource player;

    void Start()
    {
        if (main == null)
        {
            main = this;
            player = GetComponent<AudioSource>();
        }
        else {
            Destroy(this.gameObject);
        }
    }

    public static void PlayPlayerShot()
    {
        main.player.PlayOneShot(main.soundPlayerShot);
    }
    public static void PlaySentryShot()
    {
        main.player.PlayOneShot(main.soundSentryShot);
    }
}

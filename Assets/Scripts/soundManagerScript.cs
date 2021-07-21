using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManagerScript : MonoBehaviour
{
    public static AudioClip reloadSound;
    public static AudioClip shootingSound;
    public static AudioClip dryFire;
    static AudioSource audioSrc;




    void Start()
    {
        reloadSound = Resources.Load<AudioClip>("reloading");
        shootingSound = Resources.Load<AudioClip>("gunShot");
        dryFire = Resources.Load<AudioClip>("dryFire");

        audioSrc = GetComponent<AudioSource>();
    }
    
    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "fire":
                audioSrc.PlayOneShot(shootingSound);
                break;
            case "reload":
                audioSrc.PlayOneShot(reloadSound);
                break;
            case "dryFire":
                audioSrc.PlayOneShot(dryFire);
                break;
        }
    }
}
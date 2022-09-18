using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource BackgroundMusic;
    public AudioSource GameOverMusic;

    public bool background = true;
    public bool gameover = false;

    public void PlayBackgroundMusic()
    {
        background = true;
        gameover = false;
        BackgroundMusic.Play();
    }

    public void PlayGameOverMusic()
    {
        if(BackgroundMusic.isPlaying)
        {
            background = false;
        }
        BackgroundMusic.Stop();

        if(GameOverMusic.isPlaying && gameover == false)
        {
            GameOverMusic.Play();
            gameover = true;
        }
    }
}

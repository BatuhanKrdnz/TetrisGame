﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public bool m_musicEnabled = true;

    public bool m_fxEnabled = true;

    [Range(0,1)]
    public float m_musicVolume = 1.0f;

    [Range(0, 1)]
    public float m_fxVolume = 1.0f;

    public AudioClip m_clearRowSound;

    public AudioClip m_moveSound;

    public AudioClip m_dropSound;

    public AudioClip m_errorSound;

    public AudioClip m_gameOverSound;

    //public AudioClip m_backgroundMusic;

    public AudioSource m_musicSource;

    public AudioClip[] m_musicClips;

    public AudioClip[] m_vocalClips;

    public AudioClip m_gameOverVocalClip;

    public AudioClip m_levelUpVocalClip;

    public IconToggle m_musicIconToggle;

    public IconToggle m_fxIconToggle;


    AudioClip m_randomMusicClip;

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        return randomClip;
    }

    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if(!m_musicEnabled || !musicClip || !m_musicSource)
        {
            return;
        }

        m_musicSource.Stop();

        m_musicSource.clip = musicClip;

        m_musicSource.volume = m_musicVolume;

        m_musicSource.loop = true;

        m_musicSource.Play();
    }

    void UpdateMusic()
    {
        if(m_musicSource.isPlaying != m_musicEnabled)
        {
            if (m_musicEnabled)
            {
                PlayBackgroundMusic(GetRandomClip(m_musicClips));
            }
            else
                m_musicSource.Stop();
        }
    }

    public void ToggleMusic()
    {
        m_musicEnabled = !m_musicEnabled;
        UpdateMusic();

        if (m_musicIconToggle)
        {
            m_musicIconToggle.ToggleIcon(m_musicEnabled);
        }
    }

    public void ToggleFx()
    {
        m_fxEnabled = !m_fxEnabled;

        if (m_fxIconToggle)
        {
            m_fxIconToggle.ToggleIcon(m_fxEnabled);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayBackgroundMusic(GetRandomClip(m_musicClips));

        //or
        //m_randomMusicClip = GetRandomClip(m_musicClips);
        //PlayBackgroundMusic(m_randomMusicClip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

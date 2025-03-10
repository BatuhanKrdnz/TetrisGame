﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    Board m_gameBoard;

    Spawner m_spawner;

    Shape m_activeShape;

    public float m_dropInterval = 0.1f;
    float m_dropIntervalModded;

    float m_timeToDrop;


    float m_timeToNextKeyLeftRight;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateLeftRight = 0.25f;

    float m_timeToNextKeyDown;

    [Range(0.02f, 0.5f)]
    public float m_keyRepeatRateDown = 0.01f;

    float m_timeToNextKeyRotate;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateRotate = 0.25f;

    bool m_gameOver = false;

    public GameObject m_gameOverPanel;

    SoundManager m_soundManager;

    public IconToggle m_rotateIconToggle;

    bool m_clockwise = true;

    public bool m_isPaused = false;

    public GameObject m_pausePanel;

    ScoreManager m_scoreManager;


    private void Start()
    {
        //Tag method faster
        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        m_scoreManager = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();

        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        //Alternative Method(This is slow)
        //m_gameBoard = GameObject.FindObjectOfType<Board>();
        //m_spawner = GameObject.FindObjectOfType<Spawner>();
        ////
        //also same
        //m_gameBoard = GameObject.FindObjectOfType(typeof(Board)) as Board;
        //m_spawner = GameObject.FindObjectOfType(typeof(Spawner)) as Spawner;

        if (!m_gameBoard)
            Debug.LogWarning("WARNING! MISSING GAME BOARD!!!");

        if(!m_soundManager)
            Debug.LogWarning("WARNING! MISSING SOUND MANAGER!!!");

        if (!m_spawner)
            Debug.LogWarning("WARNING! MISSING SPAWNER!!!");

        if (!m_scoreManager)
            Debug.LogWarning("WARNING! MISSING SCORE MANAGER!!!");
        else
        {
            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);
            if (!m_activeShape)
            {
                m_activeShape = m_spawner.SpawnShape();
            }

        }
        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(false);
        }

        m_dropIntervalModded = m_dropInterval;
    }

    void PlayerInput()
    {
        if (Input.GetButton("MoveRight") && Time.time > m_timeToNextKeyLeftRight || Input.GetButtonDown("MoveRight"))
        {
            m_activeShape.MoveRight();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveLeft();
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
                PlaySound(m_soundManager.m_moveSound, 0.5f);
        }

        else if (Input.GetButton("MoveLeft") && Time.time > m_timeToNextKeyLeftRight || Input.GetButtonDown("MoveLeft"))
        {
            m_activeShape.MoveLeft();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
                PlaySound(m_soundManager.m_moveSound, 0.5f);

        }
        else if (Input.GetButtonDown("Rotate") && Time.time > m_timeToNextKeyRotate)
        {
            // m_activeShape.RotateRight();
            m_activeShape.RotateClockwise(m_clockwise);
            m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                //m_activeShape.RotateLeft();
                m_activeShape.RotateClockwise(!m_clockwise);
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
                PlaySound(m_soundManager.m_moveSound, 0.4f);
        }

        else if (Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown) || (Time.time > m_timeToDrop))
        {
            m_timeToDrop = Time.time + m_dropIntervalModded;
            m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;

            m_activeShape.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                if (m_gameBoard.IsOverLimit(m_activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
                
            }
        }
        
        else if (Input.GetButtonDown("ToggleRotation"))
        {
            ToggleRotateDirection();
        }

        else if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
    }

    private void GameOver()
    {
        m_activeShape.MoveUp();

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);
        }

        PlaySound(m_soundManager.m_gameOverVocalClip, 5f);
        PlaySound(m_soundManager.m_gameOverSound, 4f);
        m_gameOver = true;
    }

    private void LandShape()
    {
        //shape landing
        //m_timeToNextKeyDown = Time.time;

        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;

        m_activeShape.MoveUp();

        m_gameBoard.StoreShapeInGrid(m_activeShape);

        PlaySound(m_soundManager.m_dropSound, 0.60f);

        m_activeShape = m_spawner.SpawnShape();

        m_gameBoard.ClearAllRows();

        if(m_gameBoard.m_completedRows > 0)
        {
            m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);

            if (m_scoreManager.m_didLevelUp)
            {
                PlaySound(m_soundManager.m_levelUpVocalClip, 2f);
                m_dropIntervalModded = Mathf.Clamp(m_dropInterval - (((float)m_scoreManager.m_level - 1) * 0.05f), 0.05f, 1f);
            }
            else
            {
                if (m_gameBoard.m_completedRows > 1)
                {
                    AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_vocalClips);
                    PlaySound(randomVocal, 0.50f);
                }
            }
          
            PlaySound(m_soundManager.m_clearRowSound, 0.50f);
        }
        
    }

    void PlaySound(AudioClip clip, float volMultiplier)
    {
        if (clip && m_soundManager.m_fxEnabled)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, Mathf.Clamp(m_soundManager.m_fxVolume * volMultiplier,0.05f,1f));

    }

    public void ToggleRotateDirection()
    {
        m_clockwise = !m_clockwise;

        if (m_rotateIconToggle)
        {
            m_rotateIconToggle.ToggleIcon(m_clockwise);
        }
    }

    public void TogglePause()
    {
        if (m_gameOver)
        {
            return;
        }

        m_isPaused = !m_isPaused;

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(m_isPaused);

            if (m_soundManager)
            {
                m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * 0.25f : m_soundManager.m_musicVolume;
            }
            Time.timeScale = (m_isPaused) ? 0 : 1;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }


    private void Update()
    {
        if (!m_gameBoard || !m_spawner || !m_activeShape || m_gameOver || !m_soundManager || !m_scoreManager)
        {
            return;
        }

        PlayerInput();
    }
}





    


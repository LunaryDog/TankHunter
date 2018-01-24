using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ScreenMenuEvent
{
    PAUSE
}

public class ScreenMenu : BaseBehaviour {
    Observer observer;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    private void Awake()
    {
        observer = Observer.Instance;
    }

    private void Start()
    {
        observer.AddListener(GameManagerEvent.WIN, this, WinMenu);
        observer.AddListener(GameManagerEvent.LOSE, this, LoseMenu);
        observer.AddListener(ScreenMenuEvent.PAUSE, this, PauseMenu);
    }
    public void RestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }

    public void NextButton()
    {
        object[] files = (object[])Resources.LoadAll("Levels");
        if (LevelsController.NumLoadLevel < files.Length)
        {
            LevelsController.NumLoadLevel++;
        }
        SceneManager.LoadScene(2);
    }

    public void ExitButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void PauseButton()
    {
        observer.SendMessage(ScreenMenuEvent.PAUSE);
    }

    public void CloseButton()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    void WinMenu(ObservParam obj)
    {
        winPanel.SetActive(true);
    }

    void LoseMenu(ObservParam obj)
    {
        losePanel.SetActive(true);
    }

    void PauseMenu(ObservParam obj)
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }
    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }
}

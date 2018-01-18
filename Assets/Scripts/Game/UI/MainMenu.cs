using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }

    public void SettingButton()
    {
    }

    public void InfoButton()
    {
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour {
    int countLevel = 1;
    Button[] levelButtons;

    private void Awake()
    {
        LevelsController.NumLoadLevel = 1;
        object[] files = (object[])Resources.LoadAll("Levels");
        countLevel = files.Length;
       
    }

    private void Start()
    {
        levelButtons = GetComponentsInChildren<Button>();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int num =  System.Convert.ToInt32( levelButtons[i].name.Substring(6));
            if (num <= countLevel )
            {
                levelButtons[i].interactable = true;                
                levelButtons[i].GetComponentInChildren<Text>().gameObject.SetActive(true);
                levelButtons[i].transform.GetChild(1).gameObject.SetActive(false);               
            }
            else
            {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponentInChildren<Text>().gameObject.SetActive(false);
                levelButtons[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public void LoadLevel(int i)
    {
        LevelsController.NumLoadLevel = i;
        if (LevelsController.NumLoadLevel > countLevel)
        {
            LevelsController.NumLoadLevel = countLevel;
        }
        SceneManager.LoadScene(2);
    }
}

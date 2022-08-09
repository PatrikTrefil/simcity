using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public void OnQuitButtonClick()
    {
        bool res = EditorUtility.DisplayDialog("Really want to go back to the main menu?", "Really want to go back? The game will be saved automatically.", "Yes", "No");
        if (res)
        {
            // TODO: save game
            SceneManager.LoadScene("StartScene");
        }
    }
}

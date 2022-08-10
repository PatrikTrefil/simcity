using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simcity
{
    public class Game : MonoBehaviour
    {
        public static bool ShouldLoadGame { get; set; }
        public City city;
        public TimeManager timeManager;
        public bool isGameLost = false;
        public void OnQuitButtonClick()
        {
            bool res = EditorUtility.DisplayDialog("Really want to go back to the main menu?", "Really want to go back? The game will be saved automatically.", "Yes", "No");
            if (res)
            {
                var gameData = new SaveSystem.GameData(this);
                SaveSystem.SaveGame(gameData);
                SceneManager.LoadScene("StartScene");
            }
        }
        private void Start()
        {
            if (ShouldLoadGame)
            {
                var gameData = SaveSystem.LoadGame();
                if (gameData != null)
                {
                    LoadGame(gameData);
                }
                else
                {
                    Debug.Log("Saved game not found");
                }
            }
        }
        private void LoadGame(SaveSystem.GameData gameData)
        {
            timeManager.LoadFromTimeManagerData(gameData.timeManagerData);
            city.LoadFromCityData(gameData.cityData);
            Debug.Log("Saved game loaded");
        }
        private void Update()
        {
            if (isGameLost)
            {
                EditorUtility.DisplayDialog("You lost", "Your balance has gone below zero.", "Go to main menu");
                SceneManager.LoadScene("StartScene");
            }
        }
    }
}

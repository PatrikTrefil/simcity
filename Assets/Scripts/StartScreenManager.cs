using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simcity
{
    public class StartScreenManager : MonoBehaviour
    {
        public void OnNewGameClick()
        {
            Game.ShouldLoadGame = false;
            SceneManager.LoadScene("GameScene");
        }
        public void OnLoadGameClick()
        {
            Game.ShouldLoadGame = true;
            SceneManager.LoadScene("GameScene");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simcity
{
    public class LostSceneManager : MonoBehaviour
    {
        public void OnBackToMainMenuButtonClick()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Simcity
{
    public sealed class TimeManager : MonoBehaviour
    {
        /// <summary>
        /// Used to convert realworld seconds from beginning
        /// to game time seconds from beginning
        /// </summary>
        private readonly int realTimeToGameTimeFactor = 60;
        /// <summary>
        /// Backing field for SecondsFromBeginning
        /// beginning is Jan 1, 0001
        /// </summary>
        private float realworldSecondsFromBeginning;
        /// <summary>
        /// beginning is Jan 1, 0001
        /// </summary>
        public float RealwordSecondsFromBeginning
        {
            get => realworldSecondsFromBeginning;
            set
            {
                realworldSecondsFromBeginning = value;
                // Update label in UI
                var gameTimeSecondsFromBeginning = RealworldSecondsToGametimeSecondsFromBeginning(realworldSecondsFromBeginning);
                textComponent.text = new DateTime().AddSeconds(gameTimeSecondsFromBeginning).ToString("HH:mm dd.MM.yyyy");
            }
        }
        public TMP_Dropdown timeScaleDropdown;
        public TMP_Text textComponent;

        public TimeManager()
        {
            realworldSecondsFromBeginning = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            RealwordSecondsFromBeginning += Time.deltaTime;
        }

        /// <summary>
        /// Get value which is selected in given dropdown.
        /// </summary>
        /// <exception cref="ArgumentException">if the dropdown provided an unknown value</exception>
        private int GetTimeScaleValueFromDropdown()
        {
            switch (timeScaleDropdown.captionText.text)
            {
                case "1x":
                    return 1;
                case "2x":
                    return 2;
                case "3x":
                    return 3;
                default:
                    throw new ArgumentException("Unknown value from time scale dropdown");
            }
        }

        public void OnTimeScaleDropdownChange()
        {
            Time.timeScale = GetTimeScaleValueFromDropdown();
        }

        public void OnPauseButtonClick()
        {
            Time.timeScale = 0;
        }

        public void OnPlayButtonClick()
        {
            Time.timeScale = GetTimeScaleValueFromDropdown();
        }

        private float RealworldSecondsToGametimeSecondsFromBeginning(float realworldSecondsFromBeginning)
        {
            return realworldSecondsFromBeginning * realTimeToGameTimeFactor;
        }

        public void LoadFromTimeManagerData(SaveSystem.GameData.TimeManagerData timeManagerData)
        {

            RealwordSecondsFromBeginning = timeManagerData.realworldSecondsFromBeginning;
        }
    }
}

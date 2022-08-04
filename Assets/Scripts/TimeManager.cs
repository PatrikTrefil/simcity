using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    private readonly int defaultTimeFactor = 60;
    private float secondsFromBeginning;
    /// <summary>
    /// beginning is Jan 1, 0001
    /// </summary>
    public float SecondsFromBeginning
    {
        get => secondsFromBeginning;
        private set
        {
            textComponent.text = new DateTime().AddSeconds(secondsFromBeginning * defaultTimeFactor).ToString("HH:mm dd.MM.yyyy");
            secondsFromBeginning = value;
        }
    }
    public TMP_Text textComponent;
    // Start is called before the first frame update
    void Start()
    {
        SecondsFromBeginning = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SecondsFromBeginning += Time.deltaTime;
    }

    public void OnTimeScaleDropdownChange(TMP_Dropdown dropdown)
    {
        Debug.Log(dropdown.captionText.text);
        int factor;
        switch (dropdown.captionText.text)
        {
            case "1x":
                factor = 1;
                break;
            case "2x":
                factor = 2;
                break;
            case "3x":
                factor = 3;
                break;
            default:
                throw new ArgumentException("Unknown value from time scale dropdown");
        }
        Time.timeScale = factor;
    }
}

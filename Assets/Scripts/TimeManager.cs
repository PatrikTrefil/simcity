using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    private readonly int defaultTimeScale = 60;
    private float secondsFromBeginning;
    /// <summary>
    /// beginning is Jan 1, 0001
    /// </summary>
    public float SecondsFromBeginning
    {
        get => secondsFromBeginning;
        private set
        {
            textComponent.text = new DateTime().AddSeconds(secondsFromBeginning).ToString("HH:mm dd.MM.yyyy");
            secondsFromBeginning = value;
        }
    }
    public TMP_Text textComponent;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = defaultTimeScale;
        SecondsFromBeginning = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SecondsFromBeginning += Time.deltaTime;
    }
}

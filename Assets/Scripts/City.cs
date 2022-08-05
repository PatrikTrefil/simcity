using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SimulateCity());    
    }

    IEnumerator SimulateCity()
    {
        while (true)
        {
            // one simulation step is run every *game-time* second
            yield return new WaitForSeconds(1);
            SimulateOneStep();
        }
    }

    void SimulateOneStep()
    {
        Debug.Log("Simulate one step");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public Population population;

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

    /// <summary>
    /// simulate one step of all people
    /// </summary>
    void SimulateOneStep()
    {
        foreach (var person in population.People)
        {
            person.SimulateOneStep();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class City : MonoBehaviour
{
    public Population population;
    public Map map;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(SimulateCity());
    }

    private IEnumerator SimulateCity()
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
    private void SimulateOneStep()
    {
        if (population.People.Count > 0)
        {
            Parallel.For(0, population.People.Count, (int index) =>
            {
                population.People[index].SimulateOneStep();
            });
        }
        else
        {
            Debug.Log("Nothing to simulate, there are no people.");
        }
    }

    /// <summary>
    /// put person on map, add to population,
    /// add to list of residents of its residence,
    /// add to list of workers of its workplace
    /// </summary>
    public void AddCityResidentToCity(CityResident person)
    {
        population.People.Add(person);
        person.CurrentBlock.PeopleHere.Add(person);
        person.Residence.Residents.Add(person);
        person.Workplace.Workers.Add(person);
    }

    /// <summary>
    /// remove person from the map, remove from population,
    /// remove from list of residents of its residence,
    /// remove from list of workers of its workplace
    /// </summary>
    /// <param name="person"></param>
    public void RemoveCityResidentFromCity(CityResident person)
    {
        population.People.Remove(person);
        person.CurrentBlock.PeopleHere.Remove(person);
        person.Residence.Residents.Remove(person);
        person.Workplace.Workers.Remove(person);
    }
}

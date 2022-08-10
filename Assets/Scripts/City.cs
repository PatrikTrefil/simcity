using Simcity.MapNamespace;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Simcity
{
    public sealed class City : MonoBehaviour
    {
        public Population population;
        public Map map;
        public FinanceManager financeManager;
        public TMPro.TMP_Text touristCountLabel;
        public readonly object touristLock;
        public List<Tourist> Tourists { get; }

        public City()
        {
            Tourists = new List<Tourist>();
            touristLock = new object();
        }
        // Start is called before the first frame update
        private void Start()
        {
            touristCountLabel.text = population.People.Count.ToString();

            StartCoroutine(SimulateCity());

            StartCoroutine(TouristVisiting());
        }

        private void Update()
        {
            touristCountLabel.text = Tourists.Count.ToString();
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
                // we have to copy the references because People can be modified
                // during simulation
                CityResident[] populationCopy = new CityResident[population.People.Count];
                population.People.CopyTo(populationCopy);

                Parallel.For(0, populationCopy.Length, (int index) =>
                {
                    populationCopy[index].SimulateOneStep();
                });
            }
            else
            {
                Debug.Log("Nothing to simulate, there are no residents.");
            }
            if (Tourists.Count > 0)
            {
                // we have to copy the references because Tourists can be modified
                // during simulation
                Tourist[] touristCopy = new Tourist[Tourists.Count];
                Tourists.CopyTo(touristCopy);

                Parallel.For(0, touristCopy.Length, (int index) =>
                {
                    touristCopy[index].SimulateOneStep();
                });
            }
            else
            {
                Debug.Log("Nothing to simulate, there are no tourists.");
            }
        }

        /// <summary>
        /// put person on map, add to population,
        /// add to list of residents of its residence,
        /// add to list of workers of its workplace
        ///
        /// not atomic, but thread-safe
        /// </summary>
        public void AddCityResidentToCity(CityResident person)
        {
            lock (population.peopleLock)
            {
                population.People.Add(person);
            }
            lock (person.CurrentBlock.peopleHereLock)
            {
                person.CurrentBlock.PeopleHere.Add(person);
            }
            lock (person.Residence.residentsLock)
            {
                person.Residence.Residents.Add(person);
            }
            lock (person.Workplace.workersLock)
            {
                person.Workplace.Workers.Add(person);
            }
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

        public void AddTouristToCity(Tourist tourist)
        {
            Tourists.Add(tourist);
            tourist.CurrentBlock.PeopleHere.Add(tourist);
        }
        /// <summary>
        /// thread-safe
        /// </summary>
        public void RemoveTouristFromCity(Tourist tourist)
        {
            lock (touristLock)
            {
                Tourists.Remove(tourist);
            }
            lock (tourist.CurrentBlock.peopleHereLock)
            {
                tourist.CurrentBlock.PeopleHere.Remove(tourist);
            }
        }
        private IEnumerator TouristVisiting()
        {
            while (true)
            {
                var rnd = Random.Range(0, 100);
                if (rnd < 40)
                {
                    // tourist comes in
                    Tourist tourist = Tourist.GenerateRandomTourist(this);
                    if (tourist != null)
                    {
                        AddTouristToCity(tourist);
                        Debug.Log($"Tourist {tourist.FirstName} just came to visit");
                    }
                }
                yield return new WaitForSeconds(1);
            }
        }
    }
}

using Simcity.MapNamespace;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<Tourist> Tourists { get; }

        public City()
        {
            Tourists = new ObservableCollection<Tourist>();
        }
        // Start is called before the first frame update
        private void Start()
        {
            touristCountLabel.text = population.People.Count.ToString();
            Tourists.CollectionChanged += OnTouristChange;

            StartCoroutine(SimulateCity());

            StartCoroutine(TouristVisiting());
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
                //Parallel.For(0, population.People.Count, (int index) =>
                //{
                //    population.People[index].SimulateOneStep();
                //});
                for (int i = 0; i < population.People.Count; i++)
                {
                    population.People[i].SimulateOneStep();
                }
            }
            else
            {
                Debug.Log("Nothing to simulate, there are no residents.");
            }
            if (Tourists.Count > 0)
            {
                for (int i = 0; i < Tourists.Count; i++)
                {
                    Tourists[i].SimulateOneStep();
                }
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

        public void AddTouristToCity(Tourist tourist)
        {
            Tourists.Add(tourist);
            tourist.CurrentBlock.PeopleHere.Add(tourist);
        }
        public void RemoveTouristFromCity(Tourist tourist)
        {
            Tourists.Remove(tourist);
            tourist.CurrentBlock.PeopleHere.Remove(tourist);
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

        private void OnTouristChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
                )
            {
                touristCountLabel.text = Tourists.Count.ToString();
            }
        }
    }
}

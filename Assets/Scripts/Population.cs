using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

namespace Simcity
{
    public sealed class Population : MonoBehaviour
    {
        public City city;
        public TMP_Text populationCountLabel;
        public ObservableCollection<CityResident> People { get; }

        public Population()
        {
            People = new ObservableCollection<CityResident>();
        }
        // Start is called before the first frame update
        private void Start()
        {
            populationCountLabel.text = People.Count.ToString();

            People.CollectionChanged += OnPeopleChange;

            StartCoroutine(MovingInOfResidents());
        }

        // Update is called once per frame
        private void Update() { }

        /// <summary>
        /// Updates population count on actions which modify count
        /// </summary>
        private void OnPeopleChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
                )
            {
                populationCountLabel.text = People.Count.ToString();
            }
        }

        /// <summary>
        /// this coroutines takes care of people moving in
        /// to the city and moving out
        /// </summary>
        private IEnumerator MovingInOfResidents()
        {
            while (true)
            {
                var rnd = Random.Range(0, 100);
                if (rnd < 40)
                {
                    // random person moves in
                    CityResident resident = CityResident.GenerateRandomCityResident(city);
                    if (resident != null)
                    {
                        Debug.Log($"Somebody moved in (rnd: {rnd})");
                        city.AddCityResidentToCity(resident);
                        Debug.Log($"Resident {resident.FirstName} just moved in");
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }
    }
}

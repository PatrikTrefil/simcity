using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Simcity
{
    public sealed class Population : MonoBehaviour
    {
        public City city;
        public TMP_Text populationCountLabel;
        public List<CityResident> People { get; }
        public readonly object peopleLock;

        public Population()
        {
            People = new List<CityResident>();
            peopleLock = new object();
        }
        // Start is called before the first frame update
        private void Start()
        {
            populationCountLabel.text = People.Count.ToString();

            StartCoroutine(MovingInOfResidents());

            StartCoroutine(PopulationAging());

            StartCoroutine(PopulationDeaths());
        }

        // Update is called once per frame
        private void Update()
        {
            populationCountLabel.text = People.Count.ToString();
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

        private IEnumerator PopulationAging()
        {
            while (true)
            {
                // wait for one year
                yield return new WaitForSeconds(60 * 24 * 7 * 52);
                // increase everyones age
                foreach (var person in People)
                {
                    person.Age++;
                }
                Debug.Log("The population's age has increased by one");
            }
        }

        private IEnumerator PopulationDeaths()
        {
            while (true)
            {
                // wait for one day
                yield return new WaitForSeconds(60 * 25);

                var rnd = UnityEngine.Random.Range(1, 101);

                if (rnd < 100 && People.Count > 0)
                {
                    // oldest person dies
                    CityResident oldestPerson = People[0];
                    foreach (var person in People)
                    {
                        if (person.Age > oldestPerson.Age)
                            oldestPerson = person;
                    }
                    Debug.Log($"[{oldestPerson.FirstName} {oldestPerson.LastName}] Died");
                    city.RemoveCityResidentFromCity(oldestPerson);
                }
            }
        }

        public void LoadFromResidentData(SaveSystem.GameData.CityData.ResidentData[] residentData)
        {
            foreach (var resident in residentData)
            {
                Debug.Log(resident.residenceCoordinates[0]);
                Debug.Log(resident.residenceCoordinates[1]);
                Debug.Log(resident.workplaceCoordinates[0]);
                Debug.Log(resident.workplaceCoordinates[1]);
                var residence = city.map.blocks[resident.residenceCoordinates[0], resident.residenceCoordinates[1]] as MapNamespace.ResidenceBlock;
                var workplace = city.map.blocks[resident.workplaceCoordinates[0], resident.workplaceCoordinates[1]] as MapNamespace.ShopBlock;

                if (residence == null || workplace == null) throw new System.Exception("Residence or workplace from data not found");

                city.AddCityResidentToCity(new CityResident(resident.firstName, resident.lastName, resident.age, city, residence, workplace));
            }
        }
    }
}

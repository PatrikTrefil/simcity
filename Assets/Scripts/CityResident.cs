using Simcity.MapNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    public sealed class CityResident : Person
    {
        /// <summary>
        /// Home
        /// </summary>
        public ResidenceBlock Residence { get; private set; }
        public ShopBlock Workplace { get; private set; }
        /// <summary>
        /// Amount of money obtained for one day of work.
        /// Paid out right after finishing work.
        /// </summary>
        public int DailyWage { get; private set; }
        private City City { get; }
        private IEnumerator simulator;

        public CityResident(string firstName, string lastName, int age, City city, ResidenceBlock residence, ShopBlock workplace) : base(firstName, lastName, age, city)
        {
            Residence = residence;
            Workplace = workplace;
            simulator = GetSimulator();
            CurrentBlock = residence;
            City = city;
        }

        public override void SimulateOneStep()
        {
            // WARNING: this runs in parallel
            simulator.MoveNext();
        }

        public override void ResetSimulation()
        {
            var status = MoveTo(Residence);
            if (status == false)
            {
                throw new Exception("Could not teleport home");
            }
            simulator = GetSimulator();
        }

        private IEnumerator GetSimulator()
        {
            while (true)
            {
                // actions
                {
                    int rnd = new System.Random().Next(1, 100);
                    if (rnd < 50)
                    {
                        // go shopping
                        var shoppingSimulator = GetShoppingSimulator(Residence);
                        var shoppingEnumerator = shoppingSimulator.GetEnumerator();
                        var notEndReached = shoppingEnumerator.MoveNext();
                        while (notEndReached)
                        {
                            notEndReached = shoppingEnumerator.MoveNext();
                            yield return null;
                        }
                    }
                    else if (rnd < 60)
                    {
                        // sleep for 8 hours
                        Debug.Log($"[{FirstName} {LastName}] is going to sleep");
                        for (int i = 0; i < 8 * 60; i++)
                        {
                            yield return null;
                        }
                        Debug.Log($"[{FirstName} {LastName}] just woke up");
                    }
                    else
                    {
                        // go to work
                        Debug.Log($"[{FirstName} {LastName}] Going to work");
                        bool wasJourneyToWorkSuccessful = true;
                        {
                            var goToWorkSimulator = SimulateGoTo(Workplace);

                            foreach (var goToWorkStatus in goToWorkSimulator)
                            {
                                if (goToWorkStatus)
                                {
                                    Debug.Log($"[{FirstName} {LastName}] is now at {CurrentBlock.Coordinates.x} {CurrentBlock.Coordinates.y}");
                                    yield return null;
                                }
                                else
                                {
                                    wasJourneyToWorkSuccessful = false;
                                    Debug.Log($"[{FirstName} {LastName}] journey was disrupted. Going home instead.");
                                    break;
                                }
                            }
                        }
                        // work
                        if (wasJourneyToWorkSuccessful)
                        {
                            Debug.Log($"[{FirstName} {LastName}] Started working");
                            // spend 8 hours working
                            for (int i = 0; i < 8 * 60; i++)
                            {
                                yield return null;
                            }
                            City.financeManager.WagePayout(DailyWage);
                            Debug.Log($"[{FirstName} {LastName}] Finished working");
                        }
                        // journey back
                        {
                            var goHomeSimulator = SimulateGoTo(Residence);
                            foreach (var goHomeStatus in goHomeSimulator)
                            {
                                if (goHomeStatus)
                                {
                                    Debug.Log($"[{FirstName} {LastName}] is now at {CurrentBlock.Coordinates.x} {CurrentBlock.Coordinates.y}");
                                    yield return null;
                                }
                                else
                                {
                                    // teleport home
                                    Debug.Log($"[{FirstName} {LastName}] teleported home");
                                    var teleportStatus = MoveTo(Residence);
                                    if (teleportStatus == false)
                                    {
                                        throw new Exception("Could not teleport home");
                                    }
                                }
                            }
                        }
                        yield return null;
                    }
                }
                // consider moving out
                {
                    int rndMoveOut = new System.Random().Next(1, 100);
                    if (rndMoveOut < City.financeManager.TaxRatePercentage / 10)
                    {
                        // move out
                        City.RemoveCityResidentFromCity(this);
                        Debug.Log($"[{FirstName} {LastName}] Moved out");
                    }
                }
                // think what to do next (necessary to prevent infinite loop)
                yield return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="map">this will be used to find a residence and workplace</param>
        /// <returns>new resident or null if there is no residence or workplace available</returns>
        public static CityResident GenerateRandomCityResident(City city)
        {
            ResidenceBlock residence;
            ShopBlock workplace;
            {
                var availableResidences = city.map.GetAvailableResidences();
                var availableWorkplaces = city.map.GetAvailableWorkplaces();

                if (availableResidences.Count == 0 || availableWorkplaces.Count == 0) return null;

                residence = availableResidences[UnityEngine.Random.Range(0, availableResidences.Count)];
                workplace = availableWorkplaces[UnityEngine.Random.Range(0, availableWorkplaces.Count)];
            }

            return new CityResident(
                Faker.Name.First(),
                Faker.Name.Last(),
                Faker.RandomNumber.Next(0, 100),
                city,
                residence,
                workplace
            );
        }

    }
}

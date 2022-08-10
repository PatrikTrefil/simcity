using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    public sealed class Tourist : Person
    {
        public IEnumerator simulator;
        private MapNamespace.MapBlock StartingBlock { get; }
        public Tourist(string firstName, string lastName, int age, City city, MapNamespace.MapBlock startingBlock) : base(firstName, lastName, age, city)
        {
            simulator = GetSimulator();
            StartingBlock = startingBlock;
            CurrentBlock = startingBlock;
        }
        public override void ResetSimulation()
        {
            var status = MoveTo(StartingBlock);
            if (status == false)
            {
                throw new System.Exception("Could not teleport home");
            }
            simulator = GetSimulator();
        }

        public override void SimulateOneStep()
        {
            simulator.MoveNext();
        }

        private IEnumerator GetSimulator()
        {
            // pick a shop, go shopping, and then go back to starting place and remove tourist
            var shoppingSimulator = GetShoppingSimulator(StartingBlock);
            var shoppingEnumerator = shoppingSimulator.GetEnumerator();
            var notEndReached = shoppingEnumerator.MoveNext();
            while (notEndReached)
            {
                notEndReached = shoppingEnumerator.MoveNext();
                yield return null;
            }
            // WARNING: not thread-safe
            City.RemoveTouristFromCity(this);
            Debug.Log($"[Tourist {FirstName} {LastName}] just left");
        }

        public static Tourist GenerateRandomTourist(City city)
        {
            var availableRoadBlocks = city.map.GetAvailableRoads();
            if (availableRoadBlocks.Count > 0)
            {
                var randomRoadBlock = availableRoadBlocks[UnityEngine.Random.Range(0, availableRoadBlocks.Count)];
                return new Tourist(Faker.Name.First(), Faker.Name.Last(), Faker.RandomNumber.Next(0, 100), city, randomRoadBlock);
            }
            else
            {
                return null;
            }
        }
    }
}

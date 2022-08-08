using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityResident : Person
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
    private IEnumerator simulator;

    public CityResident(string firstName, string lastName, int age, Map map, ResidenceBlock residence, ShopBlock workplace) : base(firstName, lastName, age, map)
    {
        Residence = residence;
        Workplace = workplace;
        simulator = GetSimulator();
        CurrentBlock = residence;
    }

    public override void SimulateOneStep()
    {
        // WARNING: this runs in parallel
        simulator.MoveNext();
    }

    public override void ResetSimulation()
    {
        // TODO: teleport home
        simulator = GetSimulator();
    }

    public IEnumerator GetSimulator()
    {
        // WARNING: this runs in parallel
        while (true)
        {
            int randomX = new System.Random().Next(0, 7);
            int randomY = new System.Random().Next(0, 7);
            Debug.Log($"Randomly move to ({randomX} {randomY})");
            MoveTo(Map.blocks[randomX, randomY]);
            yield return null;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="map">this will be used to find a residence and workplace</param>
    /// <returns>new resident or null if there is no residence or workplace available</returns>
    public static CityResident GenerateRandomCityResident(Map map)
    {
        ResidenceBlock residence;
        ShopBlock workplace;
        {
            var availableResidences = map.GetAvailableResidences();
            var availableWorkplaces = map.GetAvailableWorkplaces();

            if (availableResidences.Count == 0 || availableWorkplaces.Count == 0) return null;

            residence = availableResidences[UnityEngine.Random.Range(0, availableResidences.Count)];
            workplace = availableWorkplaces[UnityEngine.Random.Range(0, availableWorkplaces.Count)];
        }

        return new CityResident(
            Faker.Name.First(),
            Faker.Name.Last(),
            Faker.RandomNumber.Next(0, 100),
            map,
            residence,
            workplace
        );
    }

    void MoveTo(MapBlock destBlock)
    {
        if (destBlock != CurrentBlock)
        {
            lock (Map.blockLocks[destBlock.Coordinates.x, destBlock.Coordinates.y])
            {
                if (destBlock.PeopleHere.Count < destBlock.PeopleHere.Capacity)
                {
                    lock (Map.blockLocks[CurrentBlock.Coordinates.x, CurrentBlock.Coordinates.y])
                    {
                        CurrentBlock.PeopleHere.Remove(this);
                        destBlock.PeopleHere.Add(this);
                        CurrentBlock = destBlock;
                        Debug.Log($"{FirstName} {LastName} moved to ({destBlock.Coordinates.x}, {destBlock.Coordinates.y})");
                    }
                }
            }
        }
    }
}

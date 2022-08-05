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
    public CityResident(string firstName, string lastName, int age) : base(firstName, lastName, age) { }
    public override void SimulateOneStep()
    {
        Debug.Log($"Simulating one step of {FirstName} {LastName} (age: {Age}");
    }
    public override void ResetSimulation()
    {
        throw new System.NotImplementedException();
    }
    public static CityResident GenerateRandomCityResident()
    {
        return new CityResident(Faker.Name.First(), Faker.Name.Last(), Faker.RandomNumber.Next(0, 100));
    }
}

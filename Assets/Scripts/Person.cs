using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Person
{
    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; protected set; }

    /// <summary>
    /// Reference to the block where the person
    /// currently is
    /// </summary>
    public MapBlock CurrentBlock { get; protected set; }

    protected Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    public abstract void SimulateOneStep();
    /// <summary>
    /// teleport home/start position of tourist and restart behavior algorithm
    /// </summary>
    public abstract void ResetSimulation();
}

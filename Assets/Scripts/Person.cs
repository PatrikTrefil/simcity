using Simcity.MapNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    public abstract class Person
    {
        protected Map Map { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public int Age { get; protected set; }

        /// <summary>
        /// Reference to the block where the person
        /// currently is
        /// </summary>
        public MapBlock CurrentBlock { get; protected set; }

        protected Person(string firstName, string lastName, int age, Map map)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Map = map;
        }

        public abstract void SimulateOneStep();

        /// <summary>
        /// teleport home/start position of tourist and restart behavior algorithm
        /// </summary>
        public abstract void ResetSimulation();
    }
}

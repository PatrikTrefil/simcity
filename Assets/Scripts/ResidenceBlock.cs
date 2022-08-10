using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    namespace MapNamespace
    {
        public sealed class ResidenceBlock : MapBlock
        {
            public override int PeopleHereCapacity { get => ResidentsCapacity; }
            public int ResidentsCapacity { get; } = 100;
            public List<CityResident> Residents;
            public readonly object residentsLock;
            public ResidenceBlock() : base()
            {
                Residents = new List<CityResident>();
                residentsLock = new object();
            }
        }
    }
}

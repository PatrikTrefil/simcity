using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    namespace MapNamespace
    {
        sealed public class RoadBlock : MapBlock
        {
            public int Level { get; } = 100;
            public override int PeopleHereCapacity
            {
                get => Level * 100;
            }
            public RoadBlock() : base() { }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    namespace MapNamespace
    {
        sealed public class DefaultBlock : MapBlock
        {
            public override int PeopleHereCapacity { get; } = 0;
        }
    }
}

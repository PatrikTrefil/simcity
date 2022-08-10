using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    namespace MapNamespace
    {
        sealed public class RoadBlock : MapBlock
        {
            private PublicTransport publicTransport;
            public int Level { get; } = 1;
            public override int PeopleHereCapacity
            {
                get => Level * 100 * publicTransport.Level;
            }
            public RoadBlock() : base() { }
            public void Awake()
            {
                publicTransport = transform.parent.gameObject.GetComponent<MapNamespace.Map>().city.publicTransport;
            }
        }
    }
}

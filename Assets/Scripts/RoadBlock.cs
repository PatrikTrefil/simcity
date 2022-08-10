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
            private City city;
            public int Level { get; private set; } = 1;
            private readonly int maxLevel = 5;
            public override int PeopleHereCapacity
            {
                get => Level * 100 * publicTransport.Level;
            }
            public RoadBlock() : base() { }
            public void Awake()
            {
                city = transform.parent.gameObject.GetComponent<Map>().city;
                Debug.Log($"City: {city}");
                publicTransport = city.publicTransport;
            }

            public void UpgradeRoad()
            {
                if (Level < maxLevel)
                {
                    Level++;
                    city.financeManager.RoadUpgrade();
                }
            }
        }
    }
}

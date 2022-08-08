using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    namespace MapNamespace
    {
        sealed public class ShopBlock : MapBlock
        {
            public override int PeopleHereCapacity { get => WorkersCapacity + ShoppersCapacity; }
            public int WorkersCapacity { get; } = 100;
            public int ShoppersCapacity { get; } = 100;

            public List<Person> Shoppers;
            public List<CityResident> Workers;

            public ShopBlock() : base()
            {
                Workers = new List<CityResident>();
                Shoppers = new List<Person>();
            }
        }
    }
}

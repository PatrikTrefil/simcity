using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class ShopBlock : MapBlock
{
    public int Capacity { get; } = 100;
    public List<Person> Tourists;
    public List<CityResident> Workers;
    public ShopBlock()
    {
        Workers = new List<CityResident>();
        Tourists = new List<Person>();
    }
}

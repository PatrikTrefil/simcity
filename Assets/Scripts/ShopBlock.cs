using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class ShopBlock : MapBlock
{
    public int WorkersCapacity { get; } = 100;
    public int ShoppersCapacity { get; } = 100;

    public List<Person> Shoppers;
    public List<CityResident> Workers;

    public ShopBlock()
    {
        Workers = new List<CityResident>();
        Shoppers = new List<Person>();
    }
}

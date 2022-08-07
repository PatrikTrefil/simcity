using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidenceBlock : MapBlock
{
    public int Capacity { get; } = 100;
    public List<CityResident> Residents;
    public ResidenceBlock()
    {
        Residents = new List<CityResident>();
    }
}

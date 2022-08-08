using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ResidenceBlock : MapBlock
{
    public override int PeopleHereCapacity { get => ResidentsCapacity; }
    public int ResidentsCapacity { get; } = 1;
    public List<CityResident> Residents;
    public ResidenceBlock() : base()
    {
        Residents = new List<CityResident>();
    }
}

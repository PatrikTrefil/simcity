using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class RoadBlock : MapBlock
{
    public int Level { get; } = 0;
    public int Capacity
    {
        get => Level * 100;
    }
}

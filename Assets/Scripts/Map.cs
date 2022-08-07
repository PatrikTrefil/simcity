using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapBlock DefaultBlockPrefab;
    public MapBlock RoadBlockPrefab;
    public MapBlock ShopBlockPrefab;
    public MapBlock ResidenceBlockPrefab;

    public int GridSize { get; } = 8; // currently only square grids are supported
    public MapBlock[,] blocks;
    public Map()
    {
        blocks = new MapBlock[GridSize, GridSize];
    }
    void Awake()
    {
        for (int y = 0; y < GridSize; y++)
            for (int x = 0; x < GridSize; x++)
                blocks[x, y] = MapBlock.MakeMapBlock(DefaultBlockPrefab, transform, new Vector2Int(x, y));
    }

    public List<ResidenceBlock> GetAvailableResidences()
    {
        var availableResidences = new List<ResidenceBlock>();
        for (int y = 0; y < blocks.GetLength(1); y++)
        {
            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                var residenceBlock = blocks[x, y].GetComponent<ResidenceBlock>();
                if (residenceBlock != null && residenceBlock.Capacity > residenceBlock.Residents.Count)
                    availableResidences.Add(residenceBlock);

            }
        }

        return availableResidences;
    }
    public List<ShopBlock> GetAvailableWorkplaces()
    {
        var availableWorkplaces = new List<ShopBlock>();
        for (int y = 0; y < blocks.GetLength(1); y++)
        {
            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                var shopBlock = blocks[x, y].GetComponent<ShopBlock>();
                if (shopBlock != null && shopBlock.Capacity > shopBlock.Workers.Count)
                    availableWorkplaces.Add(shopBlock);
            }
        }

        return availableWorkplaces;
    }
}

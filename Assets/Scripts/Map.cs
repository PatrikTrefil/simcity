using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    namespace MapNamespace
    {
        public sealed class Map : MonoBehaviour
        {
            public MapBlock DefaultBlockPrefab;
            public MapBlock RoadBlockPrefab;
            public MapBlock ShopBlockPrefab;
            public MapBlock ResidenceBlockPrefab;

            public int GridSize { get; } = 8; // currently only square grids are supported
            public readonly MapBlock[,] blocks;

            public readonly object[,] blockLocks;
            public Map()
            {
                blocks = new MapBlock[GridSize, GridSize];
                blockLocks = new object[GridSize, GridSize];
                for (int i = 0; i < blockLocks.GetLength(0); i++)
                {
                    for (int j = 0; j < blockLocks.GetLength(1); j++)
                    {
                        blockLocks[i, j] = new object();
                    }
                }
            }
            private void Awake()
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
                        if (residenceBlock != null && residenceBlock.ResidentsCapacity > residenceBlock.Residents.Count)
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
                        if (shopBlock != null && shopBlock.WorkersCapacity > shopBlock.Workers.Count)
                            availableWorkplaces.Add(shopBlock);
                    }
                }

                return availableWorkplaces;
            }

            public List<ShopBlock> GetAvailableShops()
            {

                var availableShops = new List<ShopBlock>();
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    for (int x = 0; x < blocks.GetLength(0); x++)
                    {
                        var shopBlock = blocks[x, y] as ShopBlock;
                        if (shopBlock != null && shopBlock.ShoppersCapacity > shopBlock.Shoppers.Count)
                            availableShops.Add(shopBlock);
                    }
                }

                return availableShops;
            }
        }
    }
}

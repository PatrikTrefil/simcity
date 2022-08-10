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
            public City city;

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

            public List<RoadBlock> GetAvailableRoads()
            {
                var avalableRoads = new List<RoadBlock>();
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    for (int x = 0; x < blocks.GetLength(0); x++)
                    {
                        var roadBlock = blocks[x, y].GetComponent<RoadBlock>();
                        if (roadBlock != null && roadBlock.PeopleHereCapacity > roadBlock.PeopleHere.Count)
                            avalableRoads.Add(roadBlock);

                    }
                }

                return avalableRoads;
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

            public void LoadFromMapData(SaveSystem.GameData.CityData.MapData mapData)
            {
                foreach (var shopBlockData in mapData.shopBlockData)
                {
                    var x = shopBlockData.coordinates[0];
                    var y = shopBlockData.coordinates[1];
                    // detach from parent (to make sibling indices work)
                    blocks[x, y].transform.parent = null;
                    Destroy(blocks[x, y].gameObject);
                    blocks[x, y] = MapBlock.MakeMapBlock(ShopBlockPrefab, transform, new Vector2Int(x, y));
                    blocks[x, y].transform.SetSiblingIndex((GridSize * y) + x);
                }
                foreach (var roadBlockData in mapData.roadBlockData)
                {
                    var x = roadBlockData.coordinates[0];
                    var y = roadBlockData.coordinates[1];
                    // detach from parent (to make sibling indices work)
                    blocks[x, y].transform.parent = null;
                    Destroy(blocks[x, y].gameObject);
                    blocks[x, y] = MapBlock.MakeMapBlock(RoadBlockPrefab, transform, new Vector2Int(x, y));
                    blocks[x, y].transform.SetSiblingIndex((GridSize * y) + x);
                    (blocks[x, y] as RoadBlock).Level = roadBlockData.level;
                }
                foreach (var residenceBlockData in mapData.residenceBlockData)
                {
                    var x = residenceBlockData.coordinates[0];
                    var y = residenceBlockData.coordinates[1];
                    // detach from parent (to make sibling indices work)
                    blocks[x, y].transform.parent = null;
                    Destroy(blocks[x, y].gameObject);
                    blocks[x, y] = MapBlock.MakeMapBlock(ResidenceBlockPrefab, transform, new Vector2Int(x, y));
                    blocks[x, y].transform.SetSiblingIndex((GridSize * y) + x);
                    Debug.Log($"Placed residence at {x} {y} (sibling index: ${(GridSize * y) + x}");
                }
            }
        }
    }
}

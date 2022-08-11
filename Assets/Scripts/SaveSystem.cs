using Simcity.MapNamespace;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Simcity
{
    public static class SaveSystem
    {
        [System.Serializable]
        public class GameData
        {
            [System.Serializable]
            public class TimeManagerData
            {
                public float realworldSecondsFromBeginning;
                public TimeManagerData(TimeManager timeManager)
                {
                    realworldSecondsFromBeginning = timeManager.RealwordSecondsFromBeginning;
                }
            }
            [System.Serializable]
            public class CityData
            {
                [System.Serializable]
                public class PublicTransportData
                {
                    public int level;
                    public PublicTransportData(PublicTransport publicTransport)
                    {
                        level = publicTransport.Level;
                    }
                }
                [System.Serializable]
                public class ResidentData
                {
                    public string firstName;
                    public string lastName;
                    public int age;
                    public int dailyWage;
                    public int[] residenceCoordinates;
                    public int[] workplaceCoordinates;
                    public ResidentData(CityResident resident)
                    {
                        firstName = resident.FirstName;
                        lastName = resident.LastName;
                        age = resident.Age;
                        dailyWage = resident.DailyWage;
                        residenceCoordinates = new int[] { resident.Residence.Coordinates.x, resident.Residence.Coordinates.y };
                        workplaceCoordinates = new int[] { resident.Workplace.Coordinates.x, resident.Workplace.Coordinates.y };
                    }
                }
                [System.Serializable]
                public class FinanceManagerData
                {
                    public float taxRatePercentage;
                    public float balance;
                    public FinanceManagerData(FinanceManager financeManager)
                    {
                        taxRatePercentage = financeManager.TaxRatePercentage;
                        balance = financeManager.Balance;
                    }
                }
                [System.Serializable]
                public class MapData
                {
                    [System.Serializable]
                    public abstract class BlockData
                    {
                        public int[] coordinates;
                        protected BlockData(MapBlock block)
                        {
                            coordinates = new int[] { block.Coordinates.x, block.Coordinates.y };
                        }
                    }
                    [System.Serializable]
                    public class DefaultBlockData : BlockData
                    {
                        public DefaultBlockData(DefaultBlock block) : base(block) { }
                    }
                    [System.Serializable]
                    public class RoadBlockData : BlockData
                    {
                        public readonly int level;
                        public RoadBlockData(RoadBlock block) : base(block)
                        {
                            level = block.Level;
                        }
                    }
                    [System.Serializable]
                    public class ShopBlockData : BlockData
                    {
                        public ShopBlockData(ShopBlock block) : base(block) { }
                    }
                    [System.Serializable]
                    public class ResidenceBlockData : BlockData
                    {
                        public ResidenceBlockData(ResidenceBlock block) : base(block) { }
                    }
                    // now there is nothing to save about the default block
                    //public DefaultBlockData[] defaultBlockData;
                    public ResidenceBlockData[] residenceBlockData;
                    public RoadBlockData[] roadBlockData;
                    public ShopBlockData[] shopBlockData;
                    public MapData(Map map)
                    {
                        var residenceBlockDataList = new List<ResidenceBlockData>();
                        var roadBlockDataList = new List<RoadBlockData>();
                        var shopBlockDataList = new List<ShopBlockData>();
                        for (int x = 0; x < map.blocks.GetLength(0); x++)
                        {
                            for (int y = 0; y < map.blocks.GetLength(1); y++)
                            {
                                var currBlock = map.blocks[x, y];
                                if (currBlock is RoadBlock)
                                {
                                    roadBlockDataList.Add(new RoadBlockData(currBlock as RoadBlock));
                                }
                                else if (currBlock is ShopBlock)
                                {
                                    shopBlockDataList.Add(new ShopBlockData(currBlock as ShopBlock));
                                }
                                else if (currBlock is ResidenceBlock)
                                {
                                    residenceBlockDataList.Add(new ResidenceBlockData(currBlock as ResidenceBlock));
                                }
                                else if (currBlock is DefaultBlock) { }
                                else { throw new System.Exception("Unknown block type. Can't serialize"); }
                            }
                        }
                        roadBlockData = roadBlockDataList.ToArray();
                        shopBlockData = shopBlockDataList.ToArray();
                        residenceBlockData = residenceBlockDataList.ToArray();
                    }
                }
                public PublicTransportData publicTransportData;
                public ResidentData[] residentData;
                public FinanceManagerData financeManagerData;
                public MapData mapData;
                public CityData(City city)
                {
                    publicTransportData = new PublicTransportData(city.publicTransport);
                    residentData = new ResidentData[city.population.People.Count];
                    for (int i = 0; i < city.population.People.Count; i++)
                    {
                        residentData[i] = new ResidentData(city.population.People[i]);
                    }
                    financeManagerData = new FinanceManagerData(city.financeManager);
                    mapData = new MapData(city.map);
                }
            }
            public TimeManagerData timeManagerData;
            public CityData cityData;
            public GameData(Game game)
            {
                timeManagerData = new TimeManagerData(game.timeManager);
                cityData = new CityData(game.city);
            }
        }
        static string pathToGameData = Application.persistentDataPath + "/game.json";
        public static void SaveGame(GameData cityData)
        {
            using (var streamWriter = new StreamWriter(pathToGameData))
            {
                streamWriter.Write(JsonUtility.ToJson(cityData));
            }
            Debug.Log($"Game saved to {pathToGameData}");
        }

        public static GameData LoadGame()
        {
            GameData gameData;
            try
            {
                using (var streamReader = new StreamReader(pathToGameData))
                {
                    gameData = JsonUtility.FromJson<GameData>(streamReader.ReadToEnd());
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            return gameData;
        }

        public static void RemoveSavedGame()
        {
            File.Delete(pathToGameData);
        }
    }
}

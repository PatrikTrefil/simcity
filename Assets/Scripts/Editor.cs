using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simcity
{
    namespace MapNamespace
    {
        public sealed class Editor : MonoBehaviour
        {

            public TMPro.TMP_Dropdown EditorModeDropdown;
            public Map map;
            public City city;


            public void ReplaceMapBlock(Vector2Int coordinates)
            {
                var mode = EditorModeDropdown.captionText.text;

                Debug.Log($"Replace block on ${coordinates} (mode: ${mode})");

                MapBlock chosenPrefab;
                switch (mode)
                {
                    case "Build residence":
                        chosenPrefab = map.ResidenceBlockPrefab;
                        break;
                    case "Build road":
                        chosenPrefab = map.RoadBlockPrefab;
                        city.financeManager.RoadBlockCount++;
                        break;
                    case "Build shop":
                        chosenPrefab = map.ShopBlockPrefab;
                        break;
                    case "Bulldoze":
                        chosenPrefab = map.DefaultBlockPrefab;
                        break;
                    case "Upgrade road":
                        var roadBlock = map.blocks[coordinates.x, coordinates.y] as RoadBlock;
                        if (roadBlock != null)
                        {
                            roadBlock.UpgradeRoad();
                        }
                        return;
                    default:
                        throw new System.ArgumentException("Unknown editor mode");
                }

                var x = coordinates.x;
                var y = coordinates.y;

                if (map.blocks[x, y] is RoadBlock)
                {
                    city.financeManager.RoadBlockCount--;
                }

                // remove all people involved with this block
                {
                    // remove all people on this block
                    foreach (Person person in map.blocks[x, y].PeopleHere)
                    {
                        CityResident cityResident = person as CityResident;
                        if (cityResident != null)
                        {
                            city.RemoveCityResidentFromCity(cityResident);
                        }
                        else
                        {
                            // TODO: remove tourists too
                            throw new System.NotImplementedException();
                        }
                    }
                    // if it's a shop block, then remove all shoppers and workers
                    {
                        var shopBlock = map.blocks[x, y] as ShopBlock;
                        if (shopBlock != null)
                        {
                            foreach (CityResident shopper in shopBlock.Shoppers)
                            {
                                city.RemoveCityResidentFromCity(shopper);
                            }
                            foreach (CityResident worker in shopBlock.Workers)
                            {
                                city.RemoveCityResidentFromCity(worker);
                            }
                        }
                    }
                    // if it's a residence block, then remove all residents
                    {
                        var residenceBlock = map.blocks[x, y] as ResidenceBlock;
                        if (residenceBlock != null)
                        {
                            foreach (CityResident resident in residenceBlock.Residents)
                            {
                                city.RemoveCityResidentFromCity(resident);
                            }
                        }
                    }
                }

                Destroy(map.blocks[x, y].gameObject);


                map.blocks[x, y] = MapBlock.MakeMapBlock(chosenPrefab, map.transform, new Vector2Int(x, y));
                // set the right index to display in the right spot (displaying is done by Grid Layout group)
                map.blocks[x, y].transform.SetSiblingIndex((map.GridSize * coordinates.y) + coordinates.x);

                // pay for build
                city.financeManager.BlockBuildPayment();
            }
        }
    }
}

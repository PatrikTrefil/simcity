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
                    default:
                        throw new System.ArgumentException("Unknown editor mode");
                }
                // TODO: remove people from removed block

                var x = coordinates.x;
                var y = coordinates.y;

                if (map.blocks[x, y] is RoadBlock)
                {
                    city.financeManager.RoadBlockCount--;
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

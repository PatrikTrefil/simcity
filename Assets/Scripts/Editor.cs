using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Editor : MonoBehaviour
{

    public TMPro.TMP_Dropdown EditorModeDropdown;
    public Map map;


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

        Destroy(map.blocks[coordinates.x, coordinates.y].gameObject);

        var x = coordinates.x;
        var y = coordinates.y;

        map.blocks[x, y] = MapBlock.MakeMapBlock(chosenPrefab, map.transform, new Vector2Int(x, y));
        // set the right index to display in the right spot (displaying is done by Grid Layout group)
        map.blocks[x, y].transform.SetSiblingIndex((map.GridSize * coordinates.y) + coordinates.x);
    }
}

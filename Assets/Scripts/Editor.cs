using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    public GameObject DefaultBlockPrefab;
    public GameObject RoadBlockPrefab;
    public GameObject ShopBlockPrefab;
    public GameObject EditorModeDropdown;
    public GameObject ResidenceBlockPrefab;
    public int GridSize { get; } = 8; // currently only square grids are supported
    public GameObject[,] Map;
    public Editor()
    {
        Map = new GameObject[GridSize, GridSize];
    }
    void Awake()
    {
        for (int y = 0; y < GridSize; y++)
            for (int x = 0; x < GridSize; x++)
                Map[x, y] = MapBlock.MakeMapBlockGameObject(DefaultBlockPrefab, transform, new Vector2Int(x, y));
    }

    public void ReplaceMapBlock(Vector2Int coordinates)
    {
        var mode = EditorModeDropdown.GetComponent<TMPro.TMP_Dropdown>().captionText.text;

        Debug.Log($"Replace block on ${coordinates} (mode: ${mode})");

        GameObject chosenPrefab;
        switch (mode)
        {
            case "Build residence":
                chosenPrefab = ResidenceBlockPrefab;
                break;
            case "Build road":
                chosenPrefab = RoadBlockPrefab;
                break;
            case "Build shop":
                chosenPrefab = ShopBlockPrefab;
                break;
            case "Bulldoze":
                chosenPrefab = DefaultBlockPrefab;
                break;
            default:
                throw new System.ArgumentException("Unknown editor mode");
        }

        Destroy(Map[coordinates.x, coordinates.y]);

        var x = coordinates.x;
        var y = coordinates.y;

        Map[x, y] = MapBlock.MakeMapBlockGameObject(chosenPrefab, transform, new Vector2Int(x, y));
        // set the right index to display in the right spot (displaying is done by Grid Layout group)
        Map[x, y].transform.SetSiblingIndex((GridSize * coordinates.y) + coordinates.x);
    }
}

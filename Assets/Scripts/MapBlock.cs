using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MapBlock : MonoBehaviour
{
    public Vector2Int Coordinates { get; private set; }
    public void OnClick()
    {
        transform.parent.GetComponent<Editor>().ReplaceMapBlock(Coordinates);
    }

    public static MapBlock MakeMapBlock(MapBlock blockPrefab, Transform transform, Vector2Int coordinates)
    {
        var newBlock = Instantiate(blockPrefab.gameObject, transform).GetComponent<MapBlock>();
        newBlock.Coordinates = coordinates;
        return newBlock;
    }
}

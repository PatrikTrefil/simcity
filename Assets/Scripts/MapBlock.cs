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

    public static GameObject MakeMapBlockGameObject(GameObject blockPrefab, Transform transform, Vector2Int coordinates)
    {
        var newBlock = Instantiate(blockPrefab, transform);
        newBlock.GetComponent<MapBlock>().Coordinates = coordinates;
        return newBlock;
    }
}

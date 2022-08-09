using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    namespace MapNamespace
    {
        abstract public class MapBlock : MonoBehaviour
        {
            // TODO: price for maintainance
            public Vector2Int Coordinates { get; private set; }
            public List<Person> PeopleHere { get; }
            public abstract int PeopleHereCapacity { get; }

            public MapBlock()
            {
                PeopleHere = new List<Person>();
            }

            public void OnClick()
            {
                transform.parent.parent.GetComponent<Editor>().ReplaceMapBlock(Coordinates);
            }

            public static MapBlock MakeMapBlock(MapBlock blockPrefab, Transform transform, Vector2Int coordinates)
            {
                var newBlock = Instantiate(blockPrefab.gameObject, transform).GetComponent<MapBlock>();
                newBlock.Coordinates = coordinates;
                return newBlock;
            }
        }
    }
}

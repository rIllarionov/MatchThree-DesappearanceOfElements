using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "ItemsHolder", menuName = "ItemsSettings/ItemsHolder",order = 2)]
public class ScriptableItemsHolder : ScriptableObject
{
    [SerializeField] private ScriptableItemSettings[] _items;

    public ScriptableItemSettings[] Items => _items;
}

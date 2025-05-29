using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Pool
{
    [FormerlySerializedAs("type")] public Constants.ObjectItemType objectItemType;
    public GameObject prefab;
}
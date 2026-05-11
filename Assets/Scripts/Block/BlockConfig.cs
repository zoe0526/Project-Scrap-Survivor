using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public enum EBlockType
{
    Red,
    Green,
    Blue,
    Yellow
}
[Serializable]
public struct BlockData
{
    public EBlockType type;
    public Color color;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "BlockConfig", menuName = "Scriptable Objects/BlockConfig")]
public class BlockConfig : ScriptableObject
{
    [SerializeField] private List<BlockData> _blockDataList;
    private Dictionary<EBlockType, BlockData> _cache;

    public BlockData GetData(EBlockType type)
    {
        if(_cache==null)
            _cache = _blockDataList.ToDictionary(x => x.type);
        return _cache[type];
    }
}

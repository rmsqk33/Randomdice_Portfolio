using FEnum;
using UnityEngine;

public class FStartPoint : FPathBase
{
    [SerializeField]
    IFFType iffType;

    public IFFType IffType { get { return iffType; } }
}

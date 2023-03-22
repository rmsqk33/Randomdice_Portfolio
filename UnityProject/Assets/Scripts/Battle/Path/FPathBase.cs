using UnityEngine;

public class FPathBase : MonoBehaviour
{
    [SerializeField]
    FPathBase nextPath;

    public Vector2 WorldPosition { get { return transform.position; } }
    public FPathBase NextPath { get { return nextPath; } }

    public virtual void OnPass(FObjectBase InObject)
    {

    }
}

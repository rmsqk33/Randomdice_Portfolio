using UnityEngine;

public class FPath : MonoBehaviour
{
    [SerializeField]
    FPath nextPath;

    public Vector2 WorldPosition { get { return transform.position; } }
    public FPath NextPath { get { return nextPath; } }

    public virtual void OnPass(FObjectBase InObject)
    {

    }
}

using FEnum;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class FCombatText : MonoBehaviour
{
    int instanceID;
    CombatTextType type;
    TextMeshPro text;
    FObjectBase target;

    public int InstanceID { get { return instanceID; } set { instanceID = value; } }
    public int Value { set { text.text = value.ToString(); } }
    public FObjectBase Target { set { target = value; } }
    public Vector2 WorldPosition { set { transform.position = value; } }
    public CombatTextType Type 
    { 
        get { return type; } 
        set 
        { 
            type = value;
         
            SortingGroup sortingGroup = GetComponent<SortingGroup>();
            if(sortingGroup != null)
            {
                sortingGroup.sortingOrder = (int)type;
            }
        }
    }

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
    }

    public void Tick()
    {
        if (target == null)
            return;

        WorldPosition = target.WorldPosition;
    }

    public void OnEndAnim()
    {
        FCombatTextManager.Instance.RemoveText(instanceID);
    }
}

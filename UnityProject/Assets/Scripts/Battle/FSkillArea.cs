using UnityEngine;

public class FSkillArea : MonoBehaviour
{
    [SerializeField]
    public int index;

    public FSkillAreaController SkillAreaController { get; set; }

    private void Start()
    {
        FObjectBase owner = FObjectManager.Instance.FindObject(index);
        if (owner == null)
            return;

        SkillAreaController = owner.FindController<FSkillAreaController>();
    }

    private void OnTriggerEnter2D(Collider2D o)
    {
        if (SkillAreaController == null)
            return;

        FEnemy target = o.gameObject.GetComponent<FEnemy>();
        if (target == null)
            return;

        SkillAreaController.OnEnterArea(target);
    }
}

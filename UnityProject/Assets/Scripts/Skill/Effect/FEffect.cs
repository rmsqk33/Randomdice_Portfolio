using FEnum;
using Packet;
using System.Collections;
using UnityEngine;

public class FEffect : MonoBehaviour
{
    protected FObjectBase owner;
    protected FObjectBase target;
    protected int effectID;
    protected float radius;
    protected float effectValue;

    public int InstanceID { get; set; }
    public Vector2 WorldPosition { get { return transform.position; } set { transform.position = value; } }

    public virtual void Initialize(FEffectData InEffectData, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        owner = InOwner;
        target = InTarget;
        effectID = InEffectData.id;
        radius = InEffectData.radius;
        effectValue = FGlobal.CalcEffectValue(InOwner, InEffectData.value, InEffectData.valuePerLevel, InEffectData.valuePerBattleLevel);

        GameObject hitEffect = Instantiate(Resources.Load<GameObject>(InEffectData.prefab), this.transform);

        if (radius != 0)
        {
            hitEffect.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        }

        Animator anim = hitEffect.GetComponent<Animator>();
        if (anim != null)
        {
            StartCoroutine(RemoveEffect(anim.GetCurrentAnimatorStateInfo(0).length));
        }
    }

    public virtual void Tick(float InDeltaTime)
    {

    }

    protected IEnumerator RemoveEffect(float InTime)
    {
        yield return new WaitForSeconds(InTime);

        FEffectManager.Instance.RemoveEffect(InstanceID);
    }

}

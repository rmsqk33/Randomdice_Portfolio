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

    public int InstanceID { get; set; }
    public Vector2 WorldPosition { get { return transform.position; } set { transform.position = value; } }

    public virtual void Initialize(FEffectData InEffectData, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        owner = InOwner;
        target = InTarget;
        effectID = InEffectData.id;
        radius = InEffectData.radius;

        transform.localScale = new Vector3(radius * 2, radius * 2, 1);

        Animator anim = GetComponent<Animator>();
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

    protected void DamageToTarget(FObjectBase InTarget, int InDamage, bool InCritical)
    {
        P2P_DAMAGE pkt = new P2P_DAMAGE();
        pkt.objectId = InTarget.ObjectID;
        pkt.damage = InDamage;
        pkt.critical = InCritical;

        FServerManager.Instance.SendMessage(pkt);

        FCombatTextManager.Instance.AddText(InCritical ? CombatTextType.Critical : CombatTextType.Normal, InDamage, InTarget);

        FStatController statController = InTarget.FindController<FStatController>();
        if (statController != null)
        {
            statController.OnDamage(InDamage);
        }
    }
}

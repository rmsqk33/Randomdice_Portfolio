using FEnum;
using Packet;
using System.Collections;
using UnityEngine;

public class FEffect : MonoBehaviour
{
    protected FObjectBase owner;
    protected FObjectBase target;
    protected int effectID;

    public int InstanceID { get; set; }
    public Vector2 WorldPosition { get { return transform.position; } set { transform.position = value; } }

    public virtual void Initialize(int InEffectID, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        owner = InOwner;
        target = InTarget;
        effectID = InEffectID;
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

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

        GameObject hitEffectPrefab = Resources.Load<GameObject>(InEffectData.prefab);
        GameObject hitEffect = Instantiate(hitEffectPrefab, this.transform);

        if (radius != 0)
        {
            hitEffectPrefab.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
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

    protected void DamageToTarget(FObjectBase InTarget, int InDamage)
    {
        FStatController statController = owner.FindController<FStatController>();
        if (statController == null)
            return;

        FStatController targetStatController = InTarget.FindController<FStatController>();
        if (targetStatController == null)
            return;

        bool critical = statController.IsCritical();
        int damage = (int)(critical ? InDamage * statController.GetStat(StatType.CriticalDamage) : InDamage);

        FCombatTextManager.Instance.AddText(critical ? CombatTextType.Critical : CombatTextType.Normal, damage, InTarget);
        targetStatController.OnDamage(InDamage);

        P2P_DAMAGE pkt = new P2P_DAMAGE();
        pkt.objectId = InTarget.ObjectID;
        pkt.damage = damage;
        pkt.critical = critical;

        FServerManager.Instance.SendMessage(pkt);
    }
}

using FEnum;
using Packet;
using UnityEngine;

public class FDamageAbnormality : FAbnormality
{
    int damage;
    float criticalDamageRate;
    float criticalChance;

    bool localPlayerOwner;

    protected override void Initialize(FAbnormalityData InAbnormalityData)
    {
        localPlayerOwner = owner.IsOwnLocalPlayer();
        if (localPlayerOwner)
        {
            FStatController statController = owner.FindController<FStatController>();
            if (statController == null)
                return;

            FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
            if (battleController == null)
                return;

            FEquipBattleDice battleDice = battleController.FindEquipBattleDice(owner.ContentID);
            if (battleDice == null)
                return;

            damage = FGlobal.CalcDamage(owner, InAbnormalityData.damage, InAbnormalityData.damagePerLevel, InAbnormalityData.damagePerBattleLevel);
            criticalDamageRate = statController.GetStat(StatType.CriticalDamage);
            criticalChance = statController.GetStat(StatType.CriticalChance);
        }
    }

    protected override void OnEffect(FAbnormalityOverlapData InAbnormalityData)
    {
        bool critical = Random.value <= criticalChance;
        int damage = (int)(critical ? this.damage * criticalDamageRate : this.damage);

        FCombatTextManager.Instance.AddText(critical ? CombatTextType.Critical : CombatTextType.Normal, damage, target);
        target.FindController<FStatController>().OnDamage(damage);

        P2P_DAMAGE pkt = new P2P_DAMAGE();
        pkt.objectId = target.ObjectID;
        pkt.damage = damage;
        pkt.critical = critical;

        FServerManager.Instance.SendMessage(pkt);
    }

    protected override void OffEffect()
    {

    }
}

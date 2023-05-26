using FEnum;
using System.Collections.Generic;
using UnityEngine;

public class FBasicAttackSkill : FSkillBase
{
    int eyeCount;
    int attackEyeIndex;

    List<Transform> eyeList = new List<Transform>();

    public FBasicAttackSkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    protected override void Initialize(FSkillData InSkillData)
    {
        FStatController statController = owner.FindController<FStatController>();
        if (statController == null)
            return;

        Transform eyeParent = statController.FindChildComponent<Transform>("Eye");
        if (eyeParent == null)
            return;

        foreach (Transform eye in eyeParent)
        {
            eyeList.Add(eye);
        }

        eyeCount = statController.GetIntStat(StatType.DiceEye);
        OriginInterval = InSkillData.interval / eyeCount;
    }

    protected override void UseSkillLocal()
    {
        target = GetTarget();
        if (target == null)
            return;

        base.UseSkillLocal();

        AttackToTarget(target);
        SendUseSkill(target);
    }

    public override void UseSkillRemote()
    {
        if (target == null)
            return;

        base.UseSkillRemote();

        AttackToTarget(target);
    }

    private void AttackToTarget(FObjectBase InTarget)
    {
        PlayAttackAnim(attackEyeIndex);
        FEffectManager.Instance.AddProjectile(projectileID, owner, GetEyePosition(attackEyeIndex), InTarget);
        attackEyeIndex = (attackEyeIndex + 1) % eyeCount;
    }
    
    private Vector2 GetEyePosition(int InEyeIndex)
    {
        if (InEyeIndex < 0 || eyeList.Count <= InEyeIndex)
            return Vector2.zero;

        return eyeList[InEyeIndex].position;
    }

    private void PlayAttackAnim(int InEyeIndex)
    {
        if (InEyeIndex < 0 || eyeList.Count <= InEyeIndex)
            return;

        Animator anim = eyeList[InEyeIndex].GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }
    }
}
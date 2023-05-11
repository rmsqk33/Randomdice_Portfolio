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

    public override void UseSkill()
    {
        base.UseSkill();

        if (target == null)
            return;

        PlayAttackAnim(attackEyeIndex);
        FEffectManager.Instance.AddProjectile(projectileID, owner, GetEyePosition(attackEyeIndex), target);
        attackEyeIndex = (attackEyeIndex + 1) % eyeCount;
    }

    public override void UseSkillInPath(float InPathRate)
    {
        FEffectManager.Instance.AddProjectile(projectileID, owner, owner.WorldPosition, InPathRate);
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
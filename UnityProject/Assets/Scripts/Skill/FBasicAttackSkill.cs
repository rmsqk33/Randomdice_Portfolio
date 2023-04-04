using FEnum;
using System.Collections.Generic;
using UnityEngine;

public class FBasicAttackSkill : FSkillBase
{
    int eyeCount;
    int attackEyeIndex;

    int projectileID;
    SkillTargetType targetType;
    float attackInterval;
    float elapsedTime;

    List<Transform> eyeList = new List<Transform>();

    public FBasicAttackSkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    protected override void Initialize(FSkillData InSkillData)
    {
        FDiceStatController diceStatController = owner.FindController<FDiceStatController>();
        if (diceStatController == null)
            return;

        Transform eyeParent = diceStatController.FindChildComponent<Transform>("Eye");
        if (eyeParent == null)
            return;

        foreach (Transform eye in eyeParent)
        {
            eyeList.Add(eye);
        }

        eyeCount = diceStatController.EyeCount;

        projectileID = InSkillData.projectileID;
        targetType = InSkillData.targetType;
        attackInterval = InSkillData.interval / eyeCount;
    }

    public override void Tick(float InDelta)
    {
        elapsedTime += InDelta;

        if (attackInterval <= elapsedTime)
        {
            UseSkill();

            elapsedTime = 0;
        }
    }

    void UseSkill()
    {
        if(owner.IsOwnLocalPlayer())
        {
            FObjectBase newTarget = GetTarget();
            if(target != newTarget)
            {
                target = newTarget;

            }
        }

        if (target == null)
            return;

        PlayAttackAnim(attackEyeIndex);
        FEffectManager.Instance.AddProjectile(projectileID, owner, GetEyePosition(attackEyeIndex), target);
        attackEyeIndex = (attackEyeIndex + 1) % eyeCount;
    }

    FObjectBase GetTarget()
    {
        switch (targetType)
        {
            case SkillTargetType.Front: return FObjectManager.Instance.FrontEnemy;
        }

        return null;
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
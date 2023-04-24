using FEnum;
using UnityEngine;

public class FSummonSkill : FSkillBase
{
    int collisionObjectID;

    public FSummonSkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }


    protected override void Initialize(FSkillData InSkillData)
    {
    }

    public override void UseSkill()
    {
        SendOnSkill();

    }
}




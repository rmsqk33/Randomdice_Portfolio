using FEnum;
using TMPro;
using UnityEngine;

public class FEnemy : FObjectBase, FStatObserver
{
    [SerializeField]
    TextMeshPro hpText;

    public void Initialize(FEnemyData InData, int InAccumulateCount, FPath InStartPoint)
    {
        ContentID = InData.id;

        AddController<FStatController>();
        
        FStatController statController = FindController<FStatController>();
        statController.AddObserver(this);
        statController.SetStat(StatType.HP, InData.hp + InData.hpIncreaseBySpawnCount * InAccumulateCount);
        statController.SetStat(StatType.SP, InData.sp);
        statController.SetStat(StatType.MoveSpeed, InData.moveSpeed);
        statController.SetStat(StatType.AttackSpeed, 1);

        AddController<FMoveController>();
        FindController<FMoveController>().SetStartPoint(InStartPoint);
     
        AddController<FSkillController>();
        FindController<FSkillController>().Initialize(InData.skillIDList);
     
        AddController<FAbnormalityController>();
    }
    public void OnStatChanged(StatType InType, float InValue)
    {
        if (InType != StatType.HP)
            return;

        SetHPText((int)InValue);
    }

    private void SetHPText(int InHP)
    {
        string text;
        if (1000 <= InHP)
            text = InHP / 1000 + "k";
        else if (InHP < 0)
            text = "";
        else
            text = InHP.ToString();

        hpText.text = text;
    }
}

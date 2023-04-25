using FEnum;
using TMPro;
using UnityEngine;

public class FEnemy : FObjectBase, FStatObserver
{
    [SerializeField]
    TextMeshPro hpText;

    public void Initialize(FEnemyData InData, int InAccumulateCount)
    {
        ContentID = InData.id;

        AddController<FStatController>();
        AddController<FMoveController>();
        AddController<FAbnormalityController>();

        FindController<FStatController>().AddObserver(this);

        FStatController statController = FindController<FStatController>();
        statController.SetStat(StatType.HP, InData.hp + InData.hpIncreaseBySpawnCount * InAccumulateCount);
        statController.SetStat(StatType.SP, InData.sp);
        statController.SetStat(StatType.MoveSpeed, InData.moveSpeed);
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

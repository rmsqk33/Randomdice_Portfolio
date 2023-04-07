using FEnum;
using TMPro;
using UnityEngine;

public class FEnemy : FObjectBase, FStatObserver
{
    [SerializeField]
    TextMeshPro hpText;

    protected override void Awake()
    {
        AddController<FIFFController>();
        AddController<FStatController>();
        AddController<FMoveController>();
        AddController<FAbnormalityController>();
        
        FindController<FStatController>().AddObserver(this);
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

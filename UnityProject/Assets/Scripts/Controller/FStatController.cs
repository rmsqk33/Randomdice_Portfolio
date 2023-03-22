using TMPro;

public class FStatController : FControllerBase
{
    int hp;
    TextMeshPro hpText;

    public int HP 
    {
        get { return hp; }
        set 
        { 
            hp = value;
            UpdateUI();
        } 
    }

    public int SP { get; set; }

    public FStatController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public override void Initialize()
    {
        base.Initialize();
        
        hpText = FindChildComponent<TextMeshPro>("hpText");
    }

    public void OnDamage(int InDamage)
    {
        HP -= InDamage;

        if(HP <= 0)
        {
            FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
            if(battleController != null)
            {
                battleController.SP += SP;
            }

            FObjectManager.Instance.RemoveEnemey(ObjectID);
        }
    }

    private void UpdateUI()
    {
        if (hpText != null)
        {
            string text;
            if (1000 <= hp)
                text = hp / 1000 + "k";
            else
                text = hp.ToString();

            hpText.text = text;
        }
    }
}


public class FSkillBase
{
    protected FObjectBase owner;
    protected FObjectBase target;

    public FObjectBase Target { set { target = value; } }

    public FSkillBase(FObjectBase owner, FSkillData InSkillData)
    {
        this.owner = owner;
        Initialize(InSkillData);
    }

    protected virtual void Initialize(FSkillData InSkillData) { }
    public virtual void Tick(float InDelta) { }
}

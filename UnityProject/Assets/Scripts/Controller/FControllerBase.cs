
public class FControllerBase
{
    protected FObjectBase SummonOwner { get { return Owner.SummonOwner; } }
    protected FObjectBase Owner { get; set; }
    public int ObjectID { get { return Owner.ObjectID; } }
    public int ContentID { get { return Owner.ContentID; } }

    public FControllerBase(FObjectBase InOwner)
    {
        Owner = InOwner;
    }

    public virtual void Initialize() { }
    public virtual void Release() { }
    public virtual void Tick(float InDeltaTime) { }

    protected T FindController<T>() where T : FControllerBase
    {
        return Owner.FindController<T>();
    }

    public T FindChildComponent<T>(string InName)
    {
        return Owner.FindChildComponent<T>(InName);
    }

    public bool IsOwnLocalPlayer()
    {
        return Owner.IsOwnLocalPlayer();
    }
}

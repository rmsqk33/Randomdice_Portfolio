
public class FAbnormality 
{
    FObjectBase target;

    public FAbnormality(FObjectBase InTarget)
    {
        target = InTarget;   
    }

    public virtual void Initialize() { }
    public virtual void  Overlap() { }
}

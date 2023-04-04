using FEnum;

public class FIFFController : FControllerBase
{
    public IFFType IFFType { get; set; }

    public FIFFController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public bool IsEnumy(IFFType InType)
    {
        if (InType == IFFType.Neutral)
            return false;

        if (IFFType == IFFType.Neutral)
            return false;

        return IFFType != InType;
    }
}

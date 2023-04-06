using FEnum;

public interface FStatObserver
{
    void OnStatChanged(StatType InType, float InValue);
}

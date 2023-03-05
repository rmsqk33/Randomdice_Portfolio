using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FDiceStatInfo : MonoBehaviour
{
    [SerializeField]
    Image statIcon;
    [SerializeField]
    TextMeshProUGUI title;
    [SerializeField]
    TextMeshProUGUI value;
    [SerializeField]
    TextMeshProUGUI upgradeValue;

    public Sprite StatIcon { set { statIcon.sprite = value; } }
    public string Title { set { title.text = value; } }
    public string Value { set { this.value.text = value; } }
    public string UpgradeValue { set { upgradeValue.text = value; } }
    public bool Upgradable { set { upgradeValue.gameObject.SetActive(value); } }
}

using UnityEngine;
using UnityEngine.UI;

public class DisplayValue : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private int max;
    [SerializeField] private string maxDisplay;
    [SerializeField] private int multiplier = 1;

    public void SetValue(float value)
    {
        int valueInt = (int)value;
        text.text = valueInt == max ? maxDisplay : (valueInt * multiplier).ToString();
    }
}

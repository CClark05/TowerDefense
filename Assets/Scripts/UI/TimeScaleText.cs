using TMPro;
using UnityEngine;

public class TimeScaleText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        text.text = $"x{Time.timeScale:0.0}";
    }
}

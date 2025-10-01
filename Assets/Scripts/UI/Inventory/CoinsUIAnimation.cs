using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CoinsUIAnimation : MonoBehaviour
{
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private TextMeshProUGUI amountText;
    private (TextMeshProUGUI mesh, int amount) currentPopup;
    private Color originalColor;
    private float spacing = 40;
    public event Action<int> OnPopupComplete;
    private float despawnAt;
    private Coroutine despawnCoroutine;
    private void Start()
    {
        PlayerInventory.Instance.OnCoinsAdded += ShowPopup;
        PlayerInventory.Instance.OnCoinsRemoved += (coins) => ShowPopup(-coins);
    }

    private void ShowPopup(int coins)
    {
        ColorUtility.TryParseHtmlString("#A53030", out var redColor);
        
        float lifeDuration = 1f;
        amountText.ForceMeshUpdate();
        float width = amountText.preferredWidth / amountText.canvas.rootCanvas.scaleFactor;
        if (currentPopup.mesh == null){
            currentPopup.mesh = Instantiate(popupPrefab, transform).GetComponent<TextMeshProUGUI>();
            originalColor = currentPopup.mesh.color;
        }
            
        currentPopup.amount += coins;
        currentPopup.mesh.color = currentPopup.amount < 0 ? redColor : originalColor;
        currentPopup.mesh.text = currentPopup.amount.ToString("+0;-0;0");
        currentPopup.mesh.rectTransform.anchoredPosition = amountText.rectTransform.anchoredPosition + new Vector2(width + spacing, 0);
        despawnAt = Time.unscaledTime + lifeDuration;
        if(despawnCoroutine == null)
            despawnCoroutine = StartCoroutine(DespawnPopup());
    }

    private IEnumerator DespawnPopup()
    {
        while (true)
        {
            if (Time.unscaledTime >= despawnAt)
                break;
            yield return null;
        }
        OnPopupComplete?.Invoke(currentPopup.amount);
        currentPopup.mesh.DOFade(0, 0.3f).OnComplete(() =>
        {
            Destroy(currentPopup.mesh.gameObject);
        }).SetUpdate(true);
        currentPopup = (null, 0);
        despawnCoroutine = null;
    }
}
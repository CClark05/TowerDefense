using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUIAnimation : MonoBehaviour
{
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image coinImage;
    private (TextMeshProUGUI mesh, int amount) currentPopup;
    private Color originalColor;
    private float spacing = -20;
    public event Action<int> OnPopupComplete;
    private float despawnAt;
    private Coroutine despawnCoroutine;
    private Tween shakeTween;
    private void Start()
    {
        PlayerInventory.Instance.OnCoinsAdded += ShowPopup;
        PlayerInventory.Instance.OnCoinsRemoved += (coins) => ShowPopup(-coins);
        BuildButtonUI.OnNotEnoughCoins += OnNotEnoughCoins;
        ShopCardUI.OnNotEnoughCoins += OnNotEnoughCoins;
        SpinToWinAnimation.Instance.OnNotEnoughCoins += OnNotEnoughCoins;
    }

    private void OnNotEnoughCoins()
    {
        shakeTween?.Kill();
        shakeTween = coinImage.rectTransform.DOShakeAnchorPos(0.3f, 5f, 15).SetUpdate(true);
    }

    private void ShowPopup(int coins)
    {
        ColorUtility.TryParseHtmlString("#A53030", out var redColor);
        
        float lifeDuration = 1f;
        amountText.ForceMeshUpdate();
        float width = amountText.preferredWidth;
        if (currentPopup.mesh == null){
            currentPopup.mesh = Instantiate(popupPrefab, amountText.transform).GetComponent<TextMeshProUGUI>();
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
        while (Time.unscaledTime < despawnAt) yield return null;
        var popup   = currentPopup.mesh;
        var amount  = currentPopup.amount;

        OnPopupComplete?.Invoke(amount);

        currentPopup = (null, 0);
        despawnCoroutine = null;

        if (popup != null)
        {
            popup.DOFade(0f, 0.6f)
                .SetUpdate(true)               
                .OnComplete(() => Destroy(popup.gameObject));
        }
    }
}
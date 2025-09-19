using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SkillCardAnimation : MonoBehaviour
{
    [SerializeField] RectTransform content;
    private Vector2 originalPos;
    private Vector3 originalScale;
    private SkillCardUI cardUI;
    private Tweener t;

    private void Awake()
    {
        cardUI = GetComponent<SkillCardUI>();
    }

    private void Start()
    {
        StartCoroutine(DelayedSetup());
        cardUI.OnSellCard += RemoveCard;
        GetComponent<CardDragDrop>().OnDropCard += RemoveCard;
        originalScale = content.localScale;
        cardUI.OnHoverCard += () => { content.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f); };
        cardUI.OnLeaveHoverCard += () => {content.DOScale(originalScale, 0.1f); };
    }

    private IEnumerator DelayedSetup()
    {
        yield return new WaitForEndOfFrame();
        originalPos = content.anchoredPosition;

        cardUI.OnClickCard += selected =>
        {
            t?.Kill();
            if (selected)
                t = content.DOAnchorPosX(originalPos.x - 20f, 0.25f).SetEase(Ease.OutQuad);
            else
                t = content.DOAnchorPosX(originalPos.x, 0.25f).SetEase(Ease.OutQuad);
        };
    }

    private void RemoveCard()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetDelay(0.25f)
            .OnComplete(() => { CoroutineRunner.Instance.StartCoroutine(RecalculateAfterDestroy()); });
    }

    private IEnumerator RecalculateAfterDestroy()
    {
        cardUI.RemoveCard(cardUI);
        Destroy(gameObject);
        yield return null;
        foreach (var card in InventoryUI.Instance.SkillCards)
            if (card != null)
                card.GetComponent<SkillCardAnimation>().RecalculatePosition();
    }

    private void RecalculatePosition()
    {
        originalPos = content.anchoredPosition;
    }
}
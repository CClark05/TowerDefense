using System;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using DG.Tweening;
using NUnit.Framework.Constraints;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class CardSelectUI : Singleton<CardSelectUI>
{
    [SerializeField] private List<SkillData> startingSkills;
    [SerializeField] private SkillRegistry skillRegistry;
    [SerializeField] private CardRaritySettings raritySettings;
    [SerializeField] private RerollSettings rerollSettings;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private Button_Base rerollButton;
    [SerializeField] private Button_Base skipButton;
    [SerializeField] private Button_Base peekButton;
    [SerializeField] private TextMeshProUGUI rerollCostText, rerollText;
    [SerializeField] private GameObject background;
    private List<GameObject> currentCards = new();
    private CardCooldowns cardCooldowns = new(6);
    private int rerollAmount;
    private int rerollCost;
    public event Action<int> OnReroll;
    public event Action OnSelectedCard;
    public bool IsActive => background.activeSelf;
    public event Action OnShowCards;

    private void Start()
    {
        rerollCost = rerollSettings.BaseCost + rerollSettings.IncreasePerRoll * rerollAmount;
        SetPriceText();
        rerollButton.OnClick.AddListener(() =>
        {
            if (PlayerInventory.Instance.Coins < rerollCost || rerollAmount >= rerollSettings.MaxRerolls) return;
            OnReroll?.Invoke(rerollCost);
            rerollAmount++;
            rerollCost = rerollSettings.BaseCost + rerollSettings.IncreasePerRoll * rerollAmount;
            GenerateRandomCards();
            SetPriceText();
        });
        skipButton.OnClick.AddListener(() =>
        {
            background.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() => background.SetActive(false));
            skipButton.GetComponent<CanvasGroup>().DOFade(0, 0.2f).SetEase(Ease.InCubic);
            currentCards.ForEach(c => c.GetComponent<CardSelectCardAnimation>().AnimateBack());
            peekButton.gameObject.SetActive(false);
            InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Stop();
            OnSelectedCard?.Invoke();
        });
        peekButton.OnClick.AddListener(() =>
        {
            background.SetActive(!background.activeSelf);
            if (IsActive) OnShowCards?.Invoke();
        });
        PlayerInventory.Instance.OnCoinsUpdated += coins => { SetPriceText(); };


        EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
        background.SetActive(false);
    }

    private void SetPriceText()
    {
        //rerollButton.gameObject.SetActive(true);
        Color color = PlayerInventory.Instance.Coins >= rerollCost ? Color.white : ColorPicker.red;
        rerollCostText.text = $"${rerollCost}";
        rerollCostText.color = color;
        if (rerollAmount < rerollSettings.MaxRerolls) return;
        rerollButton.gameObject.SetActive(false);
    }

    private int counter;

    private void OnWaveComplete()
    {
        if (PlayerLife.Instance.CurrentLives <= 0) return;
        var encounter = EncounterGenerator.Instance.GetEncounter(EnemyManager.Instance.CurrentWave - 1);
        if (encounter != null) return;
        GenerateRandomCards();
        rerollAmount = 0;
        rerollCost = rerollSettings.BaseCost + rerollSettings.IncreasePerRoll * rerollAmount;
        SetPriceText();
        cardCooldowns.DecreaseCooldowns();
        float fadeDuration = 0.5f;
        background.SetActive(true);
        background.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        background.GetComponent<Image>().DOFade(203f / 255f, fadeDuration).OnComplete(() => { OnShowCards?.Invoke(); });
    }


    private void GenerateRandomCards()
    {
        foreach (var card in currentCards)
            Destroy(card);

        currentCards.Clear();
        var availableSkills = EnemyManager.Instance.CurrentWave != 2
            ? Enumerable.ToHashSet(skillRegistry.Skills.Where(skill =>
                skill.prerequisiteSkills.Length == 0 || skill.prerequisiteSkills.Any(pr => skillRegistry.CurrentSkills.Contains(pr))))
            : Enumerable.ToHashSet(startingSkills);

        var filteredSkills = Enumerable.ToHashSet(availableSkills.Where(skill => !cardCooldowns.ContainsKey(skill)));
        var pool = filteredSkills.Count >= 3 ? filteredSkills : availableSkills;
        var cards = CardRarityPicker.PickCards(pool.ToList(), raritySettings, 3);
        cardCooldowns.SetCooldowns(cards);
        foreach (var data in cards)
        {
            var cardParent = cardSlots[currentCards.Count];
            var newCard = Instantiate(cardPrefab, cardParent.transform);
            newCard.GetComponent<SetCardData>().SetData(data);
            currentCards.Add(newCard);
            var cardAnimation = newCard.GetComponent<CardSelectCardAnimation>();
            cardAnimation.Init(currentCards.Count);
            cardAnimation.OnDoneAnimating += () =>
            {
                newCard.GetComponentInChildren<Button_Base>().OnClick.AddListener(() =>
                {
                    if (InventoryUI.Instance.CanAddCard())
                    {
                        var skillInstance = data.CreateInstance();
                       
                        peekButton.gameObject.SetActive(false);
                        rerollButton.gameObject.SetActive(false);
                        skipButton.gameObject.SetActive(false);
                        foreach (var c in currentCards.Where(c => c != newCard))
                        {
                            c.GetComponent<CardSelectCardAnimation>().AnimateBack();
                        }
                        newCard.GetComponent<CardSelectCardAnimation>().MoveToInventory(InventoryUI.Instance.GetNextCardSlot(), () =>
                        {
                            InventoryUI.Instance.AddCard(skillInstance);
                            background.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() =>
                            {
                                background.SetActive(false);
                                OnSelectedCard?.Invoke();
                            });
                        });
                        
                        return;
                    }

                    newCard.GetComponent<UIShake>().TriggerShake();
                    InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Play();
                    InventoryUI.Instance.OnRemovedCard += () => { InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Stop(); };
                });
            };
        }
        /**
        //rerollButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        peekButton.gameObject.SetActive(true);
        RectTransform rerollRT = rerollButton.GetComponent<RectTransform>();
        RectTransform skipRT = skipButton.GetComponent<RectTransform>();
        RectTransform peekRT = peekButton.GetComponent<RectTransform>();
        Vector2 rerollTarget = rerollRT.anchoredPosition;
        Vector2 skipTarget = skipRT.anchoredPosition;
        Vector2 peekTarget = peekRT.anchoredPosition;
        const float offscreenY = 775f;
        rerollRT.anchoredPosition = new Vector2(rerollTarget.x, offscreenY);
        skipRT.anchoredPosition = new Vector2(skipTarget.x, offscreenY);
        peekRT.anchoredPosition = new Vector2(peekTarget.x, offscreenY);
        rerollRT.DOAnchorPosY(rerollTarget.y, CardSelectCardAnimation.AnimationDuration)
            .SetEase(Ease.OutBounce).SetDelay(CardSelectCardAnimation.DelayPerCard * 4);
        skipRT.DOAnchorPosY(skipTarget.y, CardSelectCardAnimation.AnimationDuration)
            .SetEase(Ease.OutBounce)
            .SetDelay(CardSelectCardAnimation.DelayPerCard * 4);
        peekRT.DOAnchorPosY(peekTarget.y, CardSelectCardAnimation.AnimationDuration)
            .SetEase(Ease.OutBounce)
            .SetDelay(CardSelectCardAnimation.DelayPerCard * 4);
            */
        FunctionTimer.Create(() =>
        {
            float duration = 0.2f;
            skipButton.gameObject.SetActive(true);
            var canvasGroup = skipButton.GetComponent<CanvasGroup>();
            canvasGroup.DOFade(1, duration).SetEase(Ease.OutCubic);
            skipButton.gameObject.transform.DOLocalMoveY(-405f, duration).SetEase(Ease.OutCubic);
        }, 0.15f);

    }
}
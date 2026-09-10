using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CodeMonkey.Utils;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class UpgradeUI : Singleton<UpgradeUI>
{
    [SerializeField] private Transform cardLayoutGroup;
    [SerializeField] private GameObject cardPrefab, visualCardPrefab, outputCardPrefab;
    [SerializeField] private SOEvent onShowUpgradeUI, onShowUpgradeVisual;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button_Base leaveButton;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private Transform outputSlot;
    private UpgradeUIAnimation animation;
    private SetCardData outputCard;

    public event Action<ICardUI, Transform> OnUpgradeCard;
    public event Action<ICardUI, Transform> OnInputCard;
    public event Action<ICardUI, Transform> OnRemoveInputCard;

    private readonly Dictionary<(SkillData data, int level), List<UpgradeableCard>> cardGroups = new();
    private readonly Dictionary<(SkillData data, int level), UpgradeCardUI> listUIByKey = new();

    private (UpgradeableCard card, GameObject gameObject)[] cardsInSlots = new (UpgradeableCard, GameObject)[2];

    internal class UpgradeableCard : ShopSellPanel.SellableCard
    {
        public UpgradeableCard(SkillInstance instance, IUsesCards owner) : base(instance, owner)
        {
        }

        public int Count;
    }

    protected override void Awake()
    {
        base.Awake();
        animation = GetComponent<UpgradeUIAnimation>();
        onShowUpgradeUI.OnRaised += _ =>
        {
            if (canvas.gameObject.activeSelf) return;
            Refresh();
            canvas.gameObject.SetActive(true);
        };

        leaveButton.OnClick.AddListener(() =>
        {
            canvas.gameObject.SetActive(false);
            ClearSlots();
        });
    }

    private void Refresh()
    {
        foreach (Transform child in cardLayoutGroup)
            Destroy(child.gameObject);

        cardGroups.Clear();
        listUIByKey.Clear();

        var all = new List<UpgradeableCard>();

        all.AddRange(InventoryUI.Instance.SkillCards
            .Select(s => new UpgradeableCard(s.SkillInstance, InventoryUI.Instance)));

        foreach (var t in TowerDataHolder.ActiveTowerList)
        {
            var owner = t.GetComponent<IUsesCards>();
            all.AddRange(t.SkillInstanceList.Select(inst => new UpgradeableCard(inst, owner)));
        }

        all.RemoveAll(c => c.Instance.Laminated);
        foreach (var c in all)
        {
            var key = (c.Instance.Data, c.Instance.PlayCount);
            if (!cardGroups.TryGetValue(key, out var list))
                cardGroups[key] = list = new List<UpgradeableCard>();
            list.Add(c);
        }

        foreach (var kvp in cardGroups)
        {
            var list = kvp.Value;
            var rep = list
                .Where(c => c.Instance.RuntimeStat.HasValue)
                .OrderByDescending(c => c.Instance.RuntimeStat.Value)
                .FirstOrDefault() ?? list[0];
            rep.Count = list.Count;
            AddListCard(rep);
        }
    }

    private Transform AddListCard(UpgradeableCard repCard, float delay = 0, int? overrideCount = null)
    {
        var newCard = Instantiate(cardPrefab, cardLayoutGroup.transform);

        var setData = newCard.GetComponent<SetCardData>();
        var upgradeUI = newCard.GetComponent<UpgradeCardUI>();
        var cardUI = newCard.GetComponent<ICardUI>();

        repCard.UI = cardUI;

        setData.SetData(repCard.Instance.Data);
        setData.SetInstance(repCard.Instance);

        upgradeUI.Init(overrideCount ?? repCard.Count);

        var uiKey = (repCard.Instance.Data, repCard.Instance.PlayCount);
        listUIByKey[uiKey] = upgradeUI;
        if (delay > 0)
        {
            upgradeUI.ToggleVisibility(false);
            FunctionTimer.Create(() => { upgradeUI.ToggleVisibility(true); }, delay);
        }

        cardUI.OnHoverCard += () =>
        {
            
            foreach (var slot in cardSlots)
            {
                if (slot.childCount != 0) continue;
                slot.GetComponent<UpgradeSlotAnimation>().ToggleAnimation(true);
                break;
            }
        };

        cardUI.OnLeaveHoverCard += () =>
        {
            foreach (var slot in cardSlots)
                slot.GetComponent<UpgradeSlotAnimation>().ToggleAnimation(false);
        };

        cardUI.OnClickCard += () =>
        {
            for (int i = 0; i < cardSlots.Length; i++)
            {
                var slot = cardSlots[i];
                if (slot.childCount != 0) continue;
                if (i == 0) cardSlots[1].GetComponent<UpgradeSlotAnimation>().ToggleAnimation(true);

                var key = (repCard.Instance.Data, repCard.Instance.PlayCount);
                if (!cardGroups.TryGetValue(key, out var list) || list.Count == 0)
                    return;
                var picked = list
                    .Where(c => c.Instance.RuntimeStat.HasValue)
                    .OrderByDescending(c => c.Instance.RuntimeStat.Value)
                    .FirstOrDefault() ?? list[^1];
                list.Remove(picked);

                upgradeUI.AddCount(-1, 0);

                var slotSet = Instantiate(visualCardPrefab, slot).GetComponent<SetCardData>();
                slotSet.transform.position = cardUI.GameObject.transform.position;
                OnInputCard?.Invoke(slotSet.GetComponent<ICardUI>(), slot);

                slotSet.SetData(picked.Instance.Data);
                slotSet.SetInstance(picked.Instance);

                cardsInSlots[i] = (picked, slotSet.gameObject);

                slotSet.GetComponent<ICardUI>().OnClickCard += () =>
                {
                    slotSet.GetComponent<ICardUI>().ToggleButton(false);
                    var putKey = (picked.Instance.Data, picked.Instance.PlayCount);
                    if (!cardGroups.TryGetValue(putKey, out var putList))
                        cardGroups[putKey] = putList = new List<UpgradeableCard>();
                    putList.Add(picked);
                    var bestRep = putList
                        .Where(c => c.Instance.RuntimeStat.HasValue)
                        .OrderByDescending(c => c.Instance.RuntimeStat.Value)
                        .FirstOrDefault() ?? putList[0];
                    
                    cardsInSlots[i] = (null, null);
                    if (listUIByKey.TryGetValue(putKey, out var ui) && ui != null)
                    {
                        ui.AddCount(1, animation.AnimationDuration);
                        OnRemoveInputCard?.Invoke(slotSet.GetComponent<ICardUI>(), ui.GameObject.transform);
                    }
                    else
                    {
                        var c = AddListCard(picked, animation.AnimationDuration, 1);
                        StartCoroutine(WaitFrame());

                        IEnumerator WaitFrame()
                        {
                            yield return null;
                            c.GetComponent<CardScrollTo>().ScrollToThis();
                            OnRemoveInputCard?.Invoke(slotSet.GetComponent<ICardUI>(), c);
                            
                        }
                    }

                    ClearOutput();
                };

                if (cardSlots.All(s => s.childCount != 0))
                {
                    var c1 = cardsInSlots[0].card;
                    var c2 = cardsInSlots[1].card;
                    if (c1 == null || c2 == null) return;

                    if (c1.Instance.PlayCount == c2.Instance.PlayCount && c1.Instance.Data == c2.Instance.Data)
                    {
                        var c = (c1.Instance.RuntimeStat.HasValue, c2.Instance.RuntimeStat.HasValue) switch
                        {
                            (true, true) => c1.Instance.RuntimeStat.Value >= c2.Instance.RuntimeStat.Value ? c1 : c2,
                            (true, false) => c1,
                            (false, true) => c2,
                            _ => c1
                        };
                        CreateOrReplaceOutput(c);
                    }
                        
                }

                break;
            }
        };

        return newCard.transform;
    }

    private void CreateOrReplaceOutput(UpgradeableCard baseCard)
    {
        ClearOutput();

        var newInstance = baseCard.Instance.Data.CreateInstance();
        newInstance.PlayCount = baseCard.Instance.PlayCount + 1;
        if (baseCard.Instance.RuntimeStat.HasValue)
        {
            newInstance.SetRuntimeStat(baseCard.Instance.RuntimeStat.Value);
            Debug.Log(baseCard.Instance.RuntimeStat.Value);
        }

        outputCard = Instantiate(outputCardPrefab, outputSlot).GetComponent<SetCardData>();
        outputCard.SetData(baseCard.Instance.Data);
        outputCard.SetInstance(newInstance);

        outputCard.GetComponent<ICardUI>().OnClickCard += () =>
        {
            UpgradeableCard c1 = cardsInSlots[0].card;
            UpgradeableCard c2 = cardsInSlots[1].card;
            
            if (cardsInSlots[0].card.Instance.RuntimeStat.HasValue && cardsInSlots[1].card.Instance.RuntimeStat.HasValue)
            {
                (c1, c2) = cardsInSlots[0].card.Instance.RuntimeStat.Value >= cardsInSlots[1].card.Instance.RuntimeStat.Value
                    ? (cardsInSlots[0].card, cardsInSlots[1].card)
                    : (cardsInSlots[1].card, cardsInSlots[0].card);
            }
            
            if (c1 == null || c2 == null) return;
            
            outputCard.GetComponent<ICardUI>().ToggleButton(false);
            outputCard.DisableTabs();
            
            if (baseCard.Instance.RuntimeStat.HasValue)
                c1.Instance.SetRuntimeStat(baseCard.Instance.RuntimeStat.Value);
            c1.Instance.PlayCount++;
            c2.Owner.RemoveCard(c2.Instance);

            var c = AddUpgradedCardToUI(newInstance);
            
            StartCoroutine(WaitFrame());
            IEnumerator WaitFrame()
            {
                yield return null;
                c.GetComponent<CardScrollTo>().ScrollToThis();
                OnUpgradeCard?.Invoke(outputCard.GetComponent<ICardUI>(), c);
                ClearSlots(true);
            }
        };
    }

    private Transform AddUpgradedCardToUI(SkillInstance newInstance)
    {
        var key = (newInstance.Data, newInstance.PlayCount);

        var added = new UpgradeableCard(newInstance, InventoryUI.Instance);

        if (!cardGroups.TryGetValue(key, out var list))
            cardGroups[key] = list = new List<UpgradeableCard>();
        list.Add(added);

        if (listUIByKey.TryGetValue(key, out var ui) && ui != null)
        {
            ui.AddCount(1, animation.AnimationDuration);
            return ui.transform;
        }

        var card = AddListCard(added, animation.AnimationDuration, 1);
        return card.transform;
    }

    private void ClearOutput(bool playAnimation = false)
    {
        if (outputCard == null) return;
        if (!playAnimation)
            Destroy(outputCard.gameObject);
        outputCard = null;
    }

    private void ClearSlots(bool playAnimation = false)
    {
        foreach (var entry in cardsInSlots)
        {
            if (entry.gameObject != null)
                entry.gameObject.GetComponent<InputCardAnimation>().DestroyAnimation();
        }


        cardsInSlots = new (UpgradeableCard, GameObject)[2];
        ClearOutput(playAnimation);
    }
}
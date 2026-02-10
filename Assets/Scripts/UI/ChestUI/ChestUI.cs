using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    [SerializeField] private Button_Base chestButton;
    [SerializeField] private SkillData wildCardData;
    [SerializeField] private SkillData[] possibleCards;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform background, rewardLayout;
    private int totalRewards = 2;
    private int rewardsLeft;
    private TextMeshProUGUI rewardsLeftText;
    private List<SkillData> generatedRewards = new();
    private List<GameObject> rewardSlots = new();
    private void Start()
    {
        rewardsLeft = totalRewards;
        rewardsLeftText = chestButton.GetComponentInChildren<TextMeshProUGUI>();
        rewardsLeftText.text = totalRewards.ToString();
        for (int i = 0; i < totalRewards; i++)
        {
            var slot = new GameObject($"Slot_{i}", typeof(RectTransform)).GetComponent<RectTransform>();
            slot.SetParent(rewardLayout, false); 
            rewardSlots.Add(slot.gameObject);
        }
        chestButton.OnClick.AddListener(() =>
        {
            rewardsLeft--;
            rewardsLeftText.text = rewardsLeft.ToString();
            Transform parent = rewardSlots.First(slot => slot.transform.childCount == 0).transform;
            var card = Instantiate(cardPrefab, background.transform).GetComponent<SetCardData>();
            card.transform.position = chestButton.transform.position;
            card.GetComponent<ChestCardAnimation>().MoveTo(parent.transform.position, () =>
            {
                card.transform.parent = parent;
            });
            var reward = GenerateReward(rewardsLeft);
            card.SetData(reward);
            generatedRewards.Add(reward);
            if (rewardsLeft <= 0)
            {
                chestButton.gameObject.SetActive(false);
            }
            card.GetComponent<ICardUI>().OnClickCard += () =>
            {
                if (!InventoryUI.Instance.CanAddCard())
                {
                    card.GetComponent<UIShake>().TriggerShake();
                    InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Play();
                    InventoryUI.Instance.OnRemovedCard += () => { InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Stop(); };
                    return;
                }
                generatedRewards.Remove(reward);
               
                SkillInstance skillInstance = reward.CreateInstance();
                card.GetComponent<ChestCardAnimation>().MoveToInventory(InventoryUI.Instance.GetNextCardSlot(), () =>
                {
                    InventoryUI.Instance.AddCard(skillInstance);
                    if(generatedRewards.Count == 0)
                        background.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() => { background.gameObject.SetActive(false); });
                });
            };
        });
        background.gameObject.SetActive(false);
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            var encounter = EncounterGenerator.Instance.GetEncounter(EnemyManager.Instance.CurrentWave - 1);
            if (encounter == EncounterGenerator.Instance.BossEncounter)
            {
                background.gameObject.SetActive(true);
                rewardsLeft = totalRewards;
                background.GetComponent<Image>().DOFade(203/255f, 0.5f).OnComplete(() =>
                {
                    chestButton.gameObject.SetActive(true);
                });
                
                
            }
        };
    }

    private SkillData GenerateReward(int rewardsLeft)
    {
        return rewardsLeft == totalRewards - 1 
            ? wildCardData 
            : possibleCards[UnityEngine.Random.Range(0, possibleCards.Length)];
    }
}
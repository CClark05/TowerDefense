using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    [SerializeField] private Button_Base chestButton;
    [SerializeField] private RelicData[] possibleRelics;
    [SerializeField] private Transform background, rewardLayout;
    [SerializeField] private TextMeshProUGUI rewardCounterText;
    [SerializeField] private Sprite openChestSprite;
    [SerializeField] private GameObject relicPrefab;
    private Sprite closedChestSprite;
    private List<SkillData> generatedRewards = new();
    private List<GameObject> rewardSlots = new();
    private void Start()
    {
        closedChestSprite = chestButton.GetComponent<Image>().sprite;
        rewardCounterText.text = "1";
        chestButton.OnClick.AddListener(() =>
        {
            rewardCounterText.text = "";
            chestButton.enabled = false;
            chestButton.GetComponent<Image>().sprite = openChestSprite;
            var relicInstance = possibleRelics[Random.Range(0, possibleRelics.Length)].CreateInstance();
            relicInstance.OnPickup();
            var relic = Instantiate(relicPrefab, rewardLayout).GetComponent<RelicUI>();
            relic.Init(relicInstance);
            relic.OnClick += () =>
            {
                background.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() =>
                {
                    background.gameObject.SetActive(false);
                });
                chestButton.GetComponent<Image>().DOFade(0f, 0.5f);
            };
        });
        //background.gameObject.SetActive(false);
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            var encounter = EncounterGenerator.Instance.GetEncounter(EnemyManager.Instance.CurrentWave - 1);
            if (encounter == EncounterGenerator.Instance.BossEncounter)
            {
                chestButton.enabled = true;
                chestButton.GetComponent<Image>().sprite = closedChestSprite;
                background.gameObject.SetActive(true);
                background.GetComponent<Image>().DOFade(203/255f, 0.5f).OnComplete(() =>
                {
                    chestButton.gameObject.SetActive(true);
                });
            }
        };
    }
    
}
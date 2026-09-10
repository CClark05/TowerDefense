using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChestUI : MonoBehaviour
{
    
    [SerializeField] private Button_Base chestButton;
    [SerializeField] private RelicData[] possibleRelics;
    [SerializeField] private Transform background, rewardLayout;
    [SerializeField] private TextMeshProUGUI rewardCounterText;
    [SerializeField] private Sprite openChestSprite;
    [SerializeField] private GameObject relicPrefab;
    private Sprite closedChestSprite;
    private float chestOriginalY;
    private void Start()
    {
        chestOriginalY = chestButton.GetComponent<RectTransform>().anchoredPosition.y;
        closedChestSprite = chestButton.GetComponent<Image>().sprite;
        rewardCounterText.text = "1";
        chestButton.OnClick.AddListener(() =>
        {
            rewardCounterText.text = "";
            chestButton.enabled = false;
            chestButton.GetComponent<Image>().sprite = openChestSprite;
            var relicInstance = possibleRelics[Random.Range(0, possibleRelics.Length)].CreateInstance();
            relicInstance.OnPickup();
            var relic = Instantiate(relicPrefab, background.transform).GetComponent<RelicUI>();
            relic.transform.position = chestButton.transform.position;
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
                rewardCounterText.text = "1";
                chestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 690f);
                chestButton.gameObject.SetActive(true);
                chestButton.GetComponent<Image>().sprite = closedChestSprite;
                background.gameObject.SetActive(true);
                chestButton.GetComponent<RectTransform>().DOAnchorPosY(chestOriginalY, 0.75f).SetEase(CustomEase.EaseOutBounceCustom).OnComplete(() =>
                {
                    chestButton.enabled = true;
                });
                background.GetComponent<Image>().DOFade(203 / 255f, 0.5f);
            }
        };
    }
    
}
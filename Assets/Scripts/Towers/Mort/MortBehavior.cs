using System;
using DG.Tweening;
using UnityEngine;

public class MortBehavior : MonoBehaviour
{
    [SerializeField] private Transform boxVisual;
    [SerializeField] private Transform visual;
    [SerializeField] private Sprite openBoxSprite;
    private Sprite originalBoxSprite;
    private TowerCards towerCards;
    private void Awake()
    {
        towerCards = GetComponent<TowerCards>();
        towerCards.OnInit += () =>
        {
            EnemyManager.Instance.OnWaveStarted += OnWaveStarted;
            EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
            originalBoxSprite = boxVisual.GetComponent<SpriteRenderer>().sprite;
        };
        /**
         * Mort goes 2 tiles above his target, unless that takes him off the screen
         */
      
    }

    private void OnWaveComplete()
    {
        visual.DOKill();
        visual.DOMoveY(-1f, 0.5f).SetRelative(true).OnComplete(() =>
        {
            visual.GetComponent<SpriteRenderer>().enabled = false;
            boxVisual.GetComponent<SpriteRenderer>().sprite = originalBoxSprite;
        });
        
    }

    private void OnWaveStarted()
    {
        visual.GetComponent<SpriteRenderer>().enabled = true;
        visual.DOMoveY(2f, 0.5f).SetRelative(true).OnComplete(IdleAnimation);
        boxVisual.GetComponent<SpriteRenderer>().sprite = openBoxSprite;
    }

    private void IdleAnimation()
    {
        visual.DOLocalMoveY(0.5f, 1f).SetRelative(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    
}

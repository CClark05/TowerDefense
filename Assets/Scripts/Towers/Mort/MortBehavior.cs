using System;
using DG.Tweening;
using UnityEngine;

public class MortBehavior : MonoBehaviour, ITowerAimOverride
{
    [SerializeField] private Transform boxVisual;
    [SerializeField] private Transform visual;
    [SerializeField] private Sprite openBoxSprite;
    [SerializeField] private float followSmoothTime = 0.185f;
    [SerializeField] private float maxFollowSpeed = 20f;
    private Vector2 hoverOffset;
    private Sprite originalBoxSprite;
    private TowerCards towerCards;
    private TowerShooting towerShooting;
    private GameObject target;
    private Vector3 followVelocity;
    private Vector3 originalPosition;
    private bool readyToFly;
    private void Awake()
    {
        towerCards = GetComponent<TowerCards>();
        towerShooting = GetComponent<TowerShooting>();
        originalPosition = visual.localPosition;
        towerCards.OnInit += () =>
        {
            EnemyManager.Instance.OnWaveStarted += OnWaveStarted;
            EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
            originalBoxSprite = boxVisual.GetComponent<SpriteRenderer>().sprite;
            towerShooting.OnTargetChanged += (target) =>
            {
                if (target == null) return;
                this.target = target;
                
            };
        };
        followSmoothTime *= 1f + UnityEngine.Random.Range(-0.1f, 0.1f);
        maxFollowSpeed *= 1f + UnityEngine.Random.Range(-0.1f, 0.1f);
        hoverOffset = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(3f, 5f));
    }

    private Vector3 followPos;

    private void LateUpdate()
    {
        if (target == null || !readyToFly) return;
        Vector2 targetPosition = (Vector2)target.transform.position + hoverOffset;
        followPos = Vector3.SmoothDamp(followPos, targetPosition, ref followVelocity, followSmoothTime, maxFollowSpeed);
        visual.position = followPos;
    }

    private void OnWaveComplete()
    {
        target = null;
        readyToFly = false;
        followVelocity = Vector3.zero;
        visual.DOKill();
        visual.DOLocalMove(originalPosition, 1.5f).SetEase(Ease.InOutSine).SetDelay(0.5f).OnComplete(() =>
        {
            visual.GetComponent<SpriteRenderer>().enabled = false;
            boxVisual.GetComponent<SpriteRenderer>().sprite = originalBoxSprite;
        });

    }

    private void OnWaveStarted()
    {
        visual.GetComponent<SpriteRenderer>().enabled = true;
        visual.DOMoveY(2f, 1f).SetRelative(true).SetEase(Ease.OutSine).OnComplete(() =>
        {
            followPos = visual.position;
            followVelocity = Vector3.zero;
            readyToFly = true;
        });
        boxVisual.GetComponent<SpriteRenderer>().sprite = openBoxSprite;
    }

    public bool CanShoot(GameObject target)
    {
        float shootTolerance = 0.5f;
        if (!readyToFly || target == null) return false;
        return Mathf.Abs(visual.position.x - target.transform.position.x) <= shootTolerance;
    }

    public Vector2 GetShotDirection(Vector2 origin, GameObject target) => Vector2.down;
}
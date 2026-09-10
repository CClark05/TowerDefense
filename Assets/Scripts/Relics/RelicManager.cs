using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RelicManager : Singleton<RelicManager>
{
    [SerializeField] private HorizontalLayoutGroup layout;
    [SerializeField] private GameObject relicParent;
    public IEnumerator AddRelic(RelicUI relic)
    {
        var parent = Instantiate(relicParent, layout.transform);
        yield return null;
        float duration = 0.35f;
        relic.transform.DOMove(parent.transform.position, duration).SetEase(Ease.InCubic).OnComplete(() =>
        {
            relic.transform.SetParent(parent.transform);
        });
        relic.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), duration);
    }
    
}
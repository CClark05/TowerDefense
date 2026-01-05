using DG.Tweening;
using UnityEngine;

public class InputCardAnimation : MonoBehaviour
{
    public void DestroyAnimation()
    {
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }
}

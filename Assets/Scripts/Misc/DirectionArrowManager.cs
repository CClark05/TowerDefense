

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DirectionArrowManager : MonoBehaviour
{
    [SerializeField] private GameObject directionArrowPrefab;
    private List<DirectionArrowAnimation> arrows = new();
    private void Start()
    {
        StartCoroutine(CreateArrows());
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            foreach (var arrow in arrows)
                arrow.DestroyArrow();
            arrows.Clear();
        };
    }

    IEnumerator CreateArrows()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.1f);
            var arrow = Instantiate(directionArrowPrefab, AStarPathfinding.Instance.GetPath()[0], Quaternion.identity);
            arrows.Add(arrow.GetComponent<DirectionArrowAnimation>());
        }
    }
}



using System.Collections;
using UnityEngine;

public class DirectionArrowManager : MonoBehaviour
{
    [SerializeField] private GameObject directionArrowPrefab;
    private void Start()
    {
        StartCoroutine(CreateArrows());
    }

    IEnumerator CreateArrows()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Instantiate(directionArrowPrefab, AStarPathfinding.Instance.GetPath()[0], Quaternion.identity);
        }
    }
}

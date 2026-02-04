
using DG.Tweening;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class GridObjectPlacement : MonoBehaviour
{

    private Vector2[] preferredPositions = new Vector2[2];
    private Vector2Int[] preferredGridPositions = new Vector2Int[2];
    private GridManager gridManager;
    private GridObjectButton gridObjectButton;
    [SerializeField] private EncounterManager encounterManager;
    [SerializeField] private GameObject dustAnimationPrefab;
    private Vector3 originalPosition;
    public static float AnimationDuration = 1.5f;
    private void Awake()
    {
        gridManager = GridManager.Instance;
        gridObjectButton = GetComponent<GridObjectButton>();
        originalPosition = transform.position;
    }
    private void Start()
    {
        encounterManager.OnShowVisual.OnRaised += (object sender) =>
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            transform.position = new Vector2(40, originalPosition.y);
            var seq = DOTween.Sequence().SetUpdate(true);
            seq.Append(transform.DOMoveX(originalPosition.x, AnimationDuration).SetEase(Ease.InCubic).SetDelay(0.15f).OnComplete(() =>
            {
                Instantiate(dustAnimationPrefab, transform.position + new Vector3(-1f,-0.3f,0), Quaternion.identity);
            }));
            seq.Join(transform.DORotate(new Vector3(0, 0, 15f), 0.3f).SetDelay(1.35f).SetEase(Ease.OutCubic));
            seq.AppendInterval(0.06f);
            seq.Append(transform.DORotate(Vector3.zero, 0.3f).SetEase(Ease.OutBounce)).OnComplete(() =>
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            });
        };
        gameObject.SetActive(false);
    }
    /**
    private Vector2 FindDesiredPosition()
    {
        preferredPositions = gridObjectButton.Sprites.Select(s => (Vector2)s.transform.position).ToArray();
        for (var i = 0; i < preferredPositions.Length; i++)
        {
            gridManager.Grid.GetXY(preferredPositions[i], out int x, out int y);
            preferredGridPositions[i] = new Vector2Int(x, y);
        }
        foreach(var gridPos in preferredGridPositions)
        {
            var id = gridManager.Grid.GetValue(gridPos.x, gridPos.y).id;
            if (id == 0) continue;
            List<Vector2Int> surroundingGridPositions = new();
            for(int i = preferredGridPositions[0].x - 1; i <= preferredGridPositions[1].x + 1; i++)
            {
                for(int j = preferredGridPositions[0].y - 1; j <= preferredGridPositions[0].y + 1; j++)
                {
                    surroundingGridPositions.Add(new Vector2Int(i, j));
                }
            }
            HashSet<(Vector2Int left, Vector2Int right)> possiblePositions = new();
            foreach(var pos in surroundingGridPositions)
            {
                Vector2Int left = new Vector2Int(pos.x - 1, pos.y);
                Vector2Int right = new Vector2Int(pos.x + 1, pos.y);
                if(surroundingGridPositions.Contains(left))
                    possiblePositions.Add((left, pos));
                if(surroundingGridPositions.Contains(right))
                    possiblePositions.Add((pos, right));
            }

            foreach (var pair in possiblePositions)
            {
                var leftId = gridManager.Grid.GetValue(pair.left.x, pair.left.y).id;
                var rightId = gridManager.Grid.GetValue(pair.right.x, pair.right.y).id;
                if (rightId == 0 && leftId == 0)
                {
                    (Vector2 left, Vector2 right) desiredWorldPositions = (gridManager.Grid.GetWorldPosition(pair.left.x, pair.left.y),
                        gridManager.Grid.GetWorldPosition(pair.right.x, pair.right.y));
                    return ((desiredWorldPositions.left + desiredWorldPositions.right) / 2) + new
                        Vector2(gridManager.Grid.CellSize * 0.5f, gridManager.Grid.CellSize * 0.5f);
                }
                Debug.LogError("NO SPACE FOR SHOP NOT GOOD");
            }
            break;
        }
        return (preferredPositions[0] + preferredPositions[1]) / 2;
    }
    */
}

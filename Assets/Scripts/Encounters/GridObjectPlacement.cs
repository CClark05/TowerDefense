
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

    public void DriveAway()
    {
        var seq = DOTween.Sequence();
        seq.Append(transform.DOMoveX(-26f, 0.7f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        }));
        seq.JoinCallback(() =>
        {
            var dust = Instantiate(dustAnimationPrefab, transform.position + new Vector3(3f, -0.3f, 0), Quaternion.identity);
            dust.transform.localScale = new Vector3(-1, 1, 1);
        }).SetDelay(0.25f);

    }
}

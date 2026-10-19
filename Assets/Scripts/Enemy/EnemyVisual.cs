using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private EnemyDataHolder dataHolder;
    [SerializeField] private SpriteRenderer overlaySr;
    [SerializeField] private Sprite eliteOverlaySprite;

    private void Awake()
    {
        dataHolder = GetComponent<EnemyDataHolder>();
    }

    private void Start()
    {
        if (dataHolder.IsElite)
        {
            overlaySr.sprite = eliteOverlaySprite;
        }

        if (dataHolder.EnemiesUnderneath == null) return;
        for (var i = 0; i < dataHolder.EnemiesUnderneath.Length; i++)
        {
            var e = dataHolder.EnemiesUnderneath[i];
            var enemyVisual = new GameObject("enemy visual").AddComponent<SpriteRenderer>();
            enemyVisual.transform.SetParent(transform);
            enemyVisual.transform.localPosition = new Vector3(0, -0.125f * (i + 1), 0);
            var enemySr = enemyVisual.GetComponent<SpriteRenderer>();
            enemySr.sprite = e.EnemyData.prefab.GetComponent<SpriteRenderer>().sprite;
            enemySr.sortingLayerName = "Foreground";
            enemySr.sortingOrder = -i;
            var baseColor = e.EnemyData.prefab.GetComponent<SpriteRenderer>().color;
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);
            enemySr.color = Color.HSVToRGB(h, s, (0.8f - (0.2f * i)));
        }
    }
}
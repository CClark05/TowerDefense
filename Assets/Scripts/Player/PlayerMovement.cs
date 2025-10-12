using System;
using System.Xml.Linq;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private int cellSize;
    public event Action OnMove;
    private PlayerTurnManager turnManager;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        turnManager = GetComponent<PlayerTurnManager>();
    }
    private void Start()
    {
        cellSize = GridManager.Instance.CellSize;
    }

    private void Update()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W)) direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) direction = Vector2.right;

        if (direction != Vector2.zero && turnManager.MovesRemaining > 0 && EnemyManager.Instance.WaveState is EnemyManager.WaveStates.Idle)
        {
            Vector2 targetPos = rb.position + direction * cellSize;
            GridManager.Instance.Grid.GetXY(targetPos, out int x, out int y);
            if (GridManager.Instance.Grid.GetValue(x, y) == default) return;
            if (GridManager.Instance.Grid.GetValue(x, y).id is 0 or 1 or 6)
            {
                rb.MovePosition(targetPos);
                OnMove?.Invoke();
            }
            
        }
    }
    
}

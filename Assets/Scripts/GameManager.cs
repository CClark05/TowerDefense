using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    public enum Cursors
    {
        Default,
        OpenHand,
        ClosedHand,
        Disabled,
    }
    public Cursors CurrentCursor { get; private set; }
    [SerializeField] private Texture2D defaultCursor, handOpenCursor, handClosedCursor, disabledCursor;
    private void Start()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        CurrentCursor = Cursors.Default;
    }
    public void SetCursor(Cursors cursor)
    {
        if(CurrentCursor == cursor) return;
        CurrentCursor = cursor;
        switch (cursor)
        {
            case Cursors.Default:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case Cursors.OpenHand:
                Cursor.SetCursor(handOpenCursor, Vector2.zero, CursorMode.Auto);
                break;
            case Cursors.ClosedHand:
                Cursor.SetCursor(handClosedCursor, Vector2.zero, CursorMode.Auto);
                break;
            case Cursors.Disabled:
                Cursor.SetCursor(disabledCursor, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinToWinUI : Singleton<SpinToWinUI>
{
    [SerializeField] private Button_Base increaseWagerButton, decreaseWagerButton, leaveButton;
    [SerializeField] private TextMeshProUGUI wagerText, SpinsLeftText;
    [SerializeField] private int startingWager = 10;
    [SerializeField] private WheelUI wheelUI;
    [SerializeField] private Canvas canvas;
    [SerializeField] private SOEvent onShowUI, onShowVisual;
    private SpinToWinAnimation animation;
    public event Action<int> OnPlacedWager;
    public event Action<int> OnWagerComplete;
    [SerializeField] private int spinsLeft = 3;
    private int originalSpinsLeft;
    public int SpinsLeft
    {
        get => spinsLeft;
        set
        {
            if (spinsLeft == value) return;
            spinsLeft = value;
            SpinsLeftText.text = $"{value} Spins Left";
        }
    }
    private int currentWager;
    public int CurrentWager
    {
        get => currentWager;
        private set
        {
            if (value == currentWager) return;
            currentWager = value;
            wagerText.text = "$" + currentWager;
        }
    }
    private Coroutine currentHoldRoutine;
    private void Start()
    {
        originalSpinsLeft = spinsLeft;
        onShowVisual.OnRaised += (object sender) => SpinsLeft = originalSpinsLeft;
        onShowUI.OnRaised += (object sender) => canvas.gameObject.SetActive(true);
        leaveButton.OnClick.AddListener(() => canvas.gameObject.SetActive(false));
        SpinsLeftText.text = $"{spinsLeft} Spins Left";
        animation = GetComponent<SpinToWinAnimation>();
        animation.OnSpinStart += () =>
        {
            increaseWagerButton.SetIsActive(false);
            decreaseWagerButton.SetIsActive(false);
            OnPlacedWager?.Invoke(CurrentWager);
            SpinsLeft--;
        };
        animation.OnSpinComplete += () =>
        {
            increaseWagerButton.SetIsActive(true);
            decreaseWagerButton.SetIsActive(true);
            OnWagerComplete?.Invoke(Mathf.RoundToInt(CurrentWager * wheelUI.CurrentMult));
        };
        increaseWagerButton.OnInitialPress.AddListener(() =>
        {
            currentHoldRoutine ??= StartCoroutine(HoldWagerRoutine(1));
        });
        increaseWagerButton.OnRelease.AddListener(StopRoutine);
        decreaseWagerButton.OnInitialPress.AddListener(() =>
        {
            currentHoldRoutine ??= StartCoroutine(HoldWagerRoutine(-1));
        });
        decreaseWagerButton.OnRelease.AddListener(StopRoutine);

        void StopRoutine()
        {
            if(currentHoldRoutine != null)
            {
                StopCoroutine(currentHoldRoutine);
                currentHoldRoutine = null;
            }
        }
        CurrentWager = startingWager;
    }

    private IEnumerator HoldWagerRoutine(int dir)
    {
        float holdStart = Time.unscaledTime;
        while (true)
        {
            float heldFor = Time.unscaledTime - holdStart;
            float interval = Mathf.Lerp(0.25f, 0.01f, Mathf.Clamp01(heldFor / 2f));
            if (CurrentWager + dir > PlayerInventory.Instance.Coins && dir == 1)
                break;
            if(CurrentWager + dir < 1 && dir == -1)
                break;
            CurrentWager += dir;
            yield return new WaitForSecondsRealtime(interval);
            
        }
    }


}

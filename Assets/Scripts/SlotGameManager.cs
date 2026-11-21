
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotGameManager : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private SlotReel[] reels;

    [Header("UI Buttons")]
    [SerializeField] private Button increaseBetButton;
    [SerializeField] private Button decreaseBetButton;
    [SerializeField] private Button spinButton;

    [Header("UI Text")]
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text statusText;

    [Header("Game Settings")]
    [SerializeField] private int startingBalance = 1000;
    [SerializeField] private int startingBet = 100;
    [SerializeField] private int minimumBet = 100;
    [SerializeField] private int betStep = 100;

    private int balance;
    private int currentBet;
    private bool isSpinning;

    // Payout multipliers for each symbol index.
    // 0 = Cherry, 1 = Lemon, 2 = Orange, 3 = Diamond
    private float[] multipliers = { 1.5f, 2f, 3f, 5f };

    private void Start()
    {
        balance = startingBalance;
        currentBet = startingBet;

        // Connect button events.
        increaseBetButton.onClick.AddListener(IncreaseBet);
        decreaseBetButton.onClick.AddListener(DecreaseBet);
        spinButton.onClick.AddListener(Spin);

        UpdateUI();
        statusText.text = "Place your bet and spin!";
    }

    private void IncreaseBet()
    {
        if (isSpinning)
            return;

        // Prevent the bet from exceeding the player's balance.
        if (currentBet + betStep <= balance)
        {
            currentBet += betStep;
        }

        UpdateUI();
    }

    private void DecreaseBet()
    {
        if (isSpinning)
            return;

        // The bet cannot go below the minimum bet.
        currentBet = Mathf.Max(minimumBet, currentBet - betStep);

        UpdateUI();
    }

    private void Spin()
    {
        if (isSpinning)
            return;

        // Check whether the player can afford the current bet.
        if (balance < currentBet)
        {
            statusText.text = "Not enough balance!";
            return;
        }

        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        // Deduct the bet before the spin begins.
        balance -= currentBet;
        UpdateUI();

        // Generate all results before the reels start spinning.
        int[] results = new int[reels.Length];

        for (int i = 0; i < results.Length; i++)
        {
            results[i] = Random.Range(0, multipliers.Length);
        }

        statusText.text = "Spinning...";

        // Start all reels with their predetermined results.
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].Spin(results[i]);
        }

        // Wait until every reel has finished.
        bool reelsAreSpinning = true;

        while (reelsAreSpinning)
        {
            reelsAreSpinning = false;

            foreach (SlotReel reel in reels)
            {
                if (reel.IsSpinning)
                {
                    reelsAreSpinning = true;
                    break;
                }
            }

            yield return null;
        }

        // Check if all 3 center symbols match.
        bool isWin = results[0] == results[1] &&
                   results[1] == results[2];

        if (isWin)
        {
            // Calculate payout based on the winning symbol.
            float multiplier = multipliers[results[0]];
            int payout = Mathf.RoundToInt(currentBet * multiplier);

            balance += payout;

            statusText.text = "WIN! +" + payout;
        }
        else
        {
            statusText.text = "Try again!";
        }

        isSpinning = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        betText.text = currentBet.ToString() + "G";
        balanceText.text = balance.ToString() + "G";

        // Disable spinning if the player cannot afford the bet.
        spinButton.interactable = !isSpinning && balance >= currentBet;

        increaseBetButton.interactable =
            !isSpinning && currentBet + betStep <= balance;

        decreaseBetButton.interactable =
            !isSpinning && currentBet > minimumBet;
    }
}


using System.Collections;
using UnityEngine;

public class SlotReel : MonoBehaviour
{
    [Header("Symbols")]
    [SerializeField] private SpriteRenderer[] symbolRenderers;
    [SerializeField] private Sprite[] symbolSprites;

    [Header("Reel Settings")]
    [SerializeField] private float symbolHeight = 1f;
    [SerializeField] private float spinSpeed = 15f;
    [SerializeField] private float spinDuration = 3f;
    [SerializeField] private float stopDuration = 0.6f;

    private bool isSpinning;
    public bool IsSpinning => isSpinning;

    private float reelLength;

    private void Start()
    {
        SetupReel();
    }

    private void SetupReel()
    {
        reelLength = symbolRenderers.Length * symbolHeight;

        // Arrange the symbols vertically with equal spacing.
        for (int i = 0; i < symbolRenderers.Length; i++)
        {
            float yPosition = (2 - i) * symbolHeight;

            symbolRenderers[i].transform.localPosition =
                new Vector3(0f, yPosition, 0f);

            symbolRenderers[i].sprite =
                symbolSprites[Random.Range(0, symbolSprites.Length)];
        }
    }

    public void Spin(int targetSymbol)
    {
        if (isSpinning)
            return;

        StartCoroutine(SpinRoutine(targetSymbol));
    }

    private IEnumerator SpinRoutine(int targetSymbol)
    {
        isSpinning = true;

        // Phase 1: Spin at a constant speed.
        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            MoveSymbols(spinSpeed * Time.deltaTime, true);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Phase 2: Prepare the final symbol while it is off-screen.
        Transform targetTransform = GetHighestSymbol().transform;

        SpriteRenderer targetRenderer = GetHighestSymbol();
        targetRenderer.sprite = symbolSprites[targetSymbol];

        // Calculate how far the target symbol must travel to reach the center.
        float targetY = targetTransform.localPosition.y;

        float distanceToCenter = Mathf.Repeat(targetY, reelLength);

        // Add one extra revolution so the reel doesn't stop immediately.
        float totalStopDistance = distanceToCenter + reelLength;

        // Phase 3: Gradually slow down while moving the symbols.
        elapsed = 0f;
        float previousDistance = 0f;

        while (elapsed < stopDuration)
        {
            float progress = elapsed / stopDuration;

            // Ease-out curve: movement becomes slower over time.
            float easedProgress = 1f - Mathf.Pow(1f - progress, 2f);

            float currentDistance = totalStopDistance * easedProgress;
            float frameDistance = currentDistance - previousDistance;

            MoveSymbols(frameDistance, false);

            previousDistance = currentDistance;
            elapsed += Time.deltaTime;

            yield return null;
        }

        // Finish the remaining movement.
        MoveSymbols(totalStopDistance - previousDistance, false);

        // Align the target symbol exactly in the center.
        float finalOffset = targetTransform.localPosition.y;

        foreach (SpriteRenderer symbol in symbolRenderers)
        {
            Vector3 position = symbol.transform.localPosition;
            position.y -= finalOffset;
            symbol.transform.localPosition = position;
        }

        isSpinning = false;
    }

    private void MoveSymbols(float movement, bool randomizeOnRecycle)
    {
        // Move every symbol downward.
        foreach (SpriteRenderer symbol in symbolRenderers)
        {
            Vector3 position = symbol.transform.localPosition;
            position.y -= movement;
            symbol.transform.localPosition = position;
        }

        // Recycle symbols that have moved below the reel.
        foreach (SpriteRenderer symbol in symbolRenderers)
        {
            if (symbol.transform.localPosition.y < -2f * symbolHeight)
            {
                float highestPosition = GetHighestSymbol().transform.localPosition.y;

                Vector3 position = symbol.transform.localPosition;
                position.y = highestPosition + symbolHeight;
                symbol.transform.localPosition = position;

                // Only randomize symbols during the normal spinning phase.
                if (randomizeOnRecycle)
                {
                    symbol.sprite =
                        symbolSprites[Random.Range(0, symbolSprites.Length)];
                }
            }
        }
    }

    private SpriteRenderer GetHighestSymbol()
    {
        SpriteRenderer highestSymbol = symbolRenderers[0];

        foreach (SpriteRenderer symbol in symbolRenderers)
        {
            if (symbol.transform.localPosition.y >
                highestSymbol.transform.localPosition.y)
            {
                highestSymbol = symbol;
            }
        }

        return highestSymbol;
    }
}

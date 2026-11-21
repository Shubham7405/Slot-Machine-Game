
using System.Collections;
using UnityEngine;

public class SlotReel : MonoBehaviour
{
    [Header("Symbols")]
    [SerializeField] private SpriteRenderer[] symbolRenderers;
    [SerializeField] private Sprite[] symbolSprites;

    [Header("Spin Settings")]
    [SerializeField] private float symbolHeight = 1.5f;
    [SerializeField] private float spinSpeed = 15f;
    [SerializeField] private float spinDuration = 2.5f;
    [SerializeField] private float stopDuration = 0.6f;

    private bool isSpinning;

    private void Start()
    {
        SetupReel();
    }

    private void SetupReel()
    {
        // Arrange four symbols vertically around the reel center.
        for (int i = 0; i < symbolRenderers.Length; i++)
        {
            symbolRenderers[i].transform.localPosition =
                new Vector3(0f, (1 - i) * symbolHeight, 0f);

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

        float elapsed = 0f;

        // Spin continuously for the configured duration.
        while (elapsed < spinDuration)
        {
            MoveSymbols(spinSpeed);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Gradually slow down the reel.
        float stopElapsed = 0f;

        while (stopElapsed < stopDuration)
        {
            float progress = stopElapsed / stopDuration;

            // Ease-out: the reel slows down as it approaches the stop.
            float currentSpeed = Mathf.Lerp(spinSpeed, 0f, progress);

            MoveSymbols(currentSpeed);

            stopElapsed += Time.deltaTime;
            yield return null;
        }

        // Smoothly align the symbols to their final positions.
        yield return StartCoroutine(StopOnSymbol(targetSymbol));

        isSpinning = false;
    }

    private void MoveSymbols(float speed)
    {
        float movement = speed * Time.deltaTime;

        foreach (SpriteRenderer symbol in symbolRenderers)
        {
            Transform symbolTransform = symbol.transform;

            Vector3 position = symbolTransform.localPosition;
            position.y -= movement;

            // Recycle symbols that move below the reel.
            if (position.y < -2f * symbolHeight)
            {
                position.y += 4f * symbolHeight;

                // Give the recycled symbol a random sprite.
                symbol.sprite = symbolSprites[
                    Random.Range(0, symbolSprites.Length)];
            }

            symbolTransform.localPosition = position;
        }
    }

    private IEnumerator StopOnSymbol(int targetSymbol)
    {
        // Define the final symbol arrangement.
        Sprite[] finalSprites =
        {
            symbolSprites[Random.Range(0, symbolSprites.Length)],
            symbolSprites[targetSymbol],
            symbolSprites[Random.Range(0, symbolSprites.Length)],
            symbolSprites[Random.Range(0, symbolSprites.Length)]
        };

        Vector3[] targetPositions =
        {
            new Vector3(0f, symbolHeight, 0f),
            Vector3.zero,
            new Vector3(0f, -symbolHeight, 0f),
            new Vector3(0f, -2f * symbolHeight, 0f)
        };

        Vector3[] startPositions = new Vector3[symbolRenderers.Length];

        // Save the current positions before moving the symbols.
        for (int i = 0; i < symbolRenderers.Length; i++)
        {
            startPositions[i] =
                symbolRenderers[i].transform.localPosition;

            symbolRenderers[i].sprite = finalSprites[i];
        }

        float elapsed = 0f;

        while (elapsed < stopDuration)
        {
            float progress = elapsed / stopDuration;

            // Smoothly move symbols into their final positions.
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            for (int i = 0; i < symbolRenderers.Length; i++)
            {
                symbolRenderers[i].transform.localPosition =
                    Vector3.Lerp(startPositions[i], targetPositions[i],
                    easedProgress);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure all symbols are perfectly aligned at the end.
        for (int i = 0; i < symbolRenderers.Length; i++)
        {
            symbolRenderers[i].transform.localPosition = targetPositions[i];
        }
    }
}

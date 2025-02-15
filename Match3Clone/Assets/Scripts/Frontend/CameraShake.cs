using UnityEngine;
using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using GameVanilla.Core;
using GameVanilla.Game.Common;
using GameVanilla.Game.Popups;
using GameVanilla.Game.UI;
using System.Threading.Tasks;
using SaveData;

using UnityEngine;

public class CanvasShake : MonoBehaviour
{
    public float shakeDuration = 0.5f; // Duration of the shake
    public float shakeMagnitude = 10.0f; // Magnitude of the shake

    private RectTransform canvasRectTransform;
    private Vector3 initialPosition;

    private void Start()
    {
        // Find and assign the RectTransform of the Canvas
        canvasRectTransform = GetComponent<RectTransform>();

        if (canvasRectTransform == null)
        {
            Debug.LogError("RectTransform component not found.");
            return;
        }

        // Save the initial position of the canvas
        initialPosition = canvasRectTransform.localPosition;
    }

    public void ShakeCanvas()
    {
        // Start the shake coroutine
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < shakeDuration)
        {
            // Generate a random offset for the shake
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            canvasRectTransform.localPosition = initialPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the canvas position after shaking
        canvasRectTransform.localPosition = initialPosition;
    }
}


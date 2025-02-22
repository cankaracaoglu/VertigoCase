using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class RotateWheel : MonoBehaviour
{
    private SceneManager sceneManager;  

    public float minSpinTime = 2f; // Minimum spin duration
    public float maxSpinTime = 4f; // Maximum spin duration
    public float deceleration = 100f; // Controls how fast the wheel slows down
    public Button spinButton; // Reference to the Spin button

    private bool isSpinning = false;
    private float currentSpeed;

    void Start()
    {
        sceneManager = FindObjectOfType<SceneManager>();// Find the SceneManager in the scene
        spinButton.onClick.AddListener(() => StartCoroutine(SpinWheel()));
    }

    IEnumerator SpinWheel()
    {
        if (isSpinning) yield break; // Prevent multiple spins at once

        isSpinning = true;
        spinButton.interactable = false; // Disable button while spinning
        float spinDuration = UnityEngine.Random.Range(minSpinTime, maxSpinTime);
        float initialSpeed = UnityEngine.Random.Range(500f, 1000f); // Starting speed of the spin
        float totalTime = 0f;

        currentSpeed = initialSpeed;

        while (totalTime < spinDuration)
        {
            transform.Rotate(0, 0, -currentSpeed * Time.deltaTime); // Rotate wheel
            currentSpeed = Mathf.Lerp(initialSpeed, 0, totalTime / spinDuration); // Gradually slow down
            totalTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the closest final position
        float finalAngle = transform.eulerAngles.z;
        finalAngle = Mathf.Round(finalAngle / 10f) * 10f; // Assuming 8 slices of 45° each
        Debug.Log("Final Angle after: " + finalAngle);
        transform.rotation = Quaternion.Euler(0, 0, finalAngle);

        sceneManager.DetectWinningReward(finalAngle);

        isSpinning = false;
        spinButton.interactable = true; // Enable the button again
    }

    

}

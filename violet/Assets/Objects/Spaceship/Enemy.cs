using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed = 4f;
    private float lifetime = 8f;
    public string poolTag;
    private Vector3 originalScale; // Store the original scale

    void Awake()
    {
        originalScale = transform.localScale; // Capture the original scale on Awake
    }

    void OnEnable()
    {
        transform.localScale = Vector3.zero; // Set scale to 0 when enabled
        StartCoroutine(ScaleUp());

        // Subscribe to the speed adjustment event
        ScoreBoard.OnSpeedAdjustment += HandleSpeedAdjustment;
    }

    void OnDisable()
    {
        // Unsubscribe from the speed adjustment event
        ScoreBoard.OnSpeedAdjustment -= HandleSpeedAdjustment;
    }

    private IEnumerator ScaleUp()
    {
        float scaleTime = 1f; // Time to scale up
        float startTime = Time.time;

        while (Time.time - startTime < scaleTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, (Time.time - startTime) / scaleTime);
            yield return null;
        }
    }

    private void HandleSpeedAdjustment(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    void Start()
    {
        StartCoroutine(MoveForward());
        StartCoroutine(ScaleDownAndDestroy());
    }

    private IEnumerator MoveForward()
    {
        float startTime = Time.time;
        while (Time.time - startTime < lifetime)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Comet"))
        {
            ReturnToPool();
        }
    }

    private IEnumerator ScaleDownAndDestroy()
    {
        yield return new WaitForSeconds(lifetime);

        // Scale down logic
        float scaleTime = 1f; // Time to scale down
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero; // Scale down to 0
        float startTime = Time.time;

        while (Time.time - startTime < scaleTime)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, (Time.time - startTime) / scaleTime);
            yield return null;
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnToPool(poolTag, gameObject);
    }
}
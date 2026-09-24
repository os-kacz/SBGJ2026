using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float defualtShakeDuration = 0.2f; // Default duration of the shake
    public float defualyShakeMagnitude = 0.1f; // Default magnitude of the shake

    public void TriggerShake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;
        while (elapsed < defualtShakeDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = transform.localPosition + Random.insideUnitSphere * defualyShakeMagnitude;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}

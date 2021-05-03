using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public float shakeDuration = 0f;
	public float shakeAmount = 0.7f;

	private Vector3 originalPos;

	public bool test;
    private void Awake()
    {
		originalPos = transform.localPosition;
	}

	void Update()
	{
		HandleDebug();

        if (shakeDuration > 0) {
			ShakeUpdate();
			return;
		}
		shakeDuration = 0f;
		transform.localPosition = originalPos;
	}

    private void HandleDebug()
    {
		if (!test)
			return;
		test = false;
		Shake();
	}

	public void Shake(float duration = 0.3f) {
		shakeDuration = duration;
	}

    private void ShakeUpdate()
	{
		transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
		shakeDuration -= Time.deltaTime;
	}
}
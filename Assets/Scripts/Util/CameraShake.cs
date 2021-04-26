using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Collections;

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
        if (test)
        {
			test = false;
			Shake();
		}
		if (shakeDuration > 0) {
			ShakeUpdate();
			return;
		}
		shakeDuration = 0f;
		transform.localPosition = originalPos;
	}

	public void Shake() {
		shakeDuration = 0.3f;
	}

    private void ShakeUpdate()
	{
		transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
		shakeDuration -= Time.deltaTime;
	}
}
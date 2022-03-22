using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodedBarrel : MonoBehaviour
{
	[Header("Customizable Options")]
	public float despawnTime = 10.0f;

	private void Start()
	{
		StartCoroutine(DestroyTimer());

	}

	private IEnumerator DestroyTimer()
	{
		yield return new WaitForSeconds(despawnTime);
		Destroy(gameObject);
	}
}

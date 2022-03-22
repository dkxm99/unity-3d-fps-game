using UnityEngine;
using System.Collections;

// ----- Low Poly FPS Pack Free Version -----
public class ExplosionScript : MonoBehaviour {

	[Header("Customizable Options")]
	//How long before the explosion prefab is destroyed
	public float despawnTime = 10.0f;
	//How long the light flash is visible
	public float lightDuration = 0.02f;
	[Header("Light")]
	public Light lightFlash;

	[Header("Audio")]
	public AudioClip[] explosionSounds;
	public AudioSource audioSource;

	[Header("Exploded Barrel")]
	[SerializeField]
	private GameObject explodedBarrelPrefab;

	private void Start () {
		//Start the coroutines
		StartCoroutine(ExplodedBarrel());
		StartCoroutine (DestroyTimer ());
		StartCoroutine (LightFlash ());

		//Get a random impact sound from the array
		audioSource.clip = explosionSounds
			[Random.Range(0, explosionSounds.Length)];
		//Play the random explosion sound
		audioSource.Play();
	}

	private IEnumerator LightFlash () {
		//Show the light
		lightFlash.GetComponent<Light>().enabled = true;
		//Wait for set amount of time
		yield return new WaitForSeconds (lightDuration);
		//Hide the light
		lightFlash.GetComponent<Light>().enabled = false;
	}

	private IEnumerator DestroyTimer () {
		//Destroy the explosion prefab after set amount of seconds
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}
	private IEnumerator ExplodedBarrel()
	{
		Instantiate(explodedBarrelPrefab, transform.position, gameObject.transform.rotation);
		yield return null;
	}
}
// ----- Low Poly FPS Pack Free Version -----
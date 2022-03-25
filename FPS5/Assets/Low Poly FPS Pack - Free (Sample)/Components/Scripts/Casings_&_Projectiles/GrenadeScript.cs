using UnityEngine;
using System.Collections;

// ----- Low Poly FPS Pack Free Version -----
public class GrenadeScript : MonoBehaviour {

	[Header("Timer")]
	//Time before the grenade explodes
	[Tooltip("Time before the grenade explodes")]
	public float grenadeTimer = 5.0f;

	[Header("Explosion Prefabs")]
	//Explosion prefab
	public Transform explosionPrefab;

	[Header("Explosion Options")]
	//Radius of the explosion
	[Tooltip("The radius of the explosion force")]
	public float radius = 25.0F;
	//Intensity of the explosion
	[Tooltip("The intensity of the explosion force")]
	public float power = 350.0F;

	[Header("Throw Force")]
	[SerializeField]
	private float throwForce = 800.0f;

	[Header("Audio")]
	public AudioSource impactSound;

	private void Start () 
	{
		//Launch the projectile forward by adding force to it at start
		GetComponent<Rigidbody>().AddForce((Camera.main.transform.forward + new Vector3(0, 0.2f, 0)) * throwForce);

		//Start the explosion timer
		StartCoroutine (ExplosionTimer ());
	}

	private void OnCollisionEnter (Collision collision) 
	{
		//Play the impact sound on every collision
		impactSound.Play ();
	}

	private IEnumerator ExplosionTimer () 
	{
		//Wait set amount of time
		yield return new WaitForSeconds(grenadeTimer);

		//Raycast downwards to check ground
		RaycastHit checkGround;
		if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
		{
			//Instantiate metal explosion prefab on ground
			Instantiate (explosionPrefab, checkGround.point, 
				Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}

		//Explosion force
		Vector3 explosionPos = transform.position;
		//Use overlapshere to check for nearby colliders
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			Rigidbody rb = hit.GetComponent<Rigidbody> ();

			//Add force to nearby rigidbodies
			if (rb != null)
				rb.AddExplosionForce (power * 5, explosionPos, radius, 3.0F);

			//If the explosion hits "Target" tag and isHit is false
			/*if (hit.GetComponent<Collider>().tag == "Target" 
			    	&& hit.gameObject.GetComponent<TargetScript>().isHit == false) 
			{
				//Animate the target 
				hit.gameObject.GetComponent<Animation> ().Play("target_down");
				//Toggle "isHit" on target object
				hit.gameObject.GetComponent<TargetScript>().isHit = true;
			}*/

			if (hit.GetComponent<Collider>().tag == "Player")
			{
				hit.transform.GetComponent<PlayerController>().TakeDamage(30);
			}

			if (hit.GetComponent<Collider>().tag == "Enemy")
            {
				hit.transform.GetComponent<EnemyFSM>().TakeDamage(100);
			}

			//If the explosion hits "ExplosiveBarrel" tag
			if (hit.GetComponent<Collider>().tag == "ExplosiveObject") 
			{
				//Toggle "explode" on explosive barrel object
				hit.transform.GetComponent<ExplosiveObject>().TakeDamage(100);
			}
		}

		//Destroy the grenade object on explosion
		Destroy (gameObject);
	}
}
// ----- Low Poly FPS Pack Free Version -----
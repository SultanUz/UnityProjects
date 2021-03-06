using UnityEngine;
using System.Collections;

public class PatrollingEnemyAI : MonoBehaviour
{
		public float timer;

		public AudioClip hitsound;
		public AudioClip deadsound;
	public float attackDist = 5;
	public float FireDistance = 5;
	public Transform targetObject;
	public Texture blood;
	public float distance = 0.0f;
	public bool bloodimage = false;

	public Transform player;
	public AnimationClip FireAnim;
	public AnimationClip ReloadStayingAnim;
	public AnimationClip moveAnim;
	public AnimationClip attackAnim;
	public AnimationClip idleAnim;
	
	public AudioClip zombieFootStep;
	public AudioClip zombieAttack1;
	public AudioClip zombieAttack2;
	public int damage = 10;
	public AudioClip playerHit1;
	public AudioClip playerHit2;
	RaycastHit hit;
	public float health;
	public float damageP = 1;
	public Transform model;
	private Transform _me;
	//this the character
	public Transform zombieForDist;
	public Transform playerForDist;
	// for distance calculation
	
	private float tempspeed;
	public int zombieActivationDistance = 30;
	public float localSpeed = 5.0f;
	// 9 for 'Run' animation : 2 for 'Walk' , 'Crawl'  : 7 for 'zombierun' : 1.7f for 'Shamble'
	private bool isZombieHit = false;
	private bool isZombieRun = false;
	public static bool isZombieHitPlayer = false;
	public bool isDrawBloodTexture = true;
	public static bool isEnemyFire = false;
	private bool isEnemyFireComplete = false;

	private float time = 0.0f;
	private float alpha;
	//public Texture blood;
	
	void OnGUI ()
	{
		if (bloodimage == true) {
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), blood);
		}
	}

	void  Start ()
	{
				timer=0;
		player = GameObject.Find ("Player").transform;
		tempspeed = localSpeed;
		if (!_me)
			_me = transform;//cache for better performance

		model.GetComponent<Animation> () [moveAnim.name].wrapMode = WrapMode.Loop;
		model.GetComponent<Animation> () [attackAnim.name].wrapMode = WrapMode.Loop;
		model.GetComponent<Animation> () [idleAnim.name].wrapMode = WrapMode.Once;
	}

	void  Update ()
	{



				if(AS_Bullet.deadbool==true && !Timer.soundbnd)
				{
						this.gameObject.GetComponent<AudioSource>().PlayOneShot(deadsound);
						AS_Bullet.deadbool=false;
				}
				timer++;
		Vector3 fwd2 = transform.TransformDirection (Vector3.forward);
		if (targetObject != null) {
			distance = Vector3.Distance (transform.position, this.targetObject.position);
			if (distance > 2.0f) {
				bloodimage = false;
			}
		}
		if (Vector3.Distance (playerForDist.position, zombieForDist.position) <= zombieActivationDistance) {
			//minibulet();
			//animation.Play (FireAnim.name);
			//playerhealth.health -= damage * Time.deltaTime;
			if (Vector3.Distance (_me.position, player.position) <= FireDistance && !isEnemyFire && !isEnemyFireComplete) {
				isEnemyFire = true;
				model.GetComponent<Animation> ().Stop (moveAnim.name);
				//model.animation[FireAnim.name].layer = 7;
				GetComponent<Animation> ().Play (FireAnim.name);
				PlayerHealth.health -= damageP * Time.deltaTime;
				//	bloodimage= !bloodimage;
				//Invoke ("FireAnimStop",2);
				GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
				GetComponent<Rigidbody> ().useGravity = false;
				isEnemyFireComplete = true;
				//}

//			//minibulet();
//			//animation.Play (FireAnim.name);
//			//playerhealth.health -= damage * Time.deltaTime;
//			if (Vector3.Distance (_me.position, player.position) <= FireDistance && !isEnemyFire && !isEnemyFireComplete) {
//				isEnemyFire = true;
//				model.GetComponent<Animation> ().Stop (moveAnim.name);
//				//model.animation[FireAnim.name].layer = 7;
//				GetComponent<Animation> ().Play (FireAnim.name);
//				PlayerHealth.health -= damageP * Time.deltaTime;
//				//	bloodimage= !bloodimage;
//				//Invoke ("FireAnimStop",2);
//				GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
//				GetComponent<Rigidbody> ().useGravity = false;
//				isEnemyFireComplete = true;
			} else if (Vector3.Distance (_me.position, player.position) > attackDist && !isEnemyFire) {
				
				this.GetComponent<Rigidbody> ().useGravity = true;
				//if (!(model.transform.animation.IsPlaying("hit1") || model.transform.animation.IsPlaying("fallBack") || model.transform.animation.IsPlaying("fallToFace"))) { 
				model.GetComponent<Animation> ().Stop (attackAnim.name);
			
				model.GetComponent<Animation> ().Stop (FireAnim.name);
				localSpeed = tempspeed;
				GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
				isZombieHitPlayer = false;
				_me.rotation = Quaternion.Slerp (_me.rotation, Quaternion.LookRotation (player.position - _me.position), Time.deltaTime * 9);
				_me.eulerAngles = new Vector3 (0, _me.eulerAngles.y, 0);
				Vector3 fwd = _me.TransformDirection (0, 0, localSpeed);
				GetComponent<Rigidbody> ().MovePosition (new Vector3 (GetComponent<Rigidbody> ().position.x, GetComponent<Rigidbody> ().position.y, GetComponent<Rigidbody> ().position.z) + fwd * Time.deltaTime);
				model.GetComponent<Animation> ().Play (moveAnim.name);
					


				
			} else if (Vector3.Distance (_me.position, player.position) <= attackDist) {
				localSpeed = 0;
		
				//model.animation.Play(FireAnim.name);
				Attack ();
				
				GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
			} else {
				//	model.animation[attackAnim.name].layer = -1;///add this if not it will not play move anim
				localSpeed = tempspeed;
			}
		} else {

			GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;

			
		}






	}
	//update

	
	//	private float damage = 0.5f;//0.5f;
	
	void Attack ()
	{
				if(Application.loadedLevel==11 || Application.loadedLevel==12 || Application.loadedLevel==13) 
				{
						if(timer>=70 && !Timer.soundbnd){
						this.gameObject.GetComponent<AudioSource>().PlayOneShot(hitsound);
								timer=0;
						}
						PlayerHealth.health -= damageP * Time.deltaTime*1.2f;
						}
				else
						
				{
						if(timer>=70  && !Timer.soundbnd){	
						this.gameObject.GetComponent<AudioSource>().PlayOneShot(hitsound);
								timer=0;
						}
						PlayerHealth.health -= damageP * Time.deltaTime*.5f;
				}
		//PatrollingEnemyAI.bloodimage = true;
		bloodimage = true;
		_me.rotation = Quaternion.Slerp (_me.rotation, Quaternion.LookRotation (player.position - _me.position), Time.deltaTime * 9);
		_me.eulerAngles = new Vector3 (0, _me.eulerAngles.y, 0);
		Vector3 fwd = _me.TransformDirection (0, 0, localSpeed);
		GetComponent<Rigidbody> ().MovePosition (new Vector3 (GetComponent<Rigidbody> ().position.x, GetComponent<Rigidbody> ().position.y, GetComponent<Rigidbody> ().position.z) + fwd * Time.deltaTime);
		isZombieRun = false;
		model.GetComponent<Animation> ().Play (FireAnim.name);
	}

	void SoliderHitPlayer ()
	{
		PlayAudioClip (zombieAttack1, transform.position, 0.7f);
	}

	void FireAnimStop ()
	{
		isEnemyFire = false;
	}

	void PlayAudioClip (AudioClip clip, Vector3 position, float volume)
	{
		GameObject go = new GameObject ("One shot audio");
		go.transform.position = position;
		AudioSource source = go.AddComponent <AudioSource> ();
		source.clip = clip;
		source.volume = volume;
		source.minDistance = 20;
		source.Play ();
		GetComponent<AudioSource> ().PlayOneShot (clip, 2);
		Destroy (go, clip.length);
		isZombieHit = false;
	}

}
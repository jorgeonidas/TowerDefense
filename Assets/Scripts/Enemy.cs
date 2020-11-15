using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	private int target = 0; //indice de el arreglo de checkpoints
	[SerializeField] private Transform exitPoint;
	[SerializeField] private Transform[] waypoints;
	[SerializeField] private float navigationUpdate;
	[SerializeField] private int healthPoints;
	[SerializeField] private int rewardAmt;
	
	private Transform enemy;
	private Collider2D enemyCollider;
	private Animator enemyAnim; 
	private float navigationTime = 0;
	public float speedMultiplier = 0.5f;
	private bool isDead = false;
	// Use this for initialization
	void Start () {
		enemy = GetComponent<Transform>();
		GameManager.Instance.RegisterEnemy(this);
		enemyCollider = GetComponent<Collider2D>();
		enemyAnim = GetComponent<Animator>();	
	}
	
	// Update is called once per frame
	void Update () {
		if(waypoints != null && !isDead ){
			navigationTime += Time.deltaTime;
			if(navigationTime > navigationUpdate){
				if(target < waypoints.Length){ //recorriendo los waypoints					
					enemy.position = Vector2.MoveTowards(enemy.position, waypoints[target].position, navigationTime*speedMultiplier);
				}else{//si ya no hay mas waypoints se va al punto de salida					
					enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime*speedMultiplier);
				}
				navigationTime = 0;
			}
		}
	}
	//cada checkpoint que colisiona aumenta el index del array de checkpoints para pasar al siguiente
	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "checkpoint"){
			target += 1;
			//Debug.Log("target = " + target);
		}else if (other.tag == "Finish"){
			GameManager.Instance.RoundEscaped += 1;
			GameManager.Instance.TotalEscaped +=1;
			GameManager.Instance.UnregisterEnemy(this);
			GameManager.Instance.isWaveOver();
		}else if (other.tag == "projectile"){
			Projectile newProj = other.gameObject.GetComponent<Projectile>();
			EnemyHit(newProj.AtackStrength);
			Destroy(other.gameObject);
			
		}
	}

	public void EnemyHit(int hitPoints){
		if(healthPoints - hitPoints > 0 ){
			healthPoints -= hitPoints;
			enemyAnim.Play("Hurt");
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
		}else{
			enemyAnim.SetTrigger("didDie");
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
			Die();
		}
	}

	public void Die(){
		isDead = true;
		enemyCollider.enabled = false;
		GameManager.Instance.Totalkilled += 1;
		GameManager.Instance.AddMoney(rewardAmt);
		GameManager.Instance.isWaveOver();
	}

	public bool IsDead{
		get{
			return isDead;
		}
	}

}

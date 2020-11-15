using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

	[SerializeField] private float timeBetweenAtacks;
	[SerializeField] private float atackRadius;
	[SerializeField] private Projectile projectile;
	private Enemy targetEnemy = null;
	private float atackCounter;
	private bool isAtacking = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		atackCounter -= Time.deltaTime;
		//si no tengo enemigos para atacar busco al mas cercano
		if(targetEnemy == null || targetEnemy.IsDead){
			Enemy nearesEnemy = GetNearestEnemyInRange();
			if(nearesEnemy != null && Vector2.Distance(transform.localPosition, nearesEnemy.transform.localPosition) <= atackRadius){
				targetEnemy = nearesEnemy;
			}
		}else{
			//si el delay entre ataques llega a 0 ataco
			if(atackCounter <= 0 ){
				isAtacking = true;
				//reset atackcounter
				atackCounter = timeBetweenAtacks;
			}else{//si es mayor no ataco
				isAtacking = false;
			}//si el enemigo objetivo actual sale del rango de la torre no lo va a seguir atacando
			if(Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > atackRadius){
				targetEnemy = null;
			}
		}

	
	}

	void FixedUpdate(){
		if(isAtacking)
			Atack();
	}

	public void Atack(){
		isAtacking = false;
		Projectile newProjectile = Instantiate(projectile) as Projectile;
		newProjectile.transform.localPosition = transform.localPosition;
		//escoger sonido dependiendo del projectil
		if(newProjectile.ProjectileType == ProjectileType.fireball){
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Fireball);
		}else if(newProjectile.ProjectileType == ProjectileType.rock){
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Rock);
		}else if(newProjectile.ProjectileType == ProjectileType.arrow){
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Arrow);
		}
				
		if(targetEnemy == null){
			Destroy(newProjectile.gameObject);
		}else {
			//move projectile to enemy
			StartCoroutine(MoveProjectile(newProjectile));
		}


	}

	IEnumerator MoveProjectile(Projectile pro){
		while (GetTargetDistance(targetEnemy) > 0.20f && pro != null && targetEnemy != null)
		{
			//calcular direccion y angulo 
			var dir = targetEnemy.transform.localPosition - transform.localPosition;
			var angleDirection = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
			//cambio la rotacion y muevo el projectil en direccion al enemigo objetivo actual
			pro.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
			pro.transform.localPosition = Vector2.MoveTowards(pro.transform.localPosition, targetEnemy.transform.localPosition, 5f*Time.deltaTime);
			yield return null;
		}
		//puede que no se logro instanciar el projectil o el enemigo ya este fuera del alcance o muerto
		if(pro != null || targetEnemy == null){
			Destroy(pro.gameObject);
		}
	}

	private float GetTargetDistance(Enemy enemy){
		if(enemy == null){
			enemy = GetNearestEnemyInRange();
			if(enemy != null){
				return 0;
			}
		}
		return Mathf.Abs(Vector2.Distance(transform.localPosition, enemy.transform.localPosition));
	}

	private List<Enemy> GetEnemiesInRange(){
		List<Enemy> enemiesInRange = new List<Enemy>();
		foreach(Enemy enemy in GameManager.Instance.EnemyList){
			if(Vector2.Distance(transform.localPosition,enemy.transform.localPosition) <= atackRadius){
				enemiesInRange.Add(enemy);
			}
		}
		return enemiesInRange;
	}

	private Enemy GetNearestEnemyInRange(){
		Enemy nearestEnemy = null;
		float smallestDistance = float.PositiveInfinity;

		foreach(Enemy enemy in GetEnemiesInRange()){
			if(Vector2.Distance(transform.localPosition,enemy.transform.localPosition) < smallestDistance ){
				smallestDistance = Vector2.Distance(transform.localPosition,enemy.transform.localPosition);
				nearestEnemy = enemy;
			}
		}

		return nearestEnemy;
	}
	
}

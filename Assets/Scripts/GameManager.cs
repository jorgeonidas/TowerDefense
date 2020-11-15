using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum GameStatus{
	next, play, gameover, win
}

//heredando de la clase singleton
public class GameManager : Singleton<GameManager> {
	
	[SerializeField] private int totalWaves = 10;
	[SerializeField] private Text totalMoneyLbl;
	[SerializeField] private Text currentWaveLbl;
	[SerializeField] private Text totalScapedLbl;
	[SerializeField] private GameObject spawnPoint;
	[SerializeField] private Enemy[] enemies;
	[SerializeField] private int totalEnemies = 3;
	[SerializeField] private int enemiesPerSpawn;
	[SerializeField] private int enemiesEscapedLimit;
	const float spawnDelay = 0.5f;
	public List<Enemy> EnemyList = new List<Enemy>();

	[SerializeField] private Text playBtnLbl;
	[SerializeField] private Button playBtn;
	[SerializeField] private Image winLoseBanner;
	[SerializeField] private Text winLoseLbl; 
	
	private int waveNumber = 0;
	private int totalMoney = 10;
	private int totalEscaped = 0;
	private int roundEscaped = 0;
	private int totalKilled = 0;
	private int wichEnemiesToSpawn = 0;
	private int enemiesToSpawn = 0;
	private GameStatus currentState = GameStatus.play;
	private AudioSource audioSource;

	//getter y setter
	public int TotalEscaped{
		get{
			return totalEscaped;
		}

		set{
			totalEscaped = value;
		}
	}

	public int RoundEscaped{
		get{
			return roundEscaped;
		}

		set{
			roundEscaped = value;
		}
	}

	public int Totalkilled{
		get{
			return totalKilled;
		}

		set{
			totalKilled = value;
		}
	}

	public int TotalMoney{
		get{
			return totalMoney;
		}

		set{
			totalMoney = value;
			totalMoneyLbl.text = totalMoney.ToString();
		}
	}

	public AudioSource AudioSource{
		get{
			return audioSource;
		}
	}

	public int EnemiesEscapedLimit{
		get{
			return enemiesEscapedLimit;
		}
	}

	// Use this for initialization
	void Start () {
		playBtn.gameObject.SetActive(false);
		audioSource = GetComponent<AudioSource>();
		winLoseBanner.gameObject.SetActive(false);
		setTotalEscapedLbl(TotalEscaped,enemiesEscapedLimit);
		showMenu();
	}

	void Update(){
		handleEscape();
	}

    IEnumerator spawn(){
		if(enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies){
			for(int i = 0; i< enemiesPerSpawn; i++){
				if(EnemyList.Count < totalEnemies){
					Enemy newEnemie = Instantiate(enemies[UnityEngine.Random.Range(0,enemiesToSpawn)]) as Enemy;
					newEnemie.transform.position = spawnPoint.transform.position;
				}

			}
			yield return new WaitForSeconds(spawnDelay);
			StartCoroutine(spawn());
		}
	}
	//agrega enemigo a la lista de enemigos
	public void RegisterEnemy(Enemy enemy){
		EnemyList.Add(enemy);
	}

	public void UnregisterEnemy(Enemy enemy){
		EnemyList.Remove(enemy);
		Destroy(enemy.gameObject);
	}

	public void DestroyAllEnemies(){
		foreach (Enemy enemy in EnemyList)
		{
			Destroy(enemy.gameObject);
		}
		EnemyList.Clear();
	}

	//gui functions
	public void AddMoney(int ammount){
		TotalMoney += ammount;
	}

	public void substractMoney(int ammount){
		TotalMoney -= ammount;
	}

	//chequea el estado actual del juego se llamara desde Enemy cada vez que alguno escape o muera
	public void isWaveOver(){
		setTotalEscapedLbl(TotalEscaped,enemiesEscapedLimit);
		if((RoundEscaped + Totalkilled) == totalEnemies){
			if(waveNumber <= enemies.Length){
				enemiesToSpawn = waveNumber;
			}
			setCurrentGameState();
			showMenu();
		}
	}

	//Asigna el estado actual del juego
	public void setCurrentGameState(){
		if(TotalEscaped >= EnemiesEscapedLimit){
			currentState = GameStatus.gameover;
		}else if(waveNumber == 0 && (Totalkilled + RoundEscaped ) == 0) {
			currentState = GameStatus.play;
		}else if(waveNumber >= totalWaves){
			currentState = GameStatus.win;
		}else{
			currentState = GameStatus.next;
		}
	}
	//Muestra el menu y cambia el texto dependiendo del estado actual del juego
    private void showMenu()
    {
        switch (currentState)
		{
			case GameStatus.gameover:				
				playBtnLbl.text = "Play Again?!";
				AudioSource.PlayOneShot(SoundManager.Instance.GameOver);
				setLoseBanner();
				break;
			case GameStatus.next:
				playBtnLbl.text = "Next Wave";
				break;
			case GameStatus.play:
				playBtnLbl.text = "Play!";
				break;
			case GameStatus.win:
				playBtnLbl.text = "Play!";
				setWinBanner();
				break;
		}
		playBtn.gameObject.SetActive(true);
    }

	//boton para iniciar la partida
	public void playButtonPressed(){
		//Debug.Log("You push play");
		switch (currentState)
		{
			//siguiente oleada
			case GameStatus.next:
				waveNumber += 1;
				//le sumo mas enemigos a la oleada igual a el numero actual de la oleada
				totalEnemies += waveNumber;
			break;
			//moriste o apenas iniciaste el juego
			//reinicia los valores
			default:
				TowerManager.Instance.DestroyAllTowers();
				TowerManager.Instance.renameTagsBuildSites();
				totalEnemies = 3;
				TotalEscaped = 0;
				TotalMoney = 10;
				waveNumber = 0;
				enemiesToSpawn = 0;
				totalMoneyLbl.text = TotalMoney.ToString();
				setTotalEscapedLbl(TotalEscaped,enemiesEscapedLimit);
				//totalScapedLbl.text = "Escaped: "+TotalEscaped+"/"+enemiesEscapedLimit;
				AudioSource.PlayOneShot(SoundManager.Instance.NewGame);
				winLoseBanner.gameObject.SetActive(false);
			break;			
		}
		//resetea el tablero, asigna el numero de la oleada actual
		DestroyAllEnemies();
		Totalkilled = 0;
		RoundEscaped = 0;
		currentWaveLbl.text = "Wave "+ (waveNumber +1);
		//spawnear enemigos
		StartCoroutine(spawn());
		playBtn.gameObject.SetActive(false);
	}
	//cancela la seleccion de una torre presionando la tecla "Esc"
	private void handleEscape(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			TowerManager.Instance.disableDragSprite();
			TowerManager.Instance.towerButtonPressed = null;
		}
	}

	public void setWinBanner(){
		winLoseBanner.gameObject.SetActive(true);
		winLoseLbl.text = "YOU WIN!";
	}
	public void setLoseBanner(){
		winLoseBanner.gameObject.SetActive(true);
		winLoseLbl.text = "YOU LOSE!";
	}

	public void setTotalEscapedLbl(int escapedEnemies, int escapeLimit){
		totalScapedLbl.text = "Escaped: "+escapedEnemies+"/"+escapeLimit;
	}
}

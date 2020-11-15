using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {
	//getter y setter en una linea
	public TowerBtn towerButtonPressed {get; set;}

	private SpriteRenderer spriteRenderer;
	// Use this for initialization
	private List<Tower> Towerlist = new List<Tower>();
	private List<Collider2D> BuildList = new List<Collider2D>();
	private Collider2D buildTile;
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		buildTile = GetComponent<Collider2D>();
		spriteRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			//obtengo el punto donde el mouse clickea
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//desde 0,0 hasta donde hicimos click
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero); 
			if(hit.collider.tag == "BuildSite" && towerButtonPressed != null){
				buildTile = hit.collider;
				buildTile.tag = "buildSiteFull";	
				registerBuildSite(buildTile);
				placetower(hit);
			}			
		}
		if(spriteRenderer.enabled){
				followMouse();
		}
	}
	//registar cada buildsite en una lista
	public void registerBuildSite(Collider2D buildTag){
		BuildList.Add(buildTag);
	}
	//registar cada torre en una lista
	public void registerTower(Tower tower){
		Towerlist.Add(tower);
	}
	//renombro el tag de todos los builsites
	public void renameTagsBuildSites(){
		foreach (Collider2D buildTag in BuildList)
		{
			buildTag.tag = "BuildSite";
		}
		BuildList.Clear();
	}
	//destruir todas las torres
	public void DestroyAllTowers(){
		foreach (Tower tower in Towerlist)
		{
			Destroy(tower.gameObject);
		}
		Towerlist.Clear();
	}

    private void followMouse()
    {
       transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	   //asegurar la posicion en x y
	   transform.position = new Vector2(transform.position.x, transform.position.y);
    }

	public void enabledDragSprite(Sprite sprite){
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

	public void disableDragSprite(){
		spriteRenderer.enabled = false;
	}

	public void buyTower(int price){
		GameManager.Instance.substractMoney(price);
	}

    public void placetower(RaycastHit2D hit){
		//si no clickea un boton && tiene una torre seleccionada
		if(!EventSystem.current.IsPointerOverGameObject() && towerButtonPressed != null){
			Tower newTower = Instantiate(towerButtonPressed.TowerObject);
			newTower.transform.position = hit.transform.position;
			buyTower(towerButtonPressed.TowerPrice);
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuild);
			registerTower(newTower);
			disableDragSprite();
			towerButtonPressed = null;
		}
	}

	public void selectedTower(TowerBtn towerSelected){
		if(towerSelected.TowerPrice <= GameManager.Instance.TotalMoney){
			towerButtonPressed = towerSelected;
			enabledDragSprite(towerButtonPressed.DragSprite);
		}
	}
}

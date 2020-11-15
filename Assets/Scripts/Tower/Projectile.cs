using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Clase tipo Enumerador
public enum ProjectileType{
	rock, arrow, fireball
};

public class Projectile : MonoBehaviour {
	[SerializeField] private int atacktStrength;
	[SerializeField] private ProjectileType projectileType;

	public int AtackStrength{
		get{
			return atacktStrength;
		}
	}

	public ProjectileType ProjectileType{
		get{
			return projectileType;
		}
	}
}

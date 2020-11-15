using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//clase generica donde T tambien hereda de MonoBehaviour
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
		private static T instance;

		//solo quiero mantener una instancia de el singleton respectivo durante el juego 
		public static T Instance{
			get{
				if(instance == null){
					instance = FindObjectOfType<T>();
				}else if(instance !=FindObjectOfType<T>()){
					Destroy(FindObjectOfType<T>());
				}
				//mantiene la persistencia del objeto durante todo el juego
				DontDestroyOnLoad(FindObjectOfType<T>());

				return instance;
			}
		}
}

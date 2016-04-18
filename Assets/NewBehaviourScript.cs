using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewBehaviourScript : MonoBehaviour {

	public string Name;
	public bool BoolVariable;
//	public List TransformList;

	[System.Serializable]
	public class Car
	{
		public string model;
		public string colour;
	}

	public Car MyPublicCar;

	// the rest of your code goes here!
	// you need to DO something with all those variables!
}

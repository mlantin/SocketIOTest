﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class SocketServerConnection : MonoBehaviour {

	public GameObject maskPrefab;
	private Dictionary<string,GameObject> userList = new Dictionary<string,GameObject>();
	private SocketIOComponent socket;
	private JSONObject gearhead = new JSONObject (JSONObject.Type.OBJECT);
	private Quaternion rot = new Quaternion();
	private string deviceName;

	// Use this for initialization
	void Start () {

		deviceName = SystemInfo.deviceName;
		gearhead.keys.Add ("position");
		gearhead.keys.Add ("rotation");
		Vector3 pos = new Vector3 ();
		gearhead.list.Add(JSONTemplates.FromVector3(pos));
		gearhead.list.Add(JSONTemplates.FromQuaternion (rot)); 

		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("open", TestOpen);
		socket.On("leap-motion", TestLeap);
		socket.On ("newuser", createNewUser);
		socket.On ("userList", createUsers);
		socket.On ("disconnect-user", handleUserLeft); 
		socket.On("gearhead", updateUserPosition);
		socket.On("error", TestError);
		socket.On("close", TestClose);
	}

	// Update is called once per frame
	void Update () {
		//string updateData = System.String.Format ("{{ \"position\": [ {0}, {1}, {2} ], \"rotation\": [ {3}, {4}, {5}, {6} ]}}",
		//	                    Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z,
		//	                    Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z, Camera.main.transform.rotation.w);

		gearhead [0].SetField ("x", Camera.main.transform.position.x);
		gearhead [0].SetField ("y", Camera.main.transform.position.y);
		gearhead [0].SetField ("z", Camera.main.transform.position.z);
		gearhead [1].SetField ("x", Camera.main.transform.rotation.x);
		gearhead [1].SetField ("y", Camera.main.transform.rotation.y);
		gearhead [1].SetField ("z", Camera.main.transform.rotation.z);
		gearhead [1].SetField ("w", Camera.main.transform.rotation.w);


		//socket.Emit("gearhead", new JSONObject(updateData));
		socket.Emit("gearhead", gearhead);

	}

	public void TestOpen(SocketIOEvent e)
	{
		socket.Emit("set-username", new JSONObject("{\"username\": \"" + deviceName + "\"}"));
		socket.Emit ("get-userlist");
		Debug.Log("[SocketIO] Open received: " + deviceName);
	}

	public void handleUserLeft(SocketIOEvent e) {
		Debug.Log ("[SocketIO] User " + e.data.GetField ("username") + " left");
		GameObject usermask;
		userList.TryGetValue (e.data ["username"].str,out usermask);
		if (usermask) {
			userList.Remove (e.data ["username"].str);
			Destroy (usermask);
		}
	}

	public void TestLeap(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Leap Motion message received: " + e.name + " " + e.data);

		if (e.data == null) { return; }

		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
		);
	}

	public void createUsers(SocketIOEvent e) {
		int i = 0;
		foreach (JSONObject userid in e.data["userList"].list) {
			userList.Add (userid.str, createUser(i));
			i++;
		}
	}

	public GameObject createUser(int index) {
		Vector3 pos = new Vector3 ();
		pos.x = Mathf.Sin (Mathf.PI / 5.0f * index)*3;
		pos.z = Mathf.Cos (Mathf.PI / 5.0f * index)*3;
		pos.y = 0;
		return (GameObject) Instantiate (maskPrefab,pos,Quaternion.LookRotation(-pos));
	}

	public void createNewUser(SocketIOEvent e) {
		userList.Add (e.data ["username"].str, createUser(userList.Count));
		Debug.Log ("[Socket IO] New User Connected: " + e.data);
	}

	public void updateUserPosition(SocketIOEvent e) {
		JSONObject userrot = e.data.GetField ("data").GetField("rotation");
		rot.Set(userrot [0].f, userrot [1].f, userrot [2].f, userrot [3].f);
		GameObject userMask = userList [e.data ["username"].str];
		if (userMask) userMask.transform.rotation = rot;
		//user1.transform.rotation.Set (userrot [0].f, userrot [1].f, userrot [2].f, userrot [3].f);
	}

	public void TestError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}

	public void TestClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}
}
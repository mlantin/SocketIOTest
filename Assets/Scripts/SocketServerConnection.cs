using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class SocketServerConnection : MonoBehaviour {

	private SocketIOComponent socket;
	public GameObject maskPrefab;
	private GameObject user1 = null;
	private Quaternion rot = new Quaternion();

	// Use this for initialization
	void Start () {

//		user1 = GameObject.Find ("");
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("open", TestOpen);
		socket.On("leap-motion", TestLeap);
		socket.On ("newuser", createNewUser);
		socket.On ("disconnect-user", handleUserLeft); 
		socket.On("gearhead", updateUserPosition);
		socket.On("error", TestError);
		socket.On("close", TestClose);
	}

	// Update is called once per frame
	void Update () {
		string updateData = System.String.Format ("{{ \"position\": [ {0}, {1}, {2} ], \"rotation\": [ {3}, {4}, {5}, {6} ]}}",
			                    Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z,
			                    Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z, Camera.main.transform.rotation.w);
		
		socket.Emit("gearhead", new JSONObject(updateData));
	}

	public void TestOpen(SocketIOEvent e)
	{
		socket.Emit("set-username", new JSONObject("{\"username\": \"" + System.Environment.UserName + "\"}"));
		Debug.Log("[SocketIO] Open received: " + System.Environment.UserName);
	}

	public void handleUserLeft(SocketIOEvent e) {
		Debug.Log ("[SocketIO] User " + e.data.GetField ("id") + " left");
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

	public void createNewUser(SocketIOEvent e) {

		user1 = Instantiate (maskPrefab);
		Debug.Log ("[Socket IO] New User Connected: " + e.data);
	}

	public void updateUserPosition(SocketIOEvent e) {
		JSONObject userrot = e.data.GetField ("data").GetField("rotation");
		rot.Set(userrot [0].f, userrot [1].f, userrot [2].f, userrot [3].f);
		if (user1) user1.transform.rotation = rot;
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
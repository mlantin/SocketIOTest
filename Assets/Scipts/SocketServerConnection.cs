using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class SocketServerConnection : MonoBehaviour {

	private SocketIOComponent socket;
	private GameObject user1;

	// Use this for initialization
	void Start () {

		user1 = GameObject.Find ("user1");
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("open", TestOpen);
		socket.On("leap-motion", TestLeap);
		socket.On ("newuser", createNewUser);
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
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
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
		Debug.Log ("[Socket IO] New User Connected: " + e.data);
	}

	public void updateUserPosition(SocketIOEvent e) {
		JSONObject userrot = e.data.GetField ("data").GetField("rotation");
		Quaternion rot = new Quaternion(userrot [0].f, rot.y = userrot [1].f, rot.z = userrot [2].f, rot.z = userrot [3].f);
		user1.transform.rotation = rot;
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
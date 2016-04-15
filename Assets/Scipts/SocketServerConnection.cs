using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class SocketServerConnection : MonoBehaviour {

	private SocketIOComponent socket;

	// Use this for initialization
	void Start () {

		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("open", TestOpen);
		socket.On("leap-motion", TestLeap);
		socket.On ("new user", createNewUser);
		socket.On("gear-head", updateUserPosition);
		socket.On("error", TestError);
		socket.On("close", TestClose);
	}

	// Update is called once per frame
	void Update () {
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["position"] = Camera.main.transform.position.ToString();
		data["rotation"] = Camera.main.transform.rotation.ToString();
		socket.Emit("gear-head", new JSONObject(data));
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
		Debug.Log ("[Socket IO] Updated User Position; " + e.data);
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
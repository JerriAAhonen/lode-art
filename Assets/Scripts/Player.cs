using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private PlayerMovement movement;
	private MouseLook mouseLook;
	
	private void Awake()
	{
		movement = GetComponent<PlayerMovement>();
		movement.Init(this);

		mouseLook = GetComponentInChildren<MouseLook>();
		mouseLook.Init(this, movement);
	}
}

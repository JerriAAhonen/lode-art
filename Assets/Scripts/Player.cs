using UnityEngine;

public class Player : MonoBehaviour
{
	private PlayerMovement movement;
	private MouseLook mouseLook;
	
	private void Awake()
	{
		movement = GetComponent<PlayerMovement>();
		movement.Init();

		mouseLook = GetComponentInChildren<MouseLook>();
		mouseLook.Init(movement);
	}
}

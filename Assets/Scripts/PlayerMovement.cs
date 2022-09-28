using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private bool printDebugLogs;
	[SerializeField] private float gravity = -30;
	[SerializeField] private float playerHeight = 2f;
	[SerializeField] private float playerRadius = 0.25f;
	[SerializeField] private float boundsTolerance = 0.1f;
	[SerializeField] private float walkSpeed = 5;
	[SerializeField] private float sprintSpeed = 7;
	[SerializeField] private float jumpHeight = 1.5f;
	[SerializeField] private float jumpTimingForgiveness = 0.1f;

	private bool isInitialised;

	private InputManager inputManager;
	private Transform tm;
	public float verticalVel;
	private bool isGrounded;

	//------------------------------
	// Input

	private bool Sprinting { get; set; }
	private bool Crouching { get; set; }

	#region MonoBehaviour

	private void Awake()
	{
		tm = transform;
	}

	public void Init()
	{
		inputManager = InputManager.I;

		//------------------------------
		// Input

		inputManager.Jump += OnJump;
		inputManager.Crouch += OnCrouch;
		inputManager.Sprint += OnSprint;

		isInitialised = true;
	}

	private void FixedUpdate()
	{
		if (!isInitialised)
			return;

		CalculateVelocity();
		
		transform.Translate(velocity, Space.World);
	}

	private Vector3 velocity;
	private void CalculateVelocity()
	{
		if (verticalVel > gravity)
			verticalVel += gravity * Time.fixedDeltaTime;
		
		var horMovement = inputManager.MovementInput;
		velocity = tm.right * horMovement.x + tm.forward * horMovement.y;
		if (Sprinting)
			velocity *= sprintSpeed * Time.deltaTime;
		else
			velocity *= walkSpeed * Time.deltaTime;

		velocity += Vector3.up * verticalVel * Time.fixedDeltaTime;

		if (velocity.z > 0 && Front || velocity.z < 0 && Back)
			velocity.z = 0;
		if (velocity.x > 0 && Right || velocity.x < 0 && Left)
			velocity.x = 0;
		if (velocity.y > 0)
			velocity.y = CheckSpeed_Up(velocity.y);
		if (velocity.y < 0)
			velocity.y = CheckSpeed_Down(velocity.y);
	}

	private void OnDisable()
	{
		inputManager.Jump -= OnJump;
		inputManager.Crouch -= OnCrouch;
		inputManager.Sprint -= OnSprint;
	}

	#endregion

	#region Input

	public void Rotate(float yaw)
	{
		tm.eulerAngles += new Vector3(0, yaw, 0);
	}

	private void OnJump()
	{
		verticalVel = Mathf.Sqrt(-2f * jumpHeight * gravity);
	}

	private void OnCrouch(bool pressed, bool isToggle)
	{
		if (!isToggle)
			Crouching = pressed;
		else if (pressed)
			Crouching = !Crouching;

		/*
		if (!isToggle || pressed)
		{
			CrouchPressed = true;
			//UpdateMovementState();
			CrouchPressed = false;
		}*/
	}

	private void OnSprint(bool pressed, bool isToggle)
	{
		if (!isToggle)
			Sprinting = pressed;
		else if (pressed)
			Sprinting = !Sprinting;

		/*
		if (!isToggle || pressed)
		{
			SprintPressed = true;
			//UpdateMovementState();
			SprintPressed = false;
		}*/
	}

	#endregion

	#region Collision

	private float CheckSpeed_Down(float downSpeed)
	{
		var pos = transform.position;
		if (World.I.IsVoxelSolid(pos.x - playerRadius, pos.y + downSpeed, pos.z - playerRadius)
		    || World.I.IsVoxelSolid(pos.x + playerRadius, pos.y + downSpeed, pos.z - playerRadius)
		    || World.I.IsVoxelSolid(pos.x + playerRadius, pos.y + downSpeed, pos.z + playerRadius)
		    || World.I.IsVoxelSolid(pos.x - playerRadius, pos.y + downSpeed, pos.z + playerRadius))
		{
			isGrounded = true;
			return 0;
		}

		isGrounded = false;
		return downSpeed;
	}
	
	private float CheckSpeed_Up(float upSpeed)
	{
		var pos = transform.position;
		if (World.I.IsVoxelSolid(pos.x - playerRadius, pos.y + playerHeight + upSpeed, pos.z - playerRadius)
		    || World.I.IsVoxelSolid(pos.x + playerRadius, pos.y + playerHeight + upSpeed, pos.z - playerRadius)
		    || World.I.IsVoxelSolid(pos.x + playerRadius, pos.y + playerHeight + upSpeed, pos.z + playerRadius)
		    || World.I.IsVoxelSolid(pos.x - playerRadius, pos.y + playerHeight + upSpeed, pos.z + playerRadius))
		{
			return 0;
		}

		return upSpeed;
	}

	public bool Front
	{
		get
		{
			var pos = transform.position;
			return World.I.IsVoxelSolid(pos.x, pos.y, pos.z + playerRadius)
					|| World.I.IsVoxelSolid(pos.x, pos.y + 1, pos.z + playerRadius);
		}
	}
	
	public bool Back
	{
		get
		{
			var pos = transform.position;
			return World.I.IsVoxelSolid(pos.x, pos.y, pos.z - playerRadius)
			       || World.I.IsVoxelSolid(pos.x, pos.y + 1, pos.z - playerRadius);
		}
	}
	
	public bool Left
	{
		get
		{
			var pos = transform.position;
			return World.I.IsVoxelSolid(pos.x - playerRadius, pos.y, pos.z)
			       || World.I.IsVoxelSolid(pos.x - playerRadius, pos.y + 1, pos.z);
		}
	}
	
	public bool Right
	{
		get
		{
			var pos = transform.position;
			return World.I.IsVoxelSolid(pos.x + playerRadius, pos.y, pos.z)
			       || World.I.IsVoxelSolid(pos.x + playerRadius, pos.y + 1, pos.z);
		}
	}

	#endregion
}
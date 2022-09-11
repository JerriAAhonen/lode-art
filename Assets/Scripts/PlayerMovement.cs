using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private bool printDebugLogs;
	[SerializeField] private float gravity = -30;
	[SerializeField] private float playerHeight = 2f;
	[SerializeField] private float playerRadius = 0.25f;
	[SerializeField] private float boundsTolerance = 0.1f;
	[SerializeField] private float speed = 5;
	[SerializeField] private float sprintSpeed = 7;
	[SerializeField] private float jumpHeight = 1.5f;
	[SerializeField] private float jumpTimingForgiveness = 0.1f;

	private bool isInitialised;

	private Player player;
	private InputManager inputManager;
	private Transform tm;
	private float verticalVel;
	private bool isGrounded;

	public bool PrintDebugLogs => printDebugLogs;

	//------------------------------
	// Input

	public bool Sprinting { get; set; }
	public bool Crouching { get; set; }

	#region MonoBehaviour

	private void Awake()
	{
		tm = transform;
	}

	public void Init(Player player)
	{
		this.player = player;
		inputManager = InputManager.I;

		//------------------------------
		// Input

		inputManager.Jump += OnJump;
		inputManager.Crouch += OnCrouch;
		inputManager.Sprint += OnSprint;

		isInitialised = true;
	}

	private void Update()
	{
		if (!isInitialised)
			return;

		HorizontalMovement();

		//------------------------------
		// VERTICAL movement

		if (!isGrounded)
			verticalVel += gravity * Time.deltaTime;

		verticalVel = CheckDownSpeed(verticalVel);
		transform.Translate(Vector3.up * (verticalVel * Time.deltaTime));
	}

	private void HorizontalMovement()
	{
		var horMovement = inputManager.MovementInput;
		var horVel = tm.right * horMovement.x + tm.forward * horMovement.y;
		if (Sprinting)
			horVel *= sprintSpeed * Time.deltaTime;
		else
			horVel *= speed * Time.deltaTime;

		transform.Translate(horVel, Space.World);
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

	private float CheckDownSpeed(float downSpeed)
	{
		var pos = transform.position;
		if (World.I.IsVoxelSolid(pos.x - playerRadius, pos.y + downSpeed, pos.z - playerRadius)
		    || World.I.IsVoxelSolid(pos.x + playerRadius, pos.y + downSpeed, pos.z - playerRadius)
		    || World.I.IsVoxelSolid(pos.x + playerRadius, pos.y + downSpeed, pos.z + playerRadius)
		    || World.I.IsVoxelSolid(pos.x - playerRadius, pos.y + downSpeed, pos.z + playerRadius))
		{
			isGrounded = false;
			return 0;
		}

		isGrounded = false;
		return downSpeed;
	}
	
	private float CheckUpSpeed(float upSpeed)
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
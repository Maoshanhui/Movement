using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 2;

    //[SerializeField]
    //Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    //[SerializeField, Range(0f, 1f)]
    //float bounciness = 0.5f;

    Rigidbody body;

    Vector3 velocity, desiredVelocity;

    bool desiredJump;
    bool onGround;

    int jumpPhase;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        desiredJump |= Input.GetButtonDown("Jump");  // 只要不手动设置为false，desiredJump会一直为真
    }

    void FixedUpdate()
    {
        //velocity = body.velocity;
        UpdateState();
        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;   // 地面和空中最大加速度不同处理
        float maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        // jump
        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        body.velocity = velocity;

        onGround = false;

    }
    void Jump()
    {
        if (onGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0);
            }
            velocity.y += jumpSpeed;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        //onGround = true;
        EvaluateCollision(collision);
    }
    void OnCollisionStay(Collision collision)
    {
        //onGround = true;
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; ++i)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f;
        }
    }

    void UpdateState()
    {
        velocity = body.velocity;
        if (onGround)
        {
            jumpPhase = 0;
        }
    }
}

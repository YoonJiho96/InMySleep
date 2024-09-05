using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // �̵� �ӵ�
    public float jumpForce = 5f;  // ���� ��
    public float health = 1000f;  // �÷��̾� ü��
    public Transform groundCheck;  // �ٴ� üũ ����
    public LayerMask groundLayer;  // �ٴ����� �ν��� ���̾�
    private bool isGrounded;  // �ٴڿ� �ִ��� ����
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        // �¿� �� ���� �̵�
        float moveInputX = Input.GetAxis("Horizontal");
        float moveInputZ = Input.GetAxis("Vertical");
        Vector3 moveVelocity = new Vector3(moveInputX * moveSpeed, rb.velocity.y, moveInputZ * moveSpeed);
        rb.velocity = moveVelocity;
    }

    void Jump()
    {
        // �ٴڿ� ���� ���� ���� ����
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("�÷��̾ ����߽��ϴ�!");
        // ��� ó�� (��: ���� ���� ȭ�� ����)
    }
}


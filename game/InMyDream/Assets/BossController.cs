using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private float health = 100f;  // ���� ü��
    private float moveSpeed = 2f;  // �̵� �ӵ�
    public GameObject player;  // �÷��̾� ����
    public Transform attackPoint;  // ���� ���� ����
    private float attackRange = 10f;  // ���� ���� ����
    private int meleeDamage = 10;  // ���� ���� ������
    public GameObject projectilePrefab;  // ���Ÿ� ���� ��ü
    public Transform firePoint;  // ���Ÿ� ���� �߻� ����
    private float projectileSpeed = 10f;  // �ʱ� �߻� �ӵ�
    private float projectileCooldown = 1f;  // ���Ÿ� ���� ��Ÿ��
    private float lastProjectileTime = 0f;  // ������ ���Ÿ� ���� �ð�
    private float projectileLifetime = 5f;  // �߻�ü�� �����ֱ� (��)

    private void Update()
    {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // �����Ÿ��� ���� �ٸ� ���� ���
        if (distanceToPlayer <= attackRange)
        {
            // ���� ����
            MeleeAttack();
        }
        else
        {
            // ���Ÿ� ����
            RangedAttack();
        }
    }

    void MeleeAttack()
    {
        Debug.Log("������ ���� ������ �õ��մϴ�!");
        // ���� ���� ���� ���� (��: �÷��̾�� ������ ������)
    }

    void RangedAttack()
    {
        if (Time.time - lastProjectileTime >= projectileCooldown)
        {
            lastProjectileTime = Time.time;

            Debug.Log("������ ������ ���Ÿ� ������ �õ��մϴ�!");
            LaunchProjectile();
        }
    }

    void LaunchProjectile()
    {
        // �߻�ü ����
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // �߻� ���� ���
        //Vector3 launchDirection = CalculateLaunchDirection();
        Vector3 launchDirection = CalculateLaunchDirection(firePoint.position, player.transform.position, 30f);
        rb.velocity = launchDirection;

        // �߻�ü�� 5�� �Ŀ� ����
        Destroy(projectile, projectileLifetime);
    }

    //Vector3 CalculateLaunchDirection()
    //{
    //    Vector3 targetPosition = player.transform.position;
    //    Vector3 direction = targetPosition - firePoint.position;

    //    float horizontalDistance = new Vector3(direction.x, 0, direction.z).magnitude;
    //    float verticalDistance = direction.y;

    //    // �߻簢�� 45���� ���� (Ȥ�� ���ϴ� ������ ����)
    //    float angleInRadians = 30 * Mathf.Deg2Rad;

    //    // XZ ��鿡���� �߻� �ӵ� ���
    //    float velocityXZ = Mathf.Sqrt(Mathf.Abs(Physics.gravity.y * horizontalDistance) / Mathf.Abs((2 * Mathf.Sin(angleInRadians) * Mathf.Cos(angleInRadians))));

    //    // NaN�� �߻��� �� �ִ� ��츦 ����� �ӵ��� ������� Ȯ��
    //    if (float.IsNaN(velocityXZ))
    //    {
    //        Debug.Log("����� �ȵǰ� �־�");
    //        velocityXZ = projectileSpeed; // �⺻ �ӵ��� ��ü
    //    }

    //    // ���� ���� �ӵ� ���
    //    float velocityY = velocityXZ * Mathf.Sin(angleInRadians);

    //    // ���� �߻� ���� ���� ���
    //    Vector3 directionXZ = new Vector3(direction.x, 0, direction.z).normalized;
    //    Vector3 launchVelocity = directionXZ * velocityXZ + Vector3.up * velocityY;

    //    return launchVelocity;
    //}

    Vector3 CalculateLaunchDirection(Vector3 player, Vector3 target, float initialAngle)
    {
        float gravity = Physics.gravity.magnitude;
        float angle = initialAngle * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarPosition = new Vector3(player.x, 0, player.z);

        float distance = Vector3.Distance(planarTarget, planarPosition);
        float yOffset = player.y - target.y;

        float initialVelocity
            = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity
            = new Vector3(0f, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects
            = Vector3.Angle(Vector3.forward, planarTarget - planarPosition) * (target.x > player.x ? 1 : -1);
        Vector3 finalVelocity
            = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
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
        Debug.Log("������ ����߽��ϴ�!");
        Destroy(gameObject);  // ���� ������Ʈ ����
    }
}

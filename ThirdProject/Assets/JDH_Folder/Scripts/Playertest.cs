using UnityEngine;

public class Playertest : MonoBehaviour
{
    public float moveSpeed = 5f;          // �̵� �ӵ�
    public float rotationSpeed = 720f;    // ȸ�� �ӵ� (��/��)

    void Update()
    {
        // �Է� �ޱ� (WASD �Ǵ� ����Ű)
        float h = Input.GetAxis("Horizontal");  // A/D, ��/��
        float v = Input.GetAxis("Vertical");    // W/S, ��/��

        // �Է� ���� ����
        Vector3 direction = new Vector3(h, 0f, v).normalized;

        // ���� �Է��� ���� ���� �̵�
        if (direction.magnitude > 0.1f)
        {
            // �̵�
            transform.position += direction * moveSpeed * Time.deltaTime;

            // ȸ�� (�Է� �������� ȸ��)
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

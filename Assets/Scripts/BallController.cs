using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private const float DIRECTION = 1f;
    public float m_moveSpeed = 0.005f * DIRECTION;
    public float m_torque;
    public Rigidbody m_ballRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_ballRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteMovement();
    }

    private void ExecuteMovement()
    {
        float distanceTraveled = m_moveSpeed * Time.deltaTime;
        Vector3 translationVector = Vector3.zero;
  
        float m_turn = Input.GetAxis("Horizontal");
        m_ballRigidbody.AddTorque(transform.up * m_torque * m_turn);

        if (Input.GetKey(KeyCode.W))
        {
            translationVector += new Vector3(0, distanceTraveled, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            translationVector += new Vector3(0, -distanceTraveled, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            translationVector += new Vector3(-distanceTraveled, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            translationVector += new Vector3(distanceTraveled, 0, 0);
        }
        translationVector.Normalize();
        transform.Translate(translationVector * GetIsShiftPressed());
    }

    private float GetIsShiftPressed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return 2f;
        }
        return 1f;
    }

}

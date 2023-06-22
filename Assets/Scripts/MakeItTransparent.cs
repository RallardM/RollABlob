// Source : https://youtu.be/xMFx9HfRknU
using System.Collections.Generic;
using UnityEngine;

public class MakeItTransparent : MonoBehaviour
{
    [SerializeField] private List<InTheWay> m_currentlyInTheWay;
    [SerializeField] private List<InTheWay> m_alreadyTransparent;
    private Transform m_player;
    private Transform m_camera;

    private void Awake()
    {
        m_player = transform.parent.Find("PlayerBlob");
        m_currentlyInTheWay = new List<InTheWay>();
        m_alreadyTransparent = new List<InTheWay>();

        m_camera = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        GetAllObjectsInTheWay();

        MakeObjectSolid();
        MakeObjectsTransparent();
    }

    private void GetAllObjectsInTheWay()
    {
        m_currentlyInTheWay.Clear();

        float cameraPlayerDistance = Vector3.Magnitude(m_camera.position - m_player.position);

        Ray rayFoward1 = new Ray(m_camera.position, m_player.position - m_camera.position);
        Ray reayBackward1 = new Ray(m_player.position, m_camera.position - m_player.position);

        var hitsForward1 = Physics.RaycastAll(rayFoward1, cameraPlayerDistance);
        var hitsBackward1 = Physics.RaycastAll(reayBackward1, cameraPlayerDistance);

        foreach (var hit in hitsForward1)
        {
            if (hit.collider.gameObject.TryGetComponent(out InTheWay inTheWay))
            {
                if (!m_currentlyInTheWay.Contains(inTheWay))
                {
                    m_currentlyInTheWay.Add(inTheWay);
                }
            }
        }

        foreach (var hit in hitsBackward1)
        {
            if (hit.collider.gameObject.TryGetComponent(out InTheWay inTheWay))
            {
                if (!m_currentlyInTheWay.Contains(inTheWay))
                {
                    m_currentlyInTheWay.Add(inTheWay);
                }
            }
        }
    }


    private void MakeObjectsTransparent()
    {
        for (int i = 0; i < m_currentlyInTheWay.Count; i++)
        {
            InTheWay inTheWay = m_currentlyInTheWay[i];

            if (!m_alreadyTransparent.Contains(inTheWay))
            {
                inTheWay.ShowTransparent();
                m_alreadyTransparent.Add(inTheWay);
            } 
        }   
    }

    private void MakeObjectSolid()
    {
        for (int i = m_alreadyTransparent.Count - 1; i >= 0; i--)
        {
            InTheWay wasInTheWay = m_alreadyTransparent[i];

            if (!m_currentlyInTheWay.Contains(wasInTheWay))
            {
                wasInTheWay.ShowSolid();
                m_alreadyTransparent.Remove(wasInTheWay);
            }
        }
    }
}

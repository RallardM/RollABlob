// Source : https://www.youtube.com/watch?v=xMFx9HfRknU
using System;
using UnityEngine;

public class InTheWay : MonoBehaviour
{
    [SerializeField] private GameObject m_solidBody;
    [SerializeField] private GameObject m_transparentBody;

    private void Awake()
    {
        ShowSolid();
    }

    public void ShowTransparent()
    {
        m_solidBody.SetActive(false);
        m_transparentBody.SetActive(true);
    }

    public void ShowSolid()
    {
        m_solidBody.SetActive(true);
        m_transparentBody.SetActive(false);
    }
}

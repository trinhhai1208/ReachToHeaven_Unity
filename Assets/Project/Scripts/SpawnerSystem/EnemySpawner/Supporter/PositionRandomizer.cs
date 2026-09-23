using UnityEngine;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class PositionRandomizer
{
    //
    [SerializeField] private Transform m_target;

    [SerializeField] private Vector2 m_outerBound;
    [SerializeField] private Vector2 m_innerBound;

    private float m_rightArea, m_topArea;

    public void Init(Transform target)
    {
        if (m_target == null) m_target = target;

        Rectangle rightRectangle = new Rectangle(m_innerBound.x, m_outerBound.x, -m_innerBound.y, m_innerBound.y);
        Rectangle topRectangle = new Rectangle(-m_outerBound.x, m_outerBound.x, m_innerBound.y, m_outerBound.y);

        m_rightArea = rightRectangle.Area;
        m_topArea = topRectangle.Area;
    }

    public Vector2 RandomizePosition()
    {
        float totalArea = m_rightArea + m_topArea;
        float scaleRight = m_rightArea / totalArea;

        float res = Random.value;

        if (res <= scaleRight)
        {
            float xPos = Random.Range(m_innerBound.x, m_outerBound.x);
            float yPos = Random.Range(-m_innerBound.y, m_innerBound.y);
            Vector2 result = new Vector2(xPos, yPos);
            if (Random.value <= 0.5f) result *= -1;

            return (Vector2)m_target.transform.position + result;
        }
        else
        {
            float yPos = Random.Range(m_innerBound.y, m_outerBound.y);
            float xPos = Random.Range(-m_outerBound.x, m_outerBound.x);
            Vector2 result = new Vector2(xPos, yPos);
            if (Random.value <= .5f) result *= -1;

            return (Vector2)m_target.transform.position + result;
        }
    }
    

}

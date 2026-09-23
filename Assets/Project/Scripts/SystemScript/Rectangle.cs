using UnityEngine;

public class Rectangle
{
    //
    public float Width { get; private set; }
    public float Height { get; private set; }
    public float Area { get; private set; }

    public Rectangle(float beginWidth, float endWidth, float beginHeight, float endHeight)
    {
        Width = Mathf.Abs(endWidth - beginWidth);
        Height = Mathf.Abs(endHeight - beginHeight);

        Area = Width * Height;
    }
}

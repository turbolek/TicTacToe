using UnityEngine;

public class Skin
{
    public Sprite XIcon;
    public Sprite OIcon;
    public Sprite Background;
    public Sprite Line;

    public bool IsValid()
    {
        return XIcon != null && OIcon != null && Background != null && Line != null;
    }
}


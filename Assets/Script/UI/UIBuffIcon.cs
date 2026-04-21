using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIBuffIcon : MonoBehaviour
{

    [SerializeField] private Image img;
    public void InitIcon(Sprite icon)
    {
        img.sprite = icon;
    }
    public void StopShow()
    {
        Manager.Pool.ReleaseGameObject(this);
    }
}
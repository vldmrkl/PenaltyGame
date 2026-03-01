using UnityEngine;

public class SafeArea : MonoBehaviour
{
    void Start()
    {
        Rect safe = Screen.safeArea;

        RectTransform rt = GetComponent<RectTransform>();

        rt.anchorMin = safe.position;
        rt.anchorMax = safe.position + safe.size;
        rt.anchorMin /= new Vector2(Screen.width, Screen.height);
        rt.anchorMax /= new Vector2(Screen.width, Screen.height);
    }
}
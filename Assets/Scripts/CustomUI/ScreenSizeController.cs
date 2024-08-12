using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeController : MonoBehaviour
{
    void Start()
    {
        // Set the minimum and maximum window sizes
        Screen.SetResolution(1024, 768, false); // Example resolution
    }

    void Update()
    {
        // For custom behavior, you can monitor the screen width and height here
        int width = Screen.width;
        int height = Screen.height;

        // You can add logic to enforce minimum or maximum size here
        if (width < 800) Screen.SetResolution(800, height, false);
        if (height < 600) Screen.SetResolution(width, 600, false);
    }
}

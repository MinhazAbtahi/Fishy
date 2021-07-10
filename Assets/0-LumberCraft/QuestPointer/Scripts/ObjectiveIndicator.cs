using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveIndicator : MonoBehaviour
{
    [System.Serializable]
    public class Indicator
    {
        // Indicator icon
        public Image img;
        // The target (location, enemy, etc..)
        public Transform target;
        // UI Text to display the distance
        public TextMeshProUGUI meter;
        // To adjust the position of the icon
        public Vector3 offset;
    }

    public Indicator[] indicators;

    private void Update()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            Indicator indicator = indicators[i];
            // Giving limits to the icon so it sticks on the screen
            // Below calculations witht the assumption that the icon anchor point is in the middle
            // Minimum X position: half of the icon width
            float minX = indicator.img.GetPixelAdjustedRect().width / 2;
            // Maximum X position: screen width - half of the icon width
            float maxX = Screen.width - minX;

            // Minimum Y position: half of the height
            float minY = indicator.img.GetPixelAdjustedRect().height / 2;
            // Maximum Y position: screen height - half of the icon height
            float maxY = Screen.height - minY;

            // Temporary variable to store the converted position from 3D world point to 2D screen point
            Vector2 pos = Camera.main.WorldToScreenPoint(indicator.target.position + indicator.offset);

            // Check if the target is behind us, to only show the icon once the target is in front
            if (Vector3.Dot((indicator.target.position - transform.position), transform.forward) < 0)
            {
                // Check if the target is on the left side of the screen
                if (pos.x < Screen.width / 2)
                {
                    // Place it on the right (Since it's behind the player, it's the opposite)
                    pos.x = maxX;
                }
                else
                {
                    // Place it on the left side
                    pos.x = minX;
                }
            }

            // Limit the X and Y positions
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            // Update the marker's position
            indicator.img.transform.position = pos;
            // Change the meter text to the distance with the meter unit 'm'
            indicator.meter.text = ((int)Vector3.Distance(indicator.target.position, transform.position)).ToString() + "m";
        }
    }
}
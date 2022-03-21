using UnityEngine;

namespace Pattern.Tools
{
    public class SafeScreen
    {
        public SafeScreen() { }

        public (Vector2, Vector2) RectOffset()
        {
            Rect screenSafeArea = Screen.safeArea;
            (Vector2 min, Vector2 max) safeArea;

            safeArea.min = screenSafeArea.min;
            safeArea.max = screenSafeArea.max - new Vector2(Screen.width, Screen.height);

            return safeArea;
        }
    }
}

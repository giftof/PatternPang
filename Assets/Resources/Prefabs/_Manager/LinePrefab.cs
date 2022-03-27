using UnityEngine;

public class LinePrefab: MonoBehaviour {
    public LineRenderer line;

    private void Awake() {
        line = GetComponent<LineRenderer>();

        line.startColor = Color.grey;
        line.endColor = Color.grey;

        line.startWidth = CONST.LINE_WIDTH;
        line.endWidth = CONST.LINE_WIDTH;
    }
}

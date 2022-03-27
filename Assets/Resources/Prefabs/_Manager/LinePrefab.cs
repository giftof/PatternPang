using UnityEngine;

public class LinePrefab: MonoBehaviour {
    public LineRenderer line;

    private void Awake() {
        line = GetComponent<LineRenderer>();
    }
    
    private void Start() {
        line.startColor = line.endColor = Color.grey;
        line.startWidth = line.endWidth = CONST.LINE_WIDTH;
    }
}

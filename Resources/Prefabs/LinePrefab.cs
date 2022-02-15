using UnityEngine;
using Pattern.Configs;

public class LinePrefab : MonoBehaviour
{
    public LineRenderer line;

    private void Start()
    {
        line.SetColors(Color.grey, Color.grey);
        line.SetWidth(CONST.LINE_WIDTH, CONST.LINE_WIDTH);
    }

    public void Position((Vector3 begin, Vector3 end) unit)
    {
        line.SetPosition(0, unit.begin);
        line.SetPosition(1, unit.end);
    }
}

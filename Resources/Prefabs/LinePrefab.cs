using UnityEngine;

public class LinePrefab : MonoBehaviour
{
    [SerializeField] LineRenderer line;

    private void Start()
    {
        line.SetColors(Color.cyan, Color.cyan);
        line.SetWidth(.1f, .1f);
    }

    public void Position((Vector3 begin, Vector3 end) unit)
    {
        line.SetPosition(0, unit.begin);
        line.SetPosition(1, unit.end);
    }
}

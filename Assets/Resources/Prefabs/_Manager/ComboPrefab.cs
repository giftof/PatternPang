using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] Image image;

    private Color m_bgColor;
    private Color m_txtColor;

    private void Awake()
    {
        m_bgColor = new Color(0, 0, 0, .75f);
        m_txtColor = Vector4.one;
    }

    public void SetText(int combo)
        => textMesh.text = $"{combo} Combo";

    public Color BGColor
    {
        get => image.color;
        set => image.color = value;
    }

    public Color TextColor
    {
        get => textMesh.color;
        set => textMesh.color = value;
    }

    public void BeginColor()
    {
        image.color = m_bgColor;
        textMesh.color = m_txtColor;
    }
}

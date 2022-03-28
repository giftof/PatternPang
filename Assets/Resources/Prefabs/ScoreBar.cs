using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar: MonoBehaviour {
    public Text score;
    public Image image;

    public string Score {
        get => score.text;
        set => score.text = value;
    }

    public float FillAmount {
        get => image.fillAmount;
        set => image.fillAmount = value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image middle;
    [SerializeField] Image cover;
    private float rate;

    private void Start()
    {
        middle.fillAmount = 1f;
        cover.fillAmount = 1f;
    }

    public void Fill(float rate)
    {
        DOTween.Kill(middle.GetInstanceID());
        DOTween.Kill(cover.GetInstanceID());

        if (this.rate > rate)
        {
            float duration = Mathf.Abs((middle.fillAmount - rate) * 10);
            cover.fillAmount = rate;
            DOTween.To(() => middle.fillAmount, x => middle.fillAmount = x, rate, duration);
        }
        else
        {
            float duration = Mathf.Abs((cover.fillAmount - rate) * 10);
            DOTween.To(() => cover.fillAmount, x => cover.fillAmount = x, rate, duration);
            middle.fillAmount = rate;
        }

        this.rate = rate;
    }

}

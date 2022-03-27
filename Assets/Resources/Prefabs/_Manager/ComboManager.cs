using UnityEngine;
using DG.Tweening;

public class ComboManager:ManagedPool < ComboPrefab > {
    protected override void Awake() {
        base.Awake();
        pool.prefab = Resources.Load<GameObject>("Prefabs/_Manager/ComboPrefab");
    }

    public void Display(Transform transform, int combo) {
        ComboPrefab prefab = Request(this.transform);
        Sequence sequence = DOTween.Sequence();

        prefab.transform.position = transform.position;
        prefab .transform .Translate(Vector3.back);
        prefab.SetText(combo);

        sequence.Append(prefab.transform.DOLocalMoveY(prefab.transform.localPosition.y + CONST.COMBOBOX_FLOATING_DISTANCE, CONST.DURATION_COMBO).SetEase(Ease.Linear));
        sequence.Join(DOTween.To(() => prefab.BGColor, c => prefab.BGColor = c, Color.clear, CONST.DURATION_COMBO).SetEase(Ease.InExpo));
        sequence.Join(DOTween.To(() => prefab.TextColor, c => prefab.TextColor = c, Color.clear, CONST.DURATION_COMBO).SetEase(Ease.InExpo));
        sequence.OnComplete(() => Release(prefab));
        sequence.Play();
    }
}

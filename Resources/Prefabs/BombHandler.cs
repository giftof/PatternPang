using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Configs;

public delegate IEnumerator IE_DELEGATE_T<T>(T arg);
public delegate IEnumerator IE_DELEGATE();

public class BombHandler : MonoBehaviour
{
    [SerializeField] BallManager m_ballHandler;
    private int m_bombLineCount;

    public DELEGATE_T<SlotPrefab> d_bomb;
    public Action d_score;
    public Action d_request;

    public IEnumerator DisposeBomb1(SlotPrefab slot)
    {
        ReleaseBombed(slot);
        IncrementBombAction();
        foreach (var offset in CONST.DIRECTION_OFFSET)
        {
            SlotPrefab target = Ray.Instance.Shot(slot.transform.position + offset);
            if (target != null && target.Generate == null)
            {
                yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);
                ReleaseBombed(target, true);
            }
        }

        if (DecrementBombAction() == 0)
            d_request?.Invoke();
            // StartCoroutine(d_request());
    }

    public IEnumerator DisposeBomb2(SlotPrefab slot)
    {
        ReleaseBombed(slot);
        yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);

        StartCoroutine(RemoveLine(slot, ClockWise.up, ClockWise.down));
    }

    public IEnumerator DisposeBomb3(SlotPrefab slot)
    {
        ReleaseBombed(slot);
        yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);

        StartCoroutine(RemoveLine(slot, ClockWise.up, ClockWise.down));
        StartCoroutine(RemoveLine(slot, ClockWise.upLeft, ClockWise.downRight));
        StartCoroutine(RemoveLine(slot, ClockWise.upRight, ClockWise.downLeft));
    }

    public IEnumerator DisposeBomb4(params SlotPrefab[] slot)
    {
        yield return null;

        foreach (var e in slot)
            ReleaseBombed(e);

        List<SlotPrefab> list = new List<SlotPrefab>();

        slot.Where(e => {
            var around = CONST.DIRECTION_OFFSET
                .Select(offset => Ray.Instance.Shot(e.transform.position + offset))
                .GroupBy(a => a?.Child != null && a.Generate == null);

            foreach (var a in around)
                if (a.Key)
                    list.AddRange(a.ToList());
            return false;
        }).Count();

        if (list.Count > 0)
        {
            yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);
            StartCoroutine(DisposeBomb4(list.ToArray()));
        }
        else
            d_request?.Invoke();
            // StartCoroutine(d_request());
    }

    IEnumerator RemoveLine(SlotPrefab slot, ClockWise dir1, ClockWise dir2)
    {
        IncrementBombAction();
        Vector3 pos1 = slot.transform.position;
        Vector3 pos2 = slot.transform.position;

        while(true)
        {
            int failCount = 0;
            SlotPrefab target1 = Ray.Instance.Shot(pos1 += CONST.DIRECTION_OFFSET[(int)dir1]);
            SlotPrefab target2 = Ray.Instance.Shot(pos2 += CONST.DIRECTION_OFFSET[(int)dir2]);

            if (target1 == null || target1.Generate != null)
                ++failCount;
            else
                ReleaseBombed(target1, true);

            if (target2 == null || target2.Generate != null)
                ++failCount;
            else
                ReleaseBombed(target2, true);

            if (failCount > 1)
                break;
            yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);
        }

        if (DecrementBombAction() == 0)
            d_request?.Invoke();
            // StartCoroutine(d_request());
    }

    private void ReleaseBombed(SlotPrefab slot, bool recursive = false)
    {
        if (recursive)
            d_bomb(slot);

        if (slot.Child)
        {
            m_ballHandler.Release(slot.Child);
            slot.Child = null;
            d_score();
        }
    }

    private int IncrementBombAction(int count = 1)
        => m_bombLineCount += count;

    private int DecrementBombAction(int count = 1)
        => m_bombLineCount -= count;
}

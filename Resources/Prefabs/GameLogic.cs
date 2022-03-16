// using System;
// using System.Linq;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using Pattern.Managers;
// using Pattern.Configs;
// using DG.Tweening;

// public class GameLogic : MonoBehaviour
// {
//     [SerializeField] EventSystem m_eventSystem;
//     [SerializeField] BoardManager m_boardHandler;
//     [SerializeField] BallManager m_ballHandler;
//     [SerializeField] LineManager m_lineHandler;
//     [SerializeField] CoverManager m_coverHandler;
//     private PatternHandler m_patternHandler;
//     private int m_matchCount;
//     private int m_bombLineCount;
//     private int m_unitScore; // nameing is suck
//     private int m_modeScore; // nameing is suck

//     public int Score { get; private set; } = 0;
//     public bool Finish = false;
//     public int BonusTimeSecond;

//     private void Awake()
//     {
//         m_patternHandler = new PatternHandler();
//         m_patternHandler.InputEnd = FinishDrag;

//         m_lineHandler.SetPatternHandler = m_patternHandler;
//         m_boardHandler.SetPatternHandler = m_patternHandler;
//         m_boardHandler.SetBombAction = Bomb;
//         m_boardHandler.SetBeginAction = m_lineHandler.Begin;
//         m_boardHandler.SetAddAction = m_lineHandler.Append;
//         m_boardHandler.SetRemoveAction = m_lineHandler.Remove;
//         m_coverHandler.transform.localScale = m_boardHandler.transform.localScale;
//     }

//     public void InitGame()
//         => m_boardHandler.Create();

//     public void CreateGame()
//     {
//         Score = 0;
//         Finish = false;
//         BonusTimeSecond = CONST.BONUS_TIMER_BEGIN_VALUE;

//         m_lineHandler.Clear();
//         RequestBall(null);
//     }

//     public void ClearGame()
//     {
//         m_lineHandler.Clear();
//         m_boardHandler.Clear();
//         m_patternHandler.Clear();
//         m_coverHandler.Clear();
//         SlotPrefab.Activate = false;
//     }

//     public void ClearBall()
//     {
//         m_lineHandler.Clear();
//         m_ballHandler.DropAndClear();
//         m_boardHandler.ClearChild();
//         m_patternHandler.Clear();
//         SlotPrefab.Activate = false;
//     }

//     public void Bomb(SlotPrefab slot)
//     {
//         if (slot.Generate != null || slot.Child == null) { return; }

//         switch (slot.Child.BallColor)
//         {
//             case SlotAttribute.bomb1:
//                 StartCoroutine(DisposeBomb1(slot));
//                 break;
//             case SlotAttribute.bomb2:
//                 StartCoroutine(DisposeBomb3(slot));
//                 break;
//             case SlotAttribute.bomb3:
//                 StartCoroutine(DisposeBomb4(slot));
//                 break;
//             default:
//                 break;
//         }
//     }

//     /*
//      * privates... complicated functions...
//      */
//     private int UnitScore
//         => m_patternHandler.Selected().Select((e, index) => (e, index)).Sum(p => p.index) + m_patternHandler.Selected().Count();

//     private int ModeScore
//         => m_patternHandler.Selected().Count - (int)CONST.MIN_SELECT;

//     private void FinishDrag()
//     {
//         m_unitScore = UnitScore;
//         m_modeScore = ModeScore;
//         Vector3[] offsetArray = m_patternHandler.ShapeOffset();
//         m_matchCount = 0;

//         if (offsetArray != null)
//             StartCoroutine(DisposeMatch(offsetArray));
//         else
//             m_lineHandler.Clear();
//     }

//     private void RecursiveMatch(Vector3[] offsetArray)
//         => StartCoroutine(DisposeMatch(offsetArray));

//     private void DisposeMatchBall(IGrouping<int, SlotPrefab> key)
//     {
//         SlotPrefab slot = m_boardHandler.Data[key.Key];
//         int matchCount = key.Count();

//         switch (matchCount)
//         {
//             case 1:
//                 m_ballHandler.Release(slot.Child);
//                 slot.Child = null;
//                 break;
//             case 2:
//                 slot.Child.BallColor = SlotAttribute.bomb1;
//                 break;
//             case 3:
//                 slot.Child.BallColor = SlotAttribute.bomb2;
//                 break;
//             default:
//                 slot.Child.BallColor = SlotAttribute.bomb3;
//                 break;
//         }
//     }

//     /* complicated functions... can be simple? */
//     private bool FillSlot(SlotPrefab slot, Action action = null)
//     {
//         bool flag = false;
//         SlotPrefab current = slot;
//         Sequence sequence = DOTween.Sequence().SetEase(Ease.Linear).OnComplete(() => {
//             if (flag)
//                 FillSlot(slot);
//             else
//             {
//                 if (action == null)
//                     m_eventSystem.enabled = true;
//                 else
//                     action?.Invoke();
//             }
// /*
//             action?.Invoke();
//             m_eventSystem.enabled = true;
// */
//         });

//         m_eventSystem.enabled = false;

//         return flag = DisposeFillSlot(slot, sequence);
//     }

//     private bool DisposeFillSlot(SlotPrefab slot, Sequence sequence)
//     {
//         bool flag = false;

//         while (true)
//         {
//             SlotPrefab above = Ray.Instance.Shot(slot.transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);
            
//             if (above == null)
//             {
//                 flag = slot.Generate?.Invoke(slot) ?? false;
//                 sequence.Play();
//                 return flag;
//             }

//             if (slot.Child == null && above.Child != null)
//             {
//                 SlotPrefab temp = slot;
//                 slot.Child = above.Child;
//                 above.Child = null;
//                 sequence.Join(DOTween.To(() => temp.Child.transform.position, pos => temp.Child.transform.position = pos, temp.transform.position, CONST.DURATION_MOVE));
//             }

//             slot = above;
//         }
//     }

//     private void RequestBall(Action action)
//     {
//         if (Finish) return;

//         m_boardHandler.BottomLine.Where(e => FillSlot(e, action)).Count();
// /*
//         if (m_boardHandler.BottomLine.Where(e => FillSlot(e)).Count() == 0)
//             action?.Invoke();
// */
//     }

//     private List<SlotPrefab> Matched(Vector3[] offsetArray)
//     {
//         List<SlotPrefab> match = new List<SlotPrefab>();

//         foreach (var unit in from origin in m_boardHandler.Data
//                              let unit = DrawMatchedElement(origin.Value, offsetArray)
//                              where unit != null
//                              select unit)
//                              {
//                                 ++m_matchCount;
//                                 Score += m_unitScore * Multi(m_matchCount, m_modeScore);
//                                 m_lineHandler.ToLine(unit);
                                
//                                 match.AddRange(unit);
//                              }
        
//         return match;
//     }
    
//     private List<SlotPrefab> DrawMatchedElement(SlotPrefab origin, Vector3[] offsetArray)
//     {
//         List<SlotPrefab> unit = new List<SlotPrefab>();
//         Vector3 position = origin.transform.position;

//         if (origin.Generate != null)
//             return null;

// var h = Ray.Instance.Shot(position - offsetArray[0]);
// Debug.Log($"h = {h}");

//         unit.Add(origin);
//         foreach (var match in from offset in offsetArray
//                               let hit = Ray.Instance.Shot(position -= offset)
//                               where 
//                                     hit != null
//                                     && hit.Generate == null 
//                                     && !hit.Child.IsBomb() 
//                                     && hit.Child.BallColor.Equals(origin.Child.BallColor)
//                               select hit)
//             unit.Add(match);

//         if (unit.Count > offsetArray.Length)
//             return unit;

//         return null;
//     }

//     IEnumerator DisposeMatch(Vector3[] offsetArray)
//     {
//         m_eventSystem.enabled = false;

//         List<SlotPrefab> matchedList = Matched(offsetArray);

//         var group = matchedList.GroupBy(e => e.GetInstanceID());

//         if (group.Count().Equals(0))
//         {
//             if (!Finish)
//                 m_eventSystem.enabled = true;
//         }
//         else
//         {
//             yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_REMOVE);

//             m_lineHandler.Clear();
//             foreach (var key in group)
//                 DisposeMatchBall(key);

//             yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_FILL_BALL);
//             RequestBall(() => { RecursiveMatch(offsetArray); });
//         }
//     }

//     private int Multi(int count, int mode)
//     {
//         int rst = count * (int)Math.Pow(count * 2, mode);
        
//         if (1 < count)
//             BonusTimeSecond += mode;
//         return rst;
//     }


//     IEnumerator DisposeBomb1(SlotPrefab slot)
//     {
//         m_eventSystem.enabled = false; /**/

//         ReleaseBombed(slot);
//         IncrementBombAction();
//         foreach (var offset in CONST.DIRECTION_OFFSET)
//         {
//             SlotPrefab target = Ray.Instance.Shot(slot.transform.position + offset);
//             if (target != null && target.Generate == null)
//             {
//                 yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);
//                 ReleaseBombed(target, true);
//             }
//         }

//         if (DecrementBombAction() == 0)
//             RequestBall(null);
//     }

//     IEnumerator DisposeBomb2(SlotPrefab slot)
//     {
//         m_eventSystem.enabled = false; /**/

//         ReleaseBombed(slot);
//         yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);

//         StartCoroutine(RemoveLine(slot, ClockWise.up, ClockWise.down));
//     }

//     IEnumerator DisposeBomb3(SlotPrefab slot)
//     {
//         m_eventSystem.enabled = false; /**/

//         ReleaseBombed(slot);
//         yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);

//         StartCoroutine(RemoveLine(slot, ClockWise.up, ClockWise.down));
//         StartCoroutine(RemoveLine(slot, ClockWise.upLeft, ClockWise.downRight));
//         StartCoroutine(RemoveLine(slot, ClockWise.upRight, ClockWise.downLeft));
//     }

//     IEnumerator DisposeBomb4(params SlotPrefab[] slot)
//     {
//         yield return null;
//         m_eventSystem.enabled = false; /**/

//         foreach (var e in slot)
//             ReleaseBombed(e);

//         List<SlotPrefab> list = new List<SlotPrefab>();

//         slot.Where(e => {
//             var around = CONST.DIRECTION_OFFSET
//                 .Select(offset => Ray.Instance.Shot(e.transform.position + offset))
//                 .GroupBy(a => a?.Child != null && a.Generate == null);

//             foreach (var a in around)
//                 if (a.Key)
//                     list.AddRange(a.ToList());
//             return false;
//         }).Count();

//         if (list.Count > 0)
//         {
//             yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);
//             StartCoroutine(DisposeBomb4(list.ToArray()));
//         }
//         else
//             RequestBall(null);
//     }

//     IEnumerator RemoveLine(SlotPrefab slot, ClockWise dir1, ClockWise dir2)
//     {
//         IncrementBombAction();
//         Vector3 pos1 = slot.transform.position;
//         Vector3 pos2 = slot.transform.position;

//         while(true)
//         {
//             int failCount = 0;
//             SlotPrefab target1 = Ray.Instance.Shot(pos1 += CONST.DIRECTION_OFFSET[(int)dir1]);
//             SlotPrefab target2 = Ray.Instance.Shot(pos2 += CONST.DIRECTION_OFFSET[(int)dir2]);

//             if (target1 == null || target1.Generate != null)
//                 ++failCount;
//             else
//                 ReleaseBombed(target1, true);

//             if (target2 == null || target2.Generate != null)
//                 ++failCount;
//             else
//                 ReleaseBombed(target2, true);

//             if (failCount > 1)
//                 break;
//             yield return new WaitForSecondsRealtime(CONST.DURATION_BOMB_STEP);
//         }

//         if (DecrementBombAction() == 0)
//             RequestBall(null);
//     }

//     private void ReleaseBombed(SlotPrefab slot, bool recursive = false)
//     {
//         if (recursive)
//             Bomb(slot);

//         if (slot.Child)
//         {
//             m_ballHandler.Release(slot.Child);
//             slot.Child = null;
//             Score += CONST.SCORE_BOMB;
//         }
//     }

//     private int IncrementBombAction(int count = 1)
//         => m_bombLineCount += count;

//     private int DecrementBombAction(int count = 1)
//         => m_bombLineCount -= count;
// }























using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pattern.Managers;
using Pattern.Configs;

public class GameLogic : MonoBehaviour
{
    [SerializeField] EventSystem m_eventSystem;
    [SerializeField] BoardManager m_boardHandler;
    [SerializeField] BallManager m_ballHandler;
    [SerializeField] LineManager m_lineHandler;
    [SerializeField] CoverManager m_coverHandler;
    [SerializeField] BombHandler m_bombHandler;
    private PatternHandler m_patternHandler;
    private int m_matchCount;
    private int m_unitScore; // nameing is suck
    private int m_modeScore; // nameing is suck

    public int Score { get; private set; } = 0;
    public bool Finish = false;
    public int BonusTimeSecond;

    private void Awake()
    {
        m_patternHandler = new PatternHandler();
        m_patternHandler.InputEnd = FinishDrag;

        m_lineHandler.SetPatternHandler = m_patternHandler;
        m_boardHandler.SetPatternHandler = m_patternHandler;
        m_boardHandler.SetBombAction = Bomb;
        m_boardHandler.SetBeginAction = m_lineHandler.Begin;
        m_boardHandler.SetAddAction = m_lineHandler.Append;
        m_boardHandler.SetRemoveAction = m_lineHandler.Remove;
        m_coverHandler.transform.localScale = m_boardHandler.transform.localScale;

        m_bombHandler.d_bomb = Bomb;
        m_bombHandler.d_score = BombScore;
        m_bombHandler.d_request = RequestBall;
    }

    public void InitGame()
        => m_boardHandler.Create();

    public void CreateGame()
    {
        Score = 0;
        Finish = false;
        BonusTimeSecond = CONST.BONUS_TIMER_BEGIN_VALUE;

        m_lineHandler.Clear();
        StartCoroutine(RequestBall(null));
    }

    public void ClearGame()
    {
        m_lineHandler.Clear();
        m_boardHandler.Clear();
        m_patternHandler.Clear();
        m_coverHandler.Clear();
        SlotPrefab.Activate = false;
    }

    public void ClearBall()
    {
        m_lineHandler.Clear();
        m_ballHandler.DropAndClear();
        m_boardHandler.ClearChild();
        m_patternHandler.Clear();
        SlotPrefab.Activate = false;
    }

    /*
     * privates... complicated functions...
     */
    private void Bomb(SlotPrefab slot)
    {
        if (slot.Generate != null || slot.Child == null) { return; }

        switch (slot.Child.BallColor)
        {
            case SlotAttribute.bomb1:
                m_eventSystem.enabled = false;
                StartCoroutine(m_bombHandler.DisposeBomb1(slot));
                break;
            case SlotAttribute.bomb2:
                m_eventSystem.enabled = false;
                StartCoroutine(m_bombHandler.DisposeBomb3(slot));
                break;
            case SlotAttribute.bomb3:
                m_eventSystem.enabled = false;
                StartCoroutine(m_bombHandler.DisposeBomb4(slot));
                break;
            default:
                break;
        }
    }

    private void BombScore()
        => Score += CONST.SCORE_BOMB;

    private int UnitScore
        => m_patternHandler.Selected().Select((e, index) => (e, index)).Sum(p => p.index) + m_patternHandler.Selected().Count();

    private int ModeScore
        => m_patternHandler.Selected().Count - (int)CONST.MIN_SELECT;

    private void FinishDrag()
    {
        m_unitScore = UnitScore;
        m_modeScore = ModeScore;
        Vector3[] offsetArray = m_patternHandler.ShapeOffset();
        m_matchCount = 0;

        if (offsetArray != null)
            StartCoroutine(DisposeMatch(offsetArray));
        else
            m_lineHandler.Clear();
    }

    private void RecursiveMatch(Vector3[] offsetArray)
        => StartCoroutine(DisposeMatch(offsetArray));

    private void DisposeMatchBall(IGrouping<int, SlotPrefab> key)
    {
        SlotPrefab slot = m_boardHandler.Data[key.Key];
        int matchCount = key.Count();

        switch (matchCount)
        {
            case 1:
                m_ballHandler.Release(slot.Child);
                slot.Child = null;
                break;
            case 2:
                slot.Child.BallColor = SlotAttribute.bomb1;
                break;
            case 3:
                slot.Child.BallColor = SlotAttribute.bomb2;
                break;
            default:
                slot.Child.BallColor = SlotAttribute.bomb3;
                break;
        }
    }

    /* complicated functions... can be simple? */
    private bool FillSlot(SlotPrefab slot)
    {
        SlotPrefab upper = Ray.Instance.Shot(slot.transform.position + CONST.DIRECTION_OFFSET[(int)ClockWise.up]);

        if (slot.Child?.IsWorking ?? false)
            return true;

        if (upper == null)
            return slot.Generate?.Invoke(slot) ?? false;
        else
        {
            if (slot.Child == null)
            {
                if (upper.Child != null && !upper.Child.IsWorking)
                {
                    upper.Child.TransferTo(slot);
                    upper.Child = null;
                }
                return FillSlot(upper);
            }
        }
        return FillSlot(upper);
    }

    /* complicated functions... can be simple? */
    IEnumerator RequestBall(Action action)
    {
        if (Finish)
            yield break;

        m_eventSystem.enabled = false;
        yield return null;
        int count = 0;

        foreach (var element in m_boardHandler.BottomLine)
        {
            if (FillSlot(element))
                count++;
        }

        if (count > 0)
            StartCoroutine(RequestBall(action));
        else
        {
            m_eventSystem.enabled = true;
            action?.Invoke();
        }
    }

    private List<SlotPrefab> Matched(Vector3[] offsetArray)
    {
        List<SlotPrefab> match = new List<SlotPrefab>();

        foreach (var unit in from origin in m_boardHandler.Data
                             let unit = DrawMatchedElement(origin.Value, offsetArray)
                             where unit != null
                             select unit)
                             {
                                ++m_matchCount;
                                Score += m_unitScore * Multi(m_matchCount, m_modeScore);
                                m_lineHandler.ToLine(unit);
                                
                                match.AddRange(unit);
                             }
        
        return match;
    }
    
    private List<SlotPrefab> DrawMatchedElement(SlotPrefab origin, Vector3[] offsetArray)
    {
        List<SlotPrefab> unit = new List<SlotPrefab>();
        Vector3 position = origin.transform.position;

        if (origin.Generate != null)
            return null;

        unit.Add(origin);
        foreach (var match in from offset in offsetArray
                              let hit = Ray.Instance.Shot(position -= offset)
                              where hit != null && hit.Generate == null && !hit.Child.IsBomb() && hit.Child.BallColor.Equals(origin.Child.BallColor)
                              select hit)
            unit.Add(match);

        if (unit.Count > offsetArray.Length)
            return unit;

        return null;
    }

    IEnumerator DisposeMatch(Vector3[] offsetArray)
    {
        m_eventSystem.enabled = false;

        List<SlotPrefab> matchedList = Matched(offsetArray);

        var group = matchedList.GroupBy(e => e.GetInstanceID());

        if (group.Count().Equals(0))
        {
            if (!Finish)
                m_eventSystem.enabled = true;
        }
        else
        {
            yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_REMOVE);

            m_lineHandler.Clear();
            foreach (var key in group)
                DisposeMatchBall(key);

            yield return new WaitForSecondsRealtime(CONST.DURATION_WAIT_FILL_BALL);
            StartCoroutine(RequestBall(() => { RecursiveMatch(offsetArray); }));
        }
    }

    private int Multi(int count, int mode)
    {
        int rst = count * (int)Math.Pow(count * 2, mode);
        
        if (1 < count)
            BonusTimeSecond += mode;
        return rst;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Objects;
using Pattern.Managers;



public class BoardPrefab : MonoBehaviour
{
    [SerializeField] int Width {get; set;}
    [SerializeField] int Height {get; set;}
    [SerializeField] SlotPrefab slot;
    [SerializeField] List<SlotPrefab> list = new List<SlotPrefab>();
    public SlotNode Board {get; set;}



    private void Clear()
    {
        foreach (var item in list)
            Destroy(item.gameObject);
        list.Clear();
    }



    public void CreateBoard(int width, int height)
    {
        Clear();

        Width = width;
        Height = height + 1;

        float heightInterval = slot.GetComponent<RectTransform>().sizeDelta.x;
        float widthInterval = slot.GetComponent<RectTransform>().sizeDelta.y * 0.75f;
        float leftBottom = width % 2 == 0 ? -widthInterval * (width / 2) + (widthInterval / 2) : -widthInterval * (width / 2);
        Vector2 position = new Vector2(leftBottom, 0);
        int index = 0;
        
        foreach (SlotNode node in SlotManager.Instance)
        {
            SlotPrefab prefab = Instantiate(slot);

            if (index % Height == 0)
            {
                position = new Vector2(leftBottom + (index / Height) * widthInterval, 0);
                if ((index / Height) % 2 == 1)
                    position += Vector2.up * heightInterval * 0.5f;
            }

            position += Vector2.up * heightInterval;
            prefab.transform.SetParent(transform);
            prefab.transform.localScale = Vector2.one;
            prefab.transform.localPosition = position;
            prefab.SlotNode = node;

            ++index;
            list.Add(prefab);
        }
    }
}

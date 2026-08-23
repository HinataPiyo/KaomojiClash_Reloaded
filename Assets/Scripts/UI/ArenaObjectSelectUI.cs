namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    public interface IArenaObjectSelectUI
    {
        void SetData(List<ArenaObjectSelectUI.Entry> selectList);
        void Hide();
    }

    public class ArenaObjectSelectUI : MonoBehaviour, IArenaObjectSelectUI
    {
        [SerializeField] Transform card_container;    
        ArenaObjectCardUI[] cards;

        public class Entry
        {
            public ArenaObjectData Data { get; private set; }
            public int Level { get; private set; }

            public Entry(ArenaObjectData data, int level)
            {
                Data = data;
                Level = level;
            }
        }


        void Awake()
        {
            ApiProvider.Register<IArenaObjectSelectUI>(this);
            cards = card_container.GetComponentsInChildren<ArenaObjectCardUI>();

            Hide();
        }

        public void SetData(List<Entry> selectList)
        {
            gameObject.SetActive(true);

            for (int i = 0; i < cards.Length; i++)
            {
                if (i < selectList.Count)
                {
                    cards[i].gameObject.SetActive(true);
                    bool isNew = selectList[i].Level == 1; // Levelが1の場合は新しいArenaObjectとみなす
                    cards[i].SetData(selectList[i].Data, selectList[i].Level, isNew);
                }
                else
                {
                    cards[i].gameObject.SetActive(false);
                }
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
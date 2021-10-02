using System.Collections.Generic;
using UnityEngine;

namespace Deslab.Utils
{
    public class RandomNoRepeate
    {
        private readonly List<int> available = new List<int>();
        private int count;

        public void Init()
        {
            available.Clear();
            for (var i = 0; i < count; i++) available.Add(i);
        }

        public void SetCount(int value)
        {
            count = value;
            Init();
        }

        public void RemoveID(int removeID)
        {
            available.Remove(removeID);
        }

        public int GetAvailable()
        {
            CheckAvailableIds();

            var availableId = Random.Range(0, available.Count);
            var id = available[availableId];
            available.RemoveAt(availableId);
            return id;
        }

        public int GetAvailableAtId(int availableId)
        {
            if (availableId < 0 || availableId >= count) return -1;

            CheckAvailableIds();

            if (availableId < available.Count)
                SetCount(count);

            var id = available[availableId];
            available.RemoveAt(availableId);

            return id;
        }

        private void CheckAvailableIds()
        {
            if (available.Count == 0) Init();
        }
    }
}
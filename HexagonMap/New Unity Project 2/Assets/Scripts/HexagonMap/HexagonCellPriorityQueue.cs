using System.Collections.Generic;

namespace HexagonMap
{
    /// <summary>
    /// 
    /// </summary>
    public class HexagonCellPriorityQueue
    {
        /// <summary>
        /// 最低优先级
        /// </summary>
        private int minimum = int.MaxValue;

        private int count = 0;

        private List<HexagonCell> list = new List<HexagonCell>();

        public int Count { get { return count; } }

        public void Enqueue(HexagonCell cell)
        {
            this.count += 1;
            int priority = cell.SearchPriority;
            if (priority < this.minimum)
            {
                this.minimum = priority;
            }
            while (priority >= this.list.Count)
            {
                this.list.Add(null);
            }
            cell.NextWithSamePriority = list[priority];
            this.list[priority] = cell;
        }

        public HexagonCell Dequeue()
        {
            this.count -= 1;
            for (; this.minimum < list.Count; minimum++)
            {
                HexagonCell cell = this.list[minimum];
                if (cell != null)
                {
                    this.list[minimum] = cell.NextWithSamePriority;
                    return cell;
                }
            }
            return null;
        }

        public void Change(HexagonCell cell, int oldPriority)
        {
            HexagonCell current = list[oldPriority];
            HexagonCell next = current.NextWithSamePriority;
            if (current == cell)
            {
                this.list[oldPriority] = next;
            }
            else
            {
                while (next != cell)
                {
                    current = next;
                    next = current.NextWithSamePriority;
                }
                current.NextWithSamePriority = cell.NextWithSamePriority;
            }
            // 单元格删除后，必须重新添加单元格，以便最终在列表中显示新的优先级。
            this.Enqueue(cell);
            // Enqueue方法增加了计数，但是实际上并没有添加新的单元格。所以我们不得不减少这个计数来进行补偿。
            this.count -= 1;
        }

        public void Clear()
        {
            list.Clear();
            this.count = 0;
            this.minimum = int.MaxValue;
        }
    }
}
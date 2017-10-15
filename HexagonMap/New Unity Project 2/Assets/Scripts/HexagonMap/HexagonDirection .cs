namespace HexagonMap
{
    /// <summary>
    /// 六边形单元格的方向
    /// </summary>
    public enum HexagonDirection
    {
        NE, E, SE, SW, W, NW
    }

    public static class HexagonDirectionExtensions
    {
        /// <summary>
        /// 获取反方向
        /// </summary>
        public static HexagonDirection Opposite(this HexagonDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }

        public static HexagonDirection Previous(this HexagonDirection direction)
        {
            return direction == HexagonDirection.NE ? HexagonDirection.NW : (direction - 1);
        }

        public static HexagonDirection Next(this HexagonDirection direction)
        {
            return direction == HexagonDirection.NW ? HexagonDirection.NE : (direction + 1);
        }

        public static HexagonDirection Previous2(this HexagonDirection direction)
        {
            direction -= 2;
            return direction >= HexagonDirection.NE ? direction : (direction + 6);
        }

        public static HexagonDirection Next2(this HexagonDirection direction)
        {
            direction += 2;
            return direction <= HexagonDirection.NW ? direction : (direction - 6);
        }
    }
}
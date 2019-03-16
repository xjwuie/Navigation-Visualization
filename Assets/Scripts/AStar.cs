using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    enum UnitType
    {
        normal,
        none
    }

    class MapUnit
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public bool isDest { get; private set; }
        public UnitType type { get; private set; }

        public MapUnit parent;
        public double G, H;
        public double F { get { return G + H; } }
        public double moveCost = 0;

        public MapUnit(int x, int y, int type) {
            this.x = x;
            this.y = y;
            //isDest = false;
            this.type = (UnitType)type;
        }

        public void SetDest() {
            isDest = true;
        }
    }

    class AStar
    {
        public bool isManhattan = true;
        public bool isDiagnal = false;

        List<List<MapUnit>> mapUnits;
        List<MapUnit> openList;
        List<MapUnit> closeList;
        int rowNum = 0, colNum = 0;

        public MapUnit start { get; private set; }
        public MapUnit destination { get; private set; }

        public AStar(List<List<int>> arr) {
            rowNum = arr.Count;
            colNum = Utils.FindMaxList<int>(arr);
            Utils.PadList<int>(ref arr, colNum, 1);

            mapUnits = new List<List<MapUnit>>();
            for(int i = 0; i < rowNum; ++i)
            {
                List<MapUnit> tmp = new List<MapUnit>();
                for(int j = 0; j < colNum; ++j)
                {
                    tmp.Add(new MapUnit(i, j, arr[i][j]));
                }
                mapUnits.Add(tmp);
            }
        }

        public void Print() {
            MapUnit tmp = destination;
            while(tmp != start)
            {
                Console.WriteLine("({0}, {1})", tmp.x, tmp.y);
                tmp = tmp.parent;
            }
        }

        public Stack<int[]> GetRouteStack() {
            MapUnit tmp = destination;
            Stack<int[]> result = new Stack<int[]>();
            while(tmp != start)
            {
                result.Push(new int[2]{ tmp.x, tmp.y});
                tmp = tmp.parent;
            }
            result.Push(new int[2] { tmp.x, tmp.y });
            return result;
        }

        public bool Find() {
            openList = new List<MapUnit>();
            closeList = new List<MapUnit>();
            openList.Add(start);

            while(openList.Count > 0)
            {
                MapUnit minFUnit = FindMinFUnit(openList);
                openList.Remove(minFUnit);
                closeList.Add(minFUnit);

                GetAround(minFUnit.x, minFUnit.y);
                if (closeList.Contains(destination))
                {
                    Console.WriteLine("found");
                    return true;
                }
            }
            return false;
        }


        public bool SetStart(int x, int y) {
            MapUnit tmp = mapUnits[x][y];
            if (tmp.type == UnitType.none)
            {
                return false;
            }
            start = tmp;
            start.G = 0f;
            
            return true;
        }

        public bool SetDest(int x, int y) {
            MapUnit tmp = mapUnits[x][y];
            if (tmp.type == UnitType.none)
            {
                return false;
            }
            destination = tmp;
            return true;
        }

        public void SetMoveCost(int x, int y, double cost) {
            mapUnits[x][y].moveCost = cost;
        }

        public void ResetMoveCost(int x, int y) {
            mapUnits[x][y].moveCost = 0;
        }

        public void ResetAllMoveCost() {
            foreach(var list in mapUnits)
            {
                foreach(MapUnit elem in list)
                {
                    elem.moveCost = 0;
                }
            }
        }

        MapUnit GetUnit(int x, int y, int offsetX, int offsetY) {
            x += offsetX;
            y += offsetY;
            if (x < 0 || x >= rowNum || y < 0 || y >= colNum)
                return null;
            MapUnit result = mapUnits[x][y];
            //if (result.type == UnitType.none)
            //return null;
            return result;
        }

        List<MapUnit> GetAround(int x, int y) {
            List<MapUnit> result = new List<MapUnit>();
            MapUnit p = mapUnits[x][y];
            result.Add(GetTop(x, y));
            result.Add(GetRight(x, y));
            result.Add(GetBottom(x, y));
            result.Add(GetLeft(x, y));
            if (isDiagnal)
            {
                result.Add(GetTopLeft(x, y));
                result.Add(GetTopRight(x, y));
                result.Add(GetBottomRight(x, y));
                result.Add(GetBottomLeft(x, y));
            }
            foreach(MapUnit elem in result)
            {
                if (elem == null)
                    continue;
                if(elem.type == UnitType.none || closeList.Contains(elem))
                {
                    continue;
                }

                double G = Distance(elem, p) + p.G + elem.moveCost;
                double H = Distance(elem, destination);

                if (openList.Contains(elem))
                {
                    if (elem.G > G)
                    {
                        elem.G = G;
                        //elem.H = H;
                        elem.parent = p;
                    }
                }
                else
                {
                    elem.G = G;
                    elem.H = H;
                    elem.parent = p;
                    openList.Add(elem);
                }
            }
            return result;
        }

        MapUnit GetTop(int x, int y) {
            return GetUnit(x, y, -1, 0);
        }
        MapUnit GetBottom(int x, int y) {
            return GetUnit(x, y, 1, 0);
        }
        MapUnit GetLeft(int x, int y) {
            return GetUnit(x, y, 0, -1);
        }
        MapUnit GetRight(int x, int y) {
            return GetUnit(x, y, 0, 1);
        }
        MapUnit GetTopLeft(int x, int y) {
            return GetUnit(x, y, -1, -1);
        }
        MapUnit GetTopRight(int x, int y) {
            return GetUnit(x, y, -1, 1);
        }
        MapUnit GetBottomLeft(int x, int y) {
            return GetUnit(x, y, 1, -1);
        }
        MapUnit GetBottomRight(int x, int y) {
            return GetUnit(x, y, 1, 1);
        }

        MapUnit FindMinFUnit(List<MapUnit> arr) {
            if (arr.Count == 0)
                return null;
            MapUnit tmp = arr[0];
            for(int i = 1; i < arr.Count; ++i)
            {
                if (arr[i].F < tmp.F)
                    tmp = arr[i];
            }
            return tmp;
        }

        double Distance(MapUnit u1, MapUnit u2) {
            double deltaX = u2.x - u1.x;
            double deltaY = u2.y - u1.y;
            if (isManhattan)
            {
                return Math.Abs(deltaX) + Math.Abs(deltaY);
            }
            else
            {
                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }
        }

        void AppendToOpenList(List<MapUnit> arr) {
            foreach(MapUnit elem in arr)
            {
                if (elem.type == UnitType.none || closeList.Contains(elem))
                    continue;
                if (!openList.Contains(elem))
                {
                    openList.Add(elem);
                }

            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;

public enum MouseMode
{
    none,
    brush,
    start,
    dest
}

public class UnitContainer : MonoBehaviour {

    GameObject plane;

    float planeWidth, planeHeight;
    float topPos, bottomPos, leftPos, rightPos;

    float unitSize;
    int rowNum, colNum;

    public MouseMode mouse = MouseMode.none;

    List<List<Unit>> units;
    List<Unit> route;
    Unit startUnit, destUnit;

    Material materialNormal, materialBlock, materialRoute;
	
	void Start () {
        units = new List<List<Unit>>();
        route = new List<Unit>();
        plane = GameObject.Find("Plane");
        Transform planeTrans = plane.transform;
        planeWidth = plane.transform.localScale.x * 10;
        planeHeight = plane.transform.localScale.z * 10;
        topPos = planeTrans.position.z + planeTrans.localScale.z * 5;
        bottomPos = planeTrans.position.z - planeTrans.localScale.z * 5;
        rightPos = planeTrans.position.x + planeTrans.localScale.x * 5;
        leftPos = planeTrans.position.x - planeTrans.localScale.x * 5;

        materialNormal = Resources.Load<Material>("Materials/UnitNormal");
        materialBlock = Resources.Load<Material>("Materials/UnitBlock");
        materialRoute = Resources.Load<Material>("Materials/UnitRoute");
    }

    void Update() {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos = new Vector2(mousePos3D.x, mousePos3D.z);
            if (!MouseInMap(mousePos))
                return;
            if(mouse == MouseMode.start)
            {
                SetStart(mousePos);
            }
            else if(mouse == MouseMode.dest)
            {
                SetDest(mousePos);
            }

        }
    }

    public void ClearChildrenInTransform() {
        for(int i = transform.childCount - 1; i >= 0; --i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void ClearChildren() {
        destUnit = null;
        startUnit = null;
        foreach(var list in units)
        {
            foreach(Unit u in list)
            {
                //list.Remove(u);
                u.DestroySelf();
                
            }
            list.Clear();
            //units.Remove(list);
        }
        units.Clear();
        
    }

    public void Fill(int rowNum) {
        ClearChildren();
        int colNum = Mathf.FloorToInt(rowNum * planeWidth / planeHeight);
        this.rowNum = rowNum;
        this.colNum = colNum;
        float size = planeHeight / rowNum;
        unitSize = size;
        Object o = Resources.Load<GameObject>("Prefabs/Unit");
        print("colNum: " + colNum);
        print("rowNum: " + rowNum);
        for (int i = 0; i < rowNum; ++i)
        {
            float posX = planeHeight / 2 - 0.5f * size - size * i;
            List<Unit> tmpList = new List<Unit>();
            for(int j = 0; j < colNum; ++j)
            {
                float posY = -planeWidth / 2 + 0.5f * size + size * j;
                GameObject unit = Instantiate((GameObject)o);
                unit.transform.SetParent(transform);
                unit.transform.position = new Vector3(posY, 1f, posX);
                unit.transform.localScale = new Vector3(size, size, size);
                
                Unit tmp = unit.GetComponent<Unit>();
                tmp.SetPos(posY, posX);
                tmp.SetIndex(i, j);
                tmp.type = 0;
                tmp.SetMaterial(materialNormal);
                //print(new Vector2(posY, posX));
                tmpList.Add(tmp);
            }
            units.Add(tmpList); 
        }
        
    }

    public void SetBlock(Vector2 o, float radius) {
        List<int> startEndIndexList = GetStartEndIndex(o, radius);
        //print(new Vector4(startEndIndexList[0], startEndIndexList[1], startEndIndexList[2], startEndIndexList[3]));
        for(int i = startEndIndexList[0]; i <= startEndIndexList[1]; ++i)
        {
            for (int j = startEndIndexList[2]; j <= startEndIndexList[3]; ++j)
            {
                Unit tmp = units[i][j];
                float dis = Distance(tmp, o);
                if (dis <= radius)
                {
                    tmp.SetMaterial(materialBlock);
                    tmp.type = 1;
                }
            }
        }
    }

    public void SetNormal(Vector2 o, float radius) {
        List<int> startEndIndexList = GetStartEndIndex(o, radius);
        //print(new Vector4(startEndIndexList[0], startEndIndexList[1], startEndIndexList[2], startEndIndexList[3]));
        for (int i = startEndIndexList[0]; i <= startEndIndexList[1]; ++i)
        {
            for (int j = startEndIndexList[2]; j <= startEndIndexList[3]; ++j)
            {
                Unit tmp = units[i][j];
                float dis = Distance(tmp, o);
                if (dis <= radius)
                {
                    tmp.SetMaterial(materialNormal);
                    tmp.type = 0;
                }
            }
        }
    }

    public void ShowRoute() {
        if (startUnit == null || destUnit == null)
            return;
        GetRoute();
        //foreach(Unit u in route) u.SetMaterial(materialRoute);
        StartCoroutine(Show());
    }

    IEnumerator Show() {
        foreach (Unit u in route)
        {
            u.SetMaterial(materialRoute);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ResetMap() {
        mouse = MouseMode.none;
        route.Clear();
        destUnit = null;
        startUnit = null;
        foreach(var list in units)
        {
            foreach(Unit elem in list)
            {
                elem.SetMaterial(materialNormal);
                elem.type = 0;
            }
        }
    }

    void GetRoute() {
        route.Clear();
        List<List<int>> map = GenerateMap();
        AStar.AStar aStar = new AStar.AStar(map);
        aStar.SetStart(startUnit.rowIndex, startUnit.colIndex);
        aStar.SetDest(destUnit.rowIndex, destUnit.colIndex);
        if (aStar.Find())
        {
            var routeStack = aStar.GetRouteStack();
            while(routeStack.Count != 0)
            {
                int[] tmp = routeStack.Pop();
                route.Add(units[tmp[0]][tmp[1]]);
            }
        }
    }





    List<int> GetStartEndIndex(Vector2 o, float radius) {
        float top = (o.y + radius < topPos) ? (o.y + radius) : topPos;
        float bottom = (o.y - radius > bottomPos) ? (o.y - radius) : bottomPos;
        float left = (o.x - radius > leftPos) ? (o.x - radius) : leftPos;
        float right = (o.x + radius < rightPos) ? (o.x + radius) : rightPos;
        int topNum = Mathf.FloorToInt((topPos - top) / unitSize);
        int bottomNum = Mathf.FloorToInt((bottom - bottomPos) / unitSize);
        int leftNum = Mathf.FloorToInt((left - leftPos) / unitSize);
        int rightNum = Mathf.FloorToInt((rightPos - right) / unitSize);

        return new List<int>() { topNum, rowNum - 1 - bottomNum, leftNum, colNum - 1 - rightNum };
    }

    float Distance(Vector2 pos1, Vector2 pos2) {
        float deltaX = pos2.x - pos1.x;
        float deltaY = pos2.y - pos1.y;
        return Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    float Distance(Unit u1, Unit u2) {
        return Distance(new Vector2(u1.x, u1.y), new Vector2(u2.x, u2.y));
    }

    float Distance(Unit u, Vector2 v) {
        return Distance(new Vector2(u.x, u.y), v);
    }



    List<List<int>> GenerateMap() {
        List<List<int>> result = new List<List<int>>();
        foreach(var unitList in units)
        {
            List<int> tmpList = new List<int>();
            foreach(Unit u in unitList)
            {
                tmpList.Add(u.type);
            }
            result.Add(tmpList);
        }
        return result;
    }

    bool MouseInMap(Vector2 mousePos) {
        return mousePos.x > leftPos &&
            mousePos.x < rightPos &&
            mousePos.y < topPos &&
            mousePos.y > bottomPos;
    }

    Unit GetMouseUnit(Vector2 mousePos) {
        int row = Mathf.FloorToInt((topPos - mousePos.y) / unitSize);
        int col = Mathf.FloorToInt((mousePos.x - leftPos) / unitSize);
        return units[row][col];
    }

    void SetStart(Vector2 mousePos) {
        Unit tmp = GetMouseUnit(mousePos);
        if (tmp.type == 1)
            return;
        if(startUnit != null)
        {
            startUnit.SetMaterial(materialNormal);
        }
        startUnit = tmp;
        startUnit.SetMaterial(materialRoute);
    }

    void SetDest(Vector2 mousePos) {
        Unit tmp = GetMouseUnit(mousePos);
        if (tmp.type == 1)
            return;
        if (destUnit != null)
        {
            destUnit.SetMaterial(materialNormal);
        }
        destUnit = tmp;
        destUnit.SetMaterial(materialRoute);
    }
}

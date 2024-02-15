using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public PhysicsMaterial2D lineMat;
    
    public GameObject Line;

    public GameObject Dot;

    [SerializeField] private GameObject currentLine;

    [SerializeField] private float resolution = 0.15f;

    private bool started = false;

    private IEnumerator drawRoutine;

    private bool isLine;
    
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        started = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            currentLine = Instantiate(Line, Vector3.zero, Quaternion.identity);
            drawRoutine = Draw();
            StartCoroutine(drawRoutine);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse released");
            if (!started)
            {
                started = true;
                Time.timeScale = 1f;
            }
            StopCoroutine(drawRoutine);
            GenerateCollider(currentLine);
            currentLine = null;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (started) Time.timeScale = 0.3f;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (started) Time.timeScale = 1;
        }
    }

    private void GenerateCollider(GameObject input)
    {
        /*GameObject parent = new GameObject();
        parent.AddComponent<Rigidbody2D>();
        EdgeCollider2D eCol = parent.AddComponent<EdgeCollider2D>();
        
        input.transform.SetParent(parent.transform);*/
        //EdgeCollider2D eCol = input.AddComponent<EdgeCollider2D>();
        if (!isLine)
        {
            Destroy(input);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Instantiate(Dot, mousePos, Quaternion.identity);
            return;
        }
        
        PolygonCollider2D eCol = input.AddComponent<PolygonCollider2D>();
        
        LineRenderer lr = input.GetComponent<LineRenderer>();
        List<Vector2> points = new List<Vector2>();
        for (int i=0; i < lr.positionCount; i++)
        {
            Vector3 temp = lr.GetPosition(i);
            float tempX = temp.x;
            float tempY = temp.y;
            points.Add(new Vector2(tempX, tempY));
        }
        
        List<Vector2> offsetPoints = new List<Vector2>();
        List<Vector2> offsetPointsNeg = new List<Vector2>();
        
        float offsetAmount = 0.06f; //line thickness is 0.12, 0.06 on either side
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 direction = Vector2.zero;

            if (i < points.Count - 1)
            {
                direction = (points[i + 1] - points[i]).normalized;
            }
            else
            {
                direction = (points[0] - points[i]).normalized;
            }

            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * offsetAmount;
            offsetPoints.Add(points[i] + perpendicular);
            offsetPointsNeg.Add(points[i] - perpendicular);
        }

        for (int i = points.Count - 1; i >= 0; i--)
        {
            offsetPoints.Add(offsetPointsNeg[i]);
        }
        eCol.SetPath(0,offsetPoints);
        //eCol.SetPoints(points);
        //eCol.edgeRadius = 0.06f;
        
        Rigidbody2D rb = input.AddComponent<Rigidbody2D>();
        
        rb.angularDrag = 0;
        rb.sharedMaterial = lineMat;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.useAutoMass = true;
        eCol.density = 2f;
    }

    /*private void OnMouseDown()
    {
        Debug.Log("Mouse Clicked");
        StartCoroutine(Draw());
    }

    private void OnMouseUp()
    {
        Debug.Log("Mouse released");
        StopCoroutine(Draw());
    }*/

    private IEnumerator Draw()
    {
        /*Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 prevPos = mousePos;
        while (true)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            if (Vector3.Distance(prevPos, mousePos) >= dotSize/2f)
            {
                GameObject dot = Instantiate(DrawDot, mousePos, Quaternion.identity);
                dot.transform.localScale = new Vector3(dotSize,dotSize,dotSize);
                prevPos = mousePos;
            }*

            yield return null;
        }*/
        
        Debug.Log("Started Drawing");
        isLine = false;
        LineRenderer lr = currentLine.GetComponent<LineRenderer>();
        lr.positionCount = 0;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 prevPos = mousePos;
        lr.positionCount++;
        lr.SetPosition(lr.positionCount - 1, prevPos);

        while (Input.GetMouseButton(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            if (Vector3.Distance(prevPos, mousePos) >= resolution && !checkHoverOver(mousePos))
            {
                if (canConnect(prevPos, mousePos))
                {
                    isLine = true;
                    prevPos = mousePos;
                    lr.positionCount++;
                    lr.SetPosition(lr.positionCount - 1, mousePos);
                }
            }

            yield return null;
        }
        
    }

    private bool canConnect(Vector3 prevPos, Vector3 currentPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(prevPos, currentPos - prevPos, Vector3.Distance(prevPos, currentPos));
        if (hit.collider)
        {
            //Debug.Log($"The raycast was stopped by {hit.collider}");
            return false;
        }
        return true;
    }

    private bool checkHoverOver(Vector3 pos)
    {
        
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider)
        {
            //Debug.Log("Collided");
            return true;
        }
        return false;
    }
}

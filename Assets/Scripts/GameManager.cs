using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject ball1, ball2;
    
    public GameObject lineParent;

    public Transform spawn1, spawn2;

    private LineDrawer drawer;
    // Start is called before the first frame update
    void Start()
    {
        lineParent = GameObject.FindWithTag("LineParent");
        drawer = this.gameObject.GetComponent<LineDrawer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public void Restart()
    {
        drawer.started = false;
        Time.timeScale = 0f;
        ball1.transform.position = spawn1.position;
        ball2.transform.position = spawn2.position;
        ball1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ball2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        foreach (var child in lineParent.GetComponentsInChildren<Transform>())
        {
            if (child != lineParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}

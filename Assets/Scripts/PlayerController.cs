using System;
using System.Collections.Generic;
using Amir.Level;
using Amir.UI;
using DG.Tweening;
using Lean.Touch;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool putToRight = true;
    private float offset = 0.19f;
    public static Action OnFinish;
    public static Action OnWineGlassFill;
    public static Action<GameObject> onObstacleHit;
    private int filledWineGlasses = 0;
    [SerializeField] private GameObject thief;
    [SerializeField] private TextMeshProUGUI wineGlassesCountText;
    private List<GameObject> bottles = new List<GameObject>();
    static public bool moving = true;

    private void Start()
    {
        bottles.Add(transform.GetChild(0).gameObject);
        OnFinish += Finish;
        OnWineGlassFill += WineGlassFill;
        onObstacleHit += ObstacleHit;
    }

    void Update()
    {
        if (moving)
            transform.Translate(0, 0, speed * Time.deltaTime);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                Vector3 touchedPos =
                    Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
                transform.position = Vector3.Lerp(transform.position, touchedPos, Time.deltaTime);
            }
        }
    }

    public void OnTriggerEnterChild(Collider other)
    {
        other.GetComponent<Collider>().enabled = false;
        other.transform.parent = transform;
        bottles.Add(other.gameObject);
        Debug.Log("Added = " + bottles.Count);
        RearrangeBottles();

        other.transform.DORotate(new Vector3(0, 180, -150), 1f).OnComplete(() =>
        {
            other.gameObject.GetComponent<Bottle>().SetActivePaintAndFluid();
            other.GetComponent<Collider>().enabled = true;
            other.GetComponent<Collider>().isTrigger = false;
            Rigidbody rb = other.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        });
        other.gameObject.GetComponent<Bottle>().enabled = true;
        other.gameObject.GetComponent<Bottle>().canTrigger = true;
    }

    void ObstacleHit(GameObject bottleToRemove)
    {
        bottles.Remove(bottleToRemove);
        if (bottles.Count == 0)
        {
            Lose();
        }

        RearrangeBottles();
    }

    void Lose()
    {
        UIManager.instance.ShowLoseSrceen();
        moving = false;
    }

    private void RearrangeBottles()
    {
        int count = 1;
        bool firstBottle = true;
        foreach (var bottle in bottles)
        {
            if (firstBottle)
            {
                bottle.transform.localPosition = Vector3.zero;
                firstBottle = false;
                continue;
            }

            if (putToRight)
            {
                bottle.transform.DOLocalMove(new Vector3(offset * count, 0, 0), 1f);
                putToRight = false;
            }
            else
            {
                bottle.transform.DOLocalMove(new Vector3(-offset * count, 0, 0), 1f);
                putToRight = true;
                count++;
            }
        }

        putToRight = true;
    }

    void Finish()
    {
        moving = false;
        Camera.main.transform.DOMoveZ(Camera.main.transform.position.z + 3f, 2f);
        GetComponent<LeanDragTranslate>().enabled = false;
        thief.GetComponent<Animator>().SetTrigger("Turn");
        thief.transform.DORotate(new Vector3(0, 0, 0), 3f).OnComplete(() =>
        {
            float winDistanceToWalk;
            if (filledWineGlasses > 18)
            {
                winDistanceToWalk = 2;
            }
            else if (filledWineGlasses > 15)
            {
                winDistanceToWalk = 5;
            }
            else if (filledWineGlasses > 10)
            {
                winDistanceToWalk = 8;
            }
            else if (filledWineGlasses > 5)
            {
                winDistanceToWalk = 11;
            }
            else winDistanceToWalk = 14;

            thief.transform.DOMoveZ(thief.transform.position.z + winDistanceToWalk, winDistanceToWalk / 2);
        });
    }

    void WineGlassFill()
    {
        filledWineGlasses++;
        wineGlassesCountText.text = filledWineGlasses.ToString();
        Debug.Log("Filled wine glasses" + filledWineGlasses);
    }
}
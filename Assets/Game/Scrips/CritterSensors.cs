using UnityEngine;
using System;

[RequireComponent(typeof(CritterCtrl))]
public class CritterSensors : BaseBehaviour
{
    private CritterCtrl critterCtrl;

    [SerializeField]
    private Transform antenaL;
    [SerializeField]
    private Transform antenaR;

    public FeedStarveManager scene;

    private GameObject[] foods;

    private float antenaLSignal = float.MaxValue;
    private float antenaRSignal = float.MaxValue;

    void Awake()
    {
        critterCtrl = GetComponent<CritterCtrl>();

        if (antenaL == null) throw new Exception("antenaL not set");
        if (antenaR == null) throw new Exception("antenaL not set");
    }

    void Start()
    {
        if (scene == null) throw new Exception("scene not set");
        if(scene.foods != null)
        {
            foods = (GameObject[])scene.foods.Clone();
        } else
        {
            Debug.LogWarning("Scene.foods not found, can't detect near food.");
        }
    }

    public int SampleLife()
    {
        return critterCtrl.getLife();
    }

    public float SampleAntenaR()
    {
        return antenaRSignal;
    }

    public float SampleAntenaL()
    {
        return antenaLSignal;
    }

    private static Color debugColor = new Color(0, 1, 1, .3f);

    private void FixedUpdate()
    {
        if(foods != null && foods.Length > 0)
        {
            InsertionSort(foods);
            GameObject closestFood = foods[0];

            Debug.DrawLine(transform.position, closestFood.transform.position, debugColor);

            antenaLSignal = Vector3.Distance(antenaL.position, closestFood.transform.position);
            antenaRSignal = Vector3.Distance(antenaR.position, closestFood.transform.position);
        }
    }

    private void InsertionSort(GameObject[] inputArray)
    {
        long j = 0;
        GameObject temp;
        for (int index = 1; index < inputArray.Length; index++)
        {
            j = index;
            temp = inputArray[index];
            float tempDist = distance(temp);
            while ((j > 0) && (distance(inputArray[j - 1]) > tempDist))
            {
                inputArray[j] = inputArray[j - 1];
                j = j - 1;
            }
            inputArray[j] = temp;
        }
    }

    private float distance(GameObject gameObject)
    {
        if(gameObject.activeInHierarchy)
        {
            Vector3 iPos = transform.position;
            Vector3 fPos = gameObject.transform.position;
            float dX = fPos.x - iPos.x;
            float dY = fPos.y - iPos.y;
            return dX * dX + dY * dY;
        } else
        {
            return float.MaxValue;
        }
    }

    private void CritterDied()
    {
        enabled = false;
    }

    private void CritterRespawned()
    {
        enabled = true;
    }
}

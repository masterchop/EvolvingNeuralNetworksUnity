using UnityEngine;
using System;

public class CritterCtrl : BaseBehaviour
{
    public enum CritterStatus { Alive, Starved, Overfeeded };

    public delegate void DiedEvent(GameObject critter);
    public event DiedEvent OnDied;

    [SerializeField]
    private int maxLife = 100;

    [SerializeField]
    private int life = 100;

    [SerializeField]
    private float lifeConsumtion = 15f;

    public CritterStatus status { get; private set; }

    public bool isAlive() { return life > 0; }

    public int getMaxLife() { return maxLife; }
    public int getLife() { return life;  }

    public void SetLife(int amout)
    {
        life = amout;
        if(amout < 0)
        {
            Starve();
        } else if(amout > maxLife)
        {
            Overfeeded();
        }
    }

    public void Feed(int amout)
    {
        SetLife(getLife() + amout);
    }

    public void Starve()
    {
        status = CritterStatus.Starved;
        Die();
    }

    public void Overfeeded()
    {
        status = CritterStatus.Overfeeded;
        Die();
    }

    public float naturalLifeSpan()
    {
        return maxLife / lifeConsumtion;
    }

    public float LifeSpan()
    {
        if(deadTime > birthTime)
        {
            return deadTime - birthTime;
        } else
        {
            return Time.time - birthTime;
        }
    }

    public void Die()
    {
        SendMessage("CritterDied");
    }

    public void Initialize()
    {
        SendMessage("CritterRespawned");
    }

    private void CritterRespawned()
    {
        life = maxLife;
        enabled = true;
        birthTime = Time.time;
        deadTime = 0;
        lastTimeSienceConsumtion = Time.time;
        status = CritterStatus.Alive;
    }

    private void CritterDied()
    {
        enabled = false;
        life = 0;
        deadTime = Time.time;
        //Debug.Log("Critter died after " + LifeSpan() + "s");
        if (OnDied != null) OnDied(gameObject);
    }

    private float lastTimeSienceConsumtion;
    private float birthTime;
    private float deadTime;

    void Start()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        float consumptionDeltaTime = Time.time - lastTimeSienceConsumtion;

        float deltaConsumption = consumptionDeltaTime * lifeConsumtion;

        if(deltaConsumption >= 1f)
        {
            int integerDeltaConsumption = (int) deltaConsumption;

            SetLife(life - integerDeltaConsumption);

            deltaConsumption -= integerDeltaConsumption;

            float deltaExtra = deltaConsumption / lifeConsumtion;
            lastTimeSienceConsumtion = Time.time - deltaExtra;
        }
    }
}

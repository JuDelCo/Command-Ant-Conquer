using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AntWars;
using DG.Tweening;
using Atto;

public class AntVisuals : MonoBehaviour
{

    public PlayerBehaviour queen;
    public Transform carryPivot;

    NavMeshAgent m_Agent;
    Animator m_animator;
    SpriteRenderer m_spriteRenderer;
    LineRenderer m_lineRenderer;
    SoldierBehaviour m_soldierBehaviour;

    bool isMoving = false;
    bool isCarrying = false;

    Vector3 startScale;
    float flipTimer = 0;

    public Color lineColor = Color.white * 0.5f;

    float acquireTimer = 0;
    public float acquireEffectTime = 1f;
    public float acquireEffectSpeed = 2f;
    Transform currentTarget;

    public GameObject carriedLarva;
    public GameObject carriedFood;

    public GameObject deadAntPrefab;
    public GameObject foodDropPrefab;

    void Start()
    {
        startScale = transform.localScale;
        m_Agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponentInChildren<Animator>();
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_lineRenderer = GetComponentInChildren<LineRenderer>();
        m_soldierBehaviour = GetComponentInChildren<SoldierBehaviour>();
        m_animator.speed = 0;

        queen = m_soldierBehaviour.soldierData.owner.behaviour;
        m_spriteRenderer.color = queen.playerColor;
        lineColor = queen.playerColor;

        var gameManager = Core.Get<GameManager>();
        gameManager.onSoldierChangeState += AcquireTarget;
        gameManager.onSoldierDestroy += DieEffect;
        gameManager.onAddFoodInLair += DropFoodInLair;
    }

	void OnDestroy()
	{
		var gameManager = Core.Get<GameManager>();
        gameManager.onSoldierChangeState -= AcquireTarget;
        gameManager.onSoldierDestroy -= DieEffect;
        gameManager.onAddFoodInLair -= DropFoodInLair;
	}

	void DropFoodInLair(Soldier soldier, Lair lair)
    {
        if (soldier.behaviour == m_soldierBehaviour)
        {
            var foodObject = Instantiate(foodDropPrefab, carriedFood.transform.position, Quaternion.identity) as GameObject;
            foodObject.GetComponent<FoodDropEffect>().SetEffect(carriedFood.transform.position, lair.behaviour.transform.position);
        }
    }

    void Update()
    {
        UpdateAcquireTarget();

        m_animator.SetBool("Carry", isCarrying);

        //if (queen != null)
        //{
        //    m_spriteRenderer.color = queen.playerColor;
        //}

        if (!isMoving && m_Agent.velocity.magnitude > 0.05f)
        {
            isMoving = true;
            m_animator.speed = 1;
        }
        if (isMoving && m_Agent.velocity.magnitude < 0.05f)
        {
            isMoving = false;
            m_animator.Play(m_animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
            m_animator.speed = 0;
        }

        int mirrorDirection = Vector3.Dot(m_Agent.velocity, Camera.main.transform.right) > 0 ? 1 : -1;

        flipTimer += Time.deltaTime;
        if (flipTimer > 0.25f && m_Agent.velocity.magnitude > 0.2f)
        {
            transform.localScale = new Vector3(startScale.x * mirrorDirection, startScale.y, startScale.z);
            flipTimer = 0;
        }
    }

    public void DieEffect(Soldier soldier)
    {
        if (soldier.behaviour == m_soldierBehaviour)
        {
            Instantiate(deadAntPrefab, transform.position, Quaternion.identity);
        }
    }

    /*public void ReceiveDamageEffect(Player player, PlayerAction playerAction, Soldier relatedSoldier)
    {
        m_spriteRenderer.transform.DOShakePosition(0.5f, Camera.main.transform.right, 100, 90);
    }*/

    public void AttackEffect()
    {
        m_spriteRenderer.transform.DOPunchScale(Vector3.right * 0.5f, 0.25f, 0, 0);
        m_spriteRenderer.transform.DOPunchPosition(0.2f * (Vector3.right + Vector3.forward * 2), 0.5f, 0);
    }

    public void CarryLarva()
    {
        carriedLarva.SetActive(true);
        carriedFood.SetActive(false);
        isCarrying = true;
    }

    public void CarryFood()
    {
        carriedLarva.SetActive(false);
        carriedFood.SetActive(true);
        isCarrying = true;
    }

    public void CarryNone()
    {
        carriedLarva.SetActive(false);
        carriedFood.SetActive(false);
        isCarrying = false;
    }

    public void AcquireTarget(Soldier soldier, SoldierState previousState)
    {
        if (soldier.behaviour == m_soldierBehaviour && previousState != soldier.state)
        {
            switch (soldier.state)
            {
                case SoldierState.Protect:
                    currentTarget = queen.transform;
                    break;
                case SoldierState.Attack:
                    currentTarget = soldier.attackTarget.behaviour.transform;
                    break;
                case SoldierState.Harvest:
                    if (soldier.harvestPileTarget == null)
                    {
                        currentTarget = soldier.harvestUnitTarget.behaviour.transform;
                    }
                    else
                    {
                        currentTarget = soldier.harvestPileTarget.behaviour.transform;
                    }
                    break;
                case SoldierState.StealEgg:
                    currentTarget = soldier.stealEggTarget.behaviour.transform;
                    break;
            }
            acquireTimer = 0;
        }
    }

    void UpdateAcquireTarget()
    {
        if (acquireTimer >= 0 && acquireTimer < acquireEffectTime + 1 && currentTarget != null)
        {
            m_lineRenderer.enabled = true;
            int resolution = 32;
            var vertices = new Vector3[resolution];

            m_lineRenderer.positionCount = resolution;

            acquireTimer += Time.deltaTime * acquireEffectSpeed;

            for (int i = 0; i < vertices.Length; i++)
            {
                float alpha = Mathf.Clamp((float)i / (float)vertices.Length, acquireTimer - acquireEffectTime, acquireTimer);
                var origin = m_spriteRenderer.transform.position;
                var end = currentTarget.transform.position;
                vertices[i] = Vector3.Lerp(origin, end, alpha);
                vertices[i].y += alpha * (1 - alpha) * (origin - end).magnitude;
            }

            m_lineRenderer.SetPositions(vertices);
            m_lineRenderer.startColor = lineColor;
            m_lineRenderer.endColor = lineColor;
        }
        else
        {
            m_lineRenderer.enabled = false;
        }
    }

}

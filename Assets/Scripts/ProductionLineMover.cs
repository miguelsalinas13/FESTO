using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionLineMover : MonoBehaviour
{
    [Header("Object")]
    public GameObject objectToMove;
    public bool hideObjectAtStart = true;

    [Header("Child Object Reveal")]
    public bool revealChildAfterTime = false;
    public GameObject childObjectToReveal;
    public float childRevealDelay = 2f;

    [Header("Path")]
    public Transform spawnPoint;
    public List<Transform> pathPoints = new List<Transform>();

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float reachDistance = 0.02f;

    [Header("Start Delay")]
    public bool useStartDelay = false;
    public float startDelay = 8f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip movingSound;

    private bool isMoving = false;
    private bool isWaitingToStart = false;
    private Coroutine moveCoroutine;
    private Coroutine revealCoroutine;
    private Rigidbody objectRb;

    void Start()
    {
        if (objectToMove != null)
        {
            objectRb = objectToMove.GetComponent<Rigidbody>();

            if (hideObjectAtStart)
                objectToMove.SetActive(false);
        }

        if (childObjectToReveal != null)
        {
            childObjectToReveal.SetActive(false);
        }
    }

    public void StartProduction()
    {
        if (isMoving || isWaitingToStart)
            return;

        if (objectToMove == null || spawnPoint == null || pathPoints == null || pathPoints.Count == 0)
        {
            Debug.LogWarning("ProductionLineMover: Missing references.");
            return;
        }

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        if (revealCoroutine != null)
            StopCoroutine(revealCoroutine);

        moveCoroutine = StartCoroutine(StartProductionRoutine());
    }

    private IEnumerator StartProductionRoutine()
    {
        isWaitingToStart = true;

        if (useStartDelay && startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        isWaitingToStart = false;
        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;

        objectToMove.SetActive(true);

        objectToMove.transform.position = spawnPoint.position;
        objectToMove.transform.rotation = spawnPoint.rotation;

        if (childObjectToReveal != null)
        {
            childObjectToReveal.SetActive(false);
        }

        if (revealChildAfterTime && childObjectToReveal != null)
        {
            revealCoroutine = StartCoroutine(RevealChildRoutine());
        }

        if (objectRb != null)
        {
            objectRb.linearVelocity = Vector3.zero;
            objectRb.angularVelocity = Vector3.zero;
            objectRb.isKinematic = true;
        }

        if (audioSource != null && movingSound != null)
        {
            audioSource.clip = movingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        for (int i = 0; i < pathPoints.Count; i++)
        {
            if (pathPoints[i] == null)
                continue;

            Vector3 targetPosition = pathPoints[i].position;

            while ((objectToMove.transform.position - targetPosition).sqrMagnitude > reachDistance * reachDistance)
            {
                objectToMove.transform.position = Vector3.MoveTowards(
                    objectToMove.transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            objectToMove.transform.position = targetPosition;
        }

        Vector3 finalPosition = pathPoints[pathPoints.Count - 1].position;
        objectToMove.transform.position = finalPosition;

        if (objectRb != null)
        {
            objectRb.linearVelocity = Vector3.zero;
            objectRb.angularVelocity = Vector3.zero;
        }

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        isMoving = false;
        moveCoroutine = null;
    }

    private IEnumerator RevealChildRoutine()
    {
        yield return new WaitForSeconds(childRevealDelay);

        if (childObjectToReveal != null && objectToMove != null && objectToMove.activeInHierarchy)
        {
            childObjectToReveal.SetActive(true);
        }

        revealCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        if (spawnPoint == null || pathPoints == null || pathPoints.Count == 0)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnPoint.position, 0.05f);

        Vector3 previousPosition = spawnPoint.position;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            if (pathPoints[i] == null)
                continue;

            Gizmos.DrawSphere(pathPoints[i].position, 0.05f);
            Gizmos.DrawLine(previousPosition, pathPoints[i].position);
            previousPosition = pathPoints[i].position;
        }
    }
}

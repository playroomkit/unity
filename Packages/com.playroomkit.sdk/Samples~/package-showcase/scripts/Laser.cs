using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    public Transform player;
    public Transform laserOrigin;
    public float gunRange = 50f;
    public float fireRate = 0.2f;
    public float laserDuration = 0.05f;

    LineRenderer laserLine;
    float fireTimer;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    public int ShootLaser(int score)
    {
        fireTimer += Time.deltaTime;


        score += 10;
        fireTimer = 0;
        laserLine.SetPosition(0, laserOrigin.position);

        Vector3 shootDirection = player.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(laserOrigin.position, shootDirection, out hit, gunRange))
        {
            laserLine.SetPosition(1, hit.point);
            Destroy(hit.transform.gameObject);
        }
        else
        {
            laserLine.SetPosition(1, laserOrigin.position + shootDirection * gunRange);
        }

        StartCoroutine(Shoot());
        return score;
    }

    IEnumerator Shoot()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }
}
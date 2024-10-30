using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastGun : MonoBehaviour
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
        
        // if (Input.GetButtonDown("Jump") && fireTimer > fireRate)
        // {
            score += 10;
            fireTimer = 0;
            laserLine.SetPosition(0, laserOrigin.position);

            // Use the player's forward direction to cast the ray
            Vector3 shootDirection = player.transform.forward;
            RaycastHit hit;

            // Cast a ray from the player's position forward
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
        // }

        return score;
    }

    IEnumerator Shoot()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }


}


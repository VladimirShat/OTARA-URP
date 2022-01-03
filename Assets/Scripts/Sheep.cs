using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    [Space]
    [Header("Raycast Layers")]
    public LayerMask groundMask;
    public LayerMask obstacleMask;
    public LayerMask sheepMask;

    [Space]
    [Header("Settings")]
    [Range(0, 10)]
    public float Speed = 1f;
    [Range(0, 10)]
    public float RunSpeed = 1f;
    public float EatTime = 1f;
    public float DstToObstacles = 2f;
    public float DstBetweenSheeps = 1f;
    public float ViewingAngle = 120;
    public int DensityOfView = 10;
    public MeshRenderer mesh;

    [Space]
    public CharacterController controller;

    [HideInInspector]
    public Vector3 dogPosition;

    [HideInInspector]
    public bool runing;
    bool going;
    bool eating;
    bool scared;


    Vector3 target;
    float distanceToTarget;

    float defaultRunSpeed;

    void Start()
    {
        defaultRunSpeed = RunSpeed;

        #region start standing on the ground

        var ray = new Ray(transform.position + controller.center, -transform.up);

        if (Physics.Raycast(ray, out var hitInfo, 100f, groundMask))
        {
            if (Physics.SphereCast(transform.position, 3, -transform.up, out var hit, 100f, obstacleMask))
            {
                Destroy(gameObject);
            }
        }
        else Destroy(gameObject);
        #endregion
    }

    private void Update()
    {
        if (!runing)
        {
            //just graze, be-ee-ee
            Graze();
        }

        RunSpeed = Mathf.Lerp(RunSpeed, defaultRunSpeed, 0.1f);
        if (runing)
        {
            target = Vector3.zero;
            StopAllCoroutines();
            eating = false;
            going = false;

            //run while sheep in dog range
            Run();
        }
    }

    void Graze()
    {
        //create new graze zone target
        if (!going && !eating)
        {
            var newTarget = GetRandomPointOnTheGroundInRange(4);
            if (newTarget != Vector3.zero) target = newTarget;

            going = true;
        }

        if (going)
        {
            Vector3 xzTargetPos = target;
            xzTargetPos.y = transform.position.y;
            Vector3 relativePos = xzTargetPos - transform.position;
            var rot = Quaternion.LookRotation(relativePos);

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 3 * Time.deltaTime);

            if (CanGoForward())
            {
                distanceToTarget = (transform.position - target).sqrMagnitude;

                if (distanceToTarget > 0.5f * 0.5f)
                {
                    //rotates perpendicular to the ground
                    Vector3 forward = transform.TransformDirection(Vector3.forward);
                    controller.SimpleMove(forward * Speed);
                }
                else
                {
                    bool wantEat = Random.Range(0, 1f) < 0.2f;

                    if (wantEat)
                    {
                        StartCoroutine(Eating(EatTime));
                    }

                    going = false;
                }
            }
            else
            {
                going = false;
            }
        }
    }

    void Run()
    {
        var dir = -(dogPosition - transform.position).normalized;
        dir.y = transform.forward.y;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, DstToObstacles, obstacleMask))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
            }
        }
        if (Physics.SphereCast(transform.position, 1, transform.forward, out hit, DstBetweenSheeps, sheepMask))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
            }
        }

        var dirL = Quaternion.AngleAxis(-25, transform.up) * transform.forward * (1 + transform.localScale.x / 2);
        dirL.y = transform.forward.y;

        var dirR = Quaternion.AngleAxis(25, transform.up) * transform.forward * (1 + transform.localScale.x / 2);
        dirR.y = transform.forward.y;

        if (Physics.Raycast(transform.position, dirL, out hit, DstToObstacles, obstacleMask))
        {
            if (hit.transform != transform)
            {
                dir -= hit.normal * 50;
            }
        }
        if (Physics.SphereCast(transform.position,1, dirL, out hit, DstBetweenSheeps, sheepMask))
        {
            if (hit.transform != transform)
            {
                dir -= hit.normal * 50;
            }
        }

        if (Physics.Raycast(transform.position, dirR, out hit, DstToObstacles, obstacleMask))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
            }
        }
        if (Physics.SphereCast(transform.position,1, dirR, out hit, DstBetweenSheeps, sheepMask))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
            }
        }

        if (scared)
        {
            dir += -(dogPosition - transform.position).normalized * 1000;
        }

        var rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 3 * Time.deltaTime);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        controller.SimpleMove(forward * RunSpeed);
    }

    //temp eat animation
    IEnumerator Eating(float time)
    {
        eating = true;
        var clr = mesh.material.color;
        for (float ft = time; ft >= 0; ft -= 0.01f)
        {
            if (ft < 0.01f) ft = 0;

            mesh.material.color = new Color(ft, ft, ft);

            yield return new WaitForSeconds(0.01f);
        }
        mesh.material.color = clr;
        eating = false;
    }

    public void Scared()
    {
        StartCoroutine(BarkScare());
    }

    IEnumerator BarkScare()
    {
        scared = true;
        yield return new WaitForSeconds(3);
        scared = false;
        StopCoroutine(BarkScare());
    }

    Vector3 GetRandomPointOnTheGroundInRange(float range)
    {
        Vector3 rndPoint = new Vector3(Random.Range(-range, range), 10, Random.Range(-range, range));

        if (Physics.Raycast(transform.position + rndPoint, Vector3.down, out var hitG, 100, groundMask))
        {
            if (!Physics.SphereCast(transform.position + rndPoint, transform.localScale.x + 0.1f, Vector3.down, out var hitO, 100f, obstacleMask))
            {
                rndPoint = new Vector3(hitG.point.x, hitG.point.y + (transform.localScale.y / 2) + 0.01f, hitG.point.z);

                if (!Physics.Raycast(transform.position, rndPoint - transform.position, out var hitP, 100f, obstacleMask))
                {
                    return rndPoint;
                }
            }
        }

        return Vector3.zero;
    }

    RaycastHit[] hitO;
    RaycastHit[] hitS;


    void ObstaclesAvoidance()
    {
        var avoidanceDir = transform.forward.normalized;

        hitO = new RaycastHit[DensityOfView];
        hitS = new RaycastHit[DensityOfView];

        float angle = ViewingAngle;
        float angleDif = angle * 2 / DensityOfView;
        var raysOrigin = transform.position + controller.center;

        for (int i = 0; i < DensityOfView; i++)
        {
            var dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward * DstBetweenSheeps;
            dir.y = transform.forward.y;
            Debug.DrawRay(raysOrigin, dir, Color.blue);
            if (Physics.SphereCast(raysOrigin, transform.localScale.x, dir, out hitO[i], DstToObstacles, obstacleMask))
            {
                avoidanceDir += hitO[i].normal * 50;
                break;
            }
            else if (Physics.SphereCast(raysOrigin, transform.localScale.x, dir, out hitS[i], DstBetweenSheeps, sheepMask))
            {
                avoidanceDir += hitS[i].normal * 50;
                break;
            }
            angle = angle - angleDif;
        }
        var rot = Quaternion.LookRotation(avoidanceDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.5f);
    }

    bool CanGoForward()
    {
        hitO = new RaycastHit[DensityOfView];
        hitS = new RaycastHit[DensityOfView];

        float angle = ViewingAngle;
        float angleDif = angle * 2 / DensityOfView;
        var raysOrigin = transform.position + controller.center;

        for (int i = 0; i < DensityOfView; i++)
        {
            var dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward * DstBetweenSheeps;
            dir.y = transform.forward.y;
            Debug.DrawRay(raysOrigin, dir, Color.blue);
            if (Physics.SphereCast(raysOrigin, transform.localScale.x, dir, out hitO[i], DstToObstacles, obstacleMask))
            {
                return false;
            }
            else if (Physics.SphereCast(raysOrigin, transform.localScale.x, dir, out hitS[i], DstBetweenSheeps, sheepMask))
            {
                return false;
            }
            angle = angle - angleDif;
        }

        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hitO != null)
        {
            //draw point of collision with obstacles
            for (int i = 0; i < hitO.Length; i++)
            {
                if (hitO[i].point != Vector3.zero)
                    Gizmos.DrawSphere(hitO[i].point, 0.2f);
            }
        }
        if (hitO != null)
        {
            //draw point of collision with another sheep
            for (int i = 0; i < hitS.Length; i++)
            {
                if (hitS[i].point != Vector3.zero)
                    Gizmos.DrawSphere(hitS[i].point, 0.1f);
            }
        }

        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(transform.position + controller.center, DstBetweenSheeps);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 0.1f);
    }
}
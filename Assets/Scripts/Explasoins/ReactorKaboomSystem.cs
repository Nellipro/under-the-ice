using UnityEngine;

public class ReactorKaboomSystem : MonoBehaviour
{
    public GameObject BigBoom;
    public Reactor reactor;
    public float growthSpeed = 1f;

    [SerializeField] private ParticleSystem explosionParticles;


    [SerializeField] private Transform boomParent;

    private bool hasExploded;
    private Transform boomTransform;



    void Update()
    {
        if (reactor.isExploding && !hasExploded)
        {
            hasExploded = true;

            ParticleSystem particles = Instantiate(
                explosionParticles,
                transform.position,
                transform.rotation,
                boomParent.transform
            );

            GameObject boom = Instantiate(
                BigBoom,
                transform.position,
                transform.rotation,
                boomParent.transform
            );

            boomTransform = boom.transform;
        }

        // if (boomTransform != null)
        // {
        //     growthSpeed += Time.deltaTime;

        //     boomTransform.localScale += Vector3.one * growthSpeed * Time.deltaTime;
        // }
    }
}
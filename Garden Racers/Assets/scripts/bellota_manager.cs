using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class bellota_manager : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public GameObject vfxPrefab;
    private Transform bellotaPoint;
    private bool isCollected = false;
    private Animator animator;
    private static readonly int consumoHash = Animator.StringToHash("consumo");
    private static readonly int brokeHash = Animator.StringToHash("broke");


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Rotación continua de la bellota
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Si fue recogida, seguir la posición en el auto
        if (isCollected && bellotaPoint != null)
        {
            transform.position = bellotaPoint.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Debug.Log("Burbuja colisionada con el Player");
            CollectBellota(other.transform);
        }
    }

    public void CollectBellota(Transform playerTransform)
    {
        if (isCollected) return; // Evitar múltiples recolecciones

        bellotaPoint = FindBellotaPoint(playerTransform);
        if (bellotaPoint == null)
        {
            Debug.LogError("No se encontró BellotaPoint.");
            return;
        }

        isCollected = true;
        transform.SetParent(bellotaPoint);
        transform.localPosition = Vector3.zero;

        // Instanciar el efecto visual
        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            Debug.Log("VFX instanciado en: " + transform.position);
        }

        // Reiniciar el estado de recolección del auto
        if (playerTransform.TryGetComponent(out Rubemori_Car car))
        {
            car.ResetCollectingState();
        }

        StartCoroutine(PlayDestructionAnimation());
    }

    private IEnumerator PlayDestructionAnimation()
    {
        animator.SetBool(consumoHash, true);
        animator.SetTrigger(brokeHash);
        Debug.Log("Animación de destrucción activada.");

        // Esperar a que la animación termine
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        Destroy(gameObject);
    }

    private Transform FindBellotaPoint(Transform root)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            if (child.name == "BellotaPoint")
                return child;
        }
        return null;
    }
}




















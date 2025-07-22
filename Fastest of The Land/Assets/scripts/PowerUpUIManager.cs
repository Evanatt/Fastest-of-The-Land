using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Necesario para la entrada del ratón
using System.Collections;

public class PowerUpUIManager : MonoBehaviour
{
    public Image powerUpFillImage;
    public Button powerUpButton;
    public AudioClip readySound;
    public AudioClip abilitySound;
    public AudioClip disabledSound;

    private AudioSource audioSource;

    public float FILL_INCREMENT = 0.25f;
    public float MAX_FILL_AMOUNT = 1f;
    public float emptyTime = 3.0f;

    public float FILL_SPEED = 2f;

    private float currentFillAmount = 0f;
    private float targetFillAmount = 0f;
    private bool isEmptying = false;
    private bool isReadySoundPlayed = false;
    private float emptyLerpTime = 0f;

    // Referencia al script de la habilidad
    public StormAbility stormAbility;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        powerUpButton.interactable = false;
        powerUpFillImage.fillAmount = 0f;

        bellota_manager.OnBellotaCollected += IncreaseFillAmount;
    }

    private void OnDestroy()
    {
        bellota_manager.OnBellotaCollected -= IncreaseFillAmount;
    }

    void Update()
    {
        currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * FILL_SPEED);
        powerUpFillImage.fillAmount = currentFillAmount;

        if (!isReadySoundPlayed && currentFillAmount >= 0.99f)
        {
            powerUpButton.interactable = true;
            if (audioSource && readySound)
            {
                audioSource.PlayOneShot(readySound);
            }
            isReadySoundPlayed = true;
        }

        if (isEmptying)
        {
            emptyLerpTime += Time.deltaTime / emptyTime;
            currentFillAmount = Mathf.Lerp(1f, 0f, emptyLerpTime);
            powerUpFillImage.fillAmount = currentFillAmount;

            if (currentFillAmount <= 0f)
            {
                powerUpButton.interactable = false;
                isEmptying = false;
                isReadySoundPlayed = false;
                emptyLerpTime = 0f;
            }
        }

        if (!powerUpButton.interactable && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (audioSource && disabledSound)
                {
                    audioSource.PlayOneShot(disabledSound);
                }
            }
        }
    }

    public void IncreaseFillAmount()
    {
        if (targetFillAmount < MAX_FILL_AMOUNT)
        {
            targetFillAmount += FILL_INCREMENT;
            targetFillAmount = Mathf.Clamp(targetFillAmount, 0f, MAX_FILL_AMOUNT);
        }
    }

    public void ActivatePowerUp()
    {
        if (powerUpButton.interactable)
        {
            if (audioSource && abilitySound)
            {
                audioSource.PlayOneShot(abilitySound);
            }

            // Activar la habilidad de la nube
            stormAbility.StartStorm();

            isEmptying = true;
            emptyLerpTime = 0f;
            targetFillAmount = 0f;
        }
        else
        {
            if (audioSource && disabledSound)
            {
                audioSource.PlayOneShot(disabledSound);
            }
        }
    }
}

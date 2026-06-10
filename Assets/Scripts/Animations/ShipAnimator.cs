using System;
using UnityEngine;

public class ShipAnimator : MonoBehaviour
{
    [SerializeField] private Kart kart;
    [SerializeField] private GameObject boosterFX;
    [Header("Tilt settings")]
    public float maxTilt = 20f;    
    public float tiltSpeed = 10f;
    
    private void Update()
    {
        float direction = kart.MoveDirection.x; 

        TiltShip(direction);
        ToggleBoosterFX(direction != 0);
    }

    private void TiltShip(float direction)
    {
        float targetTilt = -direction * maxTilt;

        // transform.localEulerAngles restituisce angoli da 0 a 360.
        // Per evitare glitch matematici nel Lerp, convertiamo l'angolo in una rotazione Quaternion
        Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, targetTilt);
        
        // Sfumiamo la rotazione attuale verso quella desiderata
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

    private void ToggleBoosterFX(bool isMoving)
    {
        boosterFX.SetActive(isMoving);
    }
}

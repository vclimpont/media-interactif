using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRotator : MonoBehaviour
{
    public Transform anchorTransform;
    public float rotationLimit;
    public float cancelSpeed;

    float mouseSpeed;

    // Update is called once per frame
    void Update()
    {
        mouseSpeed = Input.GetAxis("Mouse X"); // Recupère la vitesse de la souris en X ( < 0 si dép. vers la gauche | > 0 si dép. vers la droite)
    }

    private void RotateCard(float z) // Effectue la rotation de la carte. La valeur de la rotation est z
    {
        anchorTransform.Rotate(new Vector3(0, 0, z), Space.World);
    }

    private float GetAngle(float _angle) // Transforme un angle d'euler en valeur d'angle inscrit dans l'inspecteur Unity
    {
        if(_angle >= 200f && _angle <= 360f)
        {
            return _angle - 360f;
        }
        else
        {
            return _angle;
        }
    }

    IEnumerator CancelRotation() // Interpole la rotation de la carte jusqu'à ce qu'elle revenienne à sa rotation initale (0,0,0)
    {
        Quaternion currRot = anchorTransform.rotation; // récupère la rotation actuelle de la carte
        Quaternion rotZero = Quaternion.Euler(Vector3.zero); // définie la rotation cible (0,0,0)
        float start = Time.time; // récupère le temps actuel
        float t = start;
        float end = start + cancelSpeed; // définie le temps de rotation souhaité

        while(t < end)
        {
            t = (Time.time - start) / cancelSpeed;
            anchorTransform.rotation = Quaternion.Lerp(currRot, rotZero, t);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnMouseDrag() // Tant que la souris est enfoncée sur la carte
    {
        StopCoroutine("CancelRotation"); // arrête la potentielle interpolation

        float z = GetAngle(anchorTransform.eulerAngles.z); // récupère la rotation actuelle

        z += -mouseSpeed * 2; // ajoute une rotation relative à la vitesse de la souris

       if(!((z >= rotationLimit && mouseSpeed < 0) || (z <= -rotationLimit && mouseSpeed > 0))) // si la rotation souhaitée ne dépasse pas les limites de rotations indiquées
        {
            RotateCard(-mouseSpeed * 2); // effectue la rotation
        }
    }

    private void OnMouseUp() // Lorsque la souris est relachée de la carte
    {
        StartCoroutine("CancelRotation");
    }
}

using System.Collections;
using UnityEngine;

public class CardRotator : MonoBehaviour
{
    public Transform anchorTransform;
    public SpriteRenderer srLeftText;
    public SpriteRenderer srRightText;
    public float rotationLimit;
    public float cancelSpeed;

    public GameObject[] nextCards;

    private float mouseSpeed;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseSpeed = Input.GetAxis("Mouse X"); // Recupère la vitesse de la souris en X ( < 0 si dép. vers la gauche | > 0 si dép. vers la droite)
    }

    private void SwitchCard()
    {
        int i = Random.Range(0, nextCards.Length);
        Instantiate(nextCards[i], new Vector3(0f, 0f, 0f), Quaternion.identity);
    }

    private void DisplayTexts(float z)
    {
        Color clt = srLeftText.color;
        clt.a = (z - (rotationLimit - 5f)) / 5f;
        srLeftText.color = clt;

        Color crt = srRightText.color;
        crt.a = (z + (rotationLimit - 5f)) / -5f;
        srRightText.color = crt;
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
        float t = 0;

        while(t < 1)
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
        DisplayTexts(z);

        z += -mouseSpeed * 2; // ajoute une rotation relative à la vitesse de la souris

       if(!((z >= rotationLimit && mouseSpeed < 0) || (z <= -rotationLimit && mouseSpeed > 0))) // si la rotation souhaitée ne dépasse pas les limites de rotations indiquées
        {
            RotateCard(-mouseSpeed * 2); // effectue la rotation
        }
    }

    private void OnMouseUp() // Lorsque la souris est relachée de la carte
    {
        float z = GetAngle(anchorTransform.eulerAngles.z); // récupère la rotation actuelle

        if(z > rotationLimit - 5f || z < -rotationLimit + 5f)
        {
            SwitchCard();
            Camera.main.GetComponent<Animator>().Play("shake");
            rb.gravityScale = 20f;
            Destroy(transform.root.gameObject, 0.4f);
        }
        else
        {
            StartCoroutine("CancelRotation");
        }
    }
}

using UnityEngine;
using TMPro;

public class CherryCollision : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CherriesText;
    private int cherries = 0;

    void OnTriggerEnter2D(Collider2D cherry) 
    {
        if(cherry.gameObject.CompareTag("Cherry"))
        {
            Destroy(cherry.gameObject);
            cherries++;
            CherriesText.text = "Cherries: " + cherries;
        }
    }
}

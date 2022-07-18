using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int Health;
    public Image[] Heart;
    public Sprite Full_Heart;
    public Sprite Empty_Heart;

    private int Heart_Count = 5;
    private bool CanBeHit = true;

    [SerializeField] private Animator anim;
    [SerializeField]private Rigidbody2D rb;

    
    void Start()
    {
        Health = Heart_Count;  
    }

    
    void Update()
    {
        for(int i = 0; i < Heart.Length; i++)
        {
            if(i < Health)
            {
                Heart[i].sprite = Full_Heart;
            } else {
                Heart[i].sprite = Empty_Heart;
            }

        }
        
    }



    private void OnCollisionStay2D(Collision2D Obstacle) 
    {
        if(Obstacle.gameObject.CompareTag("Damage") && CanBeHit)
        {
            Health -= 1;
            anim.SetTrigger("Hit");
            CanBeHit = false;
            rb.AddForce(new Vector2(10,3), ForceMode2D.Impulse);
            StartCoroutine(YesItCan());
        }
           if(Health <= 0)
        {
            anim.ResetTrigger("Hit");
            die();
        }
    }
    private void OnCollisionExit2D(Collision2D other) {
        anim.ResetTrigger("Hit");
    }

    private IEnumerator YesItCan(){
        yield return new WaitForSeconds(0.5f);
        CanBeHit = true;
    }


    private void die()
    {
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<BoxCollider2D>().enabled= false;
        GetComponent<PlayerMovementVer2>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        anim.SetTrigger("Death");
        
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}

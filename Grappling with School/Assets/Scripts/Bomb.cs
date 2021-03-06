using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float bombDelay;
    public float animationDelay;

    [SerializeField]
    float bombRange;

    public Animator animator;
    bool isBlowingUp;
    SpriteRenderer sp;
    CircleCollider2D cd;

    public List<string> destroyable = new List<string> { "Player", "Assignment", "Breakable", "Hook", "Movable" };
    public List<string> triggers = new List<string> { "Player", "Hook" };

    public AudioSource audioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        isBlowingUp = false;
        sp = GetComponent<SpriteRenderer>();
        cd = GetComponent<CircleCollider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBlowingUp)
        {
            if (triggers.Contains(collision.gameObject.tag))
            {
                startBlowingUp();
            }
        }
    }

    public void startBlowingUp()
    {
        audioPlayer.Play();
        StartCoroutine(BlowUp());
    }

    IEnumerator BlowUp()
    {
        isBlowingUp = true;
        sp.color = Color.red;
        yield return new WaitForSeconds(bombDelay);
        sp.color = Color.white;
        animator.SetTrigger("blowUp");
        audioPlayer.Pause();
        audioPlayer = GameObject.Find("Bomb explode").GetComponent<AudioSource>();
        audioPlayer.Play();
        yield return new WaitForSeconds(animationDelay/2);
        checkRadius();
        yield return new WaitForSeconds(animationDelay/2);
        Destroy(this.gameObject);
    }

    public void checkRadius()
    {
        var hitColliders = Physics2D.OverlapCircleAll(transform.position, bombRange);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider);
            if (hitCollider.gameObject.CompareTag("Player"))
            {
                hitCollider.gameObject.GetComponent<PlayerController>().Damage(2);
            } else if (hitCollider.gameObject.CompareTag("Hook"))
            {
                hitCollider.gameObject.GetComponent<Hook>().Delete();
            }
            else if (hitCollider.gameObject.CompareTag("Assignment"))
            {
                if(hitCollider.gameObject.GetComponent<AssignmentController>())
                    hitCollider.gameObject.GetComponent<AssignmentController>().Die();
            }
            else if (destroyable.Contains(hitCollider.gameObject.tag))
            {
                Destroy(hitCollider.gameObject);
            }
        }
    }
}

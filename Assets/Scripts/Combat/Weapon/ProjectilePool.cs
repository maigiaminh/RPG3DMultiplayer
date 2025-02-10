using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public Arrow arrowPrefab;
    public WizardProjectile projectilePrefab;
    public int poolSize = 10;
    private int _damage;
    private Queue<Arrow> arrowPool = new Queue<Arrow>();
    private Queue<WizardProjectile> projectilePool = new Queue<WizardProjectile>();
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Arrow arrow = Instantiate(arrowPrefab);
            arrow.gameObject.SetActive(false); 
            arrowPool.Enqueue(arrow);
            arrow.transform.parent = transform;
            arrow.SetAudioSource(arrow.AddComponent<AudioSource>());

            WizardProjectile projectile = Instantiate(projectilePrefab);
            projectile.gameObject.SetActive(false); 
            projectilePool.Enqueue(projectile);
            projectile.transform.parent = transform;
        }
    }

    public Arrow GetArrow()
    {
        Arrow arrow;

        if (arrowPool.Count > 0)
        {
            arrow = arrowPool.Dequeue();
            arrow.gameObject.SetActive(true);
        }
        else
        {
            arrow = Instantiate(arrowPrefab);
            
        }

        arrow.SetDamage(_damage);
        arrow.SetParent(this);
        arrow.transform.parent = null;
        SoundManager.PlaySound(SoundType.ARROW_FLY, arrow.AudioSource);

        return arrow;
    }

    public WizardProjectile GetProjectile()
    {
        WizardProjectile projectile;
        if (projectilePool.Count > 0)
        {
            projectile = projectilePool.Dequeue();
            projectile.gameObject.SetActive(true);
            
        }
        else
        {
            projectile = Instantiate(projectilePrefab);
        }
        
        projectile.SetDamage(_damage);
        projectile.SetParent(this);
        projectile.transform.parent = null;
        return projectile;

    }
    public void ReturnArrow(Arrow arrow)
    {
        arrow.transform.parent = transform;
        arrow.gameObject.SetActive(false);
        arrowPool.Enqueue(arrow);
    }

    public void ReturnProjectile(WizardProjectile projectile){
        projectile.transform.parent = transform;
        projectile.gameObject.SetActive(false);
        projectilePool.Enqueue(projectile);
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

}   

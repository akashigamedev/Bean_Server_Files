using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingScript : MonoBehaviour
{

    [Header("Gun settings")]
    public float fireRate = 0.1f;
    public float reloadRate = 2f;
    public int clipSize = 10;
    public int ReservedAmmoCapacity = 90;
    public bool isAuto;

    //aiming
    [Header("Aiming settings")]
    public Vector3 normalLocalPosision;
    public Vector3 aimingLocalPosision;

    public float aimSmoothing = 10;
    public float weaponSwayAmount = 10;
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;
    // you only need to assign this if randomizet recoil is of
    public Vector2 recoilPattern;

    [Header("Mouse sensetivity")]

    public float mouseSensetivity = 1;
    Vector2 _currentRotation;
       

    //variables that change
    bool _canShoot;
    int _currentAmmoInClip;
    int _ammoInReserve;




    [SerializeField] KeyCode reloadKey = KeyCode.R;
    

    Animator animator;
    void Start()
    {
        _currentAmmoInClip = clipSize;
        _ammoInReserve = ReservedAmmoCapacity;
        _canShoot = true;

        animator = GetComponent<Animator>();

        ShootingEffect = GameObject.Find("bullet exit").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        determineAim();
        determineRotation();
        if(isAuto == true)
        {
            AutoShoot();
        }else if (isAuto == false)
        {
            NotAutoShoot();
        }
    }
    ParticleSystem ShootingEffect;
    void Shooting()
    {
        ShootingEffect.Play();
        animator.SetTrigger("isShooting");
        _canShoot = false;
        _currentAmmoInClip -= 1;
        StartCoroutine(ShootGun());
        soundManagerScript.PlaySound("fire");
    }
    void Reloading()
    {
        _canShoot = false;
        animator.SetTrigger("isReloading");
        StartCoroutine(ReloadGun());
        int amountNeeded = clipSize - _currentAmmoInClip;
        soundManagerScript.PlaySound("reload");

        if (amountNeeded >= _ammoInReserve)
        {
            _currentAmmoInClip += _ammoInReserve;
            _ammoInReserve -= amountNeeded;
        } else
        {
            _currentAmmoInClip = clipSize;
            _ammoInReserve -= amountNeeded;
               
        }
    }
    void AutoShoot()
    {
        if (Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
        {
            Shooting();
        }
        else if (Input.GetMouseButtonDown(0) && _canShoot && _currentAmmoInClip <= 0)
        {
            animator.SetTrigger("isShootingNoAmmo");
        }
        else if (Input.GetKeyDown(reloadKey) && _canShoot && _currentAmmoInClip < clipSize && _ammoInReserve > 0)
        {
            Reloading();
        }
    }
    void NotAutoShoot()
    {
        if (Input.GetMouseButtonDown(0) && _canShoot && _currentAmmoInClip > 0)
        {
            Shooting();
        }
        else if (Input.GetMouseButtonDown(0) && _canShoot && _currentAmmoInClip <= 0)
        {
            animator.SetTrigger("isShootingNoAmmo");
            soundManagerScript.PlaySound("dryFire");
        }
        else if (Input.GetKeyDown(reloadKey) && _currentAmmoInClip < clipSize && _ammoInReserve > 0)
        {
            Reloading();
        }
    }
    void determineRotation()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseAxis *= mouseSensetivity;
        _currentRotation += mouseAxis;
        transform.localPosition += (Vector3)mouseAxis * weaponSwayAmount / 1000;

        /*transform.root.localRotation = Quaternion.AngleAxis(_currentRotation.x, Vector3.up);
        transform.parent.localRotation = Quaternion.AngleAxis(- _currentRotation.x, Vector3.right);*/

    }
    void determineAim()
    {
        Vector3 target = normalLocalPosision;
        if (Input.GetMouseButton(1)) target = aimingLocalPosision;

        Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);

        transform.localPosition = desiredPosition;
    }
    void determineRecoil()
    {
        transform.localPosition -= Vector3.forward * 0.1f;
        if (randomizeRecoil)
        {
            float xRecoil = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
            float yRecoil = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

            Vector2 recoil = new Vector2(xRecoil, yRecoil);
        }
    }

    IEnumerator ShootGun()
    {
        determineRecoil();
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }
    IEnumerator ReloadGun()
    {
        yield return new WaitForSeconds(reloadRate);
        _canShoot = true;
    }
}
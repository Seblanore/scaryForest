using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : NetworkBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;

    [SerializeField] private RayCastController rayCastController;

    public AudioSource source;
    public AudioClip fireSound;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
    }

    public void PullTheTrigger()
    {
        gunAnimator.SetTrigger("Fire");
    }


    //This function creates the bullet behavior
    [ServerRpc]
    void ShootServerRpc()
    {
        if (muzzleFlashPrefab)
        {
            MuzzleFlashClientRpc();
        }

        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }

        // Create a bullet and add force on it in direction of the barrel
        //GameObject go = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation) as GameObject;
        GameObject go = NetworkObjectPool.Singleton.GetNetworkObject(bulletPrefab, barrelLocation.position, barrelLocation.rotation).gameObject;
        go.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        go.GetComponent<NetworkObject>().Spawn(true);
  
        // Shoot rayCast ServerRpc
        rayCastController.ShootServerRpc(barrelLocation.position, barrelLocation.forward);
    }

    [ClientRpc]
    private void MuzzleFlashClientRpc() {
        source.PlayOneShot(fireSound);
        //Create the muzzle flash
        GameObject tempFlash;
        tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

        //Destroy the muzzle flash effect
        Destroy(tempFlash, destroyTimer);
    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }
}

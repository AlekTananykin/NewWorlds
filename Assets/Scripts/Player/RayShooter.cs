using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RayShooter : MonoBehaviour
{
    [SerializeField] private GameObject _granadePrefab;
    [SerializeField] private GameObject _minePrefab;

    [SerializeField] private float _granadeThrowForce = 8f;
    [SerializeField] private float _mineThrowForce = 1.0f;

    [SerializeField] private ParticleSystem _burstPrefab;
    private ParticleSystem _burst;

    enum Weapon {LaserRay, Mine, Granade };
    private Weapon _selectedWeapon = Weapon.LaserRay;

    public void Awake()
    {
        _burst = Instantiate(_burstPrefab);
    }


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (0 == Time.timeScale)
            return;

        SelectWeapon();
        if (Input.GetMouseButtonDown(0))
        {
            Camera camera = Camera.main;
            Vector3 point = new Vector3(
                camera.pixelWidth / 2, camera.pixelHeight / 2, 0);

            Ray ray = camera.ScreenPointToRay(point);

            switch (_selectedWeapon)
            {
                case Weapon.LaserRay:
                {
                    ShooteByRay(ray);
                    break;
                }
                case Weapon.Granade:
                {
                    //ToThrowGranade(ray);
                    break;
                }
                case Weapon.Mine:
                {
                    //ToThrowMine(ray);
                    break;
                }
            }
        }
    }

    private void SelectWeapon()
    {
        if (Input.GetKey(KeyCode.Alpha1))
            _selectedWeapon = Weapon.LaserRay;
        else if (Input.GetKey(KeyCode.Alpha2))
            _selectedWeapon = Weapon.Granade;
        else if (Input.GetKey(KeyCode.Alpha3))
            _selectedWeapon = Weapon.Mine;
    }

    private void ShooteByRay(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            IReactToHit target = hitObject.GetComponent<IReactToHit>();

            if (null != target)
                target.ReactToHit(50);
            
            _burst.transform.position = hit.point;

            _burst.transform.rotation = Quaternion.LookRotation(
                transform.position - hit.point);
            _burst.Play();
        }
    }

    private IEnumerator ShareIndicator(Vector3 point)
    {
        GameObject sphere =
            GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphere.transform.position = point;

        yield return new WaitForSeconds(1);

        Destroy(sphere);
    }

    private void ToThrowGranade(Ray ray)
    {
        GameObject granade = Instantiate(_granadePrefab);
        //granade.GetComponent<GranadeReaction>().Activate();
        ToThrow(granade, ray, _granadeThrowForce);
    }

    private void ToThrowMine(Ray ray)
    {
        GameObject mine = Instantiate(_minePrefab);
        ToThrow(mine, ray, _mineThrowForce);
    }

    private void ToThrow(GameObject bomb, Ray ray, float force)
    {
        bomb.transform.position =
            transform.position + transform.forward;

        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();
        bombRb.AddForce(ray.direction * force, ForceMode.Impulse);
    }
}

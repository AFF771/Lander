using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Ship : MonoBehaviour
{
    [SerializeField]
    float fuel; // fuel quantity

    float FuelBurnedThrust = 10f; // fuel spent on thrust
    float FuelBurnedTorque = 5f; // fuel spent on torque

    [SerializeField]
        TextMeshProUGUI FuelQ; // FuelTank text display
    [SerializeField]
        float MaxVelocity = 2f; // serialized max velocity for impact
    [SerializeField]
        float MaxRotationPos = 20f; // serialized max rotation for impact
    [SerializeField]
        GameObject Explosion; // explosion animation
    [SerializeField]
        Image RightArrow; // right directional arrow
    [SerializeField]
        Image LeftArrow; // left directional arrow
    [SerializeField]
        Image DownArrow; // down directional arrow
    [SerializeField]
        Image RedLight; // red light -> ship exploded on impact
    [SerializeField]
        Image YellowLight; // yellow light -> landing off-target or empty fuel
    [SerializeField]
        Image GreenLight; // green light -> landing successful
    [SerializeField]
        float ThrustForce = 100f; // Thrust -> propuslive force of a jet or engine
    [SerializeField]
        float TorqueForce = 50f; //Torque -> tendency of a force to ratate the body to witch it is applied

    bool ShipDisabled = false; // ship is disabled after landing/crashing

    bool changescene = false; // scene change

    bool GameOver = false;

    float time = 0f;

    public void Start()
    {
        FuelQ.text = ("Fuel: " + fuel);

        float X = Random.Range(-5, -2);

        if (Random.Range(0f, 1f) > 0.5) // ship can't spawn directly over the platform
        {
            X = Mathf.Abs(Random.Range(-5, -2)); // Ship has equal change of spawning on either sides of the platform
        }

        Vector2 PositionAux = transform.position;
        PositionAux.x = X;
        transform.position = PositionAux;
    }

    public void OfflineSystems() // player has landed/crashed the ship or fuel tank is empty
    {
        ShipDisabled = true;
    }


    public void Update()
    {
        if(changescene == true)
        {
            time += Time.deltaTime;
            if (time > 2f) // scene changes  after 2 Sec
            {
                if (GameOver == true)
                {
                    time = 0f;
                    changescene = false;
                    SceneManager.LoadScene("GameOver");
                }
                else
                {
                time = 0f;
                changescene = false;
                SceneManager.LoadScene("Win");
                }
            }      
        }


        if (ShipDisabled == false) // player has no control over the ship after landing
        {

            if (Input.GetKey(KeyCode.Space))
            {
                GetComponent<Rigidbody2D>().AddForce(transform.up * ThrustForce * Time.deltaTime); //Calculates thrust applied to the ship according to <transform.up> vector
                
                Image image = DownArrow.GetComponent<Image>(); // <DownArrow> opacity = 1
                float A = 1f;
                ArrowColor(image,A);

                if (fuel > 0)
                {
                    fuel -= FuelBurnedThrust*Time.deltaTime;
                    FuelQ.text = ("Fuel: " + fuel.ToString("0"));
                }
                else
                {
                    EmptyFuelTank();
                }

            } else
            {
                Image image = DownArrow.GetComponent<Image>(); // <DownArrow> opacity = 0.2
                float A = 0.2f;
                ArrowColor(image, A);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                GetComponent<Rigidbody2D>().AddTorque(-TorqueForce * Time.deltaTime); // Calculates right Torque applied to the ship
                
                Image image = RightArrow.GetComponent<Image>(); // <RightArrow> opacity = 1
                float A = 1f;
                ArrowColor(image,A);

                if (fuel > 0)
                {
                    fuel -= FuelBurnedTorque * Time.deltaTime;
                    FuelQ.text = ("Fuel: " + fuel.ToString("0"));
                }
                else
                {
                    EmptyFuelTank();
                }

            } else
            {
                Image image = RightArrow.GetComponent<Image>(); // <RightArrow> opacity = 0.2
                float A = 0.2f;
                ArrowColor(image, A);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                GetComponent<Rigidbody2D>().AddTorque(TorqueForce * Time.deltaTime); // Calculates left Torque applied to the ship 
                
                Image image = LeftArrow.GetComponent<Image>(); // <LeftArrow> opacity = 1
                float A = 1f;
                ArrowColor(image,A);

                if (fuel > 0)
                {
                    fuel -= FuelBurnedTorque * Time.deltaTime;
                    FuelQ.text = ("Fuel: " + fuel.ToString("0"));
                }
                else
                {
                    EmptyFuelTank();
                }


            } else
            {
                Image image = LeftArrow.GetComponent<Image>(); // <LefttArrow> opacity = 0.2
                float A = 0.2f;
                ArrowColor(image, A);
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        float MaxRotationNeg = 360f - MaxRotationPos; // negative rotation is calculated for positive value (ex: -20º = 340º)

        if (ShipDisabled == false) // outcome is evaluated on the first impact
        {
            if (collision.gameObject.tag == "Platform") // Landing on platform
            {
                if (collision.relativeVelocity.magnitude > MaxVelocity ||                                                                           // set max velocity
                    (Mathf.Abs(transform.localEulerAngles.z) > MaxRotationPos)&& (Mathf.Abs(transform.localEulerAngles.z) < MaxRotationNeg) ||      // set max positive rotation
                    (Mathf.Abs(transform.localEulerAngles.z) < MaxRotationNeg) && (Mathf.Abs(transform.localEulerAngles.z) > MaxRotationPos))       // set max negative rotation
                {
                    Image light = RedLight.GetComponent<Image>(); // RedLight is displayed (opacity 1)
                    float A = 1f;
                    Light(light,A);     
                
                    ShipExplosion();
                    OfflineSystems();
                    GameOver = true;
                    changescene = true;
                    Update();
                }
                else
                {
                    Image light = GreenLight.GetComponent<Image>(); // GreenLight is displayed (opacity 1)
                    float A = 1f;
                    Light(light, A);

                    OfflineSystems();

                    changescene = true;
                    Update();
                }
            }
            else if (collision.gameObject.tag == "moon")// Landing off platform
            {
                if (collision.relativeVelocity.magnitude > MaxVelocity ||
                    (Mathf.Abs(transform.localEulerAngles.z) > MaxRotationPos) && (Mathf.Abs(transform.localEulerAngles.z) < MaxRotationNeg) ||
                    (Mathf.Abs(transform.localEulerAngles.z) < MaxRotationNeg) && (Mathf.Abs(transform.localEulerAngles.z) > MaxRotationPos))
                {
                    Image light = RedLight.GetComponent<Image>();
                    float A = 1f;
                    Light(light, A);

                    ShipExplosion();
                    OfflineSystems();
                    GameOver = true;
                    changescene = true;
                    Update();
                }
                else
                {
                    Image light = YellowLight.GetComponent<Image>();
                    float A = 1f;
                    Light(light, A);

                    OfflineSystems();
                    GameOver = true;
                    changescene = true;
                    Update();
                }
            }
        }


    }

    public void ArrowColor(Image image, float A)
    {
        Color c = image.color;
        c.a = A;
        image.color = c;
    }

    public void Light(Image light, float A)
    {
        Color c = light.color;
        c.a = A;
        light.color = c;
    }


    public void ShipExplosion() // displays explosion sprite on the place of impact
    {
        if (ShipDisabled == false)
        {
        Vector3 CurrentPosition = transform.position;
        Quaternion CurrentRotation = transform.rotation;

        Instantiate(Explosion, CurrentPosition, CurrentRotation);
        }
    }

    public void EmptyFuelTank()
    {
        Image light = YellowLight.GetComponent<Image>();
        float A = 1f;
        Light(light, A);

        OfflineSystems();
        FuelQ.text = ("EMPTY");
        changescene = true;
        GameOver = true;
        Update();
    }
 
}


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Eheli_Weaponsystem : MonoBehaviour
{
    bool is_in_view;
    public bool is_encounterrange, isground = true, IsMI17 = false,isbiggun=false;
    public int cycle = 50;
    int counter = 0;
    public int rangeofCounter = 15;
    public float rangeOfEncounter = 11000;
    public float rangeofview = 300;
    public Rigidbody Bullet;
    //	private float Nextfire=0.0f;
    public float bulletspeed = 1200;
    public GameObject Plyer;
    public GameObject closest;
    public string Ttag = "P", Ttag1;

    public float delt_time_fire = 3.5f;
    float Nextfire = 0f;
    List<Rigidbody> Roketlist;
    public int Magsize = 1;

    Quaternion targetRotaion;
    public float LockedObjectAngle,Reloadtime;
    float Angle, _Angle;
    Vector3 DirectionValue, direction;
    public FlightSystem F_Sys;

    public ParticleSystem par;
    public AudioSource fireaudio;
    GameObject[] gos, gos11;
    public GameObject[] gos1;
    public bool ismulti_obj = false, ismulti_firePoint = false,isAltilary=false;
    public Transform[] Berrel_points;
    public Animation[] Barrel_Anim;
    int j = 0,bulletcounter=0;
    bool stop = false;
    public planeAi PlaneScript;
    int counterofplaneai=0;





    void Start()
    {

        Roketlist = new List<Rigidbody>();
        for (int i = 0; i < Magsize; i++)
        {
            Rigidbody objbullet = (Rigidbody)Instantiate(Bullet);

            objbullet.gameObject.SetActive(false);
            Roketlist.Add(objbullet);
        }



        counter = Random.Range(0, rangeofCounter);
        //		Plyer = GameObject.FindWithTag ("P");
       


    }

    GameObject ClosestObj()
    {


        if (ismulti_obj == true)
        {
            gos = GameObject.FindGameObjectsWithTag(Ttag);
            gos11 = GameObject.FindGameObjectsWithTag(Ttag1);
            gos1 = gos.Concat(gos11).ToArray();
        }
        else
        {
            gos = GameObject.FindGameObjectsWithTag(Ttag);
            gos1 = gos.ToArray();
        }


        closest = null;
        float distance = rangeOfEncounter;
        Vector3 position = transform.position;
        foreach (GameObject go in gos1)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        if (closest != null)
        {
            is_encounterrange = true;
            if(PlaneScript!=null&&counterofplaneai==0)
            {
                PlaneScript.enabled = true;
                counterofplaneai = 5;
            }

        }
        else
        {
            is_encounterrange = false;

        }
        return closest;

    }
    float AngleValue(Transform Target, Transform _Transform) //return Angle Value between Target, _Transform
    {
        DirectionValue = (Target.transform.position - _Transform.position).normalized; //Set Direction Value between target and _Trasnform.
        _Angle = Vector3.Angle(DirectionValue, _Transform.forward); //Set Angle Value between DirectionValue and forward transform.

        return _Angle; // return Angle
    }

    void Update()
    {
       

        if (Plyer == null)
        {
            Plyer = GamePlayer_Selection.instance.gameplayer;
            //Debug.Log(Plyer);
        }

        counter++;
        if (counter > cycle)
        {
            ClosestObj();

            if (IsMI17 == false)
            {
                Field_Encounter();
                View();
            }
            counter = 0;

        }
        if (is_encounterrange && closest != null)
        {

            Angle = AngleValue(closest.transform, transform); // set Angle
                                                              // Distance = (closest.transform.position - transform.position).magnitude;
                                                              //			Debug.Log (LockedObjectAngle);
            if (isbiggun == true)
            {
                direction = closest.transform.position - transform.position;
                direction.y = 0;

                targetRotaion = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotaion, Time.deltaTime * 20f);
            }
            // set Distance

            if (Angle <= LockedObjectAngle)
            {
                if (Time.time >= Nextfire && bulletcounter<=Magsize)
                {
                    Nextfire = Time.time + delt_time_fire;
                    fire();
                    if (IsMI17 == true)
                    {
                        fireaudio.Play();
                        par.Play();

                    }

                }
                else if(bulletcounter == Magsize+1&& stop==false)
                {
                    stop = true;
                    StartCoroutine(waiting());
              
                }
            }
        }
    }


    void Field_Encounter()
    {
       
        if (Plyer != null)
        {
            float distance = Vector3.Distance(transform.position, Plyer.transform.position);
            is_in_view = (distance < rangeofview);
        }
        else
        {
            Plyer = GamePlayer_Selection.instance.gameplayer;
        }
    }
    void View()
    {
        if (is_in_view)
        {
            if (isground == true)
            {
                isground = false;
                F_Sys.enabled = true;
            }

        }
    }



    public void fire()
    {
        for (int i = 0; i < Roketlist.Count; i++)
        {

            if (!Roketlist[i].gameObject.activeInHierarchy)
            {

                bulletcounter++;
                
                if (ismulti_firePoint == true)
                {
                    j++;
                    if (j >= Berrel_points.Length)
                    {
                        j = 0;
                    }

                    if (isAltilary == true)
                    {
                        Barrel_Anim[j].Play();
                    }
                    Roketlist[i].transform.position = Berrel_points[j].position;
                    Roketlist[i].transform.rotation = Berrel_points[j].rotation;
                    par.transform.position = Berrel_points[j].position;
                    Roketlist[i].gameObject.SetActive(true);
                    Roketlist[i].velocity = (closest.transform.position - transform.position).normalized * bulletspeed * Time.deltaTime;

                    break;
                }
                else
                {
                    Roketlist[i].transform.position = Berrel_points[0].position;
                    Roketlist[i].transform.rotation = Berrel_points[0].rotation;
                    Roketlist[i].gameObject.SetActive(true);
                    Roketlist[i].velocity = ((closest.transform.position - transform.position).normalized * bulletspeed * Time.deltaTime);
                    break;
                }

            }

        }
    }


    IEnumerator waiting()
    {
        yield return new WaitForSeconds(Reloadtime);
        stop = false;
        bulletcounter = 0;
        
    }



}
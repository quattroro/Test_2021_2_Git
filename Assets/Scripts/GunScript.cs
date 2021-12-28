using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : BaseGunWeapon
{
    public Transform Barrel;

    [Tooltip("스크립터블오브젝트 연결 필요")]
    public GunData gundata;

    //public float BulletForce;
    //public float CoolTime;
    
    private float LastShootTime;
    //public Transform barrel;
    public GameObject Bullet;
    //public BaseGunWeapon weaponroot;
    //public int Damage;
    //public PlayerMove sc_Player;

    public LineRenderer line;

    public Test3dMove sc_Player;

    public Camera NowCamera;

    public GameObject BulletHole;

    public ParticleSystem GunFireParticle;

    public bool NowFiring;
    public void InitSetting(GunData.GunType type)
    {
        
        
    }

    public void LoadGunData(GunData.GunType type)
    {
        this.gundata = Resources.Load<GunData>($"ScriptableObject/GunDatas/{type.ToString()}");
    }

    public void PlayerInput()
    {
        if(Input.GetMouseButton(0))
        {
            NowFiring = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            NowFiring = false;
        }
    }


    public void ShotBullet()
    {
        //베럴에서 화면 중앙으로 탄을 발사 레이 캐스트를 이용 
        if(NowFiring)
        {
            if (Time.time - LastShootTime >= gundata.CoolDown)
            {
                if(!GunFireParticle.gameObject.activeSelf)
                {
                    GunFireParticle.gameObject.SetActive(true);
                    GunFireParticle.Play();
                }
                //NowFiring = true;
                
                Debug.Log("총알 발사");
                //현재 활성화된 카메라에 맞춰서 화면 중앙의 월드좌표를 알아낸다.
                this.NowCamera = sc_Player.State.isCurrentFp ? sc_Player.Com.fpCamera : sc_Player.Com.tpCamera;
                Vector2 screenCenter = new Vector2(NowCamera.pixelWidth * 0.5f, NowCamera.pixelHeight * 0.5f);

                for(int i=0;i<gundata.MaxShot;i++)
                {
                    //분산도에 따라 랜덤으로 위치 조정
                    if (Random.Range(0.0f, 1.0f) >= gundata.Variation)
                    {
                        int flag = Random.Range(0, 2);
                        float posx = Random.Range(0.0f, gundata.VariationRadius);
                        posx *= (flag == 0) ? -1 : 1;
                        screenCenter.x += posx;

                        flag = Random.Range(0, 2);
                        float posy = Random.Range(0.0f, gundata.VariationRadius);
                        posy *= (flag == 0) ? -1 : 1;
                        screenCenter.y += posy;
                    }
                    Ray shootray = NowCamera.ScreenPointToRay(screenCenter);
                    RaycastHit[] hit = Physics.RaycastAll(shootray, gundata.MaxRange);
                    foreach(var h in hit)
                    {
                        if (h.transform.tag != "EnemyRange")
                        {
                            GameObject obj = GameObject.Instantiate(BulletHole);
                            obj.transform.position = h.point + h.normal * 0.01f;
                            obj.transform.rotation = Quaternion.LookRotation(h.normal * -1);
                            Destroy(obj, 3f);


                            obj = GameObject.Instantiate(Resources.Load<GameObject>("Particle/MetalHitParticle"));
                            obj.transform.position = h.point + h.normal * 0.01f;
                            obj.transform.rotation = Quaternion.LookRotation(h.normal);



                        }
                    }
                    
                }

                LastShootTime = Time.time;
                return;
                //GameObject obj = GameObject.Instantiate(Bullet);
                //obj.GetComponent<BulletScript>().Weapon = this;
                //obj.SetActive(true);
                //obj.transform.position = Barrel.position;
                //obj.GetComponent<Rigidbody>().AddForce(barrel.forward * BulletForce);
            }
        }
        //NowFiring = false;
        //GunFireParticle.Stop();
    }

    



    // Start is called before the first frame update
    void Start()
    {
        this.gundata = Resources.Load<GunData>($"ScriptableObject/GunDatas/Shotgun");
        sc_Player = GetComponentInParent<Test3dMove>();
        GunFireParticle = GetComponentInChildren<ParticleSystem>();
        GunFireParticle.Stop();
        GunFireParticle.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        ShotBullet();
    }
}

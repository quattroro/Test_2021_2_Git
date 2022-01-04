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

    //public GameObject BulletHole;

    public ParticleSystem GunFireParticle;

    public bool NowFiring;

    public bool IsPlayer = true;

    [SerializeField]
    private int grenadenum;

    [SerializeField]
    private int currentbullet;

    public int CurrentBullet
    {
        get
        {
            return currentbullet;
        }
        set
        {
            currentbullet = value <= 0 ? 0 : value;
            UIManager.instance.UpdataGunBulletInfo(currentbullet);
        }
    }

    public int GrenadeNum
    {
        get
        {
            return grenadenum;
        }
        set
        {
            grenadenum = value <= 0 ? 0 : value;
            UIManager.instance.UpdataGrenadeInfo(grenadenum);
        }
    }

    public void LoadGunData(GunData.GunType type)
    {
        this.gundata = Resources.Load<GunData>($"ScriptableObject/GunDatas/{type.ToString()}");
        if (IsPlayer)
        {
            UIManager.instance.UpdataGunImage(type);
            CurrentBullet = gundata.OneClipBulletNum * 5;
        }
        
    }

    public void PlayerInput()
    {
        if(IsPlayer)
        {
            //Debug.Log("플레이어 클릭");
            if (Input.GetMouseButton(0))
            {
                NowFiring = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                NowFiring = false;
                GunFireParticle.gameObject.SetActive(false);
            }
            if(Input.GetKeyDown(KeyCode.G))
            {
                ThrowGrenade();
            }
        }
        
    }

    public void ThrowGrenade()
    {
        GrenadeNum = GrenadeNum - 1;
        if(GrenadeNum<=0)
        {
            return;
        }
        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Grenade"));
        obj.transform.position = Barrel.position;
        obj.GetComponent<Rigidbody>().AddForce((Barrel.forward + Vector3.up) * 400f);
    }

    public void UpdateUI()
    {
        
    }

    public void ShotBullet()
    {
        //베럴에서 화면 중앙으로 탄을 발사 레이 캐스트를 이용 
        if(NowFiring)
        {
            if (Time.time - LastShootTime >= gundata.CoolDown)
            {
                Debug.Log("총알 발사");
                Vector2 screenCenter = Vector2.zero;
                if (IsPlayer)
                {
                    //현재 활성화된 카메라에 맞춰서 화면 중앙의 월드좌표를 알아낸다.
                    this.NowCamera = sc_Player.State.isCurrentFp ? sc_Player.Com.fpCamera : sc_Player.Com.tpCamera;
                    screenCenter = new Vector2(NowCamera.pixelWidth * 0.5f, NowCamera.pixelHeight * 0.5f);
                    CurrentBullet = CurrentBullet - 1;
                    if (CurrentBullet <= 0)
                    {
                        return;
                    }
                }

                if (!GunFireParticle.gameObject.activeSelf)
                {
                    GunFireParticle.gameObject.SetActive(true);
                    GunFireParticle.Play();
                }


                for (int i = 0; i < gundata.MaxShot; i++)
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
                    Ray shootray;
                    if (IsPlayer)
                    {
                        shootray = NowCamera.ScreenPointToRay(screenCenter);
                    }
                    else
                    {
                        EnemyMove sc_enemy = GetComponentInParent<EnemyMove>();
                        Vector3 direction = sc_enemy.sc_player.transform.position - this.transform.position;
                        shootray = new Ray(this.transform.position, direction);
                    }

                    //RaycastHit[] hit = Physics.RaycastAll(shootray, gundata.MaxRange);
                    RaycastHit hit;
                    Physics.Raycast(shootray, out hit, gundata.MaxRange);
                    if (hit.transform.tag != "EnemyRange")
                    {
                        GameObject obj;

                        if (hit.transform.tag == "Enemy" || hit.transform.tag == "Player")
                        {
                            Debug.Log("캐릭터 히트");
                            obj = GameObject.Instantiate(Resources.Load<GameObject>("Particle/CharacterHitParticle"));
                            obj.transform.position = hit.point + hit.normal * 0.01f;
                            obj.transform.rotation = Quaternion.LookRotation(hit.normal);
                            if(hit.transform.tag == "Enemy")
                            {
                                hit.transform.GetComponent<EnemyMove>().EnemyHit(sc_Player, gundata.Damage);
                            }
                            else if(hit.transform.tag == "Player")
                            {
                                hit.transform.GetComponent<Test3dMove>().PlayerHit(gundata.Damage);
                            }
                        }
                        else
                        {
                            obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/BulletHole"));
                            obj.transform.position = hit.point + hit.normal * 0.01f;
                            obj.transform.rotation = Quaternion.LookRotation(hit.normal * -1);
                            Destroy(obj, 3f);


                            obj = GameObject.Instantiate(Resources.Load<GameObject>("Particle/MetalHitParticle"));
                            obj.transform.position = hit.point + hit.normal * 0.01f;
                            obj.transform.rotation = Quaternion.LookRotation(hit.normal);
                        }

                    }
                    //foreach (var h in hit)
                    //{
                    //    if (h.transform.tag != "EnemyRange")
                    //    {
                    //        GameObject obj;

                    //        if (h.transform.tag == "Enemy" || h.transform.tag == "Player")
                    //        {
                    //            Debug.Log("캐릭터 히트");
                    //            obj = GameObject.Instantiate(Resources.Load<GameObject>("Particle/CharacterHitParticle"));
                    //            obj.transform.position = h.point + h.normal * 0.01f;
                    //            obj.transform.rotation = Quaternion.LookRotation(h.normal);
                    //        }
                    //        else
                    //        {
                    //            obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/BulletHole"));
                    //            obj.transform.position = h.point + h.normal * 0.01f;
                    //            obj.transform.rotation = Quaternion.LookRotation(h.normal * -1);
                    //            Destroy(obj, 3f);


                    //            obj = GameObject.Instantiate(Resources.Load<GameObject>("Particle/MetalHitParticle"));
                    //            obj.transform.position = h.point + h.normal * 0.01f;
                    //            obj.transform.rotation = Quaternion.LookRotation(h.normal);
                    //        }


                    //        break;
                    //    }
                    //}

                }

                //Shoot(screenCenter);

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

    
    void Shoot(Vector3 screenCenter)
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        //this.gundata = Resources.Load<GunData>($"ScriptableObject/GunDatas/Rifle");
        //LoadGunData(GunData.GunType.Pistol);
        //CurrentBullet = gundata.OneClipBulletNum * 5;
        GrenadeNum = 5;
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

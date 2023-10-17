using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SisterMovement : MonoBehaviour
{
    public int max_h,min_h,max_w,min_w;

    public Vector2? target = null;

    public float speed;

    public float min_distance = 10;

    public GameObject bubble;

    public GameObject face;

    public GameObject ornament_1;

    public TextMeshProUGUI  text;

    public Sprite[] faces;

    public Sprite[] ornaments;

    public GameObject sisterPanel;

    private string[] strs = {"惊了","Deja vu","唔","道路千万条 安全第一条","饿了 去车头整点薯条","疯狂星期四V我50", "Long may the sun shine" };

    private bool right_direction = false;

    public bool random_deco_mode = false;

    public void Start()
    {
        SetTarget();
    }

    public void Update()
    {
        if (target != null)
        {
            var dir = ((Vector2)(target - transform.localPosition)).normalized;

            if (dir.x > 0 && right_direction)
            {
                
            }
            else if(dir.x < 0 && !right_direction)
            {

            }
            else {
                transform.Rotate(new Vector3(0, -180, 0));
                right_direction = !right_direction;
            }

            transform.localPosition += speed * Time.deltaTime * new Vector3(dir.x,dir.y,0f);

            if((transform.localPosition - (Vector3)target).sqrMagnitude <0.1f)
            {
                target = null;
                bubble.SetActive(true);
                bubble.transform.localPosition = transform.localPosition + new Vector3(200,50,0);
                int i = Random.Range(0, strs.Length);
                text.text = strs[i];
                int j = Random.Range(0, faces.Length);
                face.GetComponent<Image>().sprite = faces[j];
                if (random_deco_mode)
                {
                    int k = Random.Range(0, ornaments.Length);
                    ornament_1.GetComponent<Image>().sprite = ornaments[k];
                }
                Invoke("SetTarget", 3f);
                Invoke("CloseSpeak", 3f);
            }
        }
    }


    private void SetTarget()
    {
        if (target != null)
        {
            return;
        }
        var w = Random.Range(min_w, max_w);
        var h = Random.Range(min_h, max_h);


        var v = new Vector2(w, h);
        while((new Vector3(v.x,v.y,0) - transform.localPosition).magnitude < min_distance)
        {
            w = Random.Range(min_w, max_w);
            h = Random.Range(min_h, max_h);
            v = new Vector2(w, h);
        }

        target = new Vector2(w, h);
    }

    private void CloseSpeak()
    {
        bubble.SetActive(false);
    }

    public void SetDeco(int num)
    {
        ornament_1.GetComponent<Image>().sprite = ornaments[num];
    }

    public void SetPanel()
    {
        sisterPanel.SetActive(!sisterPanel.activeSelf) ;
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectController : MonoBehaviour {

    SpriteRenderer MainSpriteRenderer;
    // publicで宣言し、inspectorで設定可能にする
    public Sprite StandbySprite;
    public Sprite HoldSprite;
    public Sprite SlashSprite;

    Vector3 screenPoint;
    Vector3 offset;

    Rigidbody2D rb;
    private PolygonCollider2D polygonCollider;
    private Sprite sprite;
    private Vector3 position_now;
    private Vector3 position_prev;
    private Quaternion rotation_now;
    private Quaternion rotation_prev;
    private float distPos = 0f;
    private float simiRot = 0f;
    private float timePrev;
    
    public bool IsMove = true;
    public bool IsThrew = false;
    public bool IsHold = false;
    public bool WasHold = false;
    public float timeThrew = 0f;
    public float timeFromThrew = 0f;
    public float thresholdDistPos = 0.001f;
    public float thresholdSimiRot = 0.99f;

    // Use this for initialization
    void Start() {
        // paste the image which you write
        Sprite[] images = Resources.LoadAll<Sprite>("images/players/");
        this.GetComponent<SpriteRenderer>().sprite = GetAtRandom(images);

        rb = this.GetComponent<Rigidbody2D>();
        ResetPolygonCollider2D();
        position_now = this.transform.position;
        rotation_now = this.transform.rotation;
        UpdateTranform();
        timePrev = Time.time;
    }

    void Update() {
        if (Time.time - timePrev > 1f) {
            UpdateTranform();
        }
        DetectMotionsOfPics();

        // Process when the moment that mouse button up
        if (WasHold == true && IsHold == false) {
            IsThrew = true;
            timeThrew = Time.time;
        }

        if (IsThrew) {
            rb.gravityScale = 0.75f;
            timeFromThrew = Time.time - timeThrew;
        }

        WasHold = IsHold;
    }

    void OnMouseDown() {
        if (!IsThrew) {
            this.screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            this.offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            IsHold = true;
        }
    }

    void OnMouseUp() {
        if (!IsThrew) {
            IsHold = false;
        }
    }

    void OnMouseDrag() {
        if (!IsThrew) {
            Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + this.offset;
            transform.position = new Vector3(currentPosition.x, transform.position.y, transform.position.z);
        }
    }

    void ResetPolygonCollider2D() {
        //Debug.Log("ResetPolygonCollider2D");
        polygonCollider = GetComponent<PolygonCollider2D>();
        sprite = GetComponent<SpriteRenderer>().sprite;
        for (int i = 0; i < polygonCollider.pathCount; i++) polygonCollider.SetPath(i, null);
        polygonCollider.pathCount = sprite.GetPhysicsShapeCount();

        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < polygonCollider.pathCount; i++) {
            path.Clear();
            sprite.GetPhysicsShape(i, path);
            polygonCollider.SetPath(i, path.ToArray());
        }
    }

    void DetectMotionsOfPics() {
        distPos = Vector3.Distance(position_now, position_prev);
        simiRot = CosSimilarity(rotation_now, rotation_prev);
        if (distPos > thresholdDistPos || simiRot < thresholdSimiRot) {
            IsMove = true;
        } else {
            IsMove = false;
        }
    }

    float CosSimilarity(Quaternion a, Quaternion b) {
        float aa = Mathf.Sqrt(Quaternion.Dot(a, a));
        float bb = Mathf.Sqrt(Quaternion.Dot(b, b));
        return Quaternion.Dot(a, b) / (aa * bb);
    }

    void UpdateTranform() {
        position_prev = new Vector3(position_now.x, position_now.y, position_now.z);
        position_now = this.transform.position;
        rotation_prev = new Quaternion(rotation_now.x, rotation_now.y, rotation_now.z, rotation_now.w);
        rotation_now = this.transform.rotation;

        timePrev = Time.time;
    }

    Sprite GetAtRandom(Sprite[] list) {
        if (list.Length == 0) {
            Debug.LogError("リストが空です！");
        }
        return list[UnityEngine.Random.Range(0, list.Length)];
    }

}


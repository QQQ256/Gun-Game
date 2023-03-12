using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public float forceMin;
    public float forceMax;

    float lifeTime = 4f;
    float fadeTime = 2f;

    Material mat;
    Color initColor;

    private void OnEnable() {
        float force = Random.Range(forceMin, forceMax);
        myRigidbody.AddForce(transform.right * force);
        myRigidbody.AddTorque(Random.insideUnitSphere * force);

        MonoManager.GetInstance().StartCoroutine(Fade());
    }

    private void Start() {
        mat = this.GetComponent<Renderer>().material;
        initColor = mat.color;
    }

    IEnumerator Fade(){
        yield return new WaitForSeconds(lifeTime);

        float percent = 0;
        float fadeSpeed = 1 / fadeTime;
        // mat = this.GetComponent<Renderer>().material;

        while(percent <= 1){
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initColor, Color.clear, percent);
            yield return null;
        }
    }

    private void OnDisable() {
        MonoManager.GetInstance().StopCoroutine(Fade());
        mat.color = initColor;
    }
}

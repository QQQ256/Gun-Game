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
    // Start is called before the first frame update
    void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        myRigidbody.AddForce(transform.right * force);
        myRigidbody.AddTorque(Random.insideUnitSphere * force);

        MonoManager.GetInstance().StartCoroutine(Fade());
    }

    IEnumerator Fade(){
        yield return new WaitForSeconds(lifeTime);

        float percent = 0;
        float fadeSpeed = 1 / fadeTime;
        Material mat = this.GetComponent<Renderer>().material;
        Color initalColor = mat.color;

        while(percent <= 1){
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initalColor, Color.clear, percent);
            yield return null;
        }

        Destroy(this.gameObject);
    }
}

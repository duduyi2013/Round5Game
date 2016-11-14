using UnityEngine;
using System.Collections;

public class FadeToBlack : MonoBehaviour {

    public AnimationCurve m_AnimationCurve = AnimationCurve.Linear(0.0f, 1.0f, 5.0f, 0.0f);
    public bool m_PlayOnAwake = false;
    private Material m_Material;
    public float duration;
    void Awake ()
    {
        m_Material = new Material(Shader.Find("Hidden/FadeToBlack"));
        if (m_PlayOnAwake) {
            StartCoroutine("StartFade");
        }
    }

    public void Play ()
    {
        StartCoroutine("StartFade");
    }

    IEnumerator StartFade ()
    {
       duration = m_AnimationCurve.keys[m_AnimationCurve.length - 1].time;
        for (float f = 0.0f; f < duration; f += Time.deltaTime)
        {
            m_Material.SetFloat(Shader.PropertyToID("_Opacity"), m_AnimationCurve.Evaluate(f));
            yield return null;
        }
    }

	void OnRenderImage (RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, m_Material);
    }
}

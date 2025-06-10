using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform cameraTransform;   // 카메라 Transform
    public float shakeAmplitude = 0.05f; // 흔들림 세기(폭)
    public float shakeFrequency = 10f;   // 흔들림 속도(빈도)
    
    private Vector3 defaultPos;
    private float shakeTimer = 0f;
    private bool isSprinting = false; // 이 값은 PlayerController 등에서 연동

    void Start()
    {
        defaultPos = cameraTransform.localPosition;
    }

    void Update()
    {
        // 스프린트 중이면
        if (isSprinting)
        {
            shakeTimer += Time.deltaTime * shakeFrequency;
            float xShake = Mathf.Sin(shakeTimer) * shakeAmplitude;
            cameraTransform.localPosition = defaultPos + new Vector3(xShake, 0, 0);
        }
        else
        {
            // 흔들림 끝나면 카메라 원래 위치로 복귀
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, defaultPos, Time.deltaTime * 10f);
            shakeTimer = 0f;
        }
    }

    // PlayerController 등에서 이걸 호출해서 스프린트 상태 갱신
    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }
}

using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform cameraTransform;
    public float shakeAmplitude = 0.05f;
    public float shakeFrequency = 10f;

    private Vector3 baseLocalPos;
    private float shakeTimer = 0f;

    private float cameraCrouchHeight;
    private float cameraStandHeight;
    private bool isSprinting = false;
    private bool isCrouching = false;

    public void SetCrouchAndSprint(bool crouch, bool sprint)
    {
        isCrouching = crouch;
        isSprinting = sprint;
    }

    public void SetCrouchAndStandHeight(float crouch, float stand) 
    {
        cameraCrouchHeight = crouch;
        cameraStandHeight = stand;
    }

    void Start()
    {
        baseLocalPos = cameraTransform.localPosition;
    }

    void Update()
    {
        Vector3 targetPos = GetTargetPos();
        //Debug.Log($"isCrouching = {isCrouching}, isSprinting = {isSprinting}");
        if (isSprinting)
        {
            shakeTimer += Time.deltaTime * shakeFrequency;
            float xShake = Mathf.Sin(shakeTimer) * shakeAmplitude;
            cameraTransform.localPosition = targetPos + new Vector3(xShake, 0, 0);
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetPos,
                Time.deltaTime * 10f
            );
            shakeTimer = 0f;
        }
    }

    Vector3 GetTargetPos()
    {
        float targetY = isCrouching ? cameraCrouchHeight : cameraStandHeight;
        return new Vector3(baseLocalPos.x, targetY, baseLocalPos.z);
    }
}

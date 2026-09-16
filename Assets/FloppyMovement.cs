using UnityEngine;

public class FloppyMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerControls controls;

    public Transform leftThigh;
    public Transform rightThigh;
    public Transform leftShin;
    public Transform rightShin;
    public Transform leftUpperArm;
    public Transform rightUpperArm;
    public Transform leftForearm;
    public Transform rightForearm;
    public Transform torso;
    public Transform head;

    [Header("Run")]
    public float stepSpeed = 9f;
    public float backLegAmount = -45f;
    public float frontLegAmount = 6f;
    public float kneeBend = 40f;
    public float armForward = 35f;
    public float armBack = -40f;
    public float elbowBend = 55f;
    public float forwardLean = 8f;

    [Header("Looseness")]
    public float variation = 0.12f;
    public float torsoMovement = 3f;
    public float headMovement = 3f;
    public float returnSpeed = 12f;

    Quaternion leftThighStart;
    Quaternion rightThighStart;
    Quaternion leftShinStart;
    Quaternion rightShinStart;
    Quaternion leftArmStart;
    Quaternion rightArmStart;
    Quaternion leftForearmStart;
    Quaternion rightForearmStart;
    Quaternion torsoStart;
    Quaternion headStart;

    float stepCycle;
    float leftVariation = 1f;
    float rightVariation = 1f;
    bool lastStepPositive;

    void Start()
    {
        if (controls == null)
            controls = GetComponentInParent<PlayerControls>();

        leftThighStart = leftThigh.localRotation;
        rightThighStart = rightThigh.localRotation;

        leftShinStart = leftShin.localRotation;
        rightShinStart = rightShin.localRotation;

        leftArmStart = leftUpperArm.localRotation;
        rightArmStart = rightUpperArm.localRotation;

        leftForearmStart = leftForearm.localRotation;
        rightForearmStart = rightForearm.localRotation;

        torsoStart = torso.localRotation;
        headStart = head.localRotation;
    }

    void Update()
    {
        if (controls == null)
            return;

        float moveAmount = Mathf.Clamp01(controls.MoveInput.magnitude);

        if (moveAmount < 0.1f)
        {
            ReturnToIdle();
            return;
        }

        stepCycle += Time.deltaTime * stepSpeed * moveAmount;

        float step = Mathf.Sin(stepCycle);
        bool stepPositive = step > 0f;

        if (stepPositive != lastStepPositive)
        {
            leftVariation = Random.Range(1f - variation, 1f + variation);
            rightVariation = Random.Range(1f - variation, 1f + variation);

            lastStepPositive = stepPositive;
        }

        MoveLegs(step, moveAmount);
        MoveArms(step, moveAmount);
        MoveBody(step, moveAmount);
    }

    void MoveLegs(float step, float moveAmount)
    {
        float leftThighAngle;
        float rightThighAngle;
        float leftKnee;
        float rightKnee;

        if (step > 0f)
        {
            leftThighAngle = frontLegAmount * step * leftVariation;
            rightThighAngle = backLegAmount * step * rightVariation;

            leftKnee = kneeBend * step;
            rightKnee = 0f;
        }
        else
        {
            float amount = -step;

            rightThighAngle = frontLegAmount * amount * rightVariation;
            leftThighAngle = backLegAmount * amount * leftVariation;

            rightKnee = kneeBend * amount;
            leftKnee = 0f;
        }

        leftThigh.localRotation = Quaternion.Slerp(
            leftThigh.localRotation,
            leftThighStart * Quaternion.Euler(leftThighAngle * moveAmount, 0f, 0f),
            returnSpeed * Time.deltaTime
        );

        rightThigh.localRotation = Quaternion.Slerp(
            rightThigh.localRotation,
            rightThighStart * Quaternion.Euler(rightThighAngle * moveAmount, 0f, 0f),
            returnSpeed * Time.deltaTime
        );

        leftShin.localRotation = Quaternion.Slerp(
            leftShin.localRotation,
            leftShinStart * Quaternion.Euler(leftKnee * moveAmount, 0f, 0f),
            returnSpeed * Time.deltaTime
        );

        rightShin.localRotation = Quaternion.Slerp(
            rightShin.localRotation,
            rightShinStart * Quaternion.Euler(rightKnee * moveAmount, 0f, 0f),
            returnSpeed * Time.deltaTime
        );
    }

    void MoveArms(float step, float moveAmount)
    {
        float leftArm;
        float rightArm;

        if (step > 0f)
        {
            leftArm = armBack * step * leftVariation;
            rightArm = armForward * step * rightVariation;
        }
        else
        {
            float amount = -step;

            leftArm = armForward * amount * leftVariation;
            rightArm = armBack * amount * rightVariation;
        }

        leftUpperArm.localRotation = Quaternion.Slerp(
            leftUpperArm.localRotation,
            leftArmStart * Quaternion.Euler(leftArm * moveAmount, 0f, 0f),
            returnSpeed * Time.deltaTime
        );

        rightUpperArm.localRotation = Quaternion.Slerp(
            rightUpperArm.localRotation,
            rightArmStart * Quaternion.Euler(rightArm * moveAmount, 0f, 0f),
            returnSpeed * Time.deltaTime
        );

        leftForearm.localRotation = Quaternion.Slerp(
            leftForearm.localRotation,
            leftForearmStart * Quaternion.Euler(elbowBend * moveAmount, 0f, 0f),
            returnSpeed * Time.deltaTime
        );

        rightForearm.localRotation = Quaternion.Slerp(
            rightForearm.localRotation,
            rightForearmStart * Quaternion.Euler(elbowBend * moveAmount, 0f, 0f),
            returnSpeed * Time.deltaTime
        );
    }

    void MoveBody(float step, float moveAmount)
    {
        float looseMovement = Mathf.Sin(Time.time * 5.5f);

        torso.localRotation = Quaternion.Slerp(
            torso.localRotation,
            torsoStart * Quaternion.Euler(
                forwardLean * moveAmount,
                looseMovement * torsoMovement,
                step * torsoMovement
            ),
            7f * Time.deltaTime
        );

        head.localRotation = Quaternion.Slerp(
            head.localRotation,
            headStart * Quaternion.Euler(
                looseMovement * headMovement,
                0f,
                -step * headMovement
            ),
            6f * Time.deltaTime
        );
    }

    void ReturnToIdle()
    {
        leftThigh.localRotation = Quaternion.Slerp(
            leftThigh.localRotation,
            leftThighStart,
            returnSpeed * Time.deltaTime
        );

        rightThigh.localRotation = Quaternion.Slerp(
            rightThigh.localRotation,
            rightThighStart,
            returnSpeed * Time.deltaTime
        );

        leftShin.localRotation = Quaternion.Slerp(
            leftShin.localRotation,
            leftShinStart,
            returnSpeed * Time.deltaTime
        );

        rightShin.localRotation = Quaternion.Slerp(
            rightShin.localRotation,
            rightShinStart,
            returnSpeed * Time.deltaTime
        );

        leftUpperArm.localRotation = Quaternion.Slerp(
            leftUpperArm.localRotation,
            leftArmStart,
            returnSpeed * Time.deltaTime
        );

        rightUpperArm.localRotation = Quaternion.Slerp(
            rightUpperArm.localRotation,
            rightArmStart,
            returnSpeed * Time.deltaTime
        );

        leftForearm.localRotation = Quaternion.Slerp(
            leftForearm.localRotation,
            leftForearmStart,
            returnSpeed * Time.deltaTime
        );

        rightForearm.localRotation = Quaternion.Slerp(
            rightForearm.localRotation,
            rightForearmStart,
            returnSpeed * Time.deltaTime
        );

        torso.localRotation = Quaternion.Slerp(
            torso.localRotation,
            torsoStart,
            returnSpeed * Time.deltaTime
        );

        head.localRotation = Quaternion.Slerp(
            head.localRotation,
            headStart,
            returnSpeed * Time.deltaTime
        );
    }
}
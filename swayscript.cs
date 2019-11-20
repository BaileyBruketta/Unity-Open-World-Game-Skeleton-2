using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swayscript : MonoBehaviour
{
    public float amount = 0.02f;
    public float maxamount = 0.03f;
    public float smooth = 3;
    private Quaternion def;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float factorL = -(Input.GetAxis("Horizontal")) * amount;
        float factorX = (Input.GetAxis("Mouse Y")) * amount;
        float factorY = -(Input.GetAxis("Mouse X")) * amount;
        float factorZ = -Input.GetAxis("Vertical") * amount;
        if (factorX > maxamount)
            factorX = maxamount;

        if (factorX < -maxamount)
            factorX = -maxamount;

        if (factorY > maxamount)
            factorY = maxamount;

        if (factorY < -maxamount)
            factorY = -maxamount;

        if (factorZ > maxamount)
            factorZ = maxamount;

        if (factorZ < -maxamount)
            factorZ = -maxamount;

        if (factorL > maxamount)
            factorL = maxamount;

        if (factorL < -maxamount)
            factorL = -maxamount;
        Quaternion xinal = Quaternion.Euler(def.x + factorX, def.y + factorY, def.z + factorZ);
        Quaternion z = Quaternion.Slerp(transform.localRotation, xinal, (Time.time * smooth));
        Quaternion Final = Quaternion.Euler(0, 0, def.z + factorL);
        Quaternion h = Quaternion.Lerp(transform.localRotation, Final, (Time.deltaTime * amount) * smooth);
        Quaternion j = Quaternion.Lerp(z, h, (Time.deltaTime * amount) * smooth);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, j, (Time.deltaTime * amount) * smooth);
    }
}

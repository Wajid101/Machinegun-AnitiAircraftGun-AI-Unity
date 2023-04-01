/// <summary>
/// AI look. this script using as Terrent AI
/// </summary> 
using UnityEngine;

// add all necessary components.

public class AILook : MonoBehaviour
{
    public Eheli_Weaponsystem gunInstance;
    public New_Weapon instance;

   
    void Update()
    {

        if (gunInstance != null && gunInstance.closest != null)
        {
            // rotation facing to the target.
            Quaternion targetlook = Quaternion.LookRotation(gunInstance.closest.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetlook, Time.deltaTime * 3);

        }
        else if (instance!=null && instance.closest != null)
        {
            Quaternion targetlook = Quaternion.LookRotation(instance.closest.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetlook, Time.deltaTime * 3);
        }
        



    }
}

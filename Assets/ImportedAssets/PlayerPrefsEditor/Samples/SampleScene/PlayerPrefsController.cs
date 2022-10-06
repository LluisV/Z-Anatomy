using UnityEngine;

public class PlayerPrefsController : MonoBehaviour
{

    #region Add
    public void AddTestStrings()
    {
        PlayerPrefs.SetString("Runtime_String", "boing");
        PlayerPrefs.SetString("Runtime_String2", "foo");
        PlayerPrefs.Save();
    }

    public void AddTestInt()
    {
        PlayerPrefs.SetInt("Runtime_Int", 1234);
        PlayerPrefs.Save();
    }

    public void AddTestFloat()
    {
        PlayerPrefs.SetFloat("Runtime_Float", 3.14f);
        PlayerPrefs.Save();
    }
    #endregion

    #region Remove
    public void RemoveTestStrings()
    {
        PlayerPrefs.DeleteKey("Runtime_String");
        PlayerPrefs.DeleteKey("Runtime_String2");
        PlayerPrefs.Save();
    }

    public void RemoveTestInt()
    {
        PlayerPrefs.DeleteKey("Runtime_Int");
        PlayerPrefs.Save();
    }

    public void RemoveTestFloat()
    {
        PlayerPrefs.DeleteKey("Runtime_Float");
        PlayerPrefs.Save();
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    #endregion
}
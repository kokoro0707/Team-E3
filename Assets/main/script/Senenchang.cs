using UnityEngine;
using UnityEngine.SceneManagement;
public class Senenchang : MonoBehaviour
{
    [SerializeField] private string LoadSene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SceneChang();
    }
    public void SceneChang()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("‚Í‚ñ‚Ì‚¤");
            SceneManager.LoadScene(LoadSene);
        }
    }
}

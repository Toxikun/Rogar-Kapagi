using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için bu þart!

public class menuController : MonoBehaviour
{
    // Start Butonu için metod
    public void StartGame()
    {
        // "SampleScene" isimli sahneyi yükler. 
        // Mevcut sahne (MainMenu) otomatik olarak kapanýr.
        SceneManager.LoadScene("SampleScene");
    }

    // Quit Butonu için metod
    public void QuitGame()
    {
        Debug.Log("Oyundan çýkýldý!"); // Editörde çalýþtýðýný anlamak için
        Application.Quit(); // Build alýnmýþ oyunda çalýþýr
    }
}
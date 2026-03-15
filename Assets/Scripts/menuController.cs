using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in bu �art!

public class menuController : MonoBehaviour
{
    // Start Butonu i�in metod
    public void StartGame()
    {
        // "SampleScene" isimli sahneyi y�kler. 
        // Mevcut sahne (MainMenu) otomatik olarak kapan�r.
        SceneManager.LoadScene("gecis");
    }
        public void gecisGame()
    {
        // "SampleScene" isimli sahneyi y�kler. 
        // Mevcut sahne (MainMenu) otomatik olarak kapan�r.
        SceneManager.LoadScene("SampleScene");
    }

    // Quit Butonu i�in metod
    public void QuitGame()
    {
        Debug.Log("Oyundan ��k�ld�!"); // Edit�rde �al��t���n� anlamak i�in
        Application.Quit(); // Build al�nm�� oyunda �al���r
    }
}
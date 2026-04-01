using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject abandonedCar;
    public GameObject repairedCar;
    public GameObject carInvZone;
    public GameObject carInv;

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
        PlayerController.playerHp = 100f;
    }

    public void RepairCar()
    {
        abandonedCar.SetActive(false);
        repairedCar.SetActive(true);
        carInvZone.SetActive(false);
        carInv.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    Rigidbody rb;
    public static int ScoreP1 = 0;
    public static int ScoreP2 = 0;
    public static bool PlayerScoredLast;
    public static bool OpponentScoredLast;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public static void ResetBools()
    {
        PlayerScoredLast = false;
        OpponentScoredLast = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
            if (this.name == "P1_WinWall")
            {
                ScoreP1++;
                PlayerScoredLast = true;
                OpponentScoredLast = false;
            }
            else if (this.name == "P2_WinWall")
            {
                ScoreP2++;
                PlayerScoredLast = false;
                OpponentScoredLast = true;
            }
        }
    }
}
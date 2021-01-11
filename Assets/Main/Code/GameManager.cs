using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct Wave
    {
        public StickManEnemy[] enemiesToKill;
    }
    public static Vector3 playerPosition;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerController player;

    private static GameManager instance;
    public static bool allowAutomaticShooting =true;
    [SerializeField] private Wave[] waves;
    private static int waveIndex;
    private static StickManEnemy[] CurrentWaveEnemies
    {
        get { return instance.waves[waveIndex].enemiesToKill; }
    }

    public static StickManEnemy[] GetCurrentWaveEnemies()
    {
        return instance.waves[waveIndex].enemiesToKill;
    }
    private static bool waitingForNextWave = false;
    [Header("BossFight")]
    [SerializeField] private Boss boss;
    private static bool inBossFight = false;

   void Start()
    {
        instance = this;
        /* for (int j = 0; j < instance.waves.Length; j++)
         {
             Shooter[] shooters = instance.waves[j].shootersToKill;
             for (int i = 0; i < shooters.Length; i++)
             {
                 shooters[i].Awaken();
             }
         }*/
        // AwakeCurrentWave();


        waveIndex = -1;
        CheckWaveState();
        Routine();
        //MakeMarkersDisappear();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StickManEnemy[] enemies = CurrentWaveEnemies;
            List<Shooter> livingShooters = new List<Shooter>();

            for (int i = 0; i < enemies.Length; i++)
            {
                StickManEnemy enemy = enemies[i];
                if (enemy is Shooter && enemy.IsAlive)
                {
                    livingShooters.Add((Shooter)enemy);
                   /* Shooter shooter = (Shooter)enemies[i];
                    if (shooter.IsAlive)
                    {
                        Debug.Log("TryShoot");
                        shooter.TryShoot();
                    }*/
                }
            }
            int index = Random.Range(0, livingShooters.Count);
            livingShooters[index].TryShoot();
        }
    }

    private void Routine()
    {
        playerPosition = instance.playerTransform.position;

        Invoke("Routine", 0.05f);
    }

    public static void CheckWaveState()
    {
        if (inBossFight)
        {

        }
        else
        {
            if ( waveIndex > -1 && waveIndex < instance.waves.Length)
            {
                /* Shooter[] shooters = instance.waves[waveIndex].shootersToKill;
                 for (int i = 0; i < shooters.Length; i++)
                 {
                     if (shooters[i].IsAlive)
                     {
                         return;
                     }
                 }*/

                StickManEnemy[] enemies = CurrentWaveEnemies;
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].IsAlive)
                    {
                        return;
                    }
                }
            }

            waitingForNextWave = true;
            instance.player.StartRunning();
           MainCamera.instance.TransitionTo(CameraStates.Running);
        }
    }

    public static void StartNextWave(bool bossTrigger)
    {

        Debug.Log("Next Wave!");
        waveIndex++;
        while(instance.waves[waveIndex].enemiesToKill.Length == 0 )
        {
            waveIndex++;
        }
        if (!bossTrigger)
        {
            //return;
        }

        instance.player.StopRunning();
        AwakeCurrentWave();
        if (bossTrigger)
        {
            inBossFight = true;
            instance.StartCoroutine(instance.PlayBossScene());
            MainCamera.instance.TransitionTo(CameraStates.Boss);
        }
        else
        {
            MainCamera.instance.TransitionTo(CameraStates.Action);
        }
    }

    private static void AwakeCurrentWave()
    {
        /*Shooter[] shooters = instance.waves[waveIndex].shootersToKill;
        for (int i = 0; i < shooters.Length; i++)
        {
            shooters[i].Awaken();
        }*/

        StickManEnemy[] enemies = CurrentWaveEnemies;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].Awaken();
        }
    }

    private IEnumerator PlayBossScene()
    {
        yield return new WaitForSeconds(0.3f);
        boss.WakeUp();
        yield return new WaitForSeconds(1.85f);
        instance.player.FRENZY();
    }

    public static void OnBossDeath()
    {
        instance.StartCoroutine(instance.PlayVictoryScene());
    }

    public static void OnPlayerDeath()
    {
        MainCamera.instance.TransitionTo(CameraStates.Running);

        Wave[] waves = instance.waves;
        for (int i = waveIndex; i < waves.Length; i++)
        {
            StickManEnemy[] enemies = waves[i].enemiesToKill;

            for (int j = 0; j < enemies.Length; j++)
            {
                enemies[j].StartDancing();
            }
        }

    }

    private IEnumerator PlayVictoryScene()
    {
        player.EndFrenzy();
        yield return new WaitForSeconds(1f);
        player.DANCE();
        MainCamera.instance.TransitionTo(CameraStates.Running);
    }


}



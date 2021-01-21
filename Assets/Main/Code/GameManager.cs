using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        //TODO: I would like to make it so that cage waves do not contain bosses, boss waves do not contain cages etc.
        public WaveStates waveState;
        public StickManEnemy[] enemiesToKill;     
        public Boss boss;
        public Animator cageAnimator;
        public Animator cagedCreatureAnimator;
        public Transform cameraStaticTransform;
    }

    public static Vector3 playerPosition;
    [SerializeField] private Transform playerPositionTransform;
    [SerializeField] private PlayerController player;

    private static GameManager instance;
    public static bool allowAutomaticShooting =true;
    [Header("Waves")]

    [SerializeField] private Wave[] waves;
    private static int waveIndex;
    private static StickManEnemy[] CurrentWaveEnemies
    {
        get { return instance.waves[waveIndex].enemiesToKill; }
    }
    private static Wave CurrentWave
    {
        get { return instance.waves[waveIndex]; }
    }


    public static StickManEnemy[] GetCurrentWaveEnemies()
    {
        return instance.waves[waveIndex].enemiesToKill;
    }

    public enum WaveStates
    {
        NormalFight, BossFight, CageScene, Running
    }
    private static WaveStates DT_waveState;
    private static WaveStates WaveState
    {
        get
        {
            return DT_waveState;
        }
        set
        {
            DT_waveState = value;
            instance.waveStateText.text = "Wave State: " + DT_waveState.ToString();
        }
    }
    [SerializeField] private Text waveStateText;
    [SerializeField] private bool skipToBoss = false;

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
        EndWave();
        Routine();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            MakeRandomEnemyShoot();
        }
    }

    private void MakeRandomEnemyShoot()
    {
        StickManEnemy[] enemies = CurrentWaveEnemies;
        List<Shooter> livingShooters = new List<Shooter>();

        for (int i = 0; i < enemies.Length; i++)
        {
            StickManEnemy enemy = enemies[i];
            if (enemy is Shooter && enemy.IsAlive)
            {
                livingShooters.Add((Shooter)enemy);
            }
        }
        int index = Random.Range(0, livingShooters.Count);
        livingShooters[index].TryShoot();
    }

    private void Routine()
    {
        playerPosition = instance.playerPositionTransform.position;

        Invoke("Routine", 0.05f);
    }

    private static void CheckIfCurrentWaveIsClear()
    {
        if (WaveState == WaveStates.NormalFight)
        {
            if (waveIndex > -1 && waveIndex < instance.waves.Length)
            {

                StickManEnemy[] enemies = CurrentWaveEnemies;
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].IsAlive)
                    {
                        return;
                    }
                }
            }

            EndWave();

        }
    }

    private static void EndWave()
    {
        WaveState = WaveStates.Running;
        instance.player.StartRunning();
        MainCamera.instance.SetOrientation(CameraOrientations.Running);
    }

    public static void StartNextWave()
    {

        Debug.Log("Next Wave!");
        waveIndex++;
        WaveStates newState = CurrentWave.waveState;

       /* while (newState == WaveStates.NormalFight && instance.waves[waveIndex].enemiesToKill.Length == 0 )
        {
            StartNextWave();
        }*/
        if (instance.skipToBoss && newState == WaveStates.NormalFight)
        {
            return;
        }

        WaveState = newState;

        //Might be better to push these into the switch statement
        instance.player.StopRunning();
        AwakeCurrentWave();

        switch (newState)
        {
           case WaveStates.NormalFight:
               {
                   MainCamera.instance.SetOrientation(CameraOrientations.Action);
               }
               break;
           case WaveStates.BossFight:
               {
                   instance.StartCoroutine(instance.PlayBossScene());
                   MainCamera.instance.SetOrientation(CameraOrientations.Boss);
               }
               break;
            case WaveStates.CageScene:
                {
                    instance.StartCoroutine(instance.PlayCageScene());
                   // MainCamera.instance.TransitionTo(CameraStates.Boss);
                }
                break;
        }
    }
   
    private static void AwakeCurrentWave()
    {
        StickManEnemy[] enemies = CurrentWaveEnemies;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].Awaken();
        }
    }

    private IEnumerator PlayBossScene()
    {

        yield return new WaitForSeconds(0.3f);
        CurrentWave.boss.WakeUp();
        yield return new WaitForSeconds(1.85f);
        instance.player.FRENZY();
    }

    public static void OnEnemyDeath()
    {
        CheckIfCurrentWaveIsClear();
    }

    public static void OnBossDeath()
    {
        //instance.StartCoroutine(instance.PlayVictoryScene());
        instance.StartCoroutine(instance.OnBossDeathCoroutine());
    }

    public static void OnPlayerDeath()
    {
        MainCamera.instance.SetOrientation(CameraOrientations.Running);

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
        player.Dance();
        MainCamera.instance.SetOrientation(CameraOrientations.Running);
    }

    private IEnumerator PlayCageScene()
    {
        MainCamera.instance.GoToStaticDestination(CurrentWave.cameraStaticTransform);

        yield return new WaitForSeconds(0.5f);
        Animator cageAnimator = CurrentWave.cageAnimator;
        cageAnimator.SetTrigger("Lift");
        player.ForceLift(cageAnimator.transform.position);

        yield return new WaitForSeconds(2.9f);
        player.Dance();
        CurrentWave.cagedCreatureAnimator.SetTrigger("Dance");

    }

    private IEnumerator OnBossDeathCoroutine()
    {
        player.EndFrenzy();
        yield return new WaitForSeconds(1.5f);
        EndWave();

    }

}


//0 - 1:25 הרמה
//2:05 העפה
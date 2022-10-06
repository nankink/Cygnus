using UnityEngine;
using System.Collections;

public class AniPlayPaladin : MonoBehaviour
{
    public Transform[] transforms;
    public GUIContent[] GUIContents;
    private Animator[] animator;
    private string currentState = "";

    void Start()
    {
        animator = new Animator[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            animator[i] = transforms[i].GetComponent<Animator>();
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");
        for (int i = 0; i < GUIContents.Length; i++)
        {

            if (GUILayout.Button(GUIContents[i]))
            {
                currentState = GUIContents[i].text;
            }

            AnimatorStateInfo stateInfo = animator[0].GetCurrentAnimatorStateInfo(0);

            if (!stateInfo.IsName("Base Layer.idle"))
            {
                for (int j = 0; j < animator.Length; j++)
                {
                    animator[j].SetBool("idleToIdle01", false);
                    animator[j].SetBool("idleToWalk", false);
                    animator[j].SetBool("idleToWalkBattle", false);
                    animator[j].SetBool("idleToWalkBattleL", false);
                    animator[j].SetBool("idleToWalkBattleR", false);
                    animator[j].SetBool("idleToRun", false);
                    animator[j].SetBool("idleToJump", false);
                    animator[j].SetBool("idleToDamage", false);
                    animator[j].SetBool("idleToStun", false);
                    animator[j].SetBool("idleToAttack01", false);
                    animator[j].SetBool("idleToAttack02", false);
                    animator[j].SetBool("idleToAttack03", false);
                    animator[j].SetBool("idleToAttack04", false);
                    animator[j].SetBool("idleToWin", false);
                    animator[j].SetBool("idleToDie", false);
                    animator[j].SetBool("idleToGuard", false);
                    animator[j].SetBool("idleToResurrection", false);
                }
            }
            else
            {
                for (int j = 0; j < animator.Length; j++)
                {
                    animator[j].SetBool("walkToIdle", false);
                    animator[j].SetBool("runToIdle", false);
                    animator[j].SetBool("dieToIdle", false);

                }
            }

            if (currentState != "")
            {
                if (stateInfo.IsName("Base Layer.walk") && currentState != "walk")
                {
                    for (int j = 0; j < animator.Length; j++)
                    {
                        animator[j].SetBool("walkToIdle", true);
                    }
                }

                if (stateInfo.IsName("Base Layer.run") && currentState != "run")
                {
                    for (int j = 0; j < animator.Length; j++)
                    {
                        animator[j].SetBool("runToIdle", true);
                    }
                }

                if (stateInfo.IsName("Base Layer.die") && currentState != "die")
                {
                    for (int j = 0; j < animator.Length; j++)
                    {
                        animator[j].SetBool("dieToIdle", true);
                    }
                }

                switch (currentState)
                {

                    case "stand":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToIdle01", true);
                        }
                        break;
                    case "walk":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToWalk", true);
                        }
                        break;
                    case "walk_battle":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToWalkBattle", true);
                        }
                        break;
                    case "walk_battle_l":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToWalkBattleL", true);
                        }
                        break;
                    case "walk_battle_r":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToWalkBattleR", true);
                        }
                        break;
                    case "run":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToRun", true);
                        }
                        break;
                    case "jump":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToJump", true);
                        }
                        break;
                    case "damage":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToDamage", true);
                        }
                        break;
                    case "stun":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToStun", true);
                        }
                        break;
                    case "attack01":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToAttack01", true);
                        }
                        break;

                    case "attack02":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToAttack02", true);
                        }

                        break;
                    case "attack03":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToAttack03", true);
                        }
                        break;
                    case "attack04":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToAttack04", true);
                        }
                        break;

                    case "gaurd":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToGuard", true);
                        }

                        break;
                    case "win":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToWin", true);
                        }
                        break;
                    case "die":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToDie", true);
                        }
                        break;
                    case "resurrection":
                        for (int j = 0; j < animator.Length; j++)
                        {
                            animator[j].SetBool("idleToResurrection", true);
                        }
                        break;

                    default:
                        break;
                }
                currentState = "";
            }
        }
        GUILayout.EndVertical();
    }



}

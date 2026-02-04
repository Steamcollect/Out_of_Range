using UnityEngine;

/// <summary>
/// StateMachineBehaviour qui permet de choisir aléatoirement une transition parmi plusieurs.
/// Attachez ce behaviour à un state de l'Animator, puis configurez vos transitions avec des conditions
/// basées sur le paramètre "RandomTransition" (int).
/// </summary>
public class RandomTransitionStateBehaviour : StateMachineBehaviour
{
    [Tooltip("Nom du paramètre int dans l'Animator qui détermine quelle transition prendre")]
    public string ParameterName = "RandomTransition";
    
    [Tooltip("Nombre de transitions possibles (ex: 3 si vous avez 3 transitions différentes)")]
    [Min(1)]
    public int TransitionCount = 2;
    
    [Tooltip("Réinitialiser à -1 après la transition pour éviter les boucles")]
    public bool ResetAfterTransition = true;

    // OnStateEnter est appelé quand on entre dans le state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Choisir un nombre aléatoire entre 0 et TransitionCount-1
        int randomValue = Random.Range(0, TransitionCount);
        animator.SetInteger(ParameterName, randomValue);
    }

    // OnStateExit est appelé quand on quitte le state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ResetAfterTransition)
        {
            animator.SetInteger(ParameterName, -1);
        }
    }
}


using Powerr.Character;
using System.Linq;
using UnityEngine;

namespace Powerr.UI.Control
{
    public class KeyboardInputManager : MonoBehaviour
    {
        const string HORIZONTAL_AXIS = "Horizontal";

        CharacterMainController characterController;
        CharacterMainController Character => characterController = characterController != null
            ? characterController
            : FindObjectsOfType<CharacterMainController>().SingleOrDefault(c => c.hasAuthority);

        void Update()
        {
            if (Character == null)
            {
                return;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                Character.Attack.NormalPunch();
            }
            else if (Input.GetAxisRaw(HORIZONTAL_AXIS) > 0)
            {
                Character.Movement.StartWalk();
                Character.Movement.RotatePlayer(CharacterMovementController.WalkDirection.Right);
            }
            else if (Input.GetAxisRaw(HORIZONTAL_AXIS) < 0)
            {
                Character.Movement.StartWalk();
                Character.Movement.RotatePlayer(CharacterMovementController.WalkDirection.Left);
            }
            else
            {
                Character.Movement.StopWalk();
            }
        }
    }
}
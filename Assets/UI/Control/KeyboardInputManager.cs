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
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                Character.NormalPunch();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Character.Jump();
            }
            else if (Input.GetAxisRaw(HORIZONTAL_AXIS) > 0)
            {
                Character.MoveRight();
            }
            else if (Input.GetAxisRaw(HORIZONTAL_AXIS) < 0)
            {
                Character.MoveLeft();
            }
            else
            {
                Character.StopWalk();
            }

            Character.Crouch(Input.GetKey(KeyCode.DownArrow));
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    private MovementManager selectedCharacter;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MovementManager manager = hit.collider.GetComponent<MovementManager>();
                if (manager != null)
                {
                    SelectCharacter(manager);
                    return;
                }
            }
            DeselectCharacter();
        }
    }

    void SelectCharacter(MovementManager newSelection)
    {
        if (selectedCharacter != null && selectedCharacter != newSelection)
        {
            selectedCharacter.SelectCharacter(false); // Deselect previous character
        }

        selectedCharacter = newSelection;
        selectedCharacter.SelectCharacter(true);
    }
    void DeselectCharacter()
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.SelectCharacter(false);
            selectedCharacter = null;
        }
    }
}

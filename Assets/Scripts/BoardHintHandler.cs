using TMPro;
using UnityEngine;

public class BoardHintHandler : MonoBehaviour
{
    private readonly Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);
    private const float RAY_LENGTH = 5.0f;
    private const string BOARD_HINT_1 = "Her first husband wrote a love letter. His name must be Nial H.";
    private const string BOARD_HINT_2 = "The date 23.03.2013 is engraved on the ring. E heart R.";
    private const string BOARD_HINT_3 = "She married another man on 20-12-2015. Dean & Rose forever.";
    private const string BOARD_HINT_4 = "Missing person's report should be for the man inside who hanged himself. Liam 01-06-2018";
    private const string BOARD_HINT_5 = "The other half of the tag on the piece of meat. 5th order.";
    private const string BOARD_HINT_6 = "A torn piece of skin, 28.04.2021 written on. On the post-it underneath is a tattoo appointment for someone named Vlad.";
    private const string CAPTION_HINT_1 = "This is a love letter I guess. Poor Nial, first victim.";
    private const string CAPTION_HINT_2 = "lyk, this finger stinks, 2013 E heart R";
    private const string CAPTION_HINT_3 = "Another marriage, 2015, Dean.";
    private const string CAPTION_HINT_4 = "Liam was missing since 2018. Yeah but, he is not missing.";
    private const string CAPTION_HINT_5 = "5th husband, no name on it!";
    private const string CAPTION_HINT_6 = "It's a piece of skin and a tatto!";

    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private TextMeshProUGUI boardInteractionText;
    [SerializeField] private JournelControl journelControl;
    [SerializeField] private CaptionTextTyper captionTextTyper;

    private bool enableRay = true;
    private bool enableHint1 = true;
    private bool enableHint2 = true;
    private bool enableHint3 = true;
    private bool enableHint4 = true;
    private bool enableHint5 = true;
    private bool enableHint6 = true;

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RAY_LENGTH, interactableLayers))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (enableRay)
            {
                boardInteractionText.text = "[LMB] to Examine.";
                if (Input.GetMouseButtonDown(0))
                {
                    enableRay = false;
                    boardInteractionText.text = string.Empty;
                    switch (hitObject.tag)
                    {
                        case "hint1tag":
                            captionTextTyper.StartType(CAPTION_HINT_1, false);
                            if (enableHint1)
                            {
                                enableHint1 = false;
                                journelControl.AddNewLine(BOARD_HINT_1);
                            }
                            break;
                        case "hint2tag":
                            captionTextTyper.StartType(CAPTION_HINT_2, false);
                            if (enableHint2)
                            {
                                enableHint2 = false;
                                journelControl.AddNewLine(BOARD_HINT_2);
                            }
                            break;
                        case "hint3tag":
                            captionTextTyper.StartType(CAPTION_HINT_3, false);
                            if (enableHint3)
                            {
                                enableHint3 = false;
                                journelControl.AddNewLine(BOARD_HINT_3);
                            }
                            break;
                        case "hint4tag":
                            captionTextTyper.StartType(CAPTION_HINT_4, false);
                            if (enableHint4)
                            {
                                enableHint4 = false;
                                journelControl.AddNewLine(BOARD_HINT_4);
                            }
                            break;
                        case "hint5tag":
                            captionTextTyper.StartType(CAPTION_HINT_5, false);
                            if (enableHint5)
                            {
                                enableHint5 = false;
                                journelControl.AddNewLine(BOARD_HINT_5);
                            }
                            break;
                        case "hint6tag":
                            captionTextTyper.StartType(CAPTION_HINT_6, false);
                            if (enableHint6)
                            {
                                enableHint6 = false;
                                journelControl.AddNewLine(BOARD_HINT_6);
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
        }
        else
        {
            boardInteractionText.text = string.Empty;
            enableRay = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            enableRay = false;

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogeInfo", menuName = "Dialoge/DialogeInfo")]
public class DialogeInfo : ScriptableObject
{
    [Header("Quest Info")]
    public Sprite actorIcon;
    public string questName;
    public string actorName;

    [Header("Quest Description")]
    [Header("Requirements Not Meet Text")]
    [TextArea(3, 10)]
    public List<string> requirementsNotMetText;
    [Header("Requirements Not Meet Audio")]
    public List<AudioClip> requirementsNotMetAudio;
    [Header("Description Text")]
    [TextArea(3, 10)]
    public List<string> descriptText;
    [Header("Description Audio")]
    public List<AudioClip> descriptionAudio;
    [Header("Declined Text")]
    [TextArea(3, 10)]
    public List<string> declinedText;
    [Header("Declined Audio")]
    public List<AudioClip> declinedAudio;
    [Header("Accepted Text")]
    [TextArea(3, 10)]
    public List<string> acceptedText;
    [Header("Accepted Audio")]
    public List<AudioClip> acceptedAudio;
    [Header("Completed Text")]
    [TextArea(3, 10)]
    public List<string> completedText;
    [Header("Completed Audio")]
    public List<AudioClip> completedAudio;
    [Header("Finished Text")]
    [TextArea(3, 10)]
    public List<string> finishedText;
    [Header("Finished Audio")]
    public List<AudioClip> finishedAudio;
    [Header("In Progress Text")]
    [TextArea(3, 10)]
    public List<string> inProgressText;
    [Header("In Progress Audio")]
    public List<AudioClip> inProgressAudio;

    [Header("Player Options")]
    [Header("Player Requirements Not Met Options")]
    [TextArea(3, 10)]
    public List<string> playerRequirementsNotMetOptions; // it will split by @ for each requirement
    [Header("Player Description Options")]
    [TextArea(3, 10)]
    public List<string> playerDescriptionOptions;
    [Header("Player Declined Options")]
    [TextArea(3, 10)]
    public List<string> playerDeclinedOptions;
    [Header("Player Accepted Options")]
    [TextArea(3, 10)]
    public List<string> playerAcceptedOptions;
    [Header("Player Completed Options")]
    [TextArea(3, 10)]
    public List<string> playerCompletedOptions;
    [Header("Player Finished Options")]
    [TextArea(3, 10)]
    public List<string> playerFinishedOptions;
    [Header("Player In Progress Options")]
    [TextArea(3, 10)]
    public List<string> playerInProgressOptions;
}

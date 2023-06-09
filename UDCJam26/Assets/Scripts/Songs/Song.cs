using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Create Song", order = 1)]
public class Song : ScriptableObject {
    public string songName;
    public AudioClip song;
    public SequenceBuilder sequence;
    public Image background;
}

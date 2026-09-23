using System.Collections.Generic;
using UnityEngine;

///<summary>
///The list of maps and modes the selection screen offers. One asset, edited in the
///Inspector, is the single source of truth for selectable content.
/// </summary>
[CreateAssetMenu(fileName = "GameContentCatalog", menuName = "ScriptableObject/Framework/GameContentCatalog")]
public class GameContentCatalog : ScriptableObject
{
    //
    public List<MapDefinition> Maps = new List<MapDefinition>();
    public List<GameModeDefinition> Modes = new List<GameModeDefinition>();
}

using System;

public class Player
{        
    public int Experience { get; set; }
    public int HitPoints { get; set; }
    public int Level { get; set; }
    public bool LoggedIn { get; set; }
    public string Name { get; set; }
    public Guid Uuid { get; set; }    
    public string JsonType { get; set; }
}
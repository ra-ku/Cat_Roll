using UnityEngine;

public class Skill
{
    private string _name;
    private string _description;
    private float _cooldown;
    private float _amount;

    public string Name { get { return _name; } }
    public float Amount { get { return _amount; } }

    public Skill(string name ,float cooldown, float amount)
    {
        _name = name;
        _description = "A quick burst of speed to evade attacks or close the distance.";
        _cooldown = cooldown;
        _amount = amount;
    }
}

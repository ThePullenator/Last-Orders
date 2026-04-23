using System;
using UnityEngine;


public class Drink
{
    public string Name;
    public DrinkEffectContainer[] Effects;

    public Drink(string name)
    {
        Name = name;
    }

    public Drink(string name, DrinkEffectContainer[] effects)
    {
        Name = name;
        Effects = effects;
    }

    public void OnDrink(Person drinker)
    {
        if(Effects == null) return;
        for (int i = 0; i < Effects.Length; i++)
        {
            DrinkEffectContainer effect = Effects[i];
            
            effect.Activate(drinker);
        }
    }
}

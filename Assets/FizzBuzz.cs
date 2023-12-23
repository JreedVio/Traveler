using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class FizzBuzz : MonoBehaviour
{
    private int _enableCount ;

    void OnEnable()
    {
        // interface = defines a "contract" that all the classes inheriting from should follow

        //             An interface declares "what a class should have"
        //             An inheriting class defines "how it should do it"

        //             Benefit = security + multiple inheritance + "plug-and-play"

        Rabbit rabbit = new Rabbit();
        Hawk hawk = new Hawk();
        Fish fish = new Fish();

        rabbit.Flee();
        hawk.Hunt();
        fish.Flee();
        fish.Hunt();

        Console.ReadKey();

        //Dictionary<int, int> myDictionary = new Dictionary<int, int>();
        //myDictionary.ContainsKey();
    }
    
    public interface IPrey
    {
        void Flee();
    }
    
    public interface IPredator
    {
        void Hunt();
    }

    public class Rabbit : IPrey
    {
        public void Flee()
        {
            print("Fleeing");
        }
    }
    
    public class Hawk : IPredator
    {
        public void Hunt()
        {
            print("Hunting");
        }
    }
    
    public class Fish : IPrey, IPredator
    {
        public void Flee()
        {
            print("Fleeing");
        }
        
        public void Hunt()
        {
            print("Hunting");
        }
    }
    
}  

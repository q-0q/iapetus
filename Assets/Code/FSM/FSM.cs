using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Fsm : MonoBehaviour
{

    public class FsmState : InheritableEnum
    {
        public static int Any;
    }

    public class Trigger : InheritableEnum
    {
        public static int Finish;
        public static int NoDirection;
        public static int Direction;
        public static int Jump;
    }
    
    public Wasp.Machine<int, int> Machine;
    public StateMapConfig StateMapConfig;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

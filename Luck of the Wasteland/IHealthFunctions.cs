using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthFunctions 
{
    public void LoseHealth(int damage);
    public void RecoverHealth(int healing);
}

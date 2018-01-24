using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// временный заглушечный клас для управления загрузкой уровней
public static class LevelsController  {

	public static int NumLoadLevel { get; set; }
    static LevelsController ()
    {
        NumLoadLevel = 1;
    }
}

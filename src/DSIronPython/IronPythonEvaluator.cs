﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;

namespace DSIronPython
{
    /// <summary>
    /// 
    /// </summary>
    public class IronPythonEvaluator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="bindingNames"></param>
        /// <param name="bindingValues"></param>
        /// <returns></returns>
        public static object EvaluateIronPythonScript(string code, IList bindingNames, IList bindingValues)
        {
            var engine = Python.CreateEngine();
            var scope = engine.CreateScope();

            var amt = Math.Min(bindingNames.Count, bindingValues.Count);

            for (int i = 0; i < amt; i++)
            {
                scope.SetVariable((string)bindingNames[i], bindingValues[i]);
            }

            engine.CreateScriptSourceFromString(code).Execute(scope);

            return scope.ContainsVariable("OUT") ? scope.GetVariable("OUT") : null;
        }
    }
}
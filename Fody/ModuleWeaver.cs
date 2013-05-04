﻿using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public string ProjectDirectoryPath { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
    }

    public void Execute()
    {
        if (string.IsNullOrEmpty(ProjectDirectoryPath))
        {
            throw new WeavingException("Requires version 1.13.8 (or higher) of Fody.");
        }
        FindCoreReferences();
        foreach (var type in ModuleDefinition
            .GetTypes()
            .Where(x => x.IsClass()))
        {

            foreach (var method in type.Methods)
            {
                //skip for abstract and delegates
                if (!method.HasBody)
                {
                    continue;
                }
                var methodProcessor = new MethodProcessor
                    {
                        Method = method,
                        ModuleWeaver = this,
                        LogInfo = LogInfo
                    };
                methodProcessor.Process();
            }
        }
        CleanReferences();
    }

}
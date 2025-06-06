﻿using System.Reflection;

namespace CleanArchitectureUtility.Extensions.Abstractions.Loggers;

public class ScopeInformation : IScopeInformation
{
    public ScopeInformation()
    {
        HostScopeInfo = new Dictionary<string, string>
        {
            {
                "MachineName",
                Environment.MachineName
            },
            {
                "EntryPoint",
                Assembly.GetEntryAssembly().GetName().Name
            }
        };
        RequestScopeInfo = new Dictionary<string, string>
        {
            {
                "RequestId",
                Guid.NewGuid().ToString()
            }
        };
    }

    public Dictionary<string, string> HostScopeInfo { get; }

    public Dictionary<string, string> RequestScopeInfo { get; }
}
﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace NuGet.PackageManagement
{
    public class PackageAlreadyInstalledException : Exception
    {
        public PackageAlreadyInstalledException(string message)
            : base(message)
        {
        }
    }

    public class NuGetVersionNotSatisfiedException : Exception
    {
        public NuGetVersionNotSatisfiedException(string message)
            : base(message)
        {
        }
    }
}

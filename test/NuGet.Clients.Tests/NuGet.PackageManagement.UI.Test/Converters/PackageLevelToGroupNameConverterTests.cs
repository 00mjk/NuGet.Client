// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using NuGet.VisualStudio;
using Xunit;

namespace NuGet.PackageManagement.UI.Test.Converters
{
    public class PackageLevelToGroupNameConverterTests
    {
        public static IEnumerable<object[]> GetConvertData()
        {
            yield return new object[] { PackageLevel.TopLevel, Resources.PackageLevel_TopLevelPackageHeaderText };
            yield return new object[] { PackageLevel.Transitive, Resources.PackageLevel_TransitivePackageHeaderText };
            yield return new object[] { Resources.PackageLevel_TopLevelPackageHeaderText, null };
            yield return new object[] { "some string", null };
            yield return new object[] { new object(), null };
            yield return new object[] { 12345, null };
            yield return new object[] { null, null };
        }

        [Theory]
        [MemberData(nameof(GetConvertData))]
        public void Convert_MultipleInputs_Succeeds(object input, object expected)
        {
            var converterToTest = new PackageLevelToGroupNameConverter();

            object value = converterToTest.Convert(input, targetType: null, parameter: null, culture: null);

            Assert.Equal(expected, value);
        }
    }
}

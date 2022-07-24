using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBD.AspNetCore.Tests;

[TestClass]
public class ErrorCodeExtractTests
{
    [TestMethod]
    public void ExtractCodeFromType()
    {
        var result = ErrorCodeExtractor.Extract(typeof(ErrorCodeExtractTests).Assembly.Location, "HBD.*.dll",
            new List<string> { "ErrorCodes", "StatusMessages" });
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ExtractCodeFromType_WithDuplicate_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => ErrorCodeExtractor.Extract(
            typeof(ErrorCodeExtractTests).Assembly.Location, "HBD.*.dll",
            new List<string> { "ErrorCodes", "StatusMessages", "StatusMessagesDuplicate" }));
    }

    [TestMethod]
    public void ExtractCodeFromType_WithCustomType_ShouldSuccess()
    {
        var result = ErrorCodeExtractor.Extract(
            typeof(ErrorCodeExtractTests).Assembly.Location,
            "HBD.*.dll",
            null,
            additional: typeof(TestWithDuplicateStatusType));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("ERROR MESG2", result[0].Value);
    }

    [TestMethod]
    public void ExtractCodeFromType_WithInvalidCustomType_ShouldEmpty()
    {
        var result = ErrorCodeExtractor.Extract(
            typeof(ErrorCodeExtractTests).Assembly.Location,
            "HBD.*.dll",
            null,
            additional: typeof(TestClassAttribute));

        Assert.AreEqual(0, result.Count);
    }
}


public class TestType
{
    private class ErrorCodes
    {
        public const string Error0 = "ERROR MESG";
    }
}

public class TestWithStatusType
{
    private class StatusMessages
    {
        public const string Error1 = "ERROR MESG";
    }
}

public class TestWithDuplicateStatusType
{
    private class StatusMessagesDuplicate
    {
        public const string Error1 = "ERROR MESG2";
    }
}
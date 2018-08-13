using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace eWolfCodeAnalyser.Test
{
    [TestClass]
    public class MoreThanOneClassPerFileAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void ShoulNotReportClass()
        {
            var test = @"public class(){}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void ShouldReportMoreThanOneClass()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class TypeName
    {
    }

    public class TypeNameOther
    {
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MoreThanOneClassPerFileAnalyzer.DiagnosticId,
                Message = "Should only have one class per file",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 5)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MoreThanOneClassPerFileAnalyzer();
        }
    }
}

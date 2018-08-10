using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using eWolfCodeAnalyser;

namespace eWolfCodeAnalyser.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        [TestMethod]
        public void ShoulNotReportAnyRegions()
        {
            var test = @"// comment region
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void ShouldReportRegionAndEndRegion()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    #region test

    public class TypeName
    {
    }

    #endregion test
}";
            var expected = new DiagnosticResult
            {
                Id = RemoveRegionsAnalyzer.DiagnosticId,
                Message = "Regions are not needed any more",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 5)
                        }
            };
            var expected2 = new DiagnosticResult
            {
                Id = RemoveRegionsAnalyzer.DiagnosticId,
                Message = "Regions are not needed any more",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                     new[] {
                            new DiagnosticResultLocation("Test0.cs", 17, 5)
                         }
            };

            VerifyCSharpDiagnostic(test, new[] { expected, expected2 });

            string results = VerifyCSharpFix(test);

            Assert.AreEqual(false, results.Contains("#region"));
            Assert.AreEqual(false, results.Contains("#endregion"));
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RemoveRegionsAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new RemoveRegionsAnalyzer();
        }
    }
}
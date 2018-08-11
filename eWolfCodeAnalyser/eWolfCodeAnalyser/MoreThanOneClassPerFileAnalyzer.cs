using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace eWolfCodeAnalyser
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MoreThanOneClassPerFileAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MoreThanOneClass";

        private static readonly LocalizableString Title =
                new LocalizableResourceString(nameof(Resources.AnalyerOneClassTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyerOneClassMessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerOneClassDescription), Resources.ResourceManager, typeof(Resources));

        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            int count = 0;

            context.RegisterSyntaxNodeAction(nodeContext =>
            {
                if (count++ > 0)
                {
                    var diagnostic = Diagnostic.Create(Rule, nodeContext.Node.GetLocation());
                    nodeContext.ReportDiagnostic(diagnostic);
                }
            }, ImmutableArray.Create(SyntaxKind.ClassDeclaration));
        }
    }
}

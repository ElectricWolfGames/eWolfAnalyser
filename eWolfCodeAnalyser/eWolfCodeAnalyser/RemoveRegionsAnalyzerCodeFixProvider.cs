using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace eWolfCodeAnalyser
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRegionsAnalyzerCodeFixProvider)), Shared]
    public class RemoveRegionsAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Remove Regions";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(RemoveRegionsAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var nodeWithRegion = root.FindNode(context.Span, true, true);

            DirectiveTriviaSyntax region = nodeWithRegion as RegionDirectiveTriviaSyntax;
            if (region == null)
            {
                region = nodeWithRegion as EndRegionDirectiveTriviaSyntax;
            }

            if (region == null)
                return;

            CodeAction action = CodeAction.Create(title, CancellationToken =>
            {
                var newRoot = root.RemoveNodes(region.GetRelatedDirectives(), SyntaxRemoveOptions.AddElasticMarker);
                var newDocument = context.Document.WithSyntaxRoot(newRoot);
                return Task.FromResult(newDocument);
            });

            context.RegisterCodeFix(action, context.Diagnostics.First());
        }
    }
}

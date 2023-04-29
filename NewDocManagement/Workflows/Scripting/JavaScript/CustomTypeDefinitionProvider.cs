using Elsa.Scripting.JavaScript.Services;
using NewDocManagement.Workflows.Activities;

namespace NewDocManagement.Workflows.Scripting.JavaScript
{
    /// <summary>
    /// Register .NET types for which we want to include with JS intellisense.
    /// </summary>
    public class CustomTypeDefinitionProvider : TypeDefinitionProvider
    {
        public override IEnumerable<Type> CollectTypes(TypeDefinitionContext context)
        {
            yield return typeof(DocumentFile);
        }
    }
}

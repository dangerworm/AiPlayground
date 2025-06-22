using AiPlayground.Api.Actions;
using AiPlayground.Api.Models.Actions;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text.Json.Serialization;
using AiPlayground.Api.Attributes;

namespace AiPlayground.Api.Providers
{
    public class ActionProvider
    {
        public string GetAvailableActions()
        {
            var actions = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IAction).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            var actionDescriptions = new List<string>();
            foreach (var action in actions)
            {
                var description = action
                    .GetProperty("Description")?
                    .GetValue(Activator.CreateInstance(action))?
                    .ToString()!;

                var inputParameters = GetInputParameters(action);

                var actionModel = new ActionModel
                {
                    ActionName = action.Name,
                    Description = description,
                    InputParameters = inputParameters
                };

                actionDescriptions.Add(actionModel.ToString());
            }

            return string.Join(Environment.NewLine, actionDescriptions);
        }

        private static List<ActionInputParameterModel>? GetInputParameters(Type action)
        {
            var actionInputParameters = new List<ActionInputParameterModel>();

            var actionProperties = action.GetProperties()
                .Where(property => !string.Equals(property.Name, nameof(ActionBase.Description)));

            foreach (var property in actionProperties)
            {
                var jsonName = GetAttributeProperty<JsonPropertyNameAttribute>(property, action.Name, "Name");
                var exampleValue = GetAttributeProperty<ExampleValueAttribute>(property, action.Name, "ExampleValue");
                var isRequired = property.GetCustomAttribute<RequiredMemberAttribute>() != null;

                actionInputParameters.Add(new ActionInputParameterModel
                {
                    ExampleValue = exampleValue,
                    ParameterName = jsonName,
                    IsRequired = isRequired,
                    Type = property.PropertyType.Name
                });
            }

            return actionInputParameters.Count != 0
                ? actionInputParameters
                : null;
        }

        private static string GetAttributeProperty<TAttribute>(MemberInfo member, string actionName, string innerPropertyName)
            where TAttribute : Attribute
        {
            var attribute = member.GetCustomAttribute<TAttribute>(inherit: false)
                ?? throw new InvalidOperationException($"Property {member.Name} in action {actionName} does not have a valid {typeof(TAttribute).Name} attribute.");

            // Get the value of the inner property of the attribute by name
            var innerProperty = typeof(TAttribute).GetProperty(innerPropertyName)
                ?? throw new InvalidOperationException($"Attribute {typeof(TAttribute).Name} does not have a property named {innerPropertyName}.");

            var value = innerProperty.GetValue(attribute)?.ToString()
                ?? throw new InvalidOperationException($"Property {member.Name} in action {actionName} does not have a valid value for {innerPropertyName}.");

            return value;
        }
    }
}

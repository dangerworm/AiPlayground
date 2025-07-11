﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using AiPlayground.Api.Attributes;
using AiPlayground.Core.Models.Actions;

namespace AiPlayground.Api.Actions
{
    public class ActionProvider(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        
        private static readonly Dictionary<Type, string> _typeAlias = new()
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            // Yes, this is an odd one.  Technically it's a type though.
            { typeof(void), "void" }
        };

        public T GetActionInstance<T>(string actionName)
            where T: IAction
        {
            if (string.IsNullOrWhiteSpace(actionName))
            {
                throw new ArgumentException("Action name cannot be null or empty.", nameof(actionName));
            }

            var actionType = GetActionTypes()
                .FirstOrDefault(type => type.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"Action '{actionName}' not found.");

            T instance = (T)_serviceProvider.GetRequiredService(actionType)
                ?? throw new InvalidOperationException($"Action {actionType.Name} could not be instantiated. Ensure it is registered in the service provider.");
            
            return instance;
        }

        public IEnumerable<IAction> GetActionInstances()
        {
            var actionTypes = GetActionTypes();
            
            var instances = new List<IAction>();
            foreach (var actionType in actionTypes)
            {
                var instance = _serviceProvider.GetRequiredService(actionType) as IAction
                    ?? throw new InvalidOperationException($"Action {actionType.Name} could not be instantiated. Ensure it is registered in the service provider.");

                instances.Add(instance);
            }

            return instances;
        }

        public string GetActionDescriptions()
        {
            var actionInstances = GetActionInstances();

            var actionDescriptions = new List<string>();
            foreach (var instance in actionInstances)
            {
                var actionType = instance.GetType();

                var description = actionType
                    .GetProperty("Description")?
                    .GetValue(instance)?
                    .ToString()!;

                var inputParameters = GetInputParameters(actionType);

                var actionModel = new ActionModel
                {
                    ActionName = actionType.Name[..^6],
                    Description = description,
                    InputParameters = inputParameters
                };

                actionDescriptions.Add(actionModel.ToString());
            }

            return string.Join(Environment.NewLine, actionDescriptions);
        }

        private static IEnumerable<Type> GetActionTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IAction).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        }

        private static List<ActionInputParameterModel>? GetInputParameters(Type action)
        {
            var actionProperties = action.GetProperties()
                .Where(property => GetAttribute<IgnorePropertyDuringProcessingAttribute>(property) is null);

            var actionInputParameters = new List<ActionInputParameterModel>();
            foreach (var property in actionProperties)
            {
                var jsonName = GetAttributePropertyValue<JsonPropertyNameAttribute>(property, action.Name, "Name");
                var exampleValue = GetAttributePropertyValue<ExampleValueAttribute>(property, action.Name, "ExampleValue");
                var isRequired = property.GetCustomAttribute<RequiredMemberAttribute>() != null;

                actionInputParameters.Add(new ActionInputParameterModel
                {
                    ExampleValue = FormatRawExampleValue(exampleValue),
                    ParameterName = jsonName!.ToString()!.Trim(),
                    IsRequired = isRequired,
                    Type = TypeNameOrAlias(property.PropertyType)
                });
            }

            return actionInputParameters.Count != 0
                ? actionInputParameters
                : null;
        }

        private static string FormatRawExampleValue(string exampleValue)
        {
            object parsedValue;
            string formattedExampleValue;

            if (bool.TryParse(exampleValue, out var boolVal))
            {
                parsedValue = boolVal;
            }
            else if (int.TryParse(exampleValue, out var intVal))
            { 
                parsedValue = intVal; 
            }
            else if (double.TryParse(exampleValue, out var doubleVal))
            { 
                parsedValue = doubleVal;
            }
            else if (DateTime.TryParse(exampleValue, out var dateVal))
            { 
                parsedValue = dateVal;
            }
            else if (Guid.TryParse(exampleValue, out var guidVal))
            { 
                parsedValue = guidVal;
            }
            else
            { 
                parsedValue = exampleValue; 
            }

            if (parsedValue is bool or int or long or double or float or decimal or byte or short or sbyte or uint or ulong or ushort)
            {
                formattedExampleValue = Convert.ToString(parsedValue, System.Globalization.CultureInfo.InvariantCulture)!;
            }
            else if (parsedValue is DateTime dt)
            {
                formattedExampleValue = @$"""{dt:O}"""; // ISO 8601 format
            }
            else if (parsedValue is null)
            {
                throw new InvalidDataException($"Example values should not be null.");
            }
            else
            {
                formattedExampleValue = @$"""{parsedValue.ToString()!.Trim()}""";
            }

            return formattedExampleValue;
        }

        private static string TypeNameOrAlias(Type type)
        {
            return _typeAlias.TryGetValue(type, out var alias)
                ? alias
                : type.Name;
        }

        private static TAttribute? GetAttribute<TAttribute>(MemberInfo member)
            where TAttribute : Attribute
        {
            return member.GetCustomAttribute<TAttribute>(inherit: false);
        }

        private static string GetAttributePropertyValue<TAttribute>(MemberInfo member, string actionName, string innerPropertyName)
            where TAttribute : Attribute
        {
            var attribute = GetAttribute<TAttribute>(member)
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

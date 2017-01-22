using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Reflection;

namespace Abp.Runtime.Validation.Interception
{
    /// <summary>
    /// This class is used to validate a method call (invocation) for method arguments.
    /// </summary>
    public class MethodInvocationValidator : ITransientDependency
    {
        protected MethodInfo Method { get; private set; }
        protected object[] ParameterValues { get; private set; }
        protected ParameterInfo[] Parameters { get; private set; }
        protected List<ValidationResult> ValidationErrors { get; }
        protected List<IShouldNormalize> ObjectsToBeNormalized { get; }

        private readonly IValidationConfiguration _configuration;
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Creates a new <see cref="MethodInvocationValidator"/> instance.
        /// </summary>
        public MethodInvocationValidator(IValidationConfiguration configuration, IIocResolver iocResolver)
        {
            _configuration = configuration;
            _iocResolver = iocResolver;

            ValidationErrors = new List<ValidationResult>();
            ObjectsToBeNormalized = new List<IShouldNormalize>();
        }

        /// <param name="method">Method to be validated</param>
        /// <param name="parameterValues">List of arguments those are used to call the <paramref name="method"/>.</param>
        public virtual void Initialize(MethodInfo method, object[] parameterValues)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (parameterValues == null)
            {
                throw new ArgumentNullException(nameof(parameterValues));
            }

            Method = method;
            ParameterValues = parameterValues;
            Parameters = method.GetParameters();
        }

        /// <summary>
        /// Validates the method invocation.
        /// </summary>
        public void Validate()
        {
            CheckInitialized();

            if (!Method.IsPublic)
            {
                return;
            }

            if (IsValidationDisabled())
            {
                return;                
            }

            if (Parameters.IsNullOrEmpty())
            {
                return;
            }

            if (Parameters.Length != ParameterValues.Length)
            {
                throw new Exception("Method parameter count does not match with argument count!");
            }

            for (var i = 0; i < Parameters.Length; i++)
            {
                ValidateMethodParameter(Parameters[i], ParameterValues[i]);
            }

            if (ValidationErrors.Any())
            {
                throw new AbpValidationException(
                    "Method arguments are not valid! See ValidationErrors for details.",
                    ValidationErrors
                    );
            }

            foreach (var objectToBeNormalized in ObjectsToBeNormalized)
            {
                objectToBeNormalized.Normalize();
            }
        }

        private void CheckInitialized()
        {
            if (Method == null)
            {
                throw new AbpException("This object has not been initialized. Call Initialize method first.");
            }
        }

        protected virtual bool IsValidationDisabled()
        {
            if (Method.IsDefined(typeof(EnableValidationAttribute), true))
            {
                return false;
            }

            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableValidationAttribute>(Method) != null;
        }

        /// <summary>
        /// Validates given parameter for given value.
        /// </summary>
        /// <param name="parameterInfo">Parameter of the method to validate</param>
        /// <param name="parameterValue">Value to validate</param>
        protected virtual void ValidateMethodParameter(ParameterInfo parameterInfo, object parameterValue)
        {
            if (parameterValue == null)
            {
                if (!parameterInfo.IsOptional && 
                    !parameterInfo.IsOut && 
                    !TypeHelper.IsPrimitiveExtendedIncludingNullable(parameterInfo.ParameterType))
                {
                    ValidationErrors.Add(new ValidationResult(parameterInfo.Name + " is null!", new[] { parameterInfo.Name }));
                }

                return;
            }

            ValidateObjectRecursively(parameterValue);
        }

        protected virtual void ValidateObjectRecursively(object validatingObject)
        {
            if (validatingObject == null)
            {
                return;
            }

            SetDataAnnotationAttributeErrors(validatingObject);

            //Validate items of enumerable
            if (validatingObject is IEnumerable && !(validatingObject is IQueryable))
            {
                foreach (var item in (validatingObject as IEnumerable))
                {
                    ValidateObjectRecursively(item);
                }
            }

            //Custom validations
            (validatingObject as ICustomValidate)?.AddValidationErrors(
                new CustomValidationContext(
                    ValidationErrors,
                    _iocResolver
                )
            );

            //Add list to be normalized later
            if (validatingObject is IShouldNormalize)
            {
                ObjectsToBeNormalized.Add(validatingObject as IShouldNormalize);
            }

            //Do not recursively validate for enumerable objects
            if (validatingObject is IEnumerable)
            {
                return;
            }

            var validatingObjectType = validatingObject.GetType();

            //Do not recursively validate for primitive objects
            if (TypeHelper.IsPrimitiveExtendedIncludingNullable(validatingObjectType))
            {
                return;
            }

            if (_configuration.IgnoredTypes.Any(t => t.IsInstanceOfType(validatingObject)))
            {
                return;
            }

            var properties = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                if (property.Attributes.OfType<DisableValidationAttribute>().Any())
                {
                    continue;
                }

                ValidateObjectRecursively(property.GetValue(validatingObject));
            }
        }

        /// <summary>
        /// Checks all properties for DataAnnotations attributes.
        /// </summary>
        protected virtual void SetDataAnnotationAttributeErrors(object validatingObject)
        {
            var properties = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                var validationAttributes = property.Attributes.OfType<ValidationAttribute>().ToArray();
                if (validationAttributes.IsNullOrEmpty())
                {
                    continue;
                }

                var validationContext = new ValidationContext(validatingObject)
                {
                    DisplayName = property.DisplayName,
                    MemberName = property.Name
                };

                foreach (var attribute in validationAttributes)
                {
                    var result = attribute.GetValidationResult(property.GetValue(validatingObject), validationContext);
                    if (result != null)
                    {
                        ValidationErrors.Add(result);
                    }
                }
            }

            if (validatingObject is IValidatableObject)
            {
                var results = (validatingObject as IValidatableObject).Validate(new ValidationContext(validatingObject));
                ValidationErrors.AddRange(results);
            }
        }
    }
}

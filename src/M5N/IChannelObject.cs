using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace M5N;

public interface IChannelObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    where T : unmanaged, IChannelObject<T>
{
    public static abstract TagCode TagCode { get; }

    public IEnumerable<ValidationException> Validate()
    {
        var codes = Enum.GetValuesAsUnderlyingType<TagCode>();
        var iCode = 0;
        for (; iCode < codes.Length; ++iCode)
            if ((TagCode)codes.GetValue(iCode)! == T.TagCode)
                break;
        if (iCode == codes.Length)
            yield return new ValidationException(
                new ValidationResult("Undeclared TagCode detected.", new[] { nameof(T.TagCode) }),
                null,
                T.TagCode);

        foreach (var property in typeof(T).GetProperties())
        {
            if (property.PropertyType.IsSubclassOf(typeof(IChannelObject<T>)))
            {
                var results = (property.GetValue(this) as IChannelObject<T>)?.Validate();
                if (results is not null)
                    foreach (var result in results)
                        yield return result;
            }

            foreach (var data in property.CustomAttributes)
            {
                if (!data.AttributeType.IsSubclassOf(typeof(ValidationAttribute)))
                    continue;

                var attribute = (ValidationAttribute)data.Constructor.Invoke(
                    data.ConstructorArguments.Select(argument => argument.Value).ToArray());

                foreach (var argument in data.NamedArguments)
                {
                    if (argument.IsField)
                    {
                        var field = (FieldInfo)argument.MemberInfo;
                        field.SetValueDirect(__makeref(attribute), argument.TypedValue.Value!);
                    }
                    else
                    {
                        var prop = (PropertyInfo)argument.MemberInfo;
                        prop.SetValue(attribute, argument.TypedValue.Value);
                    }
                }

                var value = property.GetValue(this);
                if (!attribute.IsValid(value))
                {
                    var builder = new StringBuilder(64);
                    if (property.DeclaringType is not null)
                        builder.Append(property.DeclaringType).Append('.');
                    builder.Append(property.Name);
                    
                    yield return new ValidationException(
                        new ValidationResult(attribute.FormatErrorMessage(builder.ToString())),
                        attribute,
                        value);
                }
            }
        }
    }
}
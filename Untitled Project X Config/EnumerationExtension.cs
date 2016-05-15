using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Untitled_Project_X_Config
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AbbreviationAttribute: Attribute
    {
        private string _name;
        public AbbreviationAttribute(string name)
        {
            _name = name;
        }
        public static string Get(Type tp, string name)
        {
            MemberInfo[] mi = tp.GetMember(name);
            if (mi != null && mi.Length > 0)
            {
                AbbreviationAttribute attr = Attribute.GetCustomAttribute(mi[0], typeof(AbbreviationAttribute)) as AbbreviationAttribute;
                if (attr != null)
                    return attr._name;
            }
            return null;
        }
        public static string Get(object enm)
        {
            if (enm != null)
            {
                MemberInfo[] mi = enm.GetType().GetMember(enm.ToString());
                if (mi != null && mi.Length > 0)
                {
                    AbbreviationAttribute attr = Attribute.GetCustomAttribute(mi[0], typeof(AbbreviationAttribute)) as AbbreviationAttribute;
                    if (attr != null)
                        return attr._name;
                }
            }
            return null;
        }
    }

    public class EnumerationExtension : MarkupExtension
    {
        private Type _enumType;

        public EnumerationExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            EnumType = enumType;
        }

        public Type EnumType
        {
            get { return _enumType; }
            private set
            {
                if (_enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                _enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);

            return (
              from object enumValue in enumValues
              select new EnumerationMember
              {
                  Value = enumValue,
                  Description = GetDescription(enumValue),
                  Abbreviation = AbbreviationAttribute.Get(enumValue)
              }).ToArray();
        }

        private string GetDescription(object enumValue)
        {
            var descriptionAttribute = EnumType
              .GetField(enumValue.ToString())
              .GetCustomAttributes(typeof(DescriptionAttribute), false)
              .FirstOrDefault() as DescriptionAttribute;

            return descriptionAttribute != null
              ? descriptionAttribute.Description
              : enumValue.ToString();
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public string Abbreviation { get; set; }
            public object Value { get; set; }
        }
    }
}

using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace confirmUser.Data
{
    public class LocalizedRequiredAttribute : RequiredAttribute
    {
        private static IStringLocalizer _localizer;

        public static void SetLocalizer(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        public override string FormatErrorMessage(string name)
        {
            if (_localizer == null)
            {
                throw new InvalidOperationException("Localizer is not set.");
            }

            return _localizer[$"{name}Required"];
        }
    }

    public class LocalizedRegularExpressionAttribute : RegularExpressionAttribute
    {
        private static IStringLocalizer _localizer;

        public LocalizedRegularExpressionAttribute(string pattern) : base(pattern)
        {
        }

        public static void SetLocalizer(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        public override string FormatErrorMessage(string name)
        {
            if (_localizer == null)
            {
                throw new InvalidOperationException("Localizer is not set.");
            }

            return _localizer[$"{name}Format"];
        }
    }
    }
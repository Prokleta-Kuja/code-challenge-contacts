using FluentValidation.Validators;
using PhoneNumbers;

namespace PublicContacts.App.Validators
{
    public class PhoneNumberValidator : PropertyValidator
    {
        private readonly string _defaultRegion;
        public PhoneNumberValidator(string defaultRegion = "HR") : base("Wrong phone number format")
        {
            _defaultRegion = defaultRegion;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var number = context.PropertyValue.ToString();
            var lib = PhoneNumberUtil.GetInstance();
            if (!lib.IsPossibleNumber(number, _defaultRegion))
                return false;

            var parsed = lib.Parse(number, _defaultRegion);

            return lib.IsValidNumber(parsed);
        }
    }
}
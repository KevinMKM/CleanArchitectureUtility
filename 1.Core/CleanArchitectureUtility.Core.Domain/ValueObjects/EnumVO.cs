namespace CleanArchitectureUtility.Core.Domain.ValueObjects
{
    public class EnumVO : BaseValueObject<EnumVO>
    {
        public Enum Value { get; }

        private EnumVO()
        {
        }

        public EnumVO(Enum value)
        {
            Validate(value);
            Value = value;
        }

        protected virtual void Validate(Enum value)
        {
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static EnumVO FromEnum(Enum value) => new EnumVO(value);
        public static explicit operator Enum(EnumVO vo) => vo.Value;
        public static implicit operator EnumVO(Enum value) => new EnumVO(value);
    }
}

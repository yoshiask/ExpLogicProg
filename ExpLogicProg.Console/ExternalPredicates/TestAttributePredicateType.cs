using Guan.Logic;

namespace Guan;

public class TestAttributePredicateType : PredicateType
{
    private static TestAttributePredicateType? _instance;

    private class Resolver : BooleanPredicateResolver
    {
        public Resolver(CompoundTerm input, Constraint constraint, QueryContext context)
            : base(input, constraint, context)
        {
        }

        protected override Task<bool> CheckAsync()
        {
            var t1 = Input.Arguments[0].Value.GetEffectiveTerm().GetObjectValue();
            var t2 = Input.Arguments[1].Value.GetEffectiveTerm().GetObjectValue();

            return Task.FromResult(true);
        }
    }

    public static TestAttributePredicateType Singleton(string name)
    {
        return _instance ??= new TestAttributePredicateType(name);
    }

    // Note the base constructor's arguments minPositionalArguments and maxPositionalArguments. You control the minimum and maximum number of arguments the predicate supports.
    // In this case, rules that employ this external predicate must supply only 2 positional arguments.
    private TestAttributePredicateType(string name)
        : base(name, true, 2, 2)
    {

    }

    public override PredicateResolver CreateResolver(CompoundTerm input, Constraint constraint, QueryContext context)
    {
        return new Resolver(input, constraint, context);
    }
}
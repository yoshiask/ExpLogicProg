﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Guan.Logic;

namespace Guan
{
    public class TestSubPredicateType : PredicateType
    {
        private static TestSubPredicateType Instance;

        class Resolver : BooleanPredicateResolver
        {
            public Resolver(CompoundTerm input, Constraint constraint, QueryContext context)
                : base(input, constraint, context)
            {
            }

            protected override Task<bool> CheckAsync()
            {
                long t1 = (long)Input.Arguments[0].Value.GetEffectiveTerm().GetObjectValue();
                long t2 = (long)Input.Arguments[1].Value.GetEffectiveTerm().GetObjectValue();
                long result = t1 - t2;
                System.Console.WriteLine($"subresult: {result}");

                return Task.FromResult(true);
            }
        }

        public static TestSubPredicateType Singleton(string name)
        {
            return Instance ??= new TestSubPredicateType(name);
        }

        // Note the base constructor's arguments minPositionalArguments and maxPositionalArguments. You control the minimum and maximum number of arguments the predicate supports.
        // In this case, rules that employ this external predicate must supply only 2 positional arguments.
        private TestSubPredicateType(string name)
            : base(name, true, 2, 2)
        {

        }

        public override PredicateResolver CreateResolver(CompoundTerm input, Constraint constraint, QueryContext context)
        {
            return new Resolver(input, constraint, context);
        }
    }
}

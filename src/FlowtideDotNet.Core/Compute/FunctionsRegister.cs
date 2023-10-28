﻿// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using FlexBuffers;
using FlowtideDotNet.Core.Compute.Internal;
using FlowtideDotNet.Substrait.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlowtideDotNet.Core.Compute
{
    internal class FunctionsRegister : IFunctionsRegister
    {
        private Dictionary<string, FunctionDefinition> _scalarFunctions;
        private Dictionary<string, AggregateFunctionDefinition> _aggregateFunctions;

        public FunctionsRegister()
        {
            _scalarFunctions = new Dictionary<string, FunctionDefinition>(StringComparer.OrdinalIgnoreCase);
            _aggregateFunctions = new Dictionary<string, AggregateFunctionDefinition>(StringComparer.OrdinalIgnoreCase);
        }

        public bool TryGetScalarFunction(string uri, string name, out FunctionDefinition functionDefinition)
        {
            return _scalarFunctions.TryGetValue($"{uri}:{name}", out functionDefinition);
        }

        public void RegisterScalarFunction(string uri, string name, Func<ScalarFunction, ParametersInfo, ExpressionVisitor<System.Linq.Expressions.Expression, ParametersInfo>, System.Linq.Expressions.Expression> mapFunc)
        {
            _scalarFunctions.Add($"{uri}:{name}", new FunctionDefinition(uri, name, mapFunc));
        }

        public void RegisterScalarFunctionWithExpression(string uri, string name, Expression<Func<FlxValue, FlxValue>> expression)
        {
            RegisterScalarFunction(uri, name, (scalarFunction, parametersInfo, expressionVisitor) =>
            {
                if (scalarFunction.Arguments.Count != 1)
                {
                    throw new ArgumentException($"Scalar function {name} must have one argument");
                }

                var p1 = expressionVisitor.Visit(scalarFunction.Arguments[0], parametersInfo);
                var expressionBody = expression.Body;
                // TODO: Replace parameter with p1
                var expressionBodyWithParameter = new ParameterReplacerVisitor(expression.Parameters[0], p1).Visit(expressionBody);
                return expressionBodyWithParameter;
            });
        }

        public void RegisterScalarFunctionWithExpression(string uri, string name, Expression<Func<FlxValue, FlxValue, FlxValue>> expression)
        {
            RegisterScalarFunction(uri, name, (scalarFunction, parametersInfo, expressionVisitor) =>
            {
                if (scalarFunction.Arguments.Count != 2)
                {
                    throw new ArgumentException($"Scalar function {name} must have two arguments");
                }

                var p1 = expressionVisitor.Visit(scalarFunction.Arguments[0], parametersInfo);
                var p2 = expressionVisitor.Visit(scalarFunction.Arguments[1], parametersInfo);
                var expressionBody = expression.Body;
                // TODO: Replace parameter with p1
                expressionBody = new ParameterReplacerVisitor(expression.Parameters[0], p1).Visit(expressionBody);
                expressionBody = new ParameterReplacerVisitor(expression.Parameters[1], p2).Visit(expressionBody);
                return expressionBody;
            });
        }

        public void RegisterAggregateFunction(
            string uri, 
            string name, 
            Func<AggregateFunction, ParametersInfo, ExpressionVisitor<System.Linq.Expressions.Expression, ParametersInfo>, ParameterExpression, ParameterExpression, System.Linq.Expressions.Expression> mapFunc,
            Func<byte[], FlxValue> stateToValueFunc)
        {
            _aggregateFunctions.Add($"{uri}:{name}", new AggregateFunctionDefinition(uri, name, mapFunc, stateToValueFunc));
        }

        public bool TryGetAggregateFunction(string uri, string name, [NotNullWhen(true)] out AggregateFunctionDefinition? aggregateFunctionDefinition)
        {
            return _aggregateFunctions.TryGetValue($"{uri}:{name}", out aggregateFunctionDefinition);
        }
    }
}

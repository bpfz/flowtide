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
using FlowtideDotNet.Core.Compute.Group;
using FlowtideDotNet.Core.Operators.Write;
using FlowtideDotNet.Core.Storage;
using FlowtideDotNet.Storage.Serializers;
using FlowtideDotNet.Storage.StateManager;
using FlowtideDotNet.Storage.Tree;
using FlowtideDotNet.Substrait.Expressions;
using FlowtideDotNet.Substrait.FunctionExtensions;
using System.Linq.Expressions;
using static SqlParser.Ast.ConflictTarget;
using static SqlParser.Ast.WildcardExpression;

namespace FlowtideDotNet.Core.Compute.Internal.StatefulAggregations
{
    internal class MinAggregationInsertComparer : IComparer<StreamEvent>
    {
        private readonly int length;

        public MinAggregationInsertComparer(int length)
        {
            this.length = length;
        }
        public int Compare(StreamEvent x, StreamEvent y)
        {
            for (int i = 0; i < length; i++)
            {
                var c = FlxValueRefComparer.CompareTo(x.Vector.GetRef(i), y.Vector.GetRef(i));
                if (c != 0)
                {
                    return c;
                }
            }
            return 0;
        }
    }

    internal class MinAggregationSingleton
    {
        private readonly int keyLength;

        public MinAggregationSingleton(IBPlusTree<StreamEvent, int> tree, int keyLength)
        {
            Tree = tree;
            this.keyLength = keyLength;
        }

        public int KeyLength => keyLength;
        public IBPlusTree<StreamEvent, int> Tree { get; }
        public bool AreKeyEqual(StreamEvent x, StreamEvent y)
        {
            for (int i = 0; i < keyLength; i++)
            {
                var c = FlxValueRefComparer.CompareTo(x.Vector.GetRef(i), y.Vector.GetRef(i));
                if (c != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
    internal static class MinAggregationRegistration
    {
        private static FlxValue NullValue = FlxValue.FromBytes(FlexBuffer.Null());

        private static async Task<MinAggregationSingleton> Initialize(int groupingLength, IStateManagerClient stateManagerClient)
        {
            List<int> insertPrimaryKeys = new List<int>();
            for (int i = 0; i < groupingLength + 1; i++)
            {
                insertPrimaryKeys.Add(i);
            }
            List<int> searchPrimaryKeys = new List<int>();
            for (int i = 0; i < groupingLength; i++)
            {
                searchPrimaryKeys.Add(i);
            }
            var tree = await stateManagerClient.GetOrCreateTree("mintree", new FlowtideDotNet.Storage.Tree.BPlusTreeOptions<StreamEvent, int>()
            {
                Comparer = new MinAggregationInsertComparer(groupingLength + 1),
                KeySerializer = new StreamEventBPlusTreeSerializer(),
                ValueSerializer = new IntSerializer()
            });
            
            return new MinAggregationSingleton(tree, groupingLength);
        }

        

        private static System.Linq.Expressions.Expression MinMapFunction(
            Substrait.Expressions.AggregateFunction function,
            ParametersInfo parametersInfo,
            ExpressionVisitor<System.Linq.Expressions.Expression, ParametersInfo> visitor,
            ParameterExpression stateParameter,
            ParameterExpression weightParameter,
            ParameterExpression singletonAccess,
            ParameterExpression groupingKeyParameter)
        {
            if (function.Arguments.Count != 1)
            {
                throw new InvalidOperationException("Min must have one argument.");
            }
            var arg = visitor.Visit(function.Arguments[0], parametersInfo);
            var expr = GetMinBody();
            var body = expr.Body;
            var replacer = new ParameterReplacerVisitor(expr.Parameters[0], arg!);
            System.Linq.Expressions.Expression e = replacer.Visit(body);
            replacer = new ParameterReplacerVisitor(expr.Parameters[1], stateParameter);
            e = replacer.Visit(e);
            replacer = new ParameterReplacerVisitor(expr.Parameters[2], weightParameter);
            e = replacer.Visit(e);
            replacer = new ParameterReplacerVisitor(expr.Parameters[3], singletonAccess);
            e = replacer.Visit(e);
            replacer = new ParameterReplacerVisitor(expr.Parameters[4], groupingKeyParameter);
            e = replacer.Visit(e);
            return e;
        }

        private static async ValueTask<FlxValue> MinGetValue(byte[] state, StreamEvent groupingKey, MinAggregationSingleton singleton)
        {
            var vector = FlexBufferBuilder.Vector(v =>
            {
                for (int i = 0; i < groupingKey.Vector.Length; i++)
                {
                    v.Add(groupingKey.GetColumn(i));
                }
                v.AddNull();
            });
            var row = new StreamEvent(0, 0, vector);
            var iterator = singleton.Tree.CreateIterator();
            await iterator.Seek(row);
            
            await foreach(var page in iterator)
            {
                foreach(var kv in page)
                {
                    if (singleton.AreKeyEqual(kv.Key, row))
                    {
                        return kv.Key.GetColumn(singleton.KeyLength);
                    }
                    else
                    {
                        return NullValue;
                    }
                }
            }
            return NullValue;
        }

        private static Expression<Func<FlxValue, byte[], long, MinAggregationSingleton, StreamEvent, ValueTask<byte[]>>> GetMinBody()
        {
            return (ev, bytes, weight, singleton, groupingKey) => DoMin(ev, bytes, weight, singleton, groupingKey);
        }

        private static async ValueTask<byte[]> DoMin(FlxValue column, byte[] currentState, long weight, MinAggregationSingleton singleton, StreamEvent groupingKey)
        {
            var vector = FlexBufferBuilder.Vector(v =>
            {
                for (int i = 0; i < groupingKey.Vector.Length; i++)
                {
                    v.Add(groupingKey.GetColumn(i));
                }
                v.Add(column);
            });
            var row = new StreamEvent((int)weight, 0, vector);
            await singleton.Tree.RMW(row, (int)weight, (input, current, exists) =>
            {
                if (exists)
                {
                    current += input;

                    if (current == 0)
                    {
                        return (0, GenericWriteOperation.Delete);
                    }
                    return (current, GenericWriteOperation.Upsert);
                }
                return (input, GenericWriteOperation.Upsert);
            });

            return currentState;
        }

        private static async Task Commit(MinAggregationSingleton singleton)
        {
            await singleton.Tree.Commit();
        }

        public static void Register(IFunctionsRegister functionsRegister)
        {
            functionsRegister.RegisterStatefulAggregateFunction<MinAggregationSingleton>(
                FunctionsArithmetic.Uri,
                FunctionsArithmetic.Min,
                Initialize,
                (singleton) => { },
                Commit,
                MinMapFunction,
                MinGetValue
                );
        }
    }
}

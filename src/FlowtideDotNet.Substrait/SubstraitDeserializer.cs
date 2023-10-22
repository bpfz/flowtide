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

using FlowtideDotNet.Substrait.Expressions;
using FlowtideDotNet.Substrait.Expressions.IfThen;
using FlowtideDotNet.Substrait.Expressions.Literals;
using FlowtideDotNet.Substrait.Expressions.ScalarFunctions;
using FlowtideDotNet.Substrait.Relations;
using FlowtideDotNet.Substrait.Type;
using Protobuf = Substrait.Protobuf;

namespace FlowtideDotNet.Substrait
{
    public class SubstraitDeserializer
    {
        private class ExpressionDeserializerImpl
        {
            private Dictionary<uint, string> idToFunctionLookup = new Dictionary<uint, string>();

            public ExpressionDeserializerImpl(Protobuf.Plan plan)
            {
                foreach(var extension in plan.Extensions)
                {
                    var id = extension.ExtensionFunction.FunctionAnchor;
                    var uri = plan.ExtensionUris.First(x => x.ExtensionUriAnchor == extension.ExtensionFunction.ExtensionUriReference).Uri;
                    uri = uri.Substring(uri.LastIndexOf('/'));
                    idToFunctionLookup.Add(id, $"{uri}:{extension.ExtensionFunction.Name.Substring(0, extension.ExtensionFunction.Name.IndexOf(':'))}");
                }
            }

            public Expressions.Expression VisitExpression(Protobuf.Expression expression)
            {
                switch (expression.RexTypeCase)
                {
                    case Protobuf.Expression.RexTypeOneofCase.ScalarFunction:
                        return VisitScalarFunction(expression.ScalarFunction);
                        break;
                    case Protobuf.Expression.RexTypeOneofCase.Selection:
                        return VisitFieldReferencce(expression.Selection);
                        break;
                    case Protobuf.Expression.RexTypeOneofCase.Literal:
                        return VisitLiteral(expression.Literal);
                        break;
                    case Protobuf.Expression.RexTypeOneofCase.IfThen:
                        return VisitIfThen(expression.IfThen);
                        break;
                    case Protobuf.Expression.RexTypeOneofCase.Cast:
                        return VisitCast(expression.Cast);
                        break;
                }
                throw new NotImplementedException();
            }

            private Expressions.Expression VisitCast(Protobuf.Expression.Types.Cast cast)
            {
                // Skip casts for now
                return VisitExpression(cast.Input);
            } 

            private Expressions.Expression VisitIfThen(Protobuf.Expression.Types.IfThen ifThen)
            {
                List<IfClause> ifClauses = new List<IfClause>();

                foreach(var clause in ifThen.Ifs)
                {
                    ifClauses.Add(new IfClause()
                    {
                        If = VisitExpression(clause.If),
                        Then = VisitExpression(clause.Then)
                    });
                }
                return new IfThenExpression()
                {
                    Ifs = ifClauses,
                    Else = VisitExpression(ifThen.Else)
                };
            }

            private Literal VisitLiteral(Protobuf.Expression.Types.Literal literal)
            {
                switch (literal.LiteralTypeCase)
                {
                    case Protobuf.Expression.Types.Literal.LiteralTypeOneofCase.Boolean:
                        return new BoolLiteral()
                        {
                            Value = literal.Boolean
                        };
                    case Protobuf.Expression.Types.Literal.LiteralTypeOneofCase.Null:
                        return new NullLiteral();
                    case Protobuf.Expression.Types.Literal.LiteralTypeOneofCase.I32:
                        return new NumericLiteral()
                        {
                            Value = literal.I32
                        };
                    case Protobuf.Expression.Types.Literal.LiteralTypeOneofCase.I64:
                        return new NumericLiteral()
                        {
                            Value = literal.I64
                        };
                    case Protobuf.Expression.Types.Literal.LiteralTypeOneofCase.String:
                        return new StringLiteral()
                        {
                            Value = literal.String
                        };
                    case Protobuf.Expression.Types.Literal.LiteralTypeOneofCase.FixedChar:
                        return new StringLiteral()
                        {
                            Value = literal.FixedChar
                        };
                    default:
                        throw new NotImplementedException();
                }
            }

            private FieldReference VisitFieldReferencce(Protobuf.Expression.Types.FieldReference fieldReference)
            {
                switch (fieldReference.ReferenceTypeCase)
                {
                    case Protobuf.Expression.Types.FieldReference.ReferenceTypeOneofCase.DirectReference:
                        return VisitDirectReference(fieldReference.DirectReference);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            private DirectFieldReference VisitDirectReference(Protobuf.Expression.Types.ReferenceSegment referenceSegment)
            {
                switch (referenceSegment.ReferenceTypeCase)
                {
                    case Protobuf.Expression.Types.ReferenceSegment.ReferenceTypeOneofCase.StructField:
                        return VisitStructField(referenceSegment.StructField);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            private DirectFieldReference VisitStructField(Protobuf.Expression.Types.ReferenceSegment.Types.StructField structField)
            {
                return new DirectFieldReference()
                {
                    ReferenceSegment = new StructReferenceSegment()
                    {
                        Field = structField.Field
                    }
                };
            }

            private Expressions.Expression VisitScalarFunction(Protobuf.Expression.Types.ScalarFunction scalarFunction)
            {
                if (!idToFunctionLookup.TryGetValue(scalarFunction.FunctionReference, out var functionName))
                {
                    throw new NotImplementedException();
                }
                switch (functionName)
                {
                    case "/functions_comparison.yaml:equal":
                        return VisitBooleanComparison(scalarFunction, BooleanComparisonType.Equals);
                    case "/functions_comparison.yaml:not_equal":
                        return VisitBooleanComparison(scalarFunction, BooleanComparisonType.NotEqualTo);
                    case "/functions_comparison.yaml:gt":
                        return VisitBooleanComparison(scalarFunction, BooleanComparisonType.GreaterThan);
                    case "/functions_comparison.yaml:gte":
                        return VisitBooleanComparison(scalarFunction, BooleanComparisonType.GreaterThanOrEqualTo);
                    case "/functions_boolean.yaml:and":
                        return VisitAnd(scalarFunction);
                    case "/functions_string.yaml:concat":
                        return VisitConcatString(scalarFunction);
                    case "/functions_comparison.yaml:is_not_null":
                        return VisitIsNotNull(scalarFunction);
                    case "/functions_boolean.yaml:or":
                        return VisitOrFunction(scalarFunction);
                }
                throw new NotImplementedException(functionName);
            }

            private Expressions.Expression VisitOrFunction(Protobuf.Expression.Types.ScalarFunction scalarFunction)
            {
                List<Expressions.Expression> expressions = new List<Expression>();
                if (scalarFunction.Args.Count > 0)
                {
                    for (int i = 0; i < scalarFunction.Args.Count; i++)
                    {
                        expressions.Add(VisitExpression(scalarFunction.Args[i]));
                    }
                }
                else if (scalarFunction.Arguments.Count > 0)
                {
                    for (int i = 0; i < scalarFunction.Arguments.Count; i++)
                    {
                        expressions.Add(VisitExpression(scalarFunction.Arguments[i].Value));
                    }
                }
                else
                {
                    throw new InvalidOperationException("OR must have more than 0 arguments");
                }

                return new OrFunction()
                {
                    Arguments = expressions
                };
            }
            private Expressions.Expression VisitIsNotNull(Protobuf.Expression.Types.ScalarFunction scalarFunction)
            {
                Expressions.Expression? arg;
                if (scalarFunction.Args.Count == 1)
                {
                    arg = VisitExpression(scalarFunction.Args[0]);
                }
                else if (scalarFunction.Arguments.Count == 1)
                {
                    arg = VisitExpression(scalarFunction.Arguments[0].Value);
                }
                else
                {
                    throw new InvalidOperationException("IsNotNull must have 1 argument");
                }

                return new IsNotNullFunction()
                {
                    Expression = arg
                };
            }

            private Expressions.Expression VisitConcatString(Protobuf.Expression.Types.ScalarFunction scalarFunction)
            {
                List<Expressions.Expression> expressions = new List<Expressions.Expression>();
                if (scalarFunction.Args.Count > 0)
                {
                    foreach (var arg in scalarFunction.Args)
                    {
                        expressions.Add(VisitExpression(arg));
                    }
                }
                else
                {
                    foreach (var arg in scalarFunction.Arguments)
                    {
                        expressions.Add(VisitExpression(arg.Value));
                    }
                }
                
                return new ConcatFunction()
                {
                    Expressions = expressions,
                };
            }

            private Expressions.Expression VisitAnd(Protobuf.Expression.Types.ScalarFunction scalarFunction)
            {
                List<Expressions.Expression> expressions = new List<Expression>();
                if (scalarFunction.Args.Count > 0)
                {
                    for (int i = 0; i < scalarFunction.Args.Count; i++)
                    {
                        expressions.Add(VisitExpression(scalarFunction.Args[i]));
                    }
                }
                else if (scalarFunction.Arguments.Count > 0)
                {
                    for (int i = 0; i < scalarFunction.Arguments.Count; i++)
                    {
                        expressions.Add(VisitExpression(scalarFunction.Arguments[i].Value));
                    }
                }
                else
                {
                    throw new InvalidOperationException("AND must have more than 0 arguments");
                }

                return new AndFunction()
                {
                    Arguments = expressions
                };
            }

            private Expressions.Expression VisitBooleanComparison(Protobuf.Expression.Types.ScalarFunction scalarFunction, BooleanComparisonType booleanComparisonType)
            {
                Expressions.Expression? left = null;
                Expressions.Expression? right = null;

                if (scalarFunction.Args.Count == 2)
                {
                    left = VisitExpression(scalarFunction.Args[0]);
                    right = VisitExpression(scalarFunction.Args[1]);
                }
                else if (scalarFunction.Arguments.Count == 2)
                {
                    left = VisitExpression(scalarFunction.Arguments[0].Value);
                    right = VisitExpression(scalarFunction.Arguments[1].Value);
                }
                else
                {
                    throw new InvalidOperationException("Equals must have 2 arguments");
                }
                
                var output = new BooleanComparison()
                {
                    Left = left,
                    Right = right,
                    Type = booleanComparisonType
                };
                return output;
            }
        }

        private class SubstraitDeserializerImpl
        {
            private readonly Protobuf.Plan plan;
            private readonly ExpressionDeserializerImpl expressionDeserializer;

            public SubstraitDeserializerImpl(Protobuf.Plan plan) 
            {
                this.plan = plan;
                expressionDeserializer = new ExpressionDeserializerImpl(plan);
            }

            public Plan Convert()
            {
                return VisitPlan(plan);
            }

            private Plan VisitPlan(Protobuf.Plan plan)
            {
                var output = new Plan();
                output.Relations = new List<Relation>();
                foreach (var relation in plan.Relations)
                {
                    output.Relations.Add(VisitPlanRel(relation));
                }
                return output;
            }

            private Relation VisitPlanRel(Protobuf.PlanRel planRel)
            {
                switch(planRel.RelTypeCase)
                {
                    case Protobuf.PlanRel.RelTypeOneofCase.Rel:

                        break;
                    case Protobuf.PlanRel.RelTypeOneofCase.None:
                        throw new NotImplementedException();
                    case Protobuf.PlanRel.RelTypeOneofCase.Root:
                        return VisitRelRoot(planRel.Root);
                        break;
                }
                throw new NotImplementedException();
            }

            private Relation VisitRelRoot(Protobuf.RelRoot relRoot)
            {
                var input = VisitRel(relRoot.Input);
                var rootRelation = new RootRelation()
                {
                    Input = input,
                    Names = relRoot.Names.ToList()
                };
                return rootRelation;
            }

            private Relation VisitRel(Protobuf.Rel rel)
            {
                switch (rel.RelTypeCase)
                {
                    case Protobuf.Rel.RelTypeOneofCase.Project:
                        return VisitProject(rel.Project);
                        break;
                    case Protobuf.Rel.RelTypeOneofCase.Read:
                        return VisitRead(rel.Read);
                        break;
                    case Protobuf.Rel.RelTypeOneofCase.Filter:
                        return VisitFilter(rel.Filter);
                        break;
                    case Protobuf.Rel.RelTypeOneofCase.Join:
                        return VisitJoin(rel.Join);
                        break;
                    case Protobuf.Rel.RelTypeOneofCase.Set:
                        return VisitSet(rel.Set);
                    default:
                        throw new NotImplementedException();
                }
            }

            private List<int>? GetEmit(Protobuf.RelCommon relCommon)
            {
                if (relCommon == null)
                {
                    return null;
                }
                switch (relCommon.EmitKindCase)
                {
                    case Protobuf.RelCommon.EmitKindOneofCase.Direct:
                        return null;
                        break;
                    case Protobuf.RelCommon.EmitKindOneofCase.Emit:
                        return relCommon.Emit.OutputMapping.ToList();
                        break;
                }
                throw new NotImplementedException();
            }

            private Relation VisitSet(Protobuf.SetRel setRel)
            {
                var set = new SetRelation();
                set.Inputs = new List<Relation>();
                for(int i = 0; i < setRel.Inputs.Count; i++)
                {
                    set.Inputs.Add(VisitRel(setRel.Inputs[i]));
                }
                set.Operation = SetOperation.UnionAll;
                set.Emit = GetEmit(setRel.Common);
                return set;
            }

            private Relation VisitProject(Protobuf.ProjectRel projectRel)
            {
                var input = VisitRel(projectRel.Input);
                var project = new Relations.ProjectRelation()
                {
                    Input = input,
                    Expressions = new List<Expressions.Expression>(),
                    Emit = GetEmit(projectRel.Common)
                };

                if (projectRel.Expressions.Count > 0)
                {
                    foreach(var expr in projectRel.Expressions)
                    {
                        project.Expressions.Add(expressionDeserializer.VisitExpression(expr));
                    }
                }
                return project;
            }

            private Relation VisitRead(Protobuf.ReadRel readRel)
            {
                List<string> names = new List<string>();
                names.AddRange(readRel.BaseSchema.Names);

                var namedStruct = new Type.NamedStruct()
                {
                    Names = names,
                };
                List<string> namedTable = new List<string>();
                if (readRel.NamedTable != null)
                {
                    namedTable.AddRange(readRel.NamedTable.Names);
                }

                if (readRel.BaseSchema.Struct != null)
                {
                    var st = new Type.Struct();
                    st.Types = new List<Type.SubstraitBaseType>();
                    foreach(var type in readRel.BaseSchema.Struct.Types_)
                    {
                        switch (type.KindCase)
                        {
                            case Protobuf.Type.KindOneofCase.String:

                                st.Types.Add(new StringType()
                                {
                                    Nullable = type.String.Nullability != Protobuf.Type.Types.Nullability.Required
                                });
                                break;
                            case Protobuf.Type.KindOneofCase.I32:
                                st.Types.Add(new Int32Type()
                                {
                                    Nullable = type.I32.Nullability != Protobuf.Type.Types.Nullability.Required
                                });
                                break;
                            case Protobuf.Type.KindOneofCase.Date:
                                st.Types.Add(new DateType()
                                {
                                    Nullable = type.Date.Nullability != Protobuf.Type.Types.Nullability.Required
                                });
                                break;
                            case Protobuf.Type.KindOneofCase.Fp64:
                                st.Types.Add(new Fp64Type()
                                {
                                    Nullable = type.Fp64.Nullability != Protobuf.Type.Types.Nullability.Required
                                });
                                break;
                            case Protobuf.Type.KindOneofCase.I64:
                                st.Types.Add(new Int64Type()
                                {
                                    Nullable = type.I64.Nullability != Protobuf.Type.Types.Nullability.Required
                                });
                                break;
                            case Protobuf.Type.KindOneofCase.Bool:
                                st.Types.Add(new BoolType()
                                {
                                    Nullable = type.Bool.Nullability != Protobuf.Type.Types.Nullability.Required
                                });
                                break;
                            case Protobuf.Type.KindOneofCase.Fp32:
                                st.Types.Add(new Fp32Type()
                                {
                                    Nullable = type.Fp32.Nullability != Protobuf.Type.Types.Nullability.Required
                                });
                                break;
                            default:
                                throw new NotImplementedException($"Type is not yet implemented {type.KindCase}");
                        }
                    }
                    namedStruct.Struct = st;
                }

                return new ReadRelation()
                {
                    BaseSchema = namedStruct,
                    NamedTable = new Type.NamedTable()
                    {
                        Names = namedTable
                    },
                    Emit = GetEmit(readRel.Common)
                };
            }

            private Relation VisitFilter(Protobuf.FilterRel filterRel)
            {
                var input = VisitRel(filterRel.Input);

                var filter = new FilterRelation()
                {
                    Input = input,
                    Emit = GetEmit(filterRel.Common)
                };
                if (filterRel.Condition != null)
                {
                    filter.Condition = expressionDeserializer.VisitExpression(filterRel.Condition);
                }
                return filter;
            }

            private Relation VisitJoin(Protobuf.JoinRel joinRel)
            {
                var inputLeft = VisitRel(joinRel.Left);
                var inputRight = VisitRel(joinRel.Right);
                var joinType = JoinType.Unspecified;
                switch (joinRel.Type)
                {
                    case Protobuf.JoinRel.Types.JoinType.Right:
                        joinType = JoinType.Right;
                        break;
                    case Protobuf.JoinRel.Types.JoinType.Inner:
                        joinType = JoinType.Inner;
                        break;
                    case Protobuf.JoinRel.Types.JoinType.Single:
                        joinType = JoinType.Single;
                        break;
                    case Protobuf.JoinRel.Types.JoinType.Left:
                        joinType = JoinType.Left;
                        break;
                    case Protobuf.JoinRel.Types.JoinType.Anti:
                        joinType = JoinType.Anti;
                        break;
                    case Protobuf.JoinRel.Types.JoinType.Semi:
                        joinType = JoinType.Semi;
                        break;
                    default:
                        throw new NotSupportedException("Join type not supported");
                }
                var joinRelation = new JoinRelation()
                {
                    Left = inputLeft,
                    Right = inputRight,
                    Emit = GetEmit(joinRel.Common),
                    Type = joinType
                };
                if (joinRel.Expression != null)
                {
                    joinRelation.Expression = expressionDeserializer.VisitExpression(joinRel.Expression);
                }
                if (joinRel.PostJoinFilter != null)
                {
                    joinRelation.PostJoinFilter = expressionDeserializer.VisitExpression(joinRel.PostJoinFilter);
                }
                return joinRelation;
            }
        }

        public Plan Deserialize(string json)
        {
            var plan = Google.Protobuf.JsonParser.Default.Parse<Protobuf.Plan>(json);
            return Deserialize(plan);
        }

        public Plan Deserialize(Protobuf.Plan plan)
        {
            var impl = new SubstraitDeserializerImpl(plan);
            return impl.Convert();
        }
    }
}

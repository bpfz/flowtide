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

using FlowtideDotNet.Substrait.Relations;
using FlowtideDotNet.Substrait.Type;
using SqlParser;
using SqlParser.Ast;
using System.Diagnostics;

namespace FlowtideDotNet.Substrait.Sql.Internal
{
    internal class SqlSubstraitVisitor : SqlBaseVisitor<RelationData?, object?>
    {
        private readonly TablesMetadata tablesMetadata;
        private readonly SqlPlanBuilder sqlPlanBuilder;

        public SqlSubstraitVisitor(SqlPlanBuilder sqlPlanBuilder)
        {
            this.sqlPlanBuilder = sqlPlanBuilder;
            tablesMetadata = sqlPlanBuilder._tablesMetadata;
        }

        protected override RelationData? VisitInsertStatement(Statement.Insert insert, object? state)
        {
            var source = Visit(insert.Source, state);

            Debug.Assert(source != null);

            var writeRelation = new WriteRelation();
            writeRelation.Input = source.Relation;
            writeRelation.NamedObject = new FlowtideDotNet.Substrait.Type.NamedTable() { Names = insert.Name.Values.Select(x => x.Value).ToList() };

            if (insert.Columns != null && insert.Columns.Count > 0)
            {
                writeRelation.TableSchema = new FlowtideDotNet.Substrait.Type.NamedStruct();
                writeRelation.TableSchema.Names = insert.Columns.Select(x => x.Value).ToList();
                writeRelation.TableSchema.Struct = new FlowtideDotNet.Substrait.Type.Struct()
                {
                    Types = insert.Columns.Select(x => new AnyType() { Nullable = true } as SubstraitBaseType).ToList()
                };
            }
            else
            {
                var names = source.EmitData.GetNames();
                writeRelation.TableSchema = new FlowtideDotNet.Substrait.Type.NamedStruct();
                writeRelation.TableSchema.Names = names.ToList();
                writeRelation.TableSchema.Struct = new FlowtideDotNet.Substrait.Type.Struct()
                {
                    Types = names.Select(x => new AnyType() { Nullable = true } as SubstraitBaseType).ToList()
                };
            }

            return new RelationData(writeRelation, source.EmitData);
        }

        protected override RelationData? VisitCreateView(Statement.CreateView createView, object? state)
        {
            var relationData = Visit(createView.Query, state);
            Debug.Assert(relationData != null);

            var viewName = createView.Name.ToSql();
            sqlPlanBuilder._planModifier.AddPlanAsView(viewName, new FlowtideDotNet.Substrait.Plan()
            {
                Relations = new List<Relation>() { relationData.Relation }
            });
            tablesMetadata.AddTable(viewName, relationData.EmitData.GetNames());
            return default;
        }

        protected override RelationData? VisitCreateTable(Statement.CreateTable createTable, object? state)
        {
            var tableName = createTable.Name.ToSql();
            var columnNames = createTable.Columns.Select(x => x.Name.Value).ToList();
            tablesMetadata.AddTable(tableName, columnNames);
            return null;
        }

        protected override RelationData? VisitQuery(Query query, object? state)
        {
            return Visit(query.Body, state);
        }

        protected override RelationData? VisitSelect(Select select, object? state)
        {
            RelationData? outNode = default;
            if (select.From != null)
            {
                if (select.From.Count != 1)
                {
                    throw new InvalidOperationException("Only a single table in the FROM statement is supported");
                }
                var fromTable = select.From.First();

                outNode = Visit(fromTable, state);
            }

            if (outNode == null)
            {
                throw new NotImplementedException("Only queries that does 'FROM' with potential joins is supported at this time");
            }

            if (select.Selection != null)
            {
                var exprVisitor = new SqlExpressionVisitor();
                var expr = exprVisitor.Visit(select.Selection, outNode.EmitData);
                outNode = new RelationData(new FilterRelation()
                {
                    Input = outNode.Relation,
                    Condition = expr.Expr
                }, outNode.EmitData);
            }

            if (select.Projection != null)
            {
                outNode = VisitProjection(select.Projection, outNode);
            }



            return outNode;
        }

        private RelationData? VisitProjection(SqlParser.Sequence<SelectItem> selects, RelationData parent)
        {
            var projectRel = new ProjectRelation()
            {
                Input = parent.Relation
            };

            EmitData projectEmitData = new EmitData();
            List<FlowtideDotNet.Substrait.Expressions.Expression> expressions = new List<FlowtideDotNet.Substrait.Expressions.Expression>();
            List<int> emitList = new List<int>();
            int emitCounter = parent.Relation.OutputLength;
            int outputCounter = 0;
            foreach (var s in selects)
            {
                var exprVisitor = new SqlExpressionVisitor();
                if (s is SelectItem.ExpressionWithAlias exprAlias)
                {
                    var condition = exprVisitor.Visit(exprAlias.Expression, parent.EmitData);
                    expressions.Add(condition.Expr);
                    projectEmitData.Add(new Expression.CompoundIdentifier(new SqlParser.Sequence<Ident>(new List<Ident>() { new Ident(exprAlias.Alias) })), outputCounter, exprAlias.Alias);
                    outputCounter++;
                }
                if (s is SelectItem.UnnamedExpression unnamedExpr)
                {
                    var condition = exprVisitor.Visit(unnamedExpr.Expression, parent.EmitData);
                    expressions.Add(condition.Expr);
                    projectEmitData.Add(new Expression.CompoundIdentifier(new SqlParser.Sequence<Ident>(new List<Ident>() { new Ident(condition.Name) })), outputCounter, condition.Name);
                    outputCounter++;
                }
                emitList.Add(emitCounter);
                emitCounter++;
            }
            projectRel.Expressions = expressions;
            projectRel.Emit = emitList;
            return new RelationData(projectRel, projectEmitData);
        }

        protected override RelationData? VisitTableWithJoins(TableWithJoins tableWithJoins, object? state)
        {
            var parent = Visit(tableWithJoins.Relation!, state);
            Debug.Assert(parent != null);
            if (tableWithJoins.Joins != null)
            {
                foreach (var join in tableWithJoins.Joins)
                {
                    parent = VisitJoin(join, parent, state);
                }
            }
            return parent;
        }

        protected override RelationData? VisitTable(TableFactor.Table table, object? state)
        {
            var tableName = string.Join('.', table.Name.Values.Select(x => x.Value));
            if (tablesMetadata.TryGetTable(tableName, out var t))
            {
                var emitData = new EmitData();

                for (int i = 0; i < t.Columns.Count; i++)
                {
                    emitData.Add(new Expression.CompoundIdentifier(new SqlParser.Sequence<Ident>(new List<Ident>() { new Ident(t.Columns[i]) })), i, t.Columns[i]);
                }

                if (table.Alias != null)
                {
                    for (int i = 0; i < t.Columns.Count; i++)
                    {
                        emitData.AddWithAlias(new Expression.CompoundIdentifier(new SqlParser.Sequence<Ident>(new List<Ident>() { new Ident(table.Alias.Name), new Ident(t.Columns[i]) })), i);
                    }
                }

                var readRelation = new ReadRelation()
                {
                    NamedTable = new FlowtideDotNet.Substrait.Type.NamedTable()
                    {
                        Names = new List<string>() { tableName }
                    },
                    BaseSchema = new FlowtideDotNet.Substrait.Type.NamedStruct()
                    {
                        Names = t.Columns.ToList(),
                        Struct = new Struct() { Types = t.Columns.Select(x => new AnyType() as SubstraitBaseType).ToList() }
                    }
                };
                return new RelationData(readRelation, emitData);
            }
            else
            {
                throw new InvalidOperationException($"Table '{tableName}' does not exist");
            }

        }

        private RelationData? VisitJoin(Join join, RelationData left, object? state)
        {
            Debug.Assert(join.Relation != null);
            var right = Visit(join.Relation, state);
            Debug.Assert(right != null);

            var joinRelation = new JoinRelation()
            {
                Left = left.Relation,
                Right = right.Relation
            };
            EmitData joinEmitData = new EmitData();
            joinEmitData.Add(left.EmitData, 0);
            joinEmitData.Add(right.EmitData, left.Relation.OutputLength);


            if (join.JoinOperator is JoinOperator.LeftOuter leftOuter)
            {
                joinRelation.Type = JoinType.Left;

                if (leftOuter.JoinConstraint is JoinConstraint.On on)
                {
                    var exprVisitor = new SqlExpressionVisitor();
                    var condition = exprVisitor.Visit(on.Expression, joinEmitData);
                    joinRelation.Expression = condition.Expr;
                }
                else
                {
                    throw new InvalidOperationException("Left joins must have an 'ON' expression.");
                }
            }
            else if (join.JoinOperator is JoinOperator.Inner inner)
            {
                joinRelation.Type = JoinType.Inner;
                if (inner.JoinConstraint is JoinConstraint.On on)
                {
                    var exprVisitor = new SqlExpressionVisitor();
                    var condition = exprVisitor.Visit(on.Expression, joinEmitData);
                    joinRelation.Expression = condition.Expr;
                }
                else
                {
                    throw new InvalidOperationException("Left joins must have an 'ON' expression.");
                }
            }
            else
            {
                throw new NotImplementedException($"Join type '{join.JoinOperator!.GetType().Name}' is not yet supported in SQL mode.");
            }

            return new RelationData(joinRelation, joinEmitData);
        }

        protected override RelationData? VisitDerivedTable(TableFactor.Derived derived, object? state)
        {
            var relationData = VisitQuery(derived.SubQuery, state);
            Debug.Assert(relationData != null);
            if (derived.Alias != null)
            {
                // Append the alias to the emit data so its possible to find columns from the emit data
                var newEmitData = relationData.EmitData.ClonewithAlias(derived.Alias.Name.Value);
                return new RelationData(relationData.Relation, newEmitData);
            }

            return relationData;
        }

        protected override RelationData? VisitSetOperation(SetExpression.SetOperation setOperation, object? state)
        {
            if (setOperation.Op != SetOperator.Union)
            {
                throw new NotImplementedException($"The set operation {setOperation.Op.ToString()} is not yet supported in SQL.");
            }
            if (setOperation.SetQuantifier != SetQuantifier.All && setOperation.SetQuantifier != SetQuantifier.None)
            {
                throw new NotImplementedException("Only union all is supported in SQL at this time.");
            }

            var left = Visit(setOperation.Left, state);
            var right = Visit(setOperation.Right, state);

            Debug.Assert(left != null);
            Debug.Assert(right != null);

            var setRelation = new SetRelation()
            {
                Inputs = new List<Relation>()
                {
                    left.Relation,
                    right.Relation
                },
                Operation = SetOperation.UnionAll
            };

            return new RelationData(setRelation, left.EmitData);
        }
    }
}

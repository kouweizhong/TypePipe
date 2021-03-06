﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Expressions
{
  /// <summary>
  /// A base class for expressions representing unary operations, that is, an operation that has a single <see cref="Operand"/>.
  /// </summary>
  public abstract class UnaryExpressionBase : CodeGenerationExpressionBase
  {
    private readonly Expression _operand;

    protected UnaryExpressionBase (Expression operand, Type type)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("operand", operand);

      _operand = operand;
    }

    public Expression Operand
    {
      get { return _operand; }
    }

    protected abstract UnaryExpressionBase CreateSimiliar (Expression operand);

    public UnaryExpressionBase Update (Expression operand)
    {
      if (operand == Operand)
        return this;

      return CreateSimiliar (operand);
    }

    protected internal override Expression VisitChildren (ExpressionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var operand = visitor.Visit (_operand);
      return Update (operand);
    }
  }
}
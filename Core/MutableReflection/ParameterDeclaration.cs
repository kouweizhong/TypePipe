// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Holds all values required to declare a method, constructor, or property parameter.
  /// </summary>
  /// <remarks>
  /// This is used by <see cref="MutableType"/> when declaring methods, constructors, or indexed properties.
  /// </remarks>
  public class ParameterDeclaration
  {
    public static readonly ParameterDeclaration[] None = new ParameterDeclaration[0];

    private readonly Type _type;
    private readonly string _name;
    private readonly ParameterAttributes _attributes;
    private readonly ParameterExpression _expression;

    public ParameterDeclaration (Type type, string name = null, ParameterAttributes attributes = ParameterAttributes.None)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      // Name may be null.

      if (type == typeof (void))
        throw new ArgumentException ("Parameter cannot be of type void.", "type");

      _type = type;
      _name = name;
      _attributes = attributes;
      _expression = Remotion.TypePipe.Dlr.Ast.Expression.Parameter (type, name);
    }

    public Type Type
    {
      get { return _type; }
    }

    public string Name
    {
      get { return _name; }
    }

    public ParameterAttributes Attributes
    {
      get { return _attributes; }
    }

    public ParameterExpression Expression
    {
      get { return _expression; }
    }
  }
}
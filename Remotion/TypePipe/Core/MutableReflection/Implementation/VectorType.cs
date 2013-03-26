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
using System.Collections.Generic;
using System.Reflection;

namespace Remotion.TypePipe.MutableReflection.Implementation
{
  /// <summary>
  /// Represents a vector <see cref="Type"/>.
  /// </summary>
  public class VectorType : ArrayTypeBase
  {
    public VectorType (CustomType elementType, IMemberSelector memberSelector)
        : base (elementType, 1, memberSelector)
    {
    }

    protected override IEnumerable<Type> CreateInterfaces (CustomType elementType)
    {
      yield return typeof (IEnumerable<>).MakeTypePipeGenericType (elementType);
      yield return typeof (ICollection<>).MakeTypePipeGenericType (elementType);
      yield return typeof (IList<>).MakeTypePipeGenericType (elementType);

      foreach (var baseInterface in typeof (Array).GetInterfaces ())
        yield return baseInterface;
    }

    protected override IEnumerable<ConstructorInfo> CreateConstructors (int rank)
    {
      var attributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
      var parameters = new[] { new ParameterDeclaration (typeof (int), "length") };

      yield return new ConstructorOnCustomType (this, attributes, parameters);
    }
  }
}
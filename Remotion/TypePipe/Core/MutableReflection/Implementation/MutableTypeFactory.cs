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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection.Implementation.MemberFactory;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.Implementation
{
  /// <summary>
  /// Creates a <see cref="ProxyType"/> model for the given base type.
  /// </summary>
  // TODO Update doc.
  public class MutableTypeFactory : IMutableTypeFactory
  {
    private int _counter;

    // TODO: Maybe move to proxy?
    public ProxyType CreateProxyType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      _counter++;
      var name = string.Format ("{0}_Proxy{1}", baseType.Name, _counter);
      var attributes = TypeAttributes.Public | TypeAttributes.BeforeFieldInit | (baseType.IsTypePipeSerializable() ? TypeAttributes.Serializable : 0);

      var proxyType = CreateType (name, baseType.Namespace, attributes, baseType);
      CopyConstructors (baseType, proxyType);

      return proxyType;
    }

    // TODO: add Tests
    public ProxyType CreateType (string name, string @namespace, TypeAttributes attributes, Type baseType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      // Name space may be null.
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      var fullname = string.IsNullOrEmpty (@namespace) ? name : string.Format ("{0}.{1}", @namespace, name);
      var memberSelector = new MemberSelector (new BindingFlagsEvaluator());
      var interfaceMappingComputer = new InterfaceMappingComputer();
      var mutableMemberFactory = new MutableMemberFactory (new RelatedMethodFinder());

      // TODO move fullname calculation to customType
      return new ProxyType (memberSelector, baseType, name, @namespace, fullname, attributes, interfaceMappingComputer, mutableMemberFactory);
    }

    private void CopyConstructors (Type baseType, ProxyType proxyType)
    {
      var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      var accessibleInstanceCtors = baseType.GetConstructors (bindingFlags).Where (SubclassFilterUtility.IsVisibleFromSubclass);

      foreach (var ctor in accessibleInstanceCtors)
      {
        var attributes = ctor.Attributes.AdjustVisibilityForAssemblyBoundaries();
        var parameters = ctor.GetParameters().Select (p => new ParameterDeclaration (p.ParameterType, p.Name, p.Attributes));

        proxyType.AddConstructor (attributes, parameters, ctx => ctx.CallBaseConstructor (ctx.Parameters.Cast<Expression>()));
      }
    }
  }
}
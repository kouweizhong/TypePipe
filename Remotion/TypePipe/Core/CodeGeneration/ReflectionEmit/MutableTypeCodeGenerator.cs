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
using Remotion.Collections;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Implements <see cref="IMutableTypeCodeGenerator"/> by building a subclass proxy and additional types using <see cref="ITypeBuilder"/> and
  /// related interfaces. Implements forward declarations of types (stage 1), method and constructor bodies (stage 2) by deferring emission.
  /// This is necessary to allow the generation of types and method bodies which reference each other.
  /// </summary>
  public class MutableTypeCodeGenerator : IMutableTypeCodeGenerator
  {
    private readonly IReflectionEmitCodeGenerator _codeGenerator;
    private readonly IMemberEmitterFactory _memberEmitterFactory;
    private readonly IInitializationBuilder _initializationBuilder;
    private readonly IProxySerializationEnabler _proxySerializationEnabler;

    [CLSCompliant (false)]
    public MutableTypeCodeGenerator (
        IReflectionEmitCodeGenerator codeGenerator,
        IMemberEmitterFactory memberEmitterFactory,
        IInitializationBuilder initializationBuilder,
        IProxySerializationEnabler proxySerializationEnabler)
    {
      ArgumentUtility.CheckNotNull ("codeGenerator", codeGenerator);
      ArgumentUtility.CheckNotNull ("memberEmitterFactory", memberEmitterFactory);
      ArgumentUtility.CheckNotNull ("initializationBuilder", initializationBuilder);
      ArgumentUtility.CheckNotNull ("proxySerializationEnabler", proxySerializationEnabler);

      _codeGenerator = codeGenerator;
      _memberEmitterFactory = memberEmitterFactory;
      _initializationBuilder = initializationBuilder;
      _proxySerializationEnabler = proxySerializationEnabler;
    }

    public ICodeGenerator CodeGenerator
    {
      get { return _codeGenerator; }
    }

    public Type CreateProxy (TypeContext typeContext)
    {
      ArgumentUtility.CheckNotNull ("typeContext", typeContext);
      var proxyType = typeContext.ProxyType;

      var emittableOperandProvider = _codeGenerator.EmittableOperandProvider;
      var memberEmitter = _memberEmitterFactory.CreateMemberEmitter (emittableOperandProvider);

      var typeBuilder = _codeGenerator.DefineType (proxyType.FullName, proxyType.Attributes, proxyType.BaseType);
      typeBuilder.RegisterWith (emittableOperandProvider, proxyType);

      var context = new CodeGenerationContext (proxyType, typeBuilder, _codeGenerator.DebugInfoGenerator, emittableOperandProvider);

      if (proxyType.MutableTypeInitializer != null)
        memberEmitter.AddConstructor (context, proxyType.MutableTypeInitializer);

      var initializationMembers = _initializationBuilder.CreateInitializationMembers (proxyType);
      var initializationMethod = initializationMembers != null ? initializationMembers.Item2 : null;

      _proxySerializationEnabler.MakeSerializable (proxyType, initializationMethod);

      foreach (var customAttribute in proxyType.AddedCustomAttributes)
        typeBuilder.SetCustomAttribute (customAttribute);

      foreach (var ifc in proxyType.AddedInterfaces)
        typeBuilder.AddInterfaceImplementation (ifc);

      foreach (var field in proxyType.AddedFields)
        memberEmitter.AddField (context, field);
      foreach (var ctor in proxyType.AddedConstructors)
        WireAndAddConstructor (memberEmitter, context, ctor, initializationMembers);
      foreach (var method in proxyType.AddedMethods)
        memberEmitter.AddMethod (context, method);
      // Note that accessor methods must be added before their associated properties and events.
      foreach (var property in proxyType.AddedProperties)
        memberEmitter.AddProperty (context, property);
      foreach (var evt in proxyType.AddedEvents)
        memberEmitter.AddEvent (context, evt);

      context.PostDeclarationsActionManager.ExecuteAllActions();

      return typeBuilder.CreateType();
    }

    private void WireAndAddConstructor (
        IMemberEmitter member, CodeGenerationContext context, MutableConstructorInfo constructor, Tuple<FieldInfo, MethodInfo> initializationMembers)
    {
      _initializationBuilder.WireConstructorWithInitialization (constructor, initializationMembers, _proxySerializationEnabler);
      member.AddConstructor (context, constructor);
    }
  }
}
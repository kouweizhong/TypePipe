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
using System.Globalization;
using System.Linq;
using System.Reflection;
using Remotion.TypePipe.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.TypePipe.FutureReflection
{
  /// <summary>
  /// Represents an existing <see cref="Type"/> that can be changed. Changes are recorded and later applied to the existing type via an
  /// instance of <see cref="ITypeModifier"/>.
  /// </summary>
  public class ModifiedType : MutableType
  {
    private readonly Type _originalType;

    public ModifiedType (Type originalType)
    {
      ArgumentUtility.CheckNotNull ("originalType", originalType);

      if (CanNotBeSubclassed (originalType))
        throw new ArgumentException ("Original type must not be sealed, an interface, a value type, an enum, a delegate, contain generic"
          + " parameters and must have an accessible constructor.", "originalType");

      _originalType = originalType;
    }

    public Type OriginalType
    {
      get { return _originalType; }
    }

    public override Type BaseType
    {
      get { return typeof (object); }
    }

    protected override bool HasElementTypeImpl ()
    {
      return false;
    }

    public override Assembly Assembly
    {
      get { return null; }
    }

    protected override ConstructorInfo GetConstructorImpl (
      BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
    {
      return AddedConstructors.SingleOrDefault ();
    }

    protected override bool IsByRefImpl ()
    {
      return false;
    }

    public override Type UnderlyingSystemType
    {
      get { return this; }
    }

    protected override TypeAttributes GetAttributeFlagsImpl ()
    {
      return _originalType.Attributes;
    }

    private bool CanNotBeSubclassed (Type type)
    {
      return type.IsSealed
          || type.IsInterface
          || typeof(Delegate).IsAssignableFrom(type)
          || type.ContainsGenericParameters
          || !HasAccessibleConstructor(type);
    }

    private bool HasAccessibleConstructor (Type type)
    {
      return type.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
          .Any (ctor => ctor.IsPublic || ctor.IsFamily || ctor.IsFamilyOrAssembly);
    }

    #region Not Implemented from Type interface

    public override string Name
    {
      get { throw new NotImplementedException (); }
    }

    public override object[] GetCustomAttributes (bool inherit)
    {
      throw new NotImplementedException ();
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      throw new NotImplementedException ();
    }

    public override ConstructorInfo[] GetConstructors (BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    public override Type GetInterface (string name, bool ignoreCase)
    {
      throw new NotImplementedException ();
    }

    public override Type[] GetInterfaces ()
    {
      throw new NotImplementedException ();
    }

    public override EventInfo GetEvent (string name, BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    public override EventInfo[] GetEvents (BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    public override Type[] GetNestedTypes (BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    public override Type GetNestedType (string name, BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    public override Type GetElementType ()
    {
      throw new NotImplementedException ();
    }

    protected override PropertyInfo GetPropertyImpl (
        string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
    {
      throw new NotImplementedException ();
    }

    public override PropertyInfo[] GetProperties (BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    protected override MethodInfo GetMethodImpl (
        string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
    {
      throw new NotImplementedException ();
    }

    public override MethodInfo[] GetMethods (BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    public override FieldInfo GetField (string name, BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    public override FieldInfo[] GetFields (BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    public override MemberInfo[] GetMembers (BindingFlags bindingAttr)
    {
      throw new NotImplementedException ();
    }

    protected override bool IsArrayImpl ()
    {
      throw new NotImplementedException ();
    }

    protected override bool IsPointerImpl ()
    {
      throw new NotImplementedException ();
    }

    protected override bool IsPrimitiveImpl ()
    {
      throw new NotImplementedException ();
    }

    protected override bool IsCOMObjectImpl ()
    {
      throw new NotImplementedException ();
    }

    public override object InvokeMember (
        string name,
        BindingFlags invokeAttr,
        Binder binder,
        object target,
        object[] args,
        ParameterModifier[] modifiers,
        CultureInfo culture,
        string[] namedParameters)
    {
      throw new NotImplementedException ();
    }

    public override Guid GUID
    {
      get { throw new NotImplementedException (); }
    }

    public override Module Module
    {
      get { throw new NotImplementedException (); }
    }

    public override string FullName
    {
      get { throw new NotImplementedException (); }
    }

    public override string Namespace
    {
      get { throw new NotImplementedException (); }
    }

    public override string AssemblyQualifiedName
    {
      get { throw new NotImplementedException (); }
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      throw new NotImplementedException ();
    }

    #endregion
  }
}
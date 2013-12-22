// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="ParamList"/> class for scenarios where the number or types of parameters are chosen at runtime.
  /// </summary>
  public class DynamicParamList : ParamList
  {
    private readonly Type[] _parameterTypes;
    private readonly object[] _parameterValues;

    public DynamicParamList (Type[] parameterTypes, object[] parameterValues)
    {
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);
      ArgumentUtility.CheckNotNull ("parameterValues", parameterValues);

      if (parameterValues.Length != parameterTypes.Length)
        throw new ArgumentException ("The number of parameter values must match the number of parameter types.", "parameterValues");

      _parameterTypes = parameterTypes;
      _parameterValues = parameterValues;
    }

    public override Type FuncType
    {
      get { return FuncUtility.MakeClosedType (typeof (object), _parameterTypes); }
    }

    public override Type ActionType
    {
      get { return ActionUtility.MakeClosedType (_parameterTypes); }
    }

    public override void InvokeAction (Delegate action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      try
      {
        action.DynamicInvoke (_parameterValues);
      }
      catch (TargetParameterCountException)
      {
        throw CreateActionTypeException (action);
      }
      catch (ArgumentException)
      {
        throw CreateActionTypeException (action);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException.PreserveStackTrace();
      }
    }

    public override object InvokeFunc (Delegate func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      try
      {
        return func.DynamicInvoke (_parameterValues);
      }
      catch (TargetParameterCountException)
      {
        throw CreateFuncTypeException (func);
      }
      catch (ArgumentException)
      {
        throw CreateFuncTypeException (func);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException.PreserveStackTrace();
      }
    }

    public override object InvokeConstructor (IConstructorLookupInfo constructorLookupInfo)
    {
      ArgumentUtility.CheckNotNull ("constructorLookupInfo", constructorLookupInfo);

      try
      {
        return constructorLookupInfo.DynamicInvoke (_parameterTypes, _parameterValues);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException.PreserveStackTrace();
      }
    }

    public override Type[] GetParameterTypes ()
    {
      return (Type[]) _parameterTypes.Clone();
    }

    public override object[] GetParameterValues ()
    {
      return (object[]) _parameterValues.Clone();
    }

    private ArgumentException CreateActionTypeException (Delegate action)
    {
      var message = string.Format (
          "Parameter 'action' has type '{0}' when a delegate with the following parameter signature was expected: ({1}).",
          action.GetType(),
          String.Join ((string) ", ", (IEnumerable<string>) _parameterTypes.Select (t => t.FullName)));
      return new ArgumentException (message, "action");
    }

    private ArgumentException CreateFuncTypeException (Delegate func)
    {
      var message = string.Format (
          "Parameter 'func' has type '{0}' when a delegate returning System.Object with the following parameter signature was expected: ({1}).",
          func.GetType(),
          string.Join (", ", _parameterTypes.Select (t => t.FullName)));
      return new ArgumentException (message, "func");
    }
  }
}

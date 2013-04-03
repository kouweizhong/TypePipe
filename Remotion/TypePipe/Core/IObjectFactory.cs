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
using System.Runtime.Serialization;
using Remotion.Reflection;
using Remotion.TypePipe.Implementation;

namespace Remotion.TypePipe
{
  /// <summary>
  /// Defines the main entry point of the pipeline.
  /// Implementations are used by application developers to create instances for the types generated by the pipeline.
  /// </summary>
  public interface IObjectFactory
  {
    // TODO 5503: docs
    string ParticipantConfigurationID { get; }

    // TODO 5503: docs
    ICodeManager CodeManager { get; }

    T CreateObject<T> (ParamList constructorArguments = null, bool allowNonPublicConstructor = false) where T : class;
    object CreateObject (Type requestedType, ParamList constructorArguments = null, bool allowNonPublicConstructor = false);

    /// <summary>
    /// Gets the assembled type for the requested type.
    /// </summary>
    /// <param name="requestedType">The requested type.</param>
    /// <returns>The generated type for the requested type.</returns>
    Type GetAssembledType (Type requestedType);

    /// <summary>
    /// Prepares an externally created instance of an assembled type that was not created by invoking a constructor.
    /// For example, an instance that was created via <see cref="FormatterServices"/>.<see cref="FormatterServices.GetUninitializedObject"/>.
    /// </summary>
    /// <param name="instance">The assembled type instance which should be prepared.</param>
    void PrepareExternalUninitializedObject (object instance);
  }
}
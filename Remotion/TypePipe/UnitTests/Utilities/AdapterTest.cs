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
using NUnit.Framework;
using Remotion.TypePipe.Utilities;

namespace Remotion.TypePipe.UnitTests.Utilities
{
  [TestFixture]
  public class AdapterTest
  {
    [Test]
    public void Initialization ()
    {
      // Arrange
      var item = new object();

      // Act
      var adapter = new Adapter<object> (item);

      // Assert
      Assert.That (adapter.Item, Is.SameAs (item));
    }

    [Test]
    public void New ()
    {
      // Arrange
      var item = new object ();

      // Act
      var adapter = Adapter.New (item);

      // Assert
      Assert.That (adapter.Item, Is.SameAs (item));
    }

    [Test]
    public void New_ThrowsForNull ()
    {
      // Arrange
      object item = null;

      // Act
      TestDelegate action = () => Adapter.New (item);

      // Assert
      Assert.That (action, Throws.TypeOf<ArgumentNullException> ()
        .With.Property ("ParamName").EqualTo ("item"));
    }
  }
}